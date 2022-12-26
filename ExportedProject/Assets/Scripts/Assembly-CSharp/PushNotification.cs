using System;
using System.Collections.Generic;

public class PushNotification
{
	public static void ReSetNotifications()
	{
		LocalNotificationWrapper.CancelAll();
		List<string> list = new List<string>();
		List<string> list2 = new List<string>();
		list.Add("It??s survival of the fittest in VS! Can you become the top dog?");
		list.Add("Violence isn??t always the answer, but it is in VS!");
		list.Add("Your chainsaw thirsts for blood!");
		list.Add("Why wait for the zombies to kill the others when you can finish the job yourself?");
		list.Add("The only things deadlier than the undead are the living! Will you be the last man standing in VS?");
		Random random = new Random();
		foreach (string item in list)
		{
			int index = random.Next(list2.Count + 1);
			list2.Insert(index, item);
		}
		int num = 1;
		foreach (string item2 in list2)
		{
			LocalNotificationWrapper.Schedule(item2, 259200 * num);
			num++;
		}
	}
}
