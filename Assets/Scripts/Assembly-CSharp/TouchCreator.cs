using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class TouchCreator
{
	private static BindingFlags flag;

	private static Dictionary<string, FieldInfo> fields;

	private object touch;

	public float deltaTime
	{
		get
		{
			return ((Touch)touch).deltaTime;
		}
		set
		{
			fields["m_TimeDelta"].SetValue(touch, value);
		}
	}

	public int tapCount
	{
		get
		{
			return ((Touch)touch).tapCount;
		}
		set
		{
			fields["m_TapCount"].SetValue(touch, value);
		}
	}

	public TouchPhase phase
	{
		get
		{
			return ((Touch)touch).phase;
		}
		set
		{
			fields["m_Phase"].SetValue(touch, value);
		}
	}

	public Vector2 deltaPosition
	{
		get
		{
			return ((Touch)touch).deltaPosition;
		}
		set
		{
			fields["m_PositionDelta"].SetValue(touch, value);
		}
	}

	public int fingerId
	{
		get
		{
			return ((Touch)touch).fingerId;
		}
		set
		{
			fields["m_FingerId"].SetValue(touch, value);
		}
	}

	public Vector2 position
	{
		get
		{
			return ((Touch)touch).position;
		}
		set
		{
			fields["m_Position"].SetValue(touch, value);
		}
	}

	public Vector2 rawPosition
	{
		get
		{
			return ((Touch)touch).rawPosition;
		}
		set
		{
			fields["m_RawPosition"].SetValue(touch, value);
		}
	}

	public TouchCreator()
	{
		touch = default(Touch);
	}

	static TouchCreator()
	{
		flag = BindingFlags.Instance | BindingFlags.NonPublic;
		fields = new Dictionary<string, FieldInfo>();
		FieldInfo[] array = typeof(Touch).GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
		foreach (FieldInfo fieldInfo in array)
		{
			fields.Add(fieldInfo.Name, fieldInfo);
			Debug.Log("name: " + fieldInfo.Name);
		}
	}

	public Touch Create()
	{
		return (Touch)touch;
	}
}
