using UnityEngine;
using Zombie3D;

public class IAPDialog : GameDialog
{
	public const string NOT_ENOUGH_CASH = "\n\n\n  SHORT ON MONEY!";

	public const string NOT_AVAILABLE = "\n\n\nUNAVAILABLE WEAPON!";

	public IAPDialog(DialogMode mode)
		: base(mode)
	{
		SetTextAreaOffset(AutoRect.AutoValuePos(new Rect(80f, 80f, -20f, -106f)));
		SetText("font1", string.Empty, ColorName.fontColor_darkorange);
		Material material = UIResourceMgr.GetInstance().GetMaterial("ArenaMenu");
		SetText("\n\n\n  SHORT ON MONEY!");
		SetButtonTexture(material, ArenaMenuTexturePosition.GetMoneyButtonNormal, ArenaMenuTexturePosition.GetMoneyButtonPressed, AutoRect.AutoSize(ArenaMenuTexturePosition.GetMoneyButtonPressed));
		SetYesButtonOffset(AutoRect.AutoValuePos(new Vector2(26f, -42f)), AutoRect.AutoSize(ArenaMenuTexturePosition.GetMoneyButtonPressed));
		DisableNoButton();
		SetYesButtonText(string.Empty);
		SetNoButtonText(string.Empty);
		SetCloseButtonTexture(material, ArenaMenuTexturePosition.CloseButtonNormal, ArenaMenuTexturePosition.CloseButtonPressed);
		SetCloseButtonOffset(AutoRect.AutoValuePos(new Vector2(-16f, 224f)), AutoRect.AutoSize(ArenaMenuTexturePosition.CloseButtonNormal));
	}
}
