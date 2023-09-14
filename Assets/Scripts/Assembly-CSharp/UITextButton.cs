using UnityEngine;
using Zombie3D;

public class UITextButton : UIClickButton, UIContainer
{
	public static Color fontColor_orange = ColorName.fontColor_orange;

	public static Color fontColor_yellow = ColorName.fontColor_yellow;

	protected UIText m_Text = new UIText();

	protected Color m_NormalColor = fontColor_orange;

	protected Color m_PressedColor = fontColor_yellow;

	public void SetText(string font, string text, Color color)
	{
		m_Text.AlignStyle = UIText.enAlignStyle.center;
		m_Text.Rect = Rect;
		m_Text.SetParent(this);
		m_Text.Set(font, text, color);
	}

	public void SetText(string text)
	{
		m_Text.SetText(text);
	}

	public void SetTextColor(Color normalColor, Color pressedColor)
	{
		m_NormalColor = normalColor;
		m_PressedColor = pressedColor;
	}

	public override void Draw()
	{
		base.Draw();
		if (m_State == State.Normal)
		{
			m_Text.SetColor(m_NormalColor);
		}
		else if (m_State == State.Pressed)
		{
			m_Text.SetColor(m_PressedColor);
		}
		m_Text.Draw();
	}

	public void DrawSprite(UISprite sprite)
	{
		m_Parent.DrawSprite(sprite);
	}

	public void SendEvent(UIControl control, int command, float wparam, float lparam)
	{
		m_Parent.SendEvent(control, command, wparam, lparam);
	}

	public void Add(UIControl control)
	{
	}
}
