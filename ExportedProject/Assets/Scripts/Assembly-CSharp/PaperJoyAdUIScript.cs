using UnityEngine;
using Zombie3D;

public class PaperJoyAdUIScript : MonoBehaviour, UIHandler
{
	public UIManager m_UIManager;

	protected UIImage background;

	protected UITextButton EnterGameButton;

	protected UITextButton EnterJoyButton;

	private PaperJoyAdUIPosition uiPos;

	protected Timer fadeTimer = new Timer();

	protected AudioPlayer audio_player;

	protected AudioInfo button_audio;

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
		uiPos = new PaperJoyAdUIPosition();
		m_UIManager = base.gameObject.AddComponent<UIManager>();
		m_UIManager.SetParameter(8, 1, false);
		m_UIManager.SetUIHandler(this);
		m_UIManager.CLEAR = true;
		Material material = UIResourceMgr.GetInstance().GetMaterial("PaperJoyAdUI");
		background = new UIImage();
		background.SetTexture(material, PaperShowUITexturePosition.Background, AutoRect.AutoSize(PaperShowUITexturePosition.Background));
		background.Rect = AutoRect.AutoPos(uiPos.Background);
		EnterGameButton = new UITextButton();
		EnterGameButton.SetTexture(UIButtonBase.State.Pressed, material, PaperShowUITexturePosition.PlayPressed, AutoRect.AutoSize(PaperShowUITexturePosition.PlayPressed));
		EnterGameButton.Rect = AutoRect.AutoPos(uiPos.EnterGameButton);
		EnterGameButton.SetText("font1", string.Empty, ColorName.fontColor_orange);
		EnterJoyButton = new UITextButton();
		EnterJoyButton.SetTexture(UIButtonBase.State.Pressed, material, PaperShowUITexturePosition.JoyPressed, AutoRect.AutoSize(PaperShowUITexturePosition.JoyPressed));
		EnterJoyButton.Rect = AutoRect.AutoPos(uiPos.EnterJoyButton);
		EnterJoyButton.SetText("font1", string.Empty, ColorName.fontColor_orange);
		m_UIManager.Add(background);
		m_UIManager.Add(EnterGameButton);
		m_UIManager.Add(EnterJoyButton);
		GameApp.GetInstance().InitForMenu();
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
		OpenClickPlugin.Hide();
	}

	private void Update()
	{
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
			if (fadeTimer.Name == "Game")
			{
				GameApp.GetInstance().Load();
				UIResourceMgr.GetInstance().UnloadAllUIMaterials();
				SceneName.LoadLevel("StartMenuUI");
			}
			else if (fadeTimer.Name == "Joy")
			{
				GameApp.GetInstance().Load();
				UIResourceMgr.GetInstance().UnloadAllUIMaterials();
				GameApp.GetInstance().GetGameState().PaperMenuStatus = PaperUIEnterStatus.StartMenu;
				SceneName.LoadLevel("PaperJoyShowUI");
			}
			fadeTimer.Do();
		}
	}

	public void HandleEvent(UIControl control, int command, float wparam, float lparam)
	{
		if (control == EnterGameButton)
		{
			fadeTimer.Name = "Game";
			FadeAnimationScript.GetInstance().FadeInBlack();
			fadeTimer.SetTimer(0.5f, false);
			audio_player.PlayAudio("button");
		}
		else if (control == EnterJoyButton)
		{
			fadeTimer.Name = "Joy";
			FadeAnimationScript.GetInstance().FadeInBlack();
			fadeTimer.SetTimer(0.5f, false);
			audio_player.PlayAudio("button");
		}
	}
}
