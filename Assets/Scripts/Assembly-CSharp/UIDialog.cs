using UnityEngine;

public class UIDialog : UIPanel, UIHandler
{
	public enum DialogMode
	{
		TAP_TO_DISMISS = 0,
		YES_OR_NO = 1
	}

	protected DialogMode m_Mode;

	protected UIImage m_BackgroundImg;

	protected UIText m_Text;

	protected UIText m_TipText;

	protected UITextButton m_YesButton;

	protected UITextButton m_NoButton;

	protected UIClickButton m_CloseButton;

	protected UIDialogEventHandler m_EventHandler;

	protected UIBlock m_Block;

	public UIDialog(DialogMode mode)
	{
		m_Mode = mode;
		m_BackgroundImg = new UIImage();
		m_Text = new UIText();
		m_Block = new UIBlock();
		m_Block.Rect = new Rect(0f, 0f, Screen.width, Screen.height);
		Add(m_Block);
		Add(m_BackgroundImg);
		Add(m_Text);
		if (m_Mode == DialogMode.YES_OR_NO)
		{
			m_YesButton = new UITextButton();
			m_NoButton = new UITextButton();
			m_CloseButton = new UIClickButton();
			Add(m_YesButton);
			Add(m_NoButton);
			Add(m_CloseButton);
		}
		else if (m_Mode == DialogMode.TAP_TO_DISMISS)
		{
			m_TipText = new UIText();
			Add(m_TipText);
		}
		SetUIHandler(this);
	}

	public void SetDialogEventHandler(UIDialogEventHandler handler)
	{
		m_EventHandler = handler;
	}

	public void SetBackgroundTexture(Material material, Rect textRect, Rect posRect)
	{
		m_BackgroundImg.SetTexture(material, textRect, AutoRect.AutoSize(textRect));
		m_BackgroundImg.Rect = posRect;
		m_Text.Rect = posRect;
		if (m_TipText != null)
		{
			m_TipText.Rect = posRect;
		}
	}

	public void SetTipTextOffset(Rect rect)
	{
		m_TipText.Rect = new Rect(m_BackgroundImg.Rect.x + rect.x, m_BackgroundImg.Rect.y + rect.y, m_BackgroundImg.Rect.width + rect.width, m_BackgroundImg.Rect.height + rect.height);
	}

	public void SetTextAreaOffset(Rect rect)
	{
		m_Text.Rect = new Rect(m_BackgroundImg.Rect.x + rect.x, m_BackgroundImg.Rect.y + rect.y, m_BackgroundImg.Rect.width + rect.width, m_BackgroundImg.Rect.height + rect.height);
	}

	public void SetCloseButtonTexture(Material material, Rect normalRect, Rect pressedRect)
	{
		m_CloseButton.SetTexture(UIButtonBase.State.Normal, material, normalRect);
		m_CloseButton.SetTexture(UIButtonBase.State.Pressed, material, pressedRect);
	}

	public void SetButtonTexture(Material material, Rect normalRect, Rect pressedRect, Vector2 textureSize)
	{
		if (m_Mode == DialogMode.YES_OR_NO)
		{
			m_YesButton.SetTexture(UIButtonBase.State.Normal, material, normalRect, textureSize);
			m_YesButton.SetTexture(UIButtonBase.State.Pressed, material, pressedRect, textureSize);
			m_NoButton.SetTexture(UIButtonBase.State.Normal, material, normalRect, textureSize);
			m_NoButton.SetTexture(UIButtonBase.State.Pressed, material, pressedRect, textureSize);
		}
	}

	public void DisableNoButton()
	{
		m_NoButton.Visible = false;
		m_NoButton.Enable = false;
	}

	public void SetYesButtonOffset(Vector2 offset, Vector2 size)
	{
		m_YesButton.Rect = new Rect(m_BackgroundImg.Rect.x + offset.x, m_BackgroundImg.Rect.y + offset.y, size.x, size.y);
	}

	public void SetYesButtonText(string font, string text, Color color)
	{
		m_YesButton.SetText(font, text, color);
	}

	public void SetYesButtonText(string text)
	{
		m_YesButton.SetText(text);
	}

	public void SetNoButtonText(string font, string text, Color color)
	{
		m_NoButton.SetText(font, text, color);
	}

	public void SetNoButtonText(string text)
	{
		m_NoButton.SetText(text);
	}

	public void SetNoButtonOffset(Vector2 offset, Vector2 size)
	{
		m_NoButton.Rect = new Rect(m_BackgroundImg.Rect.x + offset.x, m_BackgroundImg.Rect.y + offset.y, size.x, size.y);
	}

	public void SetCloseButtonOffset(Vector2 offset, Vector2 size)
	{
		m_CloseButton.Rect = new Rect(m_BackgroundImg.Rect.x + offset.x, m_BackgroundImg.Rect.y + offset.y, size.x, size.y);
	}

	public void SetText(string font, string text, Color color)
	{
		m_Text.Set(font, text, color);
	}

	public void SetTipText(string font, string text, Color color)
	{
		m_TipText.Set(font, text, color);
	}

	public void SetText(string text)
	{
		m_Text.SetText(text);
	}

	public override void Draw()
	{
		base.Draw();
		if (m_Mode == DialogMode.YES_OR_NO)
		{
			m_YesButton.Draw();
			m_NoButton.Draw();
		}
	}

	public void HandleEvent(UIControl control, int command, float wparam, float lparam)
	{
		if (control == m_YesButton)
		{
			m_EventHandler.Yes();
		}
		else if (control == m_NoButton || control == m_CloseButton)
		{
			m_EventHandler.No();
		}
		else if (control == m_BackgroundImg && m_Mode == DialogMode.TAP_TO_DISMISS)
		{
			Hide();
			m_EventHandler.Yes();
		}
	}
}
