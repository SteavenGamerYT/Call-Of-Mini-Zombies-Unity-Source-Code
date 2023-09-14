using System;
using System.Collections;

public class UIPanel : UIControl, UIContainer
{
	protected ArrayList m_Controls = new ArrayList();

	private UIHandler m_UIHandler;

	public UIPanel()
	{
		Visible = false;
		Enable = false;
	}

	public override void Draw()
	{
		base.Draw();
		if (!Visible)
		{
			return;
		}
		IEnumerator enumerator = m_Controls.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				UIControl uIControl = (UIControl)enumerator.Current;
				if (uIControl.Visible)
				{
					uIControl.Draw();
				}
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = enumerator as IDisposable) != null)
			{
				disposable.Dispose();
			}
		}
	}

	public virtual void UpdateLogic()
	{
	}

	public virtual void Hide()
	{
		Visible = false;
		Enable = false;
	}

	public virtual void Show()
	{
		Visible = true;
		Enable = true;
	}

	public void SetUIHandler(UIHandler ui_handler)
	{
		m_UIHandler = ui_handler;
	}

	public void DrawSprite(UISprite sprite)
	{
		m_Parent.DrawSprite(sprite);
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

	public void SendEvent(UIControl control, int command, float wparam, float lparam)
	{
		if (m_UIHandler != null)
		{
			m_UIHandler.HandleEvent(control, command, wparam, lparam);
		}
		else
		{
			m_Parent.SendEvent(this, command, wparam, lparam);
		}
	}

	public virtual void Add(UIControl control)
	{
		m_Controls.Add(control);
		control.SetParent(this);
	}
}
