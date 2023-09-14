using UnityEngine;

public class UIMove : UIControl
{
	public enum Command
	{
		Begin = 0,
		Move = 1,
		End = 2,
		MovePos = 3
	}

	protected int m_FingerId;

	protected Vector2 m_TouchPosition;

	protected bool m_Move;

	protected float m_MinX;

	protected float m_MinY;

	public float MinX
	{
		get
		{
			return m_MinX;
		}
		set
		{
			m_MinX = value;
		}
	}

	public float MinY
	{
		get
		{
			return m_MinY;
		}
		set
		{
			m_MinY = value;
		}
	}

	public UIMove()
	{
		m_FingerId = -1;
		m_TouchPosition = new Vector2(0f, 0f);
		m_Move = false;
	}

	public override bool HandleInput(UITouchInner touch)
	{
		if (touch.phase == TouchPhase.Began)
		{
			if (PtInRect(touch.position))
			{
				m_FingerId = touch.fingerId;
				m_TouchPosition = touch.position;
				m_Move = false;
				m_Parent.SendEvent(this, 0, touch.position.x, touch.position.y);
			}
			return false;
		}
		if (touch.fingerId != m_FingerId)
		{
			return false;
		}
		if (touch.phase == TouchPhase.Moved)
		{
			float num = touch.position.x - m_TouchPosition.x;
			float num2 = touch.position.y - m_TouchPosition.y;
			if (m_Move)
			{
				m_TouchPosition = touch.position;
				m_Parent.SendEvent(this, 1, num, num2);
				m_Parent.SendEvent(this, 3, touch.position.x, touch.position.y);
			}
			else
			{
				float num3 = ((num >= 0f) ? num : (0f - num));
				float num4 = ((num2 >= 0f) ? num2 : (0f - num2));
				if (num3 > m_MinX || num4 > m_MinY)
				{
					m_TouchPosition = touch.position;
					m_Move = true;
					m_Parent.SendEvent(this, 1, num, num2);
				}
			}
			return true;
		}
		if (touch.phase == TouchPhase.Ended)
		{
			bool move = m_Move;
			m_FingerId = -1;
			m_TouchPosition = new Vector2(0f, 0f);
			m_Move = false;
			m_Parent.SendEvent(this, 2, touch.position.x, touch.position.y);
			return false;
		}
		return false;
	}
}
