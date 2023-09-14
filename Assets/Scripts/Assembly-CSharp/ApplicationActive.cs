using UnityEngine;

public class ApplicationActive : MonoBehaviour
{
	private void Awake()
	{
		Application.targetFrameRate = 120;
		Object.DontDestroyOnLoad(base.gameObject);
	}

	private void OnApplicationFocus(bool focus)
	{
		Debug.Log("OnApplicationFocus:" + focus);
	}

	private void OnApplicationPause(bool pause)
	{
		PushNotification.ReSetNotifications();
		Debug.Log("OnApplicationPause:" + pause);
	}
}
