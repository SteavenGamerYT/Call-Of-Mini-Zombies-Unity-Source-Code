using UnityEngine;
using Zombie3D;

public class FirEndlessDialog : UIDialog
{
	public FirEndlessDialog(DialogMode mode)
		: base(mode)
	{
		Material material = UIResourceMgr.GetInstance().GetMaterial("FirstEndless");
		SetBackgroundTexture(material, FirstEndlessTexturePosition.Board, AutoRect.AutoPos(new Rect(85f, 160f, 790f, 360f)));
		SetTextAreaOffset(AutoRect.AutoValuePos(new Rect(110f, 10f, -100f, -90f)));
		switch (mode)
		{
		case DialogMode.YES_OR_NO:
			SetText("font2", string.Empty, ColorName.fontColor_darkorange);
			SetButtonTexture(material, FirstEndlessTexturePosition.Button_normal, FirstEndlessTexturePosition.Button_high, AutoRect.AutoTex(new Vector2(440f, 102f)));
			SetYesButtonOffset(AutoRect.AutoValuePos(new Vector2(175f, -100f)), AutoRect.AutoTex(new Vector2(440f, 102f)));
			SetYesButtonText("font1", "TAP TO DISMISS", ColorName.fontColor_orange);
			DisableNoButton();
			break;
		case DialogMode.TAP_TO_DISMISS:
			SetTipTextOffset(AutoRect.AutoValuePos(new Rect(180f, -260f, 0f, 0f)));
			SetTipText("font2", "TAP TO DISMISS", ColorName.fontColor_darkorange);
			break;
		}
	}
}
