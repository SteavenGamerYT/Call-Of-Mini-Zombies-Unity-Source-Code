using UnityEngine;
using Zombie3D;

public class PaperJoyShowUIScript : MonoBehaviour, UIHandler
{
	public UIManager m_UIManager;

	protected UIImage background;

	protected UITextButton BuyButton;

	protected UITextButton UseButton;

	protected UIClickButton BackButton;

	private PaperJoyShowUIPosition uiPos;

	protected Timer fadeTimer = new Timer();

	protected AudioPlayer audio_player;

	protected AudioInfo button_audio;

	protected IAPLockPanel iapLockPanel;

	protected bool iap_Processing;

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
		uiPos = new PaperJoyShowUIPosition();
		m_UIManager = base.gameObject.AddComponent<UIManager>();
		m_UIManager.SetParameter(8, 1, false);
		m_UIManager.SetUIHandler(this);
		m_UIManager.CLEAR = true;
		Material material = UIResourceMgr.GetInstance().GetMaterial("PaperJoyShowUI");
		background = new UIImage();
		background.SetTexture(material, PaperShowUITexturePosition.Background, AutoRect.AutoSize(PaperShowUITexturePosition.Background));
		background.Rect = AutoRect.AutoPos(uiPos.Background);
		BuyButton = new UITextButton();
		BuyButton.SetTexture(UIButtonBase.State.Normal, material, PaperShowUITexturePosition.BuyNormal, AutoRect.AutoSize(PaperShowUITexturePosition.BuyNormal));
		BuyButton.SetTexture(UIButtonBase.State.Pressed, material, PaperShowUITexturePosition.BuyPressed, AutoRect.AutoSize(PaperShowUITexturePosition.BuyPressed));
		BuyButton.Rect = AutoRect.AutoPos(uiPos.BuyButton);
		BuyButton.SetText("font1", string.Empty, ColorName.fontColor_orange);
		UseButton = new UITextButton();
		UseButton.SetTexture(UIButtonBase.State.Normal, material, PaperShowUITexturePosition.UseNormal, AutoRect.AutoSize(PaperShowUITexturePosition.UseNormal));
		UseButton.SetTexture(UIButtonBase.State.Pressed, material, PaperShowUITexturePosition.UsePressed, AutoRect.AutoSize(PaperShowUITexturePosition.UsePressed));
		UseButton.Rect = AutoRect.AutoPos(uiPos.BuyButton);
		UseButton.SetText("font1", string.Empty, ColorName.fontColor_orange);
		BackButton = new UIClickButton();
		BackButton.SetTexture(UIButtonBase.State.Normal, material, PaperShowUITexturePosition.BackButtonNormal, AutoRect.AutoSize(PaperShowUITexturePosition.BackButtonNormal));
		BackButton.SetTexture(UIButtonBase.State.Pressed, material, PaperShowUITexturePosition.BackButtonPressed, AutoRect.AutoSize(PaperShowUITexturePosition.BackButtonPressed));
		BackButton.Rect = AutoRect.AutoPos(uiPos.BackButton);
		iapLockPanel = new IAPLockPanel();
		m_UIManager.Add(background);
		m_UIManager.Add(BuyButton);
		m_UIManager.Add(UseButton);
		m_UIManager.Add(BackButton);
		m_UIManager.Add(iapLockPanel);
		GameApp.GetInstance().InitForMenu();
		GameApp.GetInstance().PlayerPrefsLoad();
		CheckPaperModelStatus();
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

	private void CheckPaperModelStatus()
	{
		if (GameApp.GetInstance().GetGameState().PaperModelShow)
		{
			BuyButton.Visible = false;
			BuyButton.Enable = false;
			UseButton.Visible = true;
			UseButton.Enable = true;
		}
		else
		{
			BuyButton.Visible = true;
			BuyButton.Enable = true;
			UseButton.Visible = false;
			UseButton.Enable = false;
		}
	}

	private void Update()
	{
		if (FadeAnimationScript.GetInstance().FadeOutComplete())
		{
			GetPurchaseStatus();
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
			if (fadeTimer.Name == "Use")
			{
				GameApp.GetInstance().Load();
				UIResourceMgr.GetInstance().UnloadAllUIMaterials();
				SceneName.LoadLevel("PaperJoyUseUI");
			}
			else if (fadeTimer.Name == "back")
			{
				SceneName.LoadLevel("MainMapTUI");
			}
			fadeTimer.Do();
		}
	}

	public void HandleEvent(UIControl control, int command, float wparam, float lparam)
	{
		if (control == UseButton)
		{
			fadeTimer.Name = "Use";
			FadeAnimationScript.GetInstance().FadeInBlack();
			fadeTimer.SetTimer(0.5f, false);
			audio_player.PlayAudio("button");
		}
		else if (control == BuyButton)
		{
			audio_player.PlayAudio("button");
			IAP.NowPurchaseProduct("com.trinitigame.callofminizombies.new099cents2", "1");
			iapLockPanel.Show();
			iap_Processing = true;
		}
		else if (control == BackButton)
		{
			fadeTimer.Name = "back";
			FadeAnimationScript.GetInstance().FadeInBlack();
			fadeTimer.SetTimer(0.5f, false);
			audio_player.PlayAudio("button");
		}
	}

	public void GetPurchaseStatus()
	{
		if (iap_Processing)
		{
			int num = IAP.purchaseStatus(null);
			iapLockPanel.UpdateSpinner();
			switch (num)
			{
			case 0:
				break;
			case 1:
				GameApp.GetInstance().GetGameState().PaperModelShow = true;
				GameApp.GetInstance().PlayerPrefsSave();
				iapLockPanel.Hide();
				iap_Processing = false;
				CheckPaperModelStatus();
				break;
			default:
				iapLockPanel.Hide();
				iap_Processing = false;
				break;
			}
		}
	}
}
