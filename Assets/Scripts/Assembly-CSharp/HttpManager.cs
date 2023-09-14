using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

public class HttpManager : MonoBehaviour
{
	private class HttpSession
	{
		public int task_id;

		public string param;

		public OnResponseDelegate onResponse;

		public OnRequestTimeoutDelegate onTimeout;

		public string response;

		public int code;

		public void Callback()
		{
			if (response.Length > 0)
			{
				onResponse(task_id, param, code, response);
			}
			else
			{
				onTimeout(task_id, param);
			}
		}
	}

	public delegate void OnResponseDelegate(int task_id, string param, int code, string response);

	public delegate void OnRequestTimeoutDelegate(int task_id, string param);

	private const string PluginModule = "HttpPlugin";

	public string m_url;

	private Hashtable m_http_map = new Hashtable();

	[DllImport("HttpPlugin")]
	protected static extern int HttpSendRequest(string url, string request);

	[DllImport("HttpPlugin")]
	protected static extern int HttpRecvResponse(int task_id, ref int code, [Out] byte[] response, int buf_length, ref int response_length);

	[DllImport("HttpPlugin")]
	protected static extern int HttpRecvRespLength(int task_id, ref int code, ref int response_length);

	[DllImport("HttpPlugin")]
	protected static extern void HttpCancelTask(int task_id);

	public static HttpManager CreateInstance(string name)
	{
		GameObject gameObject = new GameObject(name);
		UnityEngine.Object.DontDestroyOnLoad(gameObject);
		return gameObject.AddComponent<HttpManager>();
	}

	public static HttpManager Instance(string name)
	{
		GameObject gameObject = GameObject.Find(name);
		if (gameObject == null)
		{
			return CreateInstance(name);
		}
		return gameObject.GetComponent("HttpManager") as HttpManager;
	}

	public static HttpManager Instance()
	{
		return Instance("HttpManager");
	}

	public void SetUrl(string url)
	{
		m_url = url;
	}

	public int SendRequest(string url, string request, string param, float timeout, OnResponseDelegate onResponse, OnRequestTimeoutDelegate onTimeout)
	{
		int num = HttpSendRequest(url, request);
		if (num < 0)
		{
			return -1;
		}
		HttpSession httpSession = new HttpSession();
		httpSession.task_id = num;
		httpSession.param = param;
		httpSession.onResponse = onResponse;
		httpSession.onTimeout = onTimeout;
		httpSession.response = string.Empty;
		httpSession.code = -1;
		m_http_map.Add(num, httpSession);
		return num;
	}

	public void FixedUpdate()
	{
		if (m_http_map.Count <= 0)
		{
			return;
		}
		int[] array = new int[m_http_map.Count];
		int num = 0;
		IDictionaryEnumerator enumerator = m_http_map.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				HttpSession httpSession = ((DictionaryEntry)enumerator.Current).Value as HttpSession;
				int code = -1;
				int response_length = 0;
				int num2 = HttpRecvRespLength(httpSession.task_id, ref code, ref response_length);
				if (num2 < 0)
				{
					array[num] = httpSession.task_id;
					num++;
					httpSession.code = code;
				}
				else if (num2 > 0)
				{
					byte[] array2 = new byte[response_length];
					num2 = HttpRecvResponse(httpSession.task_id, ref code, array2, response_length, ref response_length);
					array[num] = httpSession.task_id;
					num++;
					httpSession.code = code;
					httpSession.response = Encoding.UTF8.GetString(array2, 0, response_length);
				}
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = enumerator as IDisposable) != null)
			{
				disposable.Dispose();
			}
		}
		for (int i = 0; i < num; i++)
		{
			int num3 = array[i];
			HttpSession httpSession2 = m_http_map[num3] as HttpSession;
			m_http_map.Remove(num3);
			httpSession2.Callback();
		}
	}

	private void OnDestroy()
	{
		IDictionaryEnumerator enumerator = m_http_map.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				HttpSession httpSession = ((DictionaryEntry)enumerator.Current).Value as HttpSession;
				HttpCancelTask(httpSession.task_id);
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = enumerator as IDisposable) != null)
			{
				disposable.Dispose();
			}
		}
		m_http_map.Clear();
	}
}
