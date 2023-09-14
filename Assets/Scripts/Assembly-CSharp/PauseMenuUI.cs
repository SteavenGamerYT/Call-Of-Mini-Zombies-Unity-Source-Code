using UnityEngine;
using Zombie3D;

public class PauseMenuUI : UIPanel, UIHandler, UIDialogEventHandler
{
	protected Rect[] buttonRect;

	public UIManager m_UIManager;

	public string m_ui_material_path;

	protected Material buttonsMaterial;

	protected Material gameuiMaterial;

	protected UIImage background;

	protected UITextImage pauseLabel;

	protected UITextImage musicLabel;

	protected UITextSelectButton musicButtonOff;

	protected UITextSelectButton musicButtonOn;

	protected UITextImage soundLabel;

	protected UITextSelectButton soundButtonOff;

	protected UITextSelectButton soundButtonOn;

	protected UITextButton resumeButton;

	protected UITextButton returnButton;

	protected UIImage mask;

	private PauseMenuUIPosition uiPos;

	protected float screenRatioX;

	protected float screenRatioY;

	protected GameState gameState;

	protected GameUIScript ui;

	protected GameDialog gameDialog;

	public bool isEndlsessQuit;

	public PauseMenuUI()
	{
		uiPos = new PauseMenuUIPosition();
		gameState = GameApp.GetInstance().GetGameState();
		buttonsMaterial = UIResourceMgr.GetInstance().GetMaterial("Buttons");
		gameuiMaterial = UIResourceMgr.GetInstance().GetMaterial("GameUI");
		background = new UIImage();
		background.SetTexture(gameuiMaterial, GameUITexturePosition.Dialog, AutoRect.AutoSize(GameUITexturePosition.DialogSize));
		background.Rect = AutoRect.AutoPos(uiPos.Background);
		pauseLabel = new UITextImage();
		pauseLabel.SetTexture(buttonsMaterial, ButtonsTexturePosition.Label, AutoRect.AutoSize(ButtonsTexturePosition.Label));
		pauseLabel.Rect = AutoRect.AutoPos(uiPos.PauseLabel);
		pauseLabel.SetText("font1", " PAUSE", ColorName.fontColor_orange);
		resumeButton = new UITextButton();
		resumeButton.SetTexture(UIButtonBase.State.Normal, buttonsMaterial, ButtonsTexturePosition.ButtonNormal, AutoRect.AutoSize(ButtonsTexturePosition.SmallSizeButton));
		resumeButton.SetTexture(UIButtonBase.State.Pressed, buttonsMaterial, ButtonsTexturePosition.ButtonPressed, AutoRect.AutoSize(ButtonsTexturePosition.SmallSizeButton));
		resumeButton.Rect = AutoRect.AutoPos(uiPos.ResumeButton);
		resumeButton.SetText("font1", " RESUME", ColorName.fontColor_orange);
		musicLabel = new UITextImage();
		musicLabel.SetTexture(buttonsMaterial, ButtonsTexturePosition.Label, AutoRect.AutoSize(ButtonsTexturePosition.Label));
		musicLabel.Rect = AutoRect.AutoPos(uiPos.MusicLabel);
		musicLabel.SetText("font1", " MUSIC", ColorName.fontColor_orange);
		musicButtonOff = new UITextSelectButton();
		musicButtonOff.SetTexture(UIButtonBase.State.Normal, buttonsMaterial, ButtonsTexturePosition.SoundButtonNormal, AutoRect.AutoSize(ButtonsTexturePosition.SoundButtonNormal_Small));
		musicButtonOff.SetTexture(UIButtonBase.State.Pressed, buttonsMaterial, ButtonsTexturePosition.SoundButtonPressed, AutoRect.AutoSize(ButtonsTexturePosition.SoundButtonPressed_Small));
		musicButtonOff.Rect = AutoRect.AutoPos(uiPos.MusicButtonOff);
		musicButtonOff.SetText("font2", " OFF", ColorName.fontColor_orange);
		musicButtonOn = new UITextSelectButton();
		musicButtonOn.SetTexture(UIButtonBase.State.Normal, buttonsMaterial, ButtonsTexturePosition.SoundButtonNormal, AutoRect.AutoSize(ButtonsTexturePosition.SoundButtonNormal_Small));
		musicButtonOn.SetTexture(UIButtonBase.State.Pressed, buttonsMaterial, ButtonsTexturePosition.SoundButtonPressed, AutoRect.AutoSize(ButtonsTexturePosition.SoundButtonPressed_Small));
		musicButtonOn.Rect = AutoRect.AutoPos(uiPos.MusicButtonOn);
		musicButtonOn.SetText("font2", " ON", ColorName.fontColor_orange);
		if (gameState.MusicOn)
		{
			musicButtonOn.Set(true);
			musicButtonOff.Set(false);
		}
		else
		{
			musicButtonOn.Set(false);
			musicButtonOff.Set(true);
		}
		soundLabel = new UITextImage();
		soundLabel.SetTexture(buttonsMaterial, ButtonsTexturePosition.Label, AutoRect.AutoSize(ButtonsTexturePosition.Label));
		soundLabel.Rect = AutoRect.AutoPos(uiPos.SoundLabel);
		soundLabel.SetText("font1", " SOUND", ColorName.fontColor_orange);
		soundButtonOff = new UITextSelectButton();
		soundButtonOff.SetTexture(UIButtonBase.State.Normal, buttonsMaterial, ButtonsTexturePosition.SoundButtonNormal, AutoRect.AutoSize(ButtonsTexturePosition.SoundButtonNormal_Small));
		soundButtonOff.SetTexture(UIButtonBase.State.Pressed, buttonsMaterial, ButtonsTexturePosition.SoundButtonPressed, AutoRect.AutoSize(ButtonsTexturePosition.SoundButtonPressed_Small));
		soundButtonOff.Rect = AutoRect.AutoPos(uiPos.SoundButtonOff);
		soundButtonOff.SetText("font2", " OFF", ColorName.fontColor_orange);
		soundButtonOn = new UITextSelectButton();
		soundButtonOn.SetTexture(UIButtonBase.State.Normal, buttonsMaterial, ButtonsTexturePosition.SoundButtonNormal, AutoRect.AutoSize(ButtonsTexturePosition.SoundButtonNormal_Small));
		soundButtonOn.SetTexture(UIButtonBase.State.Pressed, buttonsMaterial, ButtonsTexturePosition.SoundButtonPressed, AutoRect.AutoSize(ButtonsTexturePosition.SoundButtonPressed_Small));
		soundButtonOn.Rect = AutoRect.AutoPos(uiPos.SoundButtonOn);
		soundButtonOn.SetText("font2", " ON", ColorName.fontColor_orange);
		if (gameState.SoundOn)
		{
			soundButtonOn.Set(true);
			soundButtonOff.Set(false);
		}
		else
		{
			soundButtonOn.Set(false);
			soundButtonOff.Set(true);
		}
		returnButton = new UITextButton();
		returnButton.SetTexture(UIButtonBase.State.Normal, buttonsMaterial, ButtonsTexturePosition.ButtonNormal, AutoRect.AutoSize(ButtonsTexturePosition.SmallSizeButton));
		returnButton.SetTexture(UIButtonBase.State.Pressed, buttonsMaterial, ButtonsTexturePosition.ButtonPressed, AutoRect.AutoSize(ButtonsTexturePosition.SmallSizeButton));
		returnButton.Rect = AutoRect.AutoPos(uiPos.ReturnButton);
		returnButton.SetText("font1", " QUIT", ColorName.fontColor_orange);
		mask = new UIImage();
		mask.SetTexture(gameuiMaterial, GameUITexturePosition.Mask, AutoRect.AutoSize(uiPos.Mask));
		mask.Rect = AutoRect.AutoValuePos(uiPos.Mask);
		Add(mask);
		Add(background);
		Add(musicLabel);
		Add(musicButtonOff);
		Add(musicButtonOn);
		Add(soundLabel);
		Add(soundButtonOff);
		Add(soundButtonOn);
		Add(resumeButton);
		Add(returnButton);
		SetUIHandler(this);
		gameDialog = new GameDialog(UIDialog.DialogMode.YES_OR_NO);
		gameDialog.SetText("font2", "\n\nAre You Sure You Want To Quit?", ColorName.fontColor_darkorange);
		gameDialog.SetDialogEventHandler(this);
		Add(gameDialog);
		isEndlsessQuit = false;
		Hide();
	}

	public void SetGameUIScript(GameUIScript guis)
	{
		ui = guis;
	}

	public void Yes()
	{
		if (GameApp.GetInstance().GetGameState().endless_level)
		{
			if (GameApp.GetInstance().GetGameState().endless_multiplayer)
			{
				GameMultiplayerScene gameMultiplayerScene = GameApp.GetInstance().GetGameScene() as GameMultiplayerScene;
				gameMultiplayerScene.TimeGameOver(1f);
			}
			else
			{
				Player player = GameApp.GetInstance().GetGameScene().GetPlayer();
				player.OnHit(player.GetMaxHp() + 1f);
				isEndlsessQuit = true;
			}
			Time.timeScale = 1f;
			Hide();
			GameApp.GetInstance().GetGameScene().GamePlayingState = PlayingState.GameQuit;
		}
		else if (GameApp.GetInstance().GetGameState().VS_mode)
		{
			GameApp.GetInstance().GetGameScene().GamePlayingState = PlayingState.GameQuit;
			Time.timeScale = 1f;
			GameVSScene gameVSScene = GameApp.GetInstance().GetGameScene() as GameVSScene;
			gameVSScene.QuitGameForDisconnect();
		}
		else
		{
			GameApp.GetInstance().GetGameScene().GamePlayingState = PlayingState.GameQuit;
			Time.timeScale = 1f;
			GameApp.GetInstance().GetGameState().user_statistics.last_day_state = "Day_" + GameApp.GetInstance().GetGameState().LevelNum + "_Quit";
			GameApp.GetInstance().GetGameState().user_statistics.mission_quit_list.Add("Day_" + GameApp.GetInstance().GetGameState().LevelNum);
			GameApp.GetInstance().GetGameState().SaveSceneStatistics();
			UIResourceMgr.GetInstance().UnloadAllUIMaterials();
			GameApp.GetInstance().Load();
			SceneName.LoadLevel("MainMapTUI");
		}
	}

	public void No()
	{
		gameDialog.Hide();
	}

	public void HandleEvent(UIControl control, int command, float wparam, float lparam)
	{
		if (control == resumeButton)
		{
			Time.timeScale = 1f;
			Hide();
			OpenClickPlugin.Hide();
		}
		if (control == musicButtonOff)
		{
			musicButtonOn.Set(false);
			gameState.MusicOn = false;
			AudioSource component = GameApp.GetInstance().GetGameScene().GetCamera()
				.GetComponent<AudioSource>();
			if (component != null)
			{
				component.mute = !gameState.MusicOn;
			}
			gameState.SaveMusicState();
		}
		else if (control == musicButtonOn)
		{
			musicButtonOff.Set(false);
			gameState.MusicOn = true;
			AudioSource component2 = GameApp.GetInstance().GetGameScene().GetCamera()
				.GetComponent<AudioSource>();
			if (component2 != null)
			{
				component2.mute = !gameState.MusicOn;
			}
			gameState.SaveMusicState();
		}
		else if (control == soundButtonOff)
		{
			soundButtonOn.Set(false);
			gameState.SoundOn = false;
			gameState.SaveMusicState();
		}
		else if (control == soundButtonOn)
		{
			soundButtonOff.Set(false);
			gameState.SoundOn = true;
			gameState.SaveMusicState();
		}
		else if (control == returnButton)
		{
			gameDialog.Show();
		}
	}
}
