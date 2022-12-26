using UnityEngine;
using Zombie3D;

public class StartMenuUIScript : MonoBehaviour, UIHandler, UIDialogEventHandler
{
	protected Rect[] buttonRect;

	public UIManager m_UIManager;

	public string m_ui_material_path;

	protected Material startMenuMaterial;

	protected Material startMenu2Material;

	protected UIImage background;

	protected UIImage background_tip;

	protected UIClickButton playButton;

	protected UIClickButton adButton;

	protected GameDialog gameDialog;

	private StartMenuUIPosition uiPos;

	protected float screenRatioX;

	protected float screenRatioY;

	protected Timer fadeTimer = new Timer();

	protected UITextButton new_ver_button;

	protected UIImage new_ver_img;

	protected AudioPlayer audio_player;

	protected AudioInfo button_audio;

	protected bool m_is_fade_out = true;

	protected float image_a = 1f;

	private void Awake()
	{
		GameScript.CheckMenuResourceConfig();
		GameScript.CheckGlobalResourceConfig();
		if (GameObject.Find("Music") == null)
		{
			GameApp.GetInstance().InitForMenu();
			GameObject gameObject = new GameObject("Music");
			gameObject.AddComponent<MusicFixer>();
			Object.DontDestroyOnLoad(gameObject);
			gameObject.transform.position = new Vector3(0f, 1f, -10f);
			AudioSource audioSource = gameObject.AddComponent<AudioSource>();
			audioSource.clip = GameApp.GetInstance().GetMenuResourceConfig().menuAudio;
			audioSource.loop = true;
			audioSource.playOnAwake = false;
			audioSource.bypassEffects = true;
			audioSource.rolloffMode = AudioRolloffMode.Linear;
		}
	}

	private void Start()
	{
		ResolutionConstant.R = (float)Screen.width / 960f;
		UIResourceMgr.GetInstance().LoadStartMenuUIMaterials();
		uiPos = new StartMenuUIPosition();
		m_UIManager = base.gameObject.AddComponent<UIManager>();
		m_UIManager.SetParameter(8, 1, false);
		m_UIManager.SetUIHandler(this);
		m_UIManager.CLEAR = true;
		startMenuMaterial = UIResourceMgr.GetInstance().GetMaterial("StartMenu");
		Material material = UIResourceMgr.GetInstance().GetMaterial("Buttons");
		background = new UIImage();
		background.SetTexture(startMenuMaterial, StartMenuTexturePosition.Background, AutoRect.AutoSize(StartMenuTexturePosition.Background));
		background.Rect = AutoRect.AutoPos(uiPos.Background);
		background_tip = new UIImage();
		background_tip.SetTexture(startMenuMaterial, StartMenuTexturePosition.Background_tip, AutoRect.AutoSize(StartMenuTexturePosition.Background_tip));
		background_tip.Rect = AutoRect.AutoPos(uiPos.Background_tip);
		playButton = new UIClickButton();
		playButton.Rect = AutoRect.AutoPos(uiPos.StartButton);
		adButton = new UIClickButton();
		adButton.Rect = AutoRect.AutoPos(uiPos.adButton);
		gameDialog = new GameDialog(UIDialog.DialogMode.YES_OR_NO);
		gameDialog.SetText("font2", "\n\nAre You Sure You Want To Erase Your Progress And Start A New Game?", ColorName.fontColor_darkorange);
		gameDialog.SetDialogEventHandler(this);
		m_UIManager.Add(background);
		m_UIManager.Add(background_tip);
		m_UIManager.Add(playButton);
		m_UIManager.Add(adButton);
		m_UIManager.Add(gameDialog);
		GameApp.GetInstance().InitForMenu();
		GameCenterPlugin.Login();
		OpenClickPlugin.Show(false);
		GameApp.GetInstance().PlayerPrefsLoad();
		GameObject gameObject = GameObject.Find("Music");
		if (gameObject != null)
		{
			AudioSource component = gameObject.GetComponent<AudioSource>();
			component.mute = ((!GameApp.GetInstance().GetGameState().MusicOn) ? true : false);
			component.Play();
		}
		button_audio = new AudioInfo();
		button_audio.audio = GetComponent<AudioSource>();
		audio_player = new AudioPlayer();
		audio_player.AddAudio(button_audio, "button");
		UIResourceMgr.GetInstance().UnloadAllUIMaterials();
		OpenClickPlugin.Show(false);
		XAdManagerWrapper.SetImageAdUrl("http://itunes.apple.com/us/app/call-of-mini-last-stand/id494829360?ls=1&mt=8");
		XAdManagerWrapper.ShowImageAd();
	}

	private void Update()
	{
		if (Input.GetKeyDown("s"))
		{
			GameApp.GetInstance().Load();
			UIResourceMgr.GetInstance().UnloadAllUIMaterials();
			SceneName.LoadLevel("MainMapTUI");
			fadeTimer.Do();
		}
		if (m_is_fade_out)
		{
			image_a -= Time.deltaTime * 2f;
			if (image_a <= 0f)
			{
				image_a = 0f;
				m_is_fade_out = false;
			}
			background_tip.SetColor(new Color(1f, 1f, 1f, image_a));
		}
		else
		{
			image_a += Time.deltaTime * 2f;
			if (image_a >= 1f)
			{
				image_a = 1f;
				m_is_fade_out = true;
			}
			background_tip.SetColor(new Color(1f, 1f, 1f, image_a));
		}
		if (FadeAnimationScript.GetInstance().FadeOutComplete())
		{
			UITouchInner[] array = ((!Application.isMobilePlatform) ? WindowsInputMgr.MockTouches() : iPhoneInputMgr.MockTouches());
			UITouchInner[] array2 = array;
			foreach (UITouchInner touch in array2)
			{
				if (!(m_UIManager != null) || m_UIManager.HandleInput(touch))
				{
				}
			}
		}
		if (fadeTimer.Ready() && FadeAnimationScript.GetInstance().FadeInComplete())
		{
			if (fadeTimer.Name == "Play")
			{
				GameApp.GetInstance().Load();
				UIResourceMgr.GetInstance().UnloadAllUIMaterials();
				SceneName.LoadLevel("MainMapTUI");
			}
			fadeTimer.Do();
		}
	}

	public void HandleEvent(UIControl control, int command, float wparam, float lparam)
	{
		if (control == playButton)
		{
			fadeTimer.Name = "Play";
			FadeAnimationScript.GetInstance().FadeInBlack();
			fadeTimer.SetTimer(0.5f, false);
			audio_player.PlayAudio("button");
			XAdManagerWrapper.HideImageAd();
		}
		else if (control == new_ver_button)
		{
			new_ver_button.Visible = false;
			new_ver_button.Enable = false;
			new_ver_img.Visible = false;
			new_ver_img.Enable = false;
		}
	}

	public void Yes()
	{
		GameApp.GetInstance().GetGameState().ClearState();
		GameApp.GetInstance().GetGameState().InitWeapons();
		FadeAnimationScript.GetInstance().FadeInBlack();
		fadeTimer.Name = "Start";
		fadeTimer.SetTimer(0.5f, false);
	}

	public void No()
	{
		gameDialog.Hide();
	}
}
