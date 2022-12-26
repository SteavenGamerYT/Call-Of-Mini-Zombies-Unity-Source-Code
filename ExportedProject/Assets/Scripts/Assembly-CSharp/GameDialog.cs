using UnityEngine;
using Zombie3D;

public class GameDialog : UIDialog
{
	public GameDialog(DialogMode mode)
		: base(mode)
	{
		Material material = UIResourceMgr.GetInstance().GetMaterial("Dialog");
		Material material2 = UIResourceMgr.GetInstance().GetMaterial("Buttons");
		SetBackgroundTexture(material, DialogTexturePosition.Dialog, AutoRect.AutoPos(new Rect(193f, 167f, 574f, 306f)));
		SetTextAreaOffset(AutoRect.AutoValuePos(new Rect(80f, 70f, -100f, -90f)));
		switch (mode)
		{
		case DialogMode.YES_OR_NO:
			SetText("font2", string.Empty, ColorName.fontColor_darkorange);
			SetButtonTexture(material2, ButtonsTexturePosition.ButtonNormal, ButtonsTexturePosition.ButtonPressed, AutoRect.AutoSize(ButtonsTexturePosition.TinySizeButton));
			SetYesButtonOffset(AutoRect.AutoValuePos(new Vector2(70f, 0f)), AutoRect.AutoSize(ButtonsTexturePosition.TinySizeButton));
			SetNoButtonOffset(AutoRect.AutoValuePos(new Vector2(300f, 0f)), AutoRect.AutoSize(ButtonsTexturePosition.TinySizeButton));
			SetYesButtonText("font2", "YES", ColorName.fontColor_orange);
			SetNoButtonText("font2", "NO", ColorName.fontColor_orange);
			break;
		case DialogMode.TAP_TO_DISMISS:
			SetTipTextOffset(AutoRect.AutoValuePos(new Rect(180f, -260f, 0f, 0f)));
			SetTipText("font2", "TAP TO DISMISS", ColorName.fontColor_darkorange);
			break;
		}
	}
}
