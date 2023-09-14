using Sfs2X;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using UnityEngine;

public class SFSHeartBeat : MonoBehaviour
{
	public float heart_rate = 3f;

	private bool inited;

	private float cur_time;

	private static SFSHeartBeat instance;

	private SmartFox smartFox;

	private bool is_wait_server;

	private bool is_time_out;

	public OnReverseHearTimeout reverse_heart_timeout_delegate;

	public OnReverseHearWaiting reverse_heart_waiting_delegate;

	public OnReverseHearRenew reverse_heart_renew_delegate;

	private float m_reverse_heart_time;

	public static SFSHeartBeat Instance
	{
		get
		{
			return instance;
		}
	}

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (!inited)
		{
			return;
		}
		cur_time += Time.deltaTime;
		if (cur_time >= heart_rate)
		{
			RequestServerTime();
			cur_time = 0f;
		}
		if (Time.time - m_reverse_heart_time > 30f && !is_time_out)
		{
			if (reverse_heart_timeout_delegate != null)
			{
				reverse_heart_timeout_delegate();
			}
			is_time_out = true;
		}
		else if (Time.time - m_reverse_heart_time > 10f && !is_wait_server)
		{
			if (reverse_heart_waiting_delegate != null)
			{
				reverse_heart_waiting_delegate();
			}
			is_wait_server = true;
		}
	}

	public void Init()
	{
		inited = true;
		m_reverse_heart_time = Time.time;
		if (SmartFoxConnection.IsInitialized)
		{
			smartFox = SmartFoxConnection.Connection;
		}
		else
		{
			Debug.LogError("smartFox is not init.");
		}
	}

	private void RequestServerTime()
	{
		if (smartFox != null)
		{
			TimeManager.Instance.TimeSyncRequest();
			smartFox.Send(new ExtensionRequest("Zone.Common.Keepalive", new SFSObject()));
		}
	}

	private void OnDestroy()
	{
		if (smartFox != null)
		{
			smartFox = null;
		}
	}

	public void OnHeartProess()
	{
		m_reverse_heart_time = Time.time;
		if (is_wait_server)
		{
			is_wait_server = false;
			if (reverse_heart_renew_delegate != null)
			{
				reverse_heart_renew_delegate();
			}
		}
	}

	public static void CleanInstance()
	{
		instance = null;
	}

	public static void DestroyInstanceObj()
	{
		if (GameObject.Find("SFSTimeManager") != null)
		{
			GameObject obj = GameObject.Find("SFSTimeManager");
			CleanInstance();
			TimeManager.CleanInstance();
			Object.Destroy(obj);
		}
	}
}
