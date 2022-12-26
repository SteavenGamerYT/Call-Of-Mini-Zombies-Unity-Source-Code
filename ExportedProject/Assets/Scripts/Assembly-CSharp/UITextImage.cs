using UnityEngine;

public class UITextImage : UIImage, UIContainer
{
	protected UIText m_Text = new UIText();

	public void SetText(string font, string text, Color color)
	{
		m_Text.Set(font, text, color);
		m_Text.AlignStyle = UIText.enAlignStyle.center;
		m_Text.Rect = Rect;
		m_Text.SetParent(this);
	}

	public void SetTextAlign(UIText.enAlignStyle style)
	{
		m_Text.AlignStyle = style;
		m_Text.Rect = Rect;
	}

	public void SetText(string text)
	{
		m_Text.SetText(text);
	}

	public override void Draw()
	{
		base.Draw();
		m_Text.Draw();
	}

	public void DrawSprite(UISprite sprite)
	{
		m_Parent.DrawSprite(sprite);
	}

	public void SendEvent(UIControl control, int command, float wparam, float lparam)
	{
	}

	public void Add(UIControl control)
	{
	}
}
