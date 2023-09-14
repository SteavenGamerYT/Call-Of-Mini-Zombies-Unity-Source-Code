using UnityEngine;

public class LocalNotificationWrapper
{
	public static void CancelAll()
	{
		Debug.Log("CancelAll");
	}

	public static void Schedule(string message, int time)
	{
		Debug.Log("Schedule");
	}
}
