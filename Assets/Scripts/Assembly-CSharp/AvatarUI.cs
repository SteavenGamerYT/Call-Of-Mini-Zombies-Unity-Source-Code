using UnityEngine;
using Zombie3D;

public class AvatarUI : UIPanel, UIHandler, UIDialogEventHandler
{
	protected const int BUTTON_NUM = 12;

	protected Rect[] buttonRect;

	protected Material arenaMenuMaterial;

	protected Material avatarLogoMaterial;

	protected Material avatarLogoMaterial1;

	protected Material avatarLogoMaterial2;

	protected UIImage background;

	protected UIClickButton getMoreMoneyButton;

	protected UIImage avatarImage;

	protected UIImage textBackground;

	protected UIImage avatarClickImage;

	protected UIClickButton returnButton;

	protected CashPanel cashPanel;

	protected UIImageScroller avatarScroller;

	protected UIText avatarInfoText;

	protected IAPDialog iapDialog;

	protected UIText returnText;

	protected UITextButton buyButton;

	private AvatarUIPosition uiPos;

	protected AvatarInfoPanel avatarInfoPanel;

	protected Avatar3DFrame avatarFrame;

	protected int currentSelectionIndex;

	public AvatarUI()
	{
		uiPos = new AvatarUIPosition();
		AvatarTexturePosition.InitLogosTexturePos();
		arenaMenuMaterial = UIResourceMgr.GetInstance().GetMaterial("ArenaMenu");
		avatarLogoMaterial = UIResourceMgr.GetInstance().GetMaterial("Avatar");
		avatarLogoMaterial1 = UIResourceMgr.GetInstance().GetMaterial("Avatar01");
		avatarLogoMaterial2 = UIResourceMgr.GetInstance().GetMaterial("Avatar02");
		Material material = UIResourceMgr.GetInstance().GetMaterial("Buttons");
		background = new UIImage();
		background.SetTexture(arenaMenuMaterial, ArenaMenuTexturePosition.Background, AutoRect.AutoSize(ArenaMenuTexturePosition.Background));
		background.Rect = AutoRect.AutoPos(uiPos.Background);
		returnButton = new UIClickButton();
		returnButton.SetTexture(UIButtonBase.State.Normal, arenaMenuMaterial, ArenaMenuTexturePosition.ReturnButtonNormal, AutoRect.AutoSize(ArenaMenuTexturePosition.ReturnButtonNormal));
		returnButton.SetTexture(UIButtonBase.State.Pressed, arenaMenuMaterial, ArenaMenuTexturePosition.ReturnButtonPressed, AutoRect.AutoSize(ArenaMenuTexturePosition.ReturnButtonPressed));
		returnButton.Rect = AutoRect.AutoPos(uiPos.ReturnButton);
		buyButton = new UITextButton();
		buyButton.SetTexture(UIButtonBase.State.Normal, material, ButtonsTexturePosition.ButtonNormal, AutoRect.AutoSize(ButtonsTexturePosition.ButtonNormal));
		buyButton.SetTexture(UIButtonBase.State.Pressed, material, ButtonsTexturePosition.ButtonPressed, AutoRect.AutoSize(ButtonsTexturePosition.ButtonPressed));
		buyButton.Rect = AutoRect.AutoPos(uiPos.BuyButton);
		buyButton.SetText("font0", " SELECT", ColorName.fontColor_orange);
		SetBuyButtonText();
		cashPanel = new CashPanel();
		getMoreMoneyButton = new UITextButton();
		getMoreMoneyButton.SetTexture(UIButtonBase.State.Normal, arenaMenuMaterial, ArenaMenuTexturePosition.GetMoneyButtonNormal, AutoRect.AutoSize(ArenaMenuTexturePosition.GetMoneyButtonSmallSize));
		getMoreMoneyButton.SetTexture(UIButtonBase.State.Pressed, arenaMenuMaterial, ArenaMenuTexturePosition.GetMoneyButtonPressed, AutoRect.AutoSize(ArenaMenuTexturePosition.GetMoneyButtonSmallSize));
		getMoreMoneyButton.Rect = AutoRect.AutoPos(uiPos.GetMoreMoneyButton);
		avatarInfoPanel = new AvatarInfoPanel();
		avatarInfoPanel.SetText("ffff");
		Add(background);
		Add(buyButton);
		avatarScroller = new UIImageScroller(AutoRect.AutoPos(new Rect(450f, 0f, 500f, 640f)), AutoRect.AutoPos(new Rect(442f, 216f, 500f, 400f)), 1, AutoRect.AutoSize(AvatarTexturePosition.AvatarLogoSize), ScrollerDir.Vertical, true);
		avatarScroller.SetImageSpacing(AutoRect.AutoSize(AvatarTexturePosition.AvatarLogoSpacing));
		Material material2 = UIResourceMgr.GetInstance().GetMaterial("ShopUI");
		avatarScroller.AddOverlay(material2, ShopTexturePosition.SmallBuyLogo);
		for (int i = 0; i < 12; i++)
		{
			switch (i)
			{
			case 11:
			{
				UIImage uIImage5 = new UIImage();
				uIImage5.SetTexture(avatarLogoMaterial, AvatarTexturePosition.AvatarLogo[0]);
				avatarScroller.Add(uIImage5);
				break;
			}
			case 0:
			{
				UIImage uIImage4 = new UIImage();
				uIImage4.SetTexture(avatarLogoMaterial2, AvatarTexturePosition.AvatarEskimo);
				avatarScroller.Add(uIImage4);
				break;
			}
			case 1:
			{
				UIImage uIImage3 = new UIImage();
				uIImage3.SetTexture(avatarLogoMaterial1, AvatarTexturePosition.AvatarPostor);
				avatarScroller.Add(uIImage3);
				break;
			}
			case 2:
			{
				UIImage uIImage2 = new UIImage();
				uIImage2.SetTexture(avatarLogoMaterial1, AvatarTexturePosition.AvatarNinja);
				avatarScroller.Add(uIImage2);
				break;
			}
			default:
			{
				UIImage uIImage = new UIImage();
				uIImage.SetTexture(avatarLogoMaterial, AvatarTexturePosition.AvatarLogo[11 - i]);
				avatarScroller.Add(uIImage);
				break;
			}
			}
		}
		avatarScroller.SetMaskImage(avatarLogoMaterial, AvatarTexturePosition.Mask);
		Add(returnButton);
		avatarScroller.SetCenterFrameTexture(avatarLogoMaterial, AvatarTexturePosition.Frame);
		avatarScroller.EnableScroll();
		Add(avatarScroller);
		Add(avatarInfoPanel);
		Add(cashPanel);
		UpdateAvatarIcon();
		avatarScroller.Show();
		avatarInfoPanel.Show();
		if (AutoRect.GetPlatform() == Platform.IPad)
		{
			avatarFrame = new Avatar3DFrame(AutoRect.AutoPos(new Rect(0f, 10f, 400f, 600f)), new Vector3(-1.2717624f, -1.0505477f, 4.420711f), new Vector3(1.5f, 1.5f, 1.5f) * 0.9f);
		}
		else
		{
			avatarFrame = new Avatar3DFrame(AutoRect.AutoPos(new Rect(0f, 10f, 400f, 600f)), new Vector3(-1.589703f, -1.1672753f, 4.420711f), new Vector3(1.5f, 1.5f, 1.5f));
		}
		Add(avatarFrame);
		Add(getMoreMoneyButton);
		UpdateCashPanel();
		iapDialog = new IAPDialog(UIDialog.DialogMode.YES_OR_NO);
		iapDialog.SetDialogEventHandler(this);
		Add(iapDialog);
		SetUIHandler(this);
		Hide();
	}

	public override void UpdateLogic()
	{
		if (avatarFrame != null)
		{
			avatarFrame.UpdateAnimation();
		}
	}

	public void SetBuyButtonText()
	{
		if (GameApp.GetInstance().GetGameState().GetAvatarData((AvatarType)(11 - currentSelectionIndex)) == AvatarState.Avaliable)
		{
			buyButton.SetText(" SELECT");
		}
		else
		{
			buyButton.SetText(" BUY");
		}
	}

	public override void Hide()
	{
		avatarFrame.Hide();
		iapDialog.Hide();
		cashPanel.Hide();
		base.Hide();
	}

	public override void Show()
	{
		cashPanel.SetCostPanelPosition(new Rect(650f, 260f, 314f, 60f));
		avatarFrame.ChangeAvatar(GameApp.GetInstance().GetGameState().Avatar);
		avatarScroller.SetSelection((int)(11 - GameApp.GetInstance().GetGameState().Avatar));
		currentSelectionIndex = (int)(11 - GameApp.GetInstance().GetGameState().Avatar);
		buyButton.SetText(" SELECT");
		avatarInfoPanel.SetText(AvatarInfo.AVATAR_INFO[(int)GameApp.GetInstance().GetGameState().Avatar].ToUpperInvariant());
		UpdateCashPanel();
		avatarFrame.Show();
		cashPanel.Show();
		base.Show();
	}

	public void UpdateAvatarIcon()
	{
		for (int i = 0; i < 12; i++)
		{
			if (GameApp.GetInstance().GetGameState().GetAvatarData((AvatarType)(11 - i)) == AvatarState.ToBuy)
			{
				avatarScroller.SetOverlay(i, 0);
			}
			else if (GameApp.GetInstance().GetGameState().GetAvatarData((AvatarType)(11 - i)) == AvatarState.Avaliable)
			{
				avatarScroller.SetOverlay(i, -1);
			}
		}
	}

	public void ChangeAvatarModel(int index)
	{
		avatarFrame.ChangeAvatar((AvatarType)index);
	}

	public void UpdateCashPanel()
	{
		if (GameApp.GetInstance().GetGameState().GetAvatarData((AvatarType)(11 - currentSelectionIndex)) == AvatarState.Avaliable)
		{
			cashPanel.DisableCost();
		}
		else
		{
			GameConfig gameConfig = GameApp.GetInstance().GetGameConfig();
			cashPanel.SetCost(gameConfig.GetAvatarConfig(11 - currentSelectionIndex).price);
		}
		cashPanel.SetCash(GameApp.GetInstance().GetGameState().GetCash());
	}

	public void HandleEvent(UIControl control, int command, float wparam, float lparam)
	{
		if (control == returnButton)
		{
			AudioPlayer.PlayAudio(ArenaMenuUI.GetInstance().GetComponent<AudioSource>(), true);
			Hide();
			ArenaMenuUI component = GameObject.Find("ArenaMenuUI").GetComponent<ArenaMenuUI>();
			component.GetPanel(0).Show();
			GameApp.GetInstance().Save();
		}
		if (control == avatarScroller && command == 0)
		{
			currentSelectionIndex = (int)wparam;
			SetBuyButtonText();
			ChangeAvatarModel(11 - currentSelectionIndex);
			avatarInfoPanel.SetText(AvatarInfo.AVATAR_INFO[11 - currentSelectionIndex].ToUpperInvariant());
			UpdateCashPanel();
		}
		if (control == buyButton)
		{
			AudioPlayer.PlayAudio(ArenaMenuUI.GetInstance().GetComponent<AudioSource>(), true);
			if (GameApp.GetInstance().GetGameState().GetAvatarData((AvatarType)(11 - currentSelectionIndex)) == AvatarState.ToBuy)
			{
				GameConfig gameConfig = GameApp.GetInstance().GetGameConfig();
				if (GameApp.GetInstance().GetGameState().BuyAvatar((AvatarType)(11 - currentSelectionIndex), gameConfig.GetAvatarConfig(11 - currentSelectionIndex).price))
				{
					SetBuyButtonText();
				}
				else
				{
					TopDialogUI.GetInstance().ShowDialog();
				}
				UpdateAvatarIcon();
				UpdateCashPanel();
			}
			else if (GameApp.GetInstance().GetGameState().GetAvatarData((AvatarType)(11 - currentSelectionIndex)) == AvatarState.Avaliable)
			{
				GameApp.GetInstance().GetGameState().Avatar = (AvatarType)(11 - currentSelectionIndex);
			}
		}
		else if (control == getMoreMoneyButton)
		{
			AudioPlayer.PlayAudio(ArenaMenuUI.GetInstance().GetComponent<AudioSource>(), true);
			Hide();
			ShopUI shopUI = GameObject.Find("ArenaMenuUI").GetComponent<ArenaMenuUI>().GetPanel(4) as ShopUI;
			shopUI.SetFromPanel(this);
			shopUI.Show();
		}
	}

	public void Yes()
	{
		Hide();
		ShopUI shopUI = ArenaMenuUI.GetInstance().GetPanel(4) as ShopUI;
		shopUI.SetFromPanel(this);
		shopUI.Show();
	}

	public void No()
	{
		iapDialog.Hide();
	}
}
