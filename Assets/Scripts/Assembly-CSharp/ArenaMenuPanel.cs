using UnityEngine;
using Zombie3D;

public class ArenaMenuPanel : UIPanel, UIHandler
{
	protected UIImage background;

	protected UITextImage avatarPanel;

	protected UITextImage daysPanel;

	protected CashPanel cashPanel;

	protected UITextButton upgradeButton;

	protected UITextButton equipmentButton;

	protected UITextButton battleButton;

	protected UITextButton avatarButton;

	protected UIClickButton returnButton;

	protected UIClickButton optionsButton;

	protected UIClickButton leaderButton;

	protected UIClickButton achieveButton;

	protected Avatar3DFrame avatar3DFrame;

	private ArenaMenuUIPosition uiPos;

	protected ArenaMenuUI ui;

	protected ReviewDialog reviewDialog;

	protected Timer fadeTimer = new Timer();

	protected float startTime;

	public bool BattlePressed { get; set; }

	public ArenaMenuPanel()
	{
		uiPos = new ArenaMenuUIPosition();
		BattlePressed = false;
		Material material = UIResourceMgr.GetInstance().GetMaterial("ArenaMenu");
		Material material2 = UIResourceMgr.GetInstance().GetMaterial("Buttons");
		background = new UIImage();
		background.SetTexture(material, ArenaMenuTexturePosition.Background, AutoRect.AutoSize(ArenaMenuTexturePosition.Background));
		background.Rect = AutoRect.AutoPos(uiPos.Background);
		Material material3 = UIResourceMgr.GetInstance().GetMaterial("ShopUI");
		daysPanel = new UITextImage();
		daysPanel.SetTexture(material3, ShopTexturePosition.DayLargePanel, AutoRect.AutoSize(ShopTexturePosition.DayLargePanel));
		daysPanel.Rect = AutoRect.AutoPos(uiPos.DaysPanel);
		daysPanel.SetText("font0", "DAY " + GameApp.GetInstance().GetGameState().LevelNum, ColorName.fontColor_darkred);
		cashPanel = new CashPanel();
		upgradeButton = new UITextButton();
		upgradeButton.SetTexture(UIButtonBase.State.Normal, material2, ButtonsTexturePosition.ButtonNormal, AutoRect.AutoSize(ButtonsTexturePosition.MiddleSizeButton));
		upgradeButton.SetTexture(UIButtonBase.State.Pressed, material2, ButtonsTexturePosition.ButtonPressed, AutoRect.AutoSize(ButtonsTexturePosition.MiddleSizeButton));
		upgradeButton.Rect = AutoRect.AutoPos(uiPos.UpgradeButton);
		upgradeButton.SetText("font1", " ARMORY", ColorName.fontColor_orange);
		equipmentButton = new UITextButton();
		equipmentButton.SetTexture(UIButtonBase.State.Normal, material2, ButtonsTexturePosition.ButtonNormal, AutoRect.AutoSize(ButtonsTexturePosition.MiddleSizeButton));
		equipmentButton.SetTexture(UIButtonBase.State.Pressed, material2, ButtonsTexturePosition.ButtonPressed, AutoRect.AutoSize(ButtonsTexturePosition.MiddleSizeButton));
		equipmentButton.Rect = AutoRect.AutoPos(uiPos.EquipmentButton);
		equipmentButton.SetText("font1", " EQUIP", ColorName.fontColor_orange);
		avatarButton = new UITextButton();
		avatarButton.SetTexture(UIButtonBase.State.Normal, material2, ButtonsTexturePosition.ButtonNormal, AutoRect.AutoSize(ButtonsTexturePosition.MiddleSizeButton));
		avatarButton.SetTexture(UIButtonBase.State.Pressed, material2, ButtonsTexturePosition.ButtonPressed, AutoRect.AutoSize(ButtonsTexturePosition.MiddleSizeButton));
		avatarButton.Rect = AutoRect.AutoPos(uiPos.AvatarButton);
		avatarButton.SetText("font1", " CHARACTER", ColorName.fontColor_orange);
		battleButton = new UITextButton();
		battleButton.SetTexture(UIButtonBase.State.Normal, material3, ShopTexturePosition.MapButtonNormal, AutoRect.AutoSize(ShopTexturePosition.MapButtonNormal));
		battleButton.SetTexture(UIButtonBase.State.Pressed, material3, ShopTexturePosition.MapButtonPressed, AutoRect.AutoSize(ShopTexturePosition.MapButtonPressed));
		battleButton.Rect = AutoRect.AutoPos(uiPos.BattleButton);
		battleButton.SetText("font0", " MAP", ColorName.fontColor_map);
		returnButton = new UIClickButton();
		returnButton.SetTexture(UIButtonBase.State.Normal, material, ArenaMenuTexturePosition.ReturnButtonNormal, AutoRect.AutoSize(ArenaMenuTexturePosition.ReturnButtonNormal));
		returnButton.SetTexture(UIButtonBase.State.Pressed, material, ArenaMenuTexturePosition.ReturnButtonPressed, AutoRect.AutoSize(ArenaMenuTexturePosition.ReturnButtonPressed));
		returnButton.Rect = AutoRect.AutoPos(uiPos.ReturnButton);
		optionsButton = new UIClickButton();
		optionsButton.SetTexture(UIButtonBase.State.Normal, material, ArenaMenuTexturePosition.OptionsButton, AutoRect.AutoSize(ArenaMenuTexturePosition.OptionsButton));
		optionsButton.SetTexture(UIButtonBase.State.Pressed, material, ArenaMenuTexturePosition.OptionsButtonPressed, AutoRect.AutoSize(ArenaMenuTexturePosition.OptionsButtonPressed));
		optionsButton.Rect = AutoRect.AutoPos(uiPos.OptionsButton);
		Material material4 = UIResourceMgr.GetInstance().GetMaterial("StartMenu");
		leaderButton = new UITextButton();
		leaderButton.SetTexture(UIButtonBase.State.Normal, material4, StartMenuTexturePosition.LeaderBoardsButtonNormal, AutoRect.AutoSize(StartMenuTexturePosition.LeaderBoardsButtonNormal));
		leaderButton.SetTexture(UIButtonBase.State.Pressed, material4, StartMenuTexturePosition.LeaderBoardsButtonPressed, AutoRect.AutoSize(StartMenuTexturePosition.LeaderBoardsButtonPressed));
		leaderButton.Rect = AutoRect.AutoPos(uiPos.LeaderBoardButton);
		achieveButton = new UITextButton();
		achieveButton.SetTexture(UIButtonBase.State.Normal, material4, StartMenuTexturePosition.AcheivementButtonNormal, AutoRect.AutoSize(StartMenuTexturePosition.AcheivementButtonNormal));
		achieveButton.SetTexture(UIButtonBase.State.Pressed, material4, StartMenuTexturePosition.AcheivementButtonPressed, AutoRect.AutoSize(StartMenuTexturePosition.AcheivementButtonPressed));
		achieveButton.Rect = AutoRect.AutoPos(uiPos.AchievementButton);
		if (AutoRect.GetPlatform() == Platform.IPad)
		{
			avatar3DFrame = new Avatar3DFrame(AutoRect.AutoPos(new Rect(0f, 10f, 500f, 600f)), new Vector3(-1.1698182f, -0.9672753f, 3.420711f), new Vector3(1.5f, 1.5f, 1.5f) * 0.9f);
		}
		else
		{
			avatar3DFrame = new Avatar3DFrame(AutoRect.AutoPos(new Rect(0f, 10f, 500f, 600f)), new Vector3(-1.299798f, -1.0672753f, 3.420711f), new Vector3(1.5f, 1.5f, 1.5f));
		}
		ui = GameObject.Find("ArenaMenuUI").GetComponent<ArenaMenuUI>();
		Add(background);
		Add(daysPanel);
		Add(cashPanel);
		Add(upgradeButton);
		Add(equipmentButton);
		Add(battleButton);
		Add(avatarButton);
		Add(leaderButton);
		Add(achieveButton);
		Add(avatar3DFrame);
		SetUIHandler(this);
		startTime = Time.time;
	}

	public override void Hide()
	{
		base.Hide();
		avatar3DFrame.Hide();
		cashPanel.Hide();
	}

	public override void Show()
	{
		base.Show();
		avatar3DFrame.ChangeAvatar(GameApp.GetInstance().GetGameState().Avatar);
		cashPanel.SetCash(GameApp.GetInstance().GetGameState().GetCash());
		cashPanel.Show();
		avatar3DFrame.Show();
	}

	public override void Update()
	{
		if (Time.time - startTime > 1.5f)
		{
			if (!GameApp.GetInstance().GetGameState().AlreadyCountered)
			{
				GameApp.GetInstance().GetGameState().AddScore(1);
				GameApp.GetInstance().GetGameState().review_count++;
				GameApp.GetInstance().GetGameState().AlreadyCountered = true;
			}
			if (!GameApp.GetInstance().GetGameState().AlreadyPopReview && GameApp.GetInstance().GetGameState().review_count == 3)
			{
				ReviewDialogUI.GetInstance().ShowDialog();
				GameApp.GetInstance().GetGameState().AlreadyPopReview = true;
			}
		}
		if (avatar3DFrame != null)
		{
			avatar3DFrame.UpdateAnimation();
		}
		if (fadeTimer.Ready())
		{
			if (fadeTimer.Name == "StartMenu")
			{
				UIResourceMgr.GetInstance().UnloadAllUIMaterials();
				SceneName.LoadLevel("StartMenuUI");
			}
			else
			{
				UIResourceMgr.GetInstance().UnloadAllUIMaterials();
				GameApp.GetInstance().GetGameState().FromShopMenu = true;
				SceneName.LoadLevel("MapUI");
			}
			fadeTimer.Do();
		}
	}

	public void HandleEvent(UIControl control, int command, float wparam, float lparam)
	{
		if (BattlePressed)
		{
			return;
		}
		if (control == upgradeButton)
		{
			ArenaMenuUI.GetInstance().GetAudioPlayer().PlayAudio("Button");
			Hide();
			ui.GetPanel(1).Show();
		}
		else if (control == equipmentButton)
		{
			ArenaMenuUI.GetInstance().GetAudioPlayer().PlayAudio("Button");
			Hide();
			ui.GetPanel(2).Show();
		}
		else if (control == battleButton)
		{
			BattlePressed = true;
			FadeAnimationScript.GetInstance().FadeInBlack(0.3f);
			fadeTimer.Name = "Continue";
			fadeTimer.SetTimer(0.3f, false);
			GameApp.GetInstance().Save();
		}
		else if (control == avatarButton)
		{
			ArenaMenuUI.GetInstance().GetAudioPlayer().PlayAudio("Button");
			Hide();
			ui.GetPanel(3).Show();
		}
		else if (control != optionsButton)
		{
			if (control == returnButton)
			{
				ArenaMenuUI.GetInstance().GetAudioPlayer().PlayAudio("Button");
				FadeAnimationScript.GetInstance().FadeInBlack();
				fadeTimer.Name = "StartMenu";
				fadeTimer.SetTimer(2f, false);
			}
			else if (control == leaderButton)
			{
				ArenaMenuUI.GetInstance().GetAudioPlayer().PlayAudio("Button");
				GameCenterPlugin.OpenLeaderboard();
			}
			else if (control == achieveButton)
			{
				ArenaMenuUI.GetInstance().GetAudioPlayer().PlayAudio("Button");
				GameCenterPlugin.OpenAchievement();
			}
		}
	}
}
