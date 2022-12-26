using UnityEngine;

public class UISelectIdButton : UISelectButton
{
	public int Button_ID;

	protected UISelectIdEventHandler m_EventHandler;

	public void SetEventHandler(UISelectIdEventHandler handler)
	{
		m_EventHandler = handler;
	}

	public override bool HandleInput(UITouchInner touch)
	{
		if (touch.phase == TouchPhase.Began)
		{
			if (PtInRect(touch.position))
			{
				m_FingerId = touch.fingerId;
				return true;
			}
			return false;
		}
		if (touch.fingerId == m_FingerId)
		{
			if (touch.phase == TouchPhase.Ended)
			{
				m_FingerId = -1;
				if (PtInRect(touch.position) && m_State == State.Normal)
				{
					m_State = State.Pressed;
					m_Parent.SendEvent(this, 0, 0f, 0f);
					if (m_EventHandler != null)
					{
						m_EventHandler.OnButtonDown(Button_ID);
					}
				}
			}
			return true;
		}
		return false;
	}
}
