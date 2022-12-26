using Sfs2X;
using UnityEngine;

public class SmartFoxConnection : MonoBehaviour
{
	private static SmartFoxConnection mInstance;

	private static SmartFox smartFox;

	public static bool is_server;

	public static SmartFox Connection
	{
		get
		{
			if (mInstance == null)
			{
				mInstance = new GameObject("SmartFoxConnection").AddComponent(typeof(SmartFoxConnection)) as SmartFoxConnection;
			}
			return smartFox;
		}
		set
		{
			if (mInstance == null)
			{
				mInstance = new GameObject("SmartFoxConnection").AddComponent(typeof(SmartFoxConnection)) as SmartFoxConnection;
			}
			smartFox = value;
		}
	}

	public static bool IsInitialized
	{
		get
		{
			return smartFox != null;
		}
	}

	private void OnApplicationQuit()
	{
		if (Application.platform != RuntimePlatform.IPhonePlayer && Application.platform != RuntimePlatform.Android && smartFox.IsConnected)
		{
			smartFox.Disconnect();
		}
	}

	public static void UnregisterSFSSceneCallbacks()
	{
		if (smartFox != null)
		{
			smartFox.RemoveAllEventListeners();
		}
	}

	public static void Disconnect()
	{
		if (Application.platform != RuntimePlatform.IPhonePlayer && Application.platform != RuntimePlatform.Android && smartFox != null && smartFox.IsConnected)
		{
			smartFox.Disconnect();
		}
		smartFox = null;
	}
}
