using UnityEngine;
using Zombie3D;

public class EndlessUIPanel : UIPanel, UIHandler, UIDialogEventHandler
{
	protected UIImage back_img;

	protected UITextImage EndlessPanel;

	protected UIClickButton return_Button;

	protected UITextButton go_Button;

	protected UIImageScroller endless_Scroller;

	protected EndlessUIPos uiPos;

	protected CashPanel cashPanel;

	protected FirEndlessDialog first_endless_panel;

	protected UIImage mask_img;

	protected UIText WelcomText;

	protected int cur_endless_stage;

	protected Timer fadeTimer = new Timer();

	public void Start()
	{
		uiPos = new EndlessUIPos();
		GameApp.GetInstance().InitForMenu();
		Material material = UIResourceMgr.GetInstance().GetMaterial("ArenaMenu");
		Material material2 = UIResourceMgr.GetInstance().GetMaterial("ShopUI");
		back_img = new UIImage();
		back_img.SetTexture(material, ArenaMenuTexturePosition.Background, AutoRect.AutoSize(ArenaMenuTexturePosition.Background));
		back_img.Rect = AutoRect.AutoPos(uiPos.Background);
		EndlessPanel = new UITextImage();
		EndlessPanel.SetTexture(material2, ShopTexturePosition.DayLargePanel, AutoRect.AutoSize(ShopTexturePosition.DayLargePanel));
		EndlessPanel.Rect = AutoRect.AutoPos(uiPos.EndlessPanel);
		EndlessPanel.SetText("font2", "PRESSURE LAB", ColorName.fontColor_darkred);
		return_Button = new UIClickButton();
		return_Button.SetTexture(UIButtonBase.State.Normal, material, ArenaMenuTexturePosition.ReturnButtonNormal, AutoRect.AutoSize(ArenaMenuTexturePosition.ReturnButtonNormal));
		return_Button.SetTexture(UIButtonBase.State.Pressed, material, ArenaMenuTexturePosition.ReturnButtonPressed, AutoRect.AutoSize(ArenaMenuTexturePosition.ReturnButtonPressed));
		return_Button.Rect = AutoRect.AutoPos(uiPos.ReturnButton);
		Material material3 = UIResourceMgr.GetInstance().GetMaterial("Buttons");
		go_Button = new UITextButton();
		go_Button.SetTexture(UIButtonBase.State.Normal, material3, ButtonsTexturePosition.ButtonNormal, AutoRect.AutoSize(ButtonsTexturePosition.ButtonNormal));
		go_Button.SetTexture(UIButtonBase.State.Pressed, material3, ButtonsTexturePosition.ButtonPressed, AutoRect.AutoSize(ButtonsTexturePosition.ButtonPressed));
		go_Button.Rect = AutoRect.AutoPos(uiPos.GoButton);
		go_Button.SetText("font0", " OK", ColorName.fontColor_orange);
		Add(back_img);
		Add(EndlessPanel);
		Add(return_Button);
		Add(go_Button);
		cashPanel = new CashPanel();
		Add(cashPanel);
		cashPanel.SetCash(GameApp.GetInstance().GetGameState().GetCash());
		cashPanel.Show();
		SetUIHandler(this);
		Material material4 = UIResourceMgr.GetInstance().GetMaterial("EndlessUI");
		Material material5 = UIResourceMgr.GetInstance().GetMaterial("EndlessUI1");
		Material material6 = UIResourceMgr.GetInstance().GetMaterial("EndlessUI2");
		endless_Scroller = new UIImageScroller(AutoRect.AutoPos(new Rect(0f, 100f, 960f, 540f)), AutoRect.AutoPos(new Rect(151f, 95f, 700f, 500f)), 1, AutoRect.AutoSize(EndlessTexturePosition.GetEndMissionRect(1)), ScrollerDir.Vertical, true);
		endless_Scroller.SetImageSpacing(AutoRect.AutoSize(WeaponsLogoTexturePosition.WeaponLogoSpacing));
		endless_Scroller.SetCenterFrameTexture(material5, EndlessTexturePosition.EndMissionBacK);
		for (int i = 0; i < 7; i++)
		{
			UIImage uIImage = new UIImage();
			switch (i)
			{
			case 6:
				uIImage.SetTexture(material6, EndlessTexturePosition.GetEndMissionRect(1), AutoRect.AutoSize(EndlessTexturePosition.GetEndMissionRect(1)));
				break;
			case 7:
				uIImage.SetTexture(material6, EndlessTexturePosition.GetEndMissionRect(2), AutoRect.AutoSize(EndlessTexturePosition.GetEndMissionRect(2)));
				break;
			default:
				if (i + 1 > 4)
				{
					uIImage.SetTexture(material5, EndlessTexturePosition.GetEndMissionRect(i + 1), AutoRect.AutoSize(EndlessTexturePosition.GetEndMissionRect(i + 1)));
				}
				else
				{
					uIImage.SetTexture(material4, EndlessTexturePosition.GetEndMissionRect(i + 1), AutoRect.AutoSize(EndlessTexturePosition.GetEndMissionRect(i + 1)));
				}
				break;
			}
			uIImage.Rect = EndlessTexturePosition.GetEndMissionRect(i + 1);
			endless_Scroller.Add(uIImage);
		}
		Add(endless_Scroller);
		endless_Scroller.EnableScroll();
		endless_Scroller.SetMaskImage(material5, EndlessTexturePosition.EndMissionMask);
		endless_Scroller.Show();
		if (GameApp.GetInstance().GetGameState().survival_welcome_view == 1)
		{
			mask_img = new UIImage();
			mask_img.SetTexture(material5, EndlessTexturePosition.EndMissionMask, AutoRect.AutoSize(EndlessTexturePosition.EndMissionMask));
			mask_img.Rect = AutoRect.AutoPos(uiPos.Background);
			mask_img.SetTextureSize(AutoRect.AutoTex(new Vector2(960f, 640f)));
			Add(mask_img);
			first_endless_panel = new FirEndlessDialog(UIDialog.DialogMode.YES_OR_NO);
			first_endless_panel.SetText("font2", "\n\n\nSTOCK UP ON AMMO, THEN CHOOSE A LEVEL\n AND PUNISH THE ZOMBIES FOR THEIR SINS!", ColorName.fontColor_red);
			first_endless_panel.SetDialogEventHandler(this);
			Add(first_endless_panel);
			first_endless_panel.Show();
			WelcomText = new UIText();
			WelcomText.Set("font1", "WELCOME TO THE PRESSURE LAB...", ColorName.fontColor_darkorange);
			WelcomText.AlignStyle = UIText.enAlignStyle.center;
			WelcomText.Rect = AutoRect.AutoPos(uiPos.WelcomeText);
			Add(WelcomText);
			GameApp.GetInstance().GetGameState().survival_welcome_view = 0;
		}
	}

	public override void Update()
	{
		if (!fadeTimer.Ready())
		{
			return;
		}
		if (fadeTimer.Name == "Back")
		{
			UIResourceMgr.GetInstance().UnloadAllUIMaterials();
			GameApp.GetInstance().GetGameState().FromShopMenu = true;
			SceneName.LoadLevel("EndlessModeTUI");
		}
		else
		{
			GameApp.GetInstance().GetGameState().endless_level = true;
			GameApp.GetInstance().GetGameState().endless_multiplayer = false;
			GameApp.GetInstance().GetGameState().VS_mode = false;
			if (fadeTimer.Name == "0")
			{
				SceneName.LoadLevel("Zombie3D_Village");
			}
			else if (fadeTimer.Name == "1")
			{
				SceneName.LoadLevel("Zombie3D_Arena");
			}
			else if (fadeTimer.Name == "2")
			{
				SceneName.LoadLevel("Zombie3D_Hospital");
			}
			else if (fadeTimer.Name == "3")
			{
				SceneName.LoadLevel("Zombie3D_ParkingLot");
			}
			else if (fadeTimer.Name == "4")
			{
				SceneName.LoadLevel("Zombie3D_Church");
			}
			else if (fadeTimer.Name == "5")
			{
				string scene = "Zombie3D_Village2";
				SceneName.LoadLevel(scene);
			}
			else if (fadeTimer.Name == "6")
			{
				SceneName.LoadLevel("Zombie3D_Recycle");
			}
		}
		fadeTimer.Do();
	}

	public void HandleEvent(UIControl control, int command, float wparam, float lparam)
	{
		if (control == return_Button)
		{
			FadeAnimationScript.GetInstance().FadeInBlack(0.3f);
			fadeTimer.Name = "Back";
			fadeTimer.SetTimer(0.3f, false);
		}
		else if (control == go_Button)
		{
			switch (cur_endless_stage)
			{
			case 0:
				FadeAnimationScript.GetInstance().FadeInBlack(0.3f);
				fadeTimer.Name = "0";
				fadeTimer.SetTimer(0.3f, false);
				break;
			case 1:
				FadeAnimationScript.GetInstance().FadeInBlack(0.3f);
				fadeTimer.Name = "1";
				fadeTimer.SetTimer(0.3f, false);
				break;
			case 2:
				FadeAnimationScript.GetInstance().FadeInBlack(0.3f);
				fadeTimer.Name = "2";
				fadeTimer.SetTimer(0.3f, false);
				break;
			case 3:
				FadeAnimationScript.GetInstance().FadeInBlack(0.3f);
				fadeTimer.Name = "3";
				fadeTimer.SetTimer(0.3f, false);
				break;
			case 4:
				FadeAnimationScript.GetInstance().FadeInBlack(0.3f);
				fadeTimer.Name = "4";
				fadeTimer.SetTimer(0.3f, false);
				break;
			case 5:
				FadeAnimationScript.GetInstance().FadeInBlack(0.3f);
				fadeTimer.Name = "5";
				fadeTimer.SetTimer(0.3f, false);
				break;
			case 6:
				FadeAnimationScript.GetInstance().FadeInBlack(0.3f);
				fadeTimer.Name = "6";
				fadeTimer.SetTimer(0.3f, false);
				break;
			}
		}
		else if (control == endless_Scroller && command == 0)
		{
			cur_endless_stage = (int)wparam;
			if (cur_endless_stage >= 0)
			{
				Debug.Log("*************current endless level = " + cur_endless_stage);
			}
		}
	}

	public override void Hide()
	{
		base.Hide();
	}

	public override void Show()
	{
		base.Show();
	}

	public void Yes()
	{
		mask_img.Visible = false;
		mask_img.Enable = false;
		first_endless_panel.Hide();
		WelcomText.Visible = false;
		WelcomText.Enable = false;
	}

	public void No()
	{
	}
}
