using System;
using UnityEngine;
using Zombie3D;

public class GameOverUI : UIPanel, UIHandler
{
	protected Rect[] buttonRect;

	protected Material gameuiMaterial;

	protected UIImage dialogImage;

	protected UITextButton retryButton;

	protected UITextButton quitButton;

	protected UITextButton endless_ok_Button;

	protected UIText newEquipmentText;

	protected UIText returnText;

	protected UIText surviveTimeText;

	protected UIText firstLineText;

	protected UIText scoreText;

	protected UIText cashText_title;

	protected UIText endless_kill_count_title;

	protected UIText endless_time_title;

	protected UIText cashText;

	protected UIText endless_kill_count;

	protected UIText endless_time;

	protected UIImage endless_mask;

	protected UIImage gameover;

	protected UIImage mask;

	private GameOverUIPosition uiPos;

	private GameOverTexturePosition texPos;

	protected float screenRatioX;

	protected float screenRatioY;

	protected bool uiInited;

	protected GameState gameState;

	protected Weapon selectedWeapon;

	protected float startTime;

	public GameOverUI()
	{
		uiPos = new GameOverUIPosition();
		texPos = new GameOverTexturePosition();
		gameState = GameApp.GetInstance().GetGameState();
		selectedWeapon = gameState.GetWeapons()[0];
		gameuiMaterial = UIResourceMgr.GetInstance().GetMaterial("GameUI");
		Material material = UIResourceMgr.GetInstance().GetMaterial("Buttons");
		retryButton = new UITextButton();
		retryButton.Rect = AutoRect.AutoPos(uiPos.RetryButton);
		retryButton.SetTexture(UIButtonBase.State.Normal, material, ButtonsTexturePosition.ButtonNormal, AutoRect.AutoSize(ButtonsTexturePosition.ButtonNormal));
		retryButton.SetTexture(UIButtonBase.State.Pressed, material, ButtonsTexturePosition.ButtonPressed, AutoRect.AutoSize(ButtonsTexturePosition.ButtonPressed));
		retryButton.SetText("font1", "RETRY", ColorName.fontColor_orange);
		quitButton = new UITextButton();
		quitButton.Rect = AutoRect.AutoPos(uiPos.QuitButton);
		quitButton.SetTexture(UIButtonBase.State.Normal, material, ButtonsTexturePosition.ButtonNormal, AutoRect.AutoSize(ButtonsTexturePosition.ButtonNormal));
		quitButton.SetTexture(UIButtonBase.State.Pressed, material, ButtonsTexturePosition.ButtonPressed, AutoRect.AutoSize(ButtonsTexturePosition.ButtonPressed));
		quitButton.SetText("font1", "QUIT", ColorName.fontColor_orange);
		endless_ok_Button = new UITextButton();
		endless_ok_Button.Rect = AutoRect.AutoPos(uiPos.OKButton);
		endless_ok_Button.SetTexture(UIButtonBase.State.Normal, material, ButtonsTexturePosition.ButtonNormal, AutoRect.AutoSize(ButtonsTexturePosition.ButtonNormal));
		endless_ok_Button.SetTexture(UIButtonBase.State.Pressed, material, ButtonsTexturePosition.ButtonPressed, AutoRect.AutoSize(ButtonsTexturePosition.ButtonPressed));
		endless_ok_Button.SetText("font1", "OK", ColorName.fontColor_orange);
		dialogImage = new UIImage();
		dialogImage.SetTexture(gameuiMaterial, GameUITexturePosition.Dialog, AutoRect.AutoSize(GameUITexturePosition.Dialog));
		dialogImage.Rect = AutoRect.AutoPos(uiPos.DialogImage);
		mask = new UIImage();
		mask.SetTexture(gameuiMaterial, GameUITexturePosition.Mask, AutoRect.AutoSize(uiPos.Mask));
		mask.Rect = AutoRect.AutoValuePos(uiPos.Mask);
		Add(mask);
		Material material2 = UIResourceMgr.GetInstance().GetMaterial("FirstEndless");
		endless_mask = new UIImage();
		endless_mask.SetTexture(material2, FirstEndlessTexturePosition.Board, AutoRect.AutoSize(FirstEndlessTexturePosition.Board_small));
		endless_mask.Rect = AutoRect.AutoPos(uiPos.Endless_mask);
		Add(endless_mask);
		cashText_title = new UIText();
		cashText_title.Set("font1", "MONEY GAINED:", ColorName.fontColor_orange);
		cashText_title.AlignStyle = UIText.enAlignStyle.left;
		cashText_title.Rect = AutoRect.AutoPos(new Rect(220f, 246f, 960f, 87f));
		Add(cashText_title);
		endless_kill_count_title = new UIText();
		endless_kill_count_title.Set("font1", "ZOMBIES KILLED:", ColorName.fontColor_orange);
		endless_kill_count_title.AlignStyle = UIText.enAlignStyle.left;
		endless_kill_count_title.Rect = AutoRect.AutoPos(new Rect(220f, 306f, 960f, 87f));
		Add(endless_kill_count_title);
		endless_time_title = new UIText();
		endless_time_title.Set("font1", "SURVIVAL TIME:", ColorName.fontColor_orange);
		endless_time_title.AlignStyle = UIText.enAlignStyle.left;
		endless_time_title.Rect = AutoRect.AutoPos(new Rect(220f, 186f, 960f, 87f));
		Add(endless_time_title);
		cashText = new UIText();
		cashText.Set("font1", string.Empty, ColorName.fontColor_orange);
		cashText.AlignStyle = UIText.enAlignStyle.left;
		cashText.Rect = AutoRect.AutoPos(new Rect(560f, 246f, 200f, 87f));
		Add(cashText);
		endless_kill_count = new UIText();
		endless_kill_count.Set("font1", string.Empty, ColorName.fontColor_orange);
		endless_kill_count.AlignStyle = UIText.enAlignStyle.left;
		endless_kill_count.Rect = AutoRect.AutoPos(new Rect(560f, 306f, 200f, 87f));
		Add(endless_kill_count);
		endless_time = new UIText();
		endless_time.Set("font1", string.Empty, ColorName.fontColor_orange);
		endless_time.AlignStyle = UIText.enAlignStyle.left;
		endless_time.Rect = AutoRect.AutoPos(new Rect(560f, 186f, 200f, 87f));
		Add(endless_time);
		firstLineText = new UIText();
		firstLineText.Set("font1", "GAME OVER", ColorName.fontColor_darkorange);
		firstLineText.AlignStyle = UIText.enAlignStyle.center;
		firstLineText.Rect = AutoRect.AutoPos(uiPos.FirstLineText);
		gameover = new UIImage();
		gameover.SetTexture(gameuiMaterial, GameUITexturePosition.GameOver, AutoRect.AutoSize(GameUITexturePosition.GameOver) * 0.1f);
		gameover.Rect = AutoRect.AutoPos(uiPos.GameOver);
		gameover.Visible = false;
		gameover.Enable = false;
		retryButton.Visible = false;
		quitButton.Visible = false;
		Add(retryButton);
		Add(quitButton);
		Add(endless_ok_Button);
		Add(gameover);
		SetUIHandler(this);
		uiInited = true;
		GameScene gameScene = GameApp.GetInstance().GetGameScene();
		if (gameScene.GetQuest() != null)
		{
			surviveTimeText.SetText("SurviveTime " + gameScene.GetQuest().GetQuestInfo());
			firstLineText.SetText("Kills " + gameScene.Killed);
		}
	}

	public override void Show()
	{
		if (GameApp.GetInstance().GetGameState().endless_level && !GameApp.GetInstance().GetGameState().endless_multiplayer)
		{
			gameover.Rect = AutoRect.AutoPos(uiPos.GameOver_Endless);
			if (GameApp.GetInstance().GetGameScene().Killed > GameApp.GetInstance().GetGameState().endless_kill_max)
			{
				GameApp.GetInstance().GetGameState().endless_kill_max = GameApp.GetInstance().GetGameScene().Killed;
			}
			int num = (int)GameApp.GetInstance().GetGameScene().endless_get_cash;
			if (num > GameApp.GetInstance().GetGameState().endless_cash_max)
			{
				GameApp.GetInstance().GetGameState().endless_cash_max = num;
			}
			int num2 = (int)(Time.time - GameApp.GetInstance().GetGameScene().endless_start_time);
			if (num2 > GameApp.GetInstance().GetGameState().endless_time_max)
			{
				GameApp.GetInstance().GetGameState().endless_time_max = num2;
			}
			GameApp.GetInstance().GetGameState().Achievement.SubmitEndlessScore(num + GameApp.GetInstance().GetGameScene().Killed);
			GameApp.GetInstance().GetGameState().Achievement.CheckAchievemnet_Endless(num2);
			TimeSpan timeSpan = new TimeSpan(0, 0, num2);
			cashText.SetText(string.Empty + num);
			endless_kill_count.SetText(string.Empty + GameApp.GetInstance().GetGameScene().Killed);
			endless_time.SetText(string.Empty + timeSpan.ToString());
			retryButton.Enable = false;
			retryButton.Visible = false;
			quitButton.Enable = false;
			quitButton.Visible = false;
			endless_ok_Button.Enable = false;
			endless_ok_Button.Visible = false;
		}
		else
		{
			endless_mask.Visible = false;
			cashText.Visible = false;
			endless_kill_count.Visible = false;
			endless_time.Visible = false;
			retryButton.Enable = false;
			retryButton.Visible = false;
			quitButton.Enable = false;
			quitButton.Visible = false;
			endless_ok_Button.Enable = false;
			endless_ok_Button.Visible = false;
			cashText_title.Enable = false;
			cashText_title.Visible = false;
			endless_kill_count_title.Enable = false;
			endless_kill_count_title.Visible = false;
			endless_time_title.Enable = false;
			endless_time_title.Visible = false;
			if (!GameApp.GetInstance().GetGameState().endless_multiplayer)
			{
				GameApp.GetInstance().GetGameState().user_statistics.last_day_state = "Day_" + GameApp.GetInstance().GetGameState().LevelNum + "_Failed";
				GameApp.GetInstance().GetGameState().user_statistics.mission_failed_list.Add("Day_" + GameApp.GetInstance().GetGameState().LevelNum);
				GameApp.GetInstance().GetGameState().SaveSceneStatistics();
			}
		}
		startTime = Time.time;
		gameover.Visible = true;
		base.Show();
		OpenClickPlugin.Show(false);
	}

	public void DisplayBattleEndUI()
	{
	}

	public override void UpdateLogic()
	{
		if (!gameover.Visible)
		{
			return;
		}
		float num = 0.1f + (Time.time - startTime) * 0.9f;
		if (Time.time - startTime > 1.2f)
		{
			num = 1.1800001f - (Time.time - startTime - 1.2f) * 0.2f;
			if (num <= 1f)
			{
				num = 1f;
				GameUIScript component = GameObject.Find("SceneGUI").GetComponent<GameUIScript>();
				PauseMenuUI pauseMenuUI = component.GetPanel(0) as PauseMenuUI;
				if (pauseMenuUI.isEndlsessQuit)
				{
					endless_ok_Button.Enable = true;
					endless_ok_Button.Visible = true;
				}
				else if (!GameApp.GetInstance().GetGameState().endless_multiplayer)
				{
					retryButton.Enable = true;
					retryButton.Visible = true;
					quitButton.Enable = true;
					quitButton.Visible = true;
				}
			}
		}
		gameover.SetTexture(gameuiMaterial, GameUITexturePosition.GameOver, AutoRect.AutoSize(GameUITexturePosition.GameOver) * num);
	}

	public void HandleEvent(UIControl control, int command, float wparam, float lparam)
	{
		if (control == retryButton)
		{
			GameApp.GetInstance().Save();
			SceneName.LoadLevel(Application.loadedLevelName);
			OpenClickPlugin.Hide();
		}
		else if (control == quitButton || control == endless_ok_Button)
		{
			UIResourceMgr.GetInstance().UnloadAllUIMaterials();
			if (GameApp.GetInstance().GetGameState().endless_level)
			{
				SceneName.LoadLevel("NetMapTUI");
			}
			else
			{
				SceneName.LoadLevel("MainMapTUI");
			}
			GameApp.GetInstance().Save();
			OpenClickPlugin.Hide();
		}
	}
}
