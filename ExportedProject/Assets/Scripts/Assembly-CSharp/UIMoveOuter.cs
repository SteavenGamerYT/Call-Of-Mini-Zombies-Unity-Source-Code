using UnityEngine;

public class UIMoveOuter : UIMove
{
	public bool IsMoving()
	{
		return m_Move;
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
			}
			return false;
		}
		if (touch.fingerId != m_FingerId)
		{
			return false;
		}
		if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
		{
			float num = touch.position.x - m_TouchPosition.x;
			float num2 = touch.position.y - m_TouchPosition.y;
			if (m_Move)
			{
				m_TouchPosition = touch.position;
				m_Parent.SendEvent(this, 1, num, num2);
			}
			else
			{
				float num3 = ((num >= 0f) ? num : (0f - num));
				float num4 = ((num2 >= 0f) ? num2 : (0f - num2));
				if (!(num3 > m_MinX) && !(num4 > m_MinY))
				{
					return false;
				}
				m_TouchPosition = touch.position;
				m_Move = true;
				m_Parent.SendEvent(this, 0, 0f, 0f);
				m_Parent.SendEvent(this, 1, num, num2);
			}
			return true;
		}
		if (touch.phase == TouchPhase.Ended)
		{
			bool move = m_Move;
			m_FingerId = -1;
			m_TouchPosition = new Vector2(0f, 0f);
			m_Move = false;
			if (move)
			{
				m_Parent.SendEvent(this, 2, 0f, 0f);
				return true;
			}
			return false;
		}
		return false;
	}
}
