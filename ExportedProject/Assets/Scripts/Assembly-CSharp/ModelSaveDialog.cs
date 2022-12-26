using UnityEngine;
using Zombie3D;

public class ModelSaveDialog : UIDialog
{
	public ModelSaveDialog(DialogMode mode)
		: base(mode)
	{
		Material material = UIResourceMgr.GetInstance().GetMaterial("PaperSaveButton");
		SetBackgroundTexture(material, PaperShowUITexturePosition.MsgBoxT, AutoRect.AutoPos(new Rect(194f, 192f, 572f, 256f)));
		SetTextAreaOffset(AutoRect.AutoValuePos(new Rect(40f, 60f, -70f, -70f)));
		if (mode == DialogMode.YES_OR_NO)
		{
			Material material2 = UIResourceMgr.GetInstance().GetMaterial("Buttons");
			SetText("font2", "\n", ColorName.fontColor_darkorange);
			SetButtonTexture(material2, ButtonsTexturePosition.ButtonNormal, ButtonsTexturePosition.ButtonPressed, AutoRect.AutoSize(ButtonsTexturePosition.TinySizeButton));
			SetYesButtonOffset(AutoRect.AutoValuePos(new Vector2(173f, -40f)), AutoRect.AutoSize(ButtonsTexturePosition.TinySizeButton));
			SetNoButtonOffset(AutoRect.AutoValuePos(new Vector2(220f, -50f)), AutoRect.AutoSize(ButtonsTexturePosition.TinySizeButton));
			SetYesButtonText("font1", "OK", ColorName.fontColor_orange);
			SetNoButtonText("font2", "BACK", ColorName.fontColor_orange);
			DisableNoButton();
			SetNoButtonText(string.Empty);
		}
	}
}
