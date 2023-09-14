using UnityEngine;
using Zombie3D;

public class OptionsMenuUI : UIPanel, UIHandler, UIDialogEventHandler
{
	protected Rect[] buttonRect;

	protected CreditsMenuUI cm;

	public string m_ui_material_path;

	protected Material buttonsMaterial;

	protected Material arenaMenuMaterial;

	protected UIImage background;

	protected UITextImage musicPanel;

	protected UITextSelectButton musicButtonOff;

	protected UITextSelectButton musicButtonOn;

	protected UITextImage soundPanel;

	protected UITextSelectButton soundButtonOff;

	protected UITextSelectButton soundButtonOn;

	protected UITextButton creditsButton;

	protected UITextButton shareButton;

	protected UITextButton reviewButton;

	protected UITextButton supportButton;

	protected UITextButton resetButton;

	protected UIClickButton returnButton;

	protected UITextImage daysPanel;

	protected CashPanel cashPanel;

	private OptionsMenuUIPosition uiPos;

	private OptionsMenuTexturePosition texPos;

	protected float screenRatioX;

	protected float screenRatioY;

	protected GameState gameState;

	protected CreditsMenuUI creditsPanel;

	protected GameDialog gameDialog;

	protected MapUI ui;

	public OptionsMenuUI()
	{
		uiPos = new OptionsMenuUIPosition();
		texPos = new OptionsMenuTexturePosition();
		gameState = GameApp.GetInstance().GetGameState();
		buttonsMaterial = UIResourceMgr.GetInstance().GetMaterial("Buttons");
		arenaMenuMaterial = UIResourceMgr.GetInstance().GetMaterial("ArenaMenu");
		background = new UIImage();
		background.SetTexture(arenaMenuMaterial, ArenaMenuTexturePosition.Background, AutoRect.AutoSize(ArenaMenuTexturePosition.Background));
		background.Rect = AutoRect.AutoPos(uiPos.Background);
		daysPanel = new UITextImage();
		daysPanel.SetTexture(arenaMenuMaterial, ArenaMenuTexturePosition.Panel, AutoRect.AutoSize(ArenaMenuTexturePosition.Panel));
		daysPanel.Rect = AutoRect.AutoPos(uiPos.DaysPanel);
		daysPanel.SetText("font1", "DAY " + GameApp.GetInstance().GetGameState().LevelNum, ColorName.fontColor_darkorange);
		cashPanel = new CashPanel();
		musicPanel = new UITextImage();
		musicPanel.SetTexture(buttonsMaterial, ButtonsTexturePosition.Label, AutoRect.AutoSize(ButtonsTexturePosition.Label));
		musicPanel.Rect = AutoRect.AutoPos(uiPos.MusicPanel);
		musicPanel.SetText("font1", "MUSIC", ColorName.fontColor_darkorange);
		musicButtonOff = new UITextSelectButton();
		musicButtonOff.SetTexture(UIButtonBase.State.Normal, buttonsMaterial, ButtonsTexturePosition.SoundButtonNormal, AutoRect.AutoSize(ButtonsTexturePosition.SoundButtonNormal));
		musicButtonOff.SetTexture(UIButtonBase.State.Pressed, buttonsMaterial, ButtonsTexturePosition.SoundButtonPressed, AutoRect.AutoSize(ButtonsTexturePosition.SoundButtonPressed));
		musicButtonOff.Rect = AutoRect.AutoPos(uiPos.MusicButtonOff);
		musicButtonOff.SetText("font1", " OFF", ColorName.fontColor_orange);
		musicButtonOn = new UITextSelectButton();
		musicButtonOn.SetTexture(UIButtonBase.State.Normal, buttonsMaterial, ButtonsTexturePosition.SoundButtonNormal, AutoRect.AutoSize(ButtonsTexturePosition.SoundButtonNormal));
		musicButtonOn.SetTexture(UIButtonBase.State.Pressed, buttonsMaterial, ButtonsTexturePosition.SoundButtonPressed, AutoRect.AutoSize(ButtonsTexturePosition.SoundButtonPressed));
		musicButtonOn.Rect = AutoRect.AutoPos(uiPos.MusicButtonOn);
		musicButtonOn.SetText("font1", " ON", ColorName.fontColor_orange);
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
		soundPanel = new UITextImage();
		soundPanel.SetTexture(buttonsMaterial, ButtonsTexturePosition.Label, AutoRect.AutoSize(ButtonsTexturePosition.Label));
		soundPanel.Rect = AutoRect.AutoPos(uiPos.SoundPanel);
		soundPanel.SetText("font1", " SOUND", ColorName.fontColor_darkorange);
		soundButtonOff = new UITextSelectButton();
		soundButtonOff.SetTexture(UIButtonBase.State.Normal, buttonsMaterial, ButtonsTexturePosition.SoundButtonNormal, AutoRect.AutoSize(ButtonsTexturePosition.SoundButtonNormal));
		soundButtonOff.SetTexture(UIButtonBase.State.Pressed, buttonsMaterial, ButtonsTexturePosition.SoundButtonPressed, AutoRect.AutoSize(ButtonsTexturePosition.SoundButtonPressed));
		soundButtonOff.Rect = AutoRect.AutoPos(uiPos.SoundButtonOff);
		soundButtonOff.SetText("font1", " OFF", ColorName.fontColor_orange);
		soundButtonOn = new UITextSelectButton();
		soundButtonOn.SetTexture(UIButtonBase.State.Normal, buttonsMaterial, ButtonsTexturePosition.SoundButtonNormal, AutoRect.AutoSize(ButtonsTexturePosition.SoundButtonNormal));
		soundButtonOn.SetTexture(UIButtonBase.State.Pressed, buttonsMaterial, ButtonsTexturePosition.SoundButtonPressed, AutoRect.AutoSize(ButtonsTexturePosition.SoundButtonPressed));
		soundButtonOn.Rect = AutoRect.AutoPos(uiPos.SoundButtonOn);
		soundButtonOn.SetText("font1", " ON", ColorName.fontColor_orange);
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
		resetButton = new UITextButton();
		resetButton.SetTexture(UIButtonBase.State.Normal, buttonsMaterial, ButtonsTexturePosition.ButtonNormal, AutoRect.AutoSize(ButtonsTexturePosition.MiddleSizeButton));
		resetButton.SetTexture(UIButtonBase.State.Pressed, buttonsMaterial, ButtonsTexturePosition.ButtonPressed, AutoRect.AutoSize(ButtonsTexturePosition.MiddleSizeButton));
		resetButton.Rect = AutoRect.AutoPos(uiPos.ResetButton);
		resetButton.SetText("font1", " RESET DATA", ColorName.fontColor_orange);
		creditsButton = new UITextButton();
		creditsButton.SetTexture(UIButtonBase.State.Normal, buttonsMaterial, ButtonsTexturePosition.ButtonNormal, AutoRect.AutoSize(ButtonsTexturePosition.SmallSizeButton));
		creditsButton.SetTexture(UIButtonBase.State.Pressed, buttonsMaterial, ButtonsTexturePosition.ButtonPressed, AutoRect.AutoSize(ButtonsTexturePosition.SmallSizeButton));
		creditsButton.Rect = AutoRect.AutoPos(uiPos.CreditsButton);
		creditsButton.SetText("font1", " CREDITS", ColorName.fontColor_orange);
		shareButton = new UITextButton();
		shareButton.SetTexture(UIButtonBase.State.Normal, buttonsMaterial, ButtonsTexturePosition.ButtonNormal, AutoRect.AutoSize(ButtonsTexturePosition.SmallSizeButton));
		shareButton.SetTexture(UIButtonBase.State.Pressed, buttonsMaterial, ButtonsTexturePosition.ButtonPressed, AutoRect.AutoSize(ButtonsTexturePosition.SmallSizeButton));
		shareButton.Rect = AutoRect.AutoPos(uiPos.ShareButton);
		shareButton.SetText("font1", " SHARE", ColorName.fontColor_darkorange);
		reviewButton = new UITextButton();
		reviewButton.SetTexture(UIButtonBase.State.Normal, buttonsMaterial, ButtonsTexturePosition.ButtonNormal, AutoRect.AutoSize(ButtonsTexturePosition.SmallSizeButton));
		reviewButton.SetTexture(UIButtonBase.State.Pressed, buttonsMaterial, ButtonsTexturePosition.ButtonPressed, AutoRect.AutoSize(ButtonsTexturePosition.SmallSizeButton));
		reviewButton.Rect = AutoRect.AutoPos(uiPos.ReviewButton);
		reviewButton.SetText("font1", " REVIEW", ColorName.fontColor_darkorange);
		supportButton = new UITextButton();
		supportButton.SetTexture(UIButtonBase.State.Normal, buttonsMaterial, ButtonsTexturePosition.ButtonNormal, AutoRect.AutoSize(ButtonsTexturePosition.SmallSizeButton));
		supportButton.SetTexture(UIButtonBase.State.Pressed, buttonsMaterial, ButtonsTexturePosition.ButtonPressed, AutoRect.AutoSize(ButtonsTexturePosition.SmallSizeButton));
		supportButton.Rect = AutoRect.AutoPos(uiPos.SupportButton);
		supportButton.SetText("font1", " SUPPORT", ColorName.fontColor_darkorange);
		returnButton = new UIClickButton();
		returnButton.SetTexture(UIButtonBase.State.Normal, arenaMenuMaterial, ArenaMenuTexturePosition.ReturnButtonNormal, AutoRect.AutoSize(ArenaMenuTexturePosition.ReturnButtonNormal));
		returnButton.SetTexture(UIButtonBase.State.Pressed, arenaMenuMaterial, ArenaMenuTexturePosition.ReturnButtonPressed, AutoRect.AutoSize(ArenaMenuTexturePosition.ReturnButtonPressed));
		returnButton.Rect = AutoRect.AutoPos(uiPos.ReturnButton);
		creditsPanel = new CreditsMenuUI();
		gameDialog = new GameDialog(UIDialog.DialogMode.YES_OR_NO);
		gameDialog.SetText("font2", "\n\nAre You Sure You Want To Erase Your Progress And Start A New Game?", ColorName.fontColor_darkorange);
		gameDialog.SetDialogEventHandler(this);
		Add(background);
		Add(daysPanel);
		Add(cashPanel);
		Add(musicPanel);
		Add(musicButtonOff);
		Add(musicButtonOn);
		Add(soundPanel);
		Add(soundButtonOff);
		Add(soundButtonOn);
		Add(creditsButton);
		Add(shareButton);
		Add(reviewButton);
		Add(supportButton);
		Add(returnButton);
		Add(resetButton);
		Add(creditsPanel);
		Add(gameDialog);
		ui = MapUI.GetInstance();
		SetUIHandler(this);
	}

	public override void Show()
	{
		base.Show();
		cashPanel.SetCash(GameApp.GetInstance().GetGameState().GetCash());
		cashPanel.Show();
		resetButton.Enable = true;
		resetButton.Visible = true;
		shareButton.Enable = true;
		shareButton.Visible = true;
		reviewButton.Enable = true;
		reviewButton.Visible = true;
		supportButton.Enable = true;
		supportButton.Visible = true;
		musicButtonOff.Enable = true;
		musicButtonOn.Enable = true;
		soundButtonOff.Enable = true;
		soundButtonOn.Enable = true;
	}

	public void EnableUI()
	{
	}

	public void HandleEvent(UIControl control, int command, float wparam, float lparam)
	{
		if (control == musicButtonOff)
		{
			musicButtonOn.Set(false);
			gameState.MusicOn = false;
			MapUI.GetInstance().GetAudioPlayer().PlayAudio("Button");
			GameApp.GetInstance().PlayerPrefsSave();
			GameObject gameObject = GameObject.Find("Music");
			if (gameObject != null)
			{
				AudioSource component = gameObject.GetComponent<AudioSource>();
				if (component != null)
				{
					component.mute = !gameState.MusicOn;
				}
			}
		}
		else if (control == musicButtonOn)
		{
			musicButtonOff.Set(false);
			gameState.MusicOn = true;
			MapUI.GetInstance().GetAudioPlayer().PlayAudio("Button");
			GameApp.GetInstance().PlayerPrefsSave();
			GameObject gameObject2 = GameObject.Find("Music");
			if (gameObject2 != null)
			{
				AudioSource component2 = gameObject2.GetComponent<AudioSource>();
				if (component2 != null)
				{
					component2.mute = !gameState.MusicOn;
				}
			}
		}
		if (control == soundButtonOff)
		{
			soundButtonOn.Set(false);
			gameState.SoundOn = false;
			MapUI.GetInstance().GetAudioPlayer().PlayAudio("Button");
			GameApp.GetInstance().PlayerPrefsSave();
		}
		else if (control == soundButtonOn)
		{
			soundButtonOff.Set(false);
			gameState.SoundOn = true;
			MapUI.GetInstance().GetAudioPlayer().PlayAudio("Button");
			GameApp.GetInstance().PlayerPrefsSave();
		}
		else if (control == creditsButton)
		{
			MapUI.GetInstance().GetAudioPlayer().PlayAudio("Button");
			creditsPanel.Show();
			resetButton.Enable = false;
			resetButton.Visible = false;
			shareButton.Enable = false;
			shareButton.Visible = false;
			reviewButton.Enable = false;
			reviewButton.Visible = false;
			supportButton.Enable = false;
			supportButton.Visible = false;
			musicButtonOff.Enable = false;
			musicButtonOn.Enable = false;
			soundButtonOff.Enable = false;
			soundButtonOn.Enable = false;
		}
		else if (control == returnButton)
		{
			MapUI.GetInstance().GetAudioPlayer().PlayAudio("Button");
			Hide();
			ui.GetMapUI().Show();
		}
		else if (control == shareButton)
		{
			string text = GameApp.GetInstance().GetGloabResourceConfig().shareHtm.text;
			Utils.ToSendMail(string.Empty, "Call Of Mini: Zombies", text);
		}
		else if (control == reviewButton)
		{
			Application.OpenURL("http://www.trinitigame.com/callofminizombies/review/");
		}
		else if (control == supportButton)
		{
			Utils.ToSendMail("support@trinitigame.com", "Call Of Mini: Zombies", string.Empty);
		}
		else if (control == resetButton)
		{
			MapUI.GetInstance().GetAudioPlayer().PlayAudio("Button");
			gameDialog.Show();
		}
	}

	public void Yes()
	{
		Debug.Log("Reset Game!");
		gameState.ResetData();
		daysPanel.SetText("font1", "DAY " + GameApp.GetInstance().GetGameState().LevelNum, ColorName.fontColor_darkorange);
		cashPanel.SetCash(GameApp.GetInstance().GetGameState().GetCash());
		MapUI.GetInstance().mapPanel.ResetData();
		gameDialog.Hide();
	}

	public void No()
	{
		gameDialog.Hide();
	}
}
