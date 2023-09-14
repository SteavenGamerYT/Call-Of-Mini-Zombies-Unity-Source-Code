using UnityEngine;
using Zombie3D;

public class ReviewDialog : UIDialog
{
	public ReviewDialog()
		: base(DialogMode.YES_OR_NO)
	{
		Material material = UIResourceMgr.GetInstance().GetMaterial("Dialog");
		Material material2 = UIResourceMgr.GetInstance().GetMaterial("Buttons");
		SetBackgroundTexture(material, DialogTexturePosition.Dialog, AutoRect.AutoPos(new Rect(193f, 167f, 574f, 306f)));
		SetTextAreaOffset(AutoRect.AutoValuePos(new Rect(80f, 70f, -100f, -90f)));
		SetText("font2", "\n                REVIEW US \nLIKE THIS GAME? WANT MORE UPDATES? PLEASE REVIEW US.", ColorName.fontColor_darkorange);
		SetButtonTexture(material2, ButtonsTexturePosition.ButtonNormal, ButtonsTexturePosition.ButtonPressed, AutoRect.AutoSize(ButtonsTexturePosition.TinySizeButton));
		SetYesButtonOffset(AutoRect.AutoValuePos(new Vector2(70f, 0f)), AutoRect.AutoSize(ButtonsTexturePosition.TinySizeButton));
		SetNoButtonOffset(AutoRect.AutoValuePos(new Vector2(300f, 0f)), AutoRect.AutoSize(ButtonsTexturePosition.TinySizeButton));
		SetYesButtonText("font2", "YES", ColorName.fontColor_orange);
		SetNoButtonText("font2", "NO,THANKS", ColorName.fontColor_orange);
	}
}
