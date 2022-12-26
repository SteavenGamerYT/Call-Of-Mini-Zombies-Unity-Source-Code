using System.Collections;
using UnityEngine;

public class UIGroupControl : UIControl, UIContainer
{
	private ArrayList m_Controls;

	public override bool Visible
	{
		get
		{
			return m_Visible;
		}
		set
		{
			m_Visible = value;
			for (int num = m_Controls.Count; num > 0; num--)
			{
				((UIControl)m_Controls[num - 1]).Visible = value;
			}
		}
	}

	public override bool Enable
	{
		get
		{
			return m_Enable;
		}
		set
		{
			m_Enable = value;
			for (int num = m_Controls.Count; num > 0; num--)
			{
				((UIControl)m_Controls[num - 1]).Enable = value;
			}
		}
	}

	public override Rect Rect
	{
		get
		{
			return base.Rect;
		}
		set
		{
			Vector2 vector = new Vector2(value.x - base.Rect.x, value.y - base.Rect.y);
			base.Rect = value;
			for (int num = m_Controls.Count; num > 0; num--)
			{
				Rect rect = ((UIControl)m_Controls[num - 1]).Rect;
				((UIControl)m_Controls[num - 1]).Rect = new Rect(rect.x + vector.x, rect.y + vector.y, rect.width, rect.height);
			}
		}
	}

	public ArrayList Controls
	{
		get
		{
			return m_Controls;
		}
	}

	public UIGroupControl()
	{
		m_Controls = new ArrayList();
	}

	public UIGroupControl(Vector2 pos)
	{
		m_Controls = new ArrayList();
	}

	public override void SetClip(Rect clip_rect)
	{
		base.SetClip(clip_rect);
		for (int num = m_Controls.Count; num > 0; num--)
		{
			((UIControl)m_Controls[num - 1]).SetClip(clip_rect);
		}
	}

	public void Add(UIControl control)
	{
		control.SetParent(this);
		m_Controls.Add(control);
		if (m_Clip)
		{
			control.SetClip(m_ClipRect);
		}
	}

	public void Remove(UIControl control)
	{
		m_Controls.Remove(control);
	}

	public UIControl GetControl(int controlId)
	{
		for (int i = 0; i < m_Controls.Count; i++)
		{
			if (((UIControl)m_Controls[i]).Id == controlId)
			{
				return (UIControl)m_Controls[i];
			}
		}
		return null;
	}

	public void Clear()
	{
		m_Controls.Clear();
	}

	public override void Draw()
	{
		for (int i = 0; i < m_Controls.Count; i++)
		{
			UIControl uIControl = (UIControl)m_Controls[i];
			uIControl.Update();
			if (uIControl.Visible)
			{
				uIControl.Draw();
			}
		}
	}

	public void DrawSprite(UISprite sprite)
	{
		m_Parent.DrawSprite(sprite);
	}

	public void SendEvent(UIControl control, int command, float wparam, float lparam)
	{
		m_Parent.SendEvent(control, command, wparam, lparam);
	}

	public override bool HandleInput(UITouchInner touch)
	{
		for (int num = m_Controls.Count - 1; num >= 0; num--)
		{
			UIControl uIControl = (UIControl)m_Controls[num];
			if (uIControl.Enable && uIControl.HandleInput(touch))
			{
				return true;
			}
		}
		return false;
	}
}
