using System.Collections.Generic;
using UnityEngine;

public class WindowsInputMgr : MonoBehaviour
{
	public static UITouchInner[] MockTouches()
	{
		UITouchInner[] array = new UITouchInner[1];
		using (List<Touch>.Enumerator enumerator = InputHelper.GetTouches().GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				Touch current = enumerator.Current;
				array[0].deltaPosition = current.deltaPosition;
				array[0].deltaTime = current.deltaTime;
				array[0].fingerId = current.fingerId;
				array[0].phase = current.phase;
				array[0].position = current.position;
				array[0].tapCount = 1;
				return array;
			}
			return array;
		}
	}
}
