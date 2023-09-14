using UnityEngine;
using Zombie3D;

public class PaperJoyUseUIScript : MonoBehaviour, UIHandler, UIDialogEventHandler
{
	protected const int model_count = 4;

	public UIManager m_UIManager;

	protected UIImage background;

	protected UIImage maskImage;

	protected UIImage saveImage;

	protected UITextButton[] model_button = new UITextButton[4];

	protected UIClickButton BackButton;

	private PaperJoyUseUIPosition uiPos;

	protected Timer fadeTimer = new Timer();

	protected AudioPlayer audio_player;

	protected AudioInfo button_audio;

	protected IAPLockPanel iapLockPanel;

	protected ModelSaveDialog MsgBox;

	protected UIText saveing_show_text;

	protected bool m_is_save_photo;

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
		uiPos = new PaperJoyUseUIPosition();
		uiPos.SetModelButtonRect(4);
		m_UIManager = base.gameObject.AddComponent<UIManager>();
		m_UIManager.SetParameter(8, 1, false);
		m_UIManager.SetUIHandler(this);
		m_UIManager.CLEAR = true;
		PaperShowUITexturePosition.SetJoyButtonRect();
		Material material = UIResourceMgr.GetInstance().GetMaterial("PaperJoyUseUI");
		Material material2 = UIResourceMgr.GetInstance().GetMaterial("PaperSaveButton");
		background = new UIImage();
		background.SetTexture(material, PaperShowUITexturePosition.Background, AutoRect.AutoSize(PaperShowUITexturePosition.Background));
		background.Rect = AutoRect.AutoPos(uiPos.Background);
		m_UIManager.Add(background);
		for (int i = 0; i < 4; i++)
		{
			model_button[i] = new UITextButton();
			model_button[i].SetTexture(UIButtonBase.State.Normal, material2, PaperShowUITexturePosition.JoyButtonNormal[i], AutoRect.AutoSize(PaperShowUITexturePosition.JoyButtonNormal[i]));
			model_button[i].SetTexture(UIButtonBase.State.Pressed, material2, PaperShowUITexturePosition.JoyButtonPressed[i], AutoRect.AutoSize(PaperShowUITexturePosition.JoyButtonPressed[i]));
			model_button[i].Rect = AutoRect.AutoPos(uiPos.ModelButton[i]);
			model_button[i].SetText("font1", string.Empty, ColorName.fontColor_orange);
			m_UIManager.Add(model_button[i]);
		}
		BackButton = new UIClickButton();
		BackButton.SetTexture(UIButtonBase.State.Normal, material, PaperShowUITexturePosition.BackButtonNormal, AutoRect.AutoSize(PaperShowUITexturePosition.BackButtonNormal));
		BackButton.SetTexture(UIButtonBase.State.Pressed, material, PaperShowUITexturePosition.BackButtonPressed, AutoRect.AutoSize(PaperShowUITexturePosition.BackButtonPressed));
		BackButton.Rect = AutoRect.AutoPos(uiPos.BackButton);
		m_UIManager.Add(BackButton);
		iapLockPanel = new IAPLockPanel();
		m_UIManager.Add(iapLockPanel);
		maskImage = new UIImage();
		Material material3 = UIResourceMgr.GetInstance().GetMaterial("Avatar");
		maskImage.SetTexture(material3, AvatarTexturePosition.Mask, AutoRect.AutoSize(ArenaMenuTexturePosition.BackgroundMask));
		maskImage.Rect = AutoRect.AutoPos(new Rect(-20f, -20f, 1200f, 1200f));
		m_UIManager.Add(maskImage);
		maskImage.Visible = false;
		maskImage.Enable = false;
		saveImage = new UIImage();
		saveImage.SetTexture(material, PaperShowUITexturePosition.MsgBox, AutoRect.AutoSize(PaperShowUITexturePosition.MsgBox));
		saveImage.Rect = AutoRect.AutoPos(uiPos.saveBK);
		m_UIManager.Add(saveImage);
		saveImage.Visible = false;
		saveImage.Enable = false;
		MsgBox = new ModelSaveDialog(UIDialog.DialogMode.YES_OR_NO);
		MsgBox.SetDialogEventHandler(this);
		m_UIManager.Add(MsgBox);
		MsgBox.Hide();
		saveing_show_text = new UIText();
		saveing_show_text.Set("font2", "\n\n  Saving to your device... ", ColorName.fontColor_orange);
		saveing_show_text.AlignStyle = UIText.enAlignStyle.center;
		saveing_show_text.Rect = AutoRect.AutoPos(new Rect(0f, 120f, 960f, 640f));
		m_UIManager.Add(saveing_show_text);
		saveing_show_text.Visible = false;
		saveing_show_text.Enable = false;
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
			GetPhotoSaveStatus();
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
			if (fadeTimer.Name == "back")
			{
				SceneName.LoadLevel("PaperJoyShowUI");
			}
			fadeTimer.Do();
		}
	}

	public void HandleEvent(UIControl control, int command, float wparam, float lparam)
	{
		for (int i = 0; i < 4; i++)
		{
			if (control == model_button[i])
			{
				Debug.Log("model." + i);
				Utils.SavePhoto(i, 2480, 3508);
				iapLockPanel.Show();
				m_is_save_photo = true;
				saveing_show_text.Visible = true;
				saveImage.Visible = true;
				break;
			}
		}
		if (control == BackButton)
		{
			fadeTimer.Name = "back";
			FadeAnimationScript.GetInstance().FadeInBlack();
			fadeTimer.SetTimer(0.5f, false);
			audio_player.PlayAudio("button");
		}
	}

	public void GetPhotoSaveStatus()
	{
		if (m_is_save_photo)
		{
			int num = Utils.OnCheckPhotoSaveStatus();
			iapLockPanel.UpdateSpinner();
			if (num != 0 && num == 1)
			{
				iapLockPanel.Hide();
				Utils.OnResetPhotoSaveStatus();
				m_is_save_photo = false;
				maskImage.Visible = true;
				maskImage.Enable = true;
				saveing_show_text.Visible = false;
				saveImage.Visible = false;
				MsgBox.Show();
			}
		}
	}

	public void Yes()
	{
		MsgBox.Hide();
		maskImage.Visible = false;
		maskImage.Enable = false;
	}

	public void No()
	{
		MsgBox.Hide();
		maskImage.Visible = false;
		maskImage.Enable = false;
	}
}
