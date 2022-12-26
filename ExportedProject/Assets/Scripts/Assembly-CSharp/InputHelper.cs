using System.Collections.Generic;
using UnityEngine;

public class InputHelper : MonoBehaviour
{
	private static TouchCreator lastFakeTouch;

	public static List<Touch> GetTouches()
	{
		List<Touch> list = new List<Touch>();
		list.AddRange(Input.touches);
		if (lastFakeTouch == null)
		{
			lastFakeTouch = new TouchCreator();
		}
		if (Input.GetMouseButtonDown(0))
		{
			lastFakeTouch.phase = TouchPhase.Began;
			lastFakeTouch.deltaPosition = new Vector2(0f, 0f);
			lastFakeTouch.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
			lastFakeTouch.fingerId = 0;
		}
		else if (Input.GetMouseButtonUp(0))
		{
			lastFakeTouch.phase = TouchPhase.Ended;
			Vector2 vector = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
			lastFakeTouch.deltaPosition = vector - lastFakeTouch.position;
			lastFakeTouch.position = vector;
			lastFakeTouch.fingerId = 0;
		}
		else if (Input.GetMouseButton(0))
		{
			lastFakeTouch.phase = TouchPhase.Moved;
			Vector2 vector2 = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
			lastFakeTouch.deltaPosition = vector2 - lastFakeTouch.position;
			lastFakeTouch.position = vector2;
			lastFakeTouch.fingerId = 0;
		}
		else
		{
			lastFakeTouch = null;
		}
		if (lastFakeTouch != null)
		{
			list.Add(lastFakeTouch.Create());
		}
		return list;
	}
}
