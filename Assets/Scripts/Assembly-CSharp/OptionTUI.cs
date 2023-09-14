using UnityEngine;
using Zombie3D;

public class OptionTUI : MonoBehaviour, TUIHandler
{
	private TUI m_tui;

	protected TUIInput[] input;

	public TUIButtonSelectText music_off_button;

	public TUIButtonSelectText music_on_button;

	public TUIButtonSelectText sound_off_button;

	public TUIButtonSelectText sound_on_button;

	public MsgBoxDelegate msg_box;

	public TUIMeshText label_day;

	public TUIMeshText label_money;

	public GameObject credits_panel;

	public GameObject Sensitivity_Ori;

	public GameObject Sensitivity_Button;

	public GameObject Sensitivity_BK;

	public GameObject Sensitivity_Label;

	public GameObject Sensitivity_Label_BK;

	protected AudioPlayer audioPlayer = new AudioPlayer();

	private void Awake()
	{
		GameScript.CheckMenuResourceConfig();
		GameScript.CheckGlobalResourceConfig();
		if (GameObject.Find("Music") == null)
		{
			GameApp.GetInstance().InitForMenu();
			GameApp.GetInstance().GetGameState().InitWeapons();
			GameObject gameObject = new GameObject("Music");
			gameObject.AddComponent<MusicFixer>();
			Object.DontDestroyOnLoad(gameObject);
			gameObject.transform.position = new Vector3(0f, 1f, -10f);
			AudioSource audioSource = gameObject.AddComponent<AudioSource>();
			audioSource.clip = GameApp.GetInstance().GetMenuResourceConfig().menuAudio;
			audioSource.loop = true;
			audioSource.bypassEffects = true;
			audioSource.rolloffMode = AudioRolloffMode.Linear;
			audioSource.mute = !GameApp.GetInstance().GetGameState().MusicOn;
			audioSource.Play();
		}
	}

	public void Start()
	{
		m_tui = TUI.Instance("TUI");
		m_tui.SetHandler(this);
		m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
			.FadeIn();
		music_off_button.SetSelected(!GameApp.GetInstance().GetGameState().MusicOn);
		music_on_button.SetSelected(GameApp.GetInstance().GetGameState().MusicOn);
		sound_off_button.SetSelected(!GameApp.GetInstance().GetGameState().SoundOn);
		sound_on_button.SetSelected(GameApp.GetInstance().GetGameState().SoundOn);
		label_day.text = "DAY " + GameApp.GetInstance().GetGameState().LevelNum;
		label_money.text = "$" + GameApp.GetInstance().GetGameState().GetCash()
			.ToString("N0");
		Transform folderTrans = base.transform.Find("Audio");
		audioPlayer.AddAudio(folderTrans, "Button", true);
		GameObject gameObject = GameObject.Find("Music");
		if (gameObject != null)
		{
			AudioSource component = gameObject.GetComponent<AudioSource>();
			if (component != null)
			{
				component.mute = !GameApp.GetInstance().GetGameState().MusicOn;
			}
		}
		if (Sensitivity_Button != null)
		{
			Sensitivity_Button.transform.localPosition = new Vector3(GameApp.GetInstance().GetGameState().macos_sen - 1f + Sensitivity_Ori.transform.localPosition.x, Sensitivity_Ori.transform.localPosition.y, Sensitivity_Ori.transform.localPosition.z);
		}
		if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform != RuntimePlatform.Android)
		{
			Sensitivity_Ori.active = false;
			Sensitivity_Button.active = false;
			Sensitivity_BK.active = false;
			Sensitivity_Label.active = false;
			Sensitivity_Label_BK.active = false;
		}
		OpenClickPlugin.Hide();
	}

	public void Update()
	{
		input = TUIInputManager.GetInput();
		for (int i = 0; i < input.Length; i++)
		{
			m_tui.HandleInput(input[i]);
		}
	}

	public void HandleEvent(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (control.name == "Back_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			SceneName.SaveSceneStatistics("MainMapTUI");
			m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
				.FadeOut("MainMapTUI");
		}
		else if (control.name == "Music_Off_Button" && eventType == 1)
		{
			GameApp.GetInstance().GetGameState().MusicOn = false;
			audioPlayer.PlayAudio("Button");
			GameApp.GetInstance().PlayerPrefsSave();
			GameObject gameObject = GameObject.Find("Music");
			if (gameObject != null)
			{
				AudioSource component = gameObject.GetComponent<AudioSource>();
				if (component != null)
				{
					component.mute = !GameApp.GetInstance().GetGameState().MusicOn;
				}
			}
		}
		else if (control.name == "Music_On_Button" && eventType == 1)
		{
			GameApp.GetInstance().GetGameState().MusicOn = true;
			audioPlayer.PlayAudio("Button");
			GameApp.GetInstance().PlayerPrefsSave();
			GameObject gameObject2 = GameObject.Find("Music");
			if (gameObject2 != null)
			{
				AudioSource component2 = gameObject2.GetComponent<AudioSource>();
				if (component2 != null)
				{
					component2.mute = !GameApp.GetInstance().GetGameState().MusicOn;
				}
			}
		}
		else if (control.name == "Sound_Off_Button" && eventType == 1)
		{
			GameApp.GetInstance().GetGameState().SoundOn = false;
			audioPlayer.PlayAudio("Button");
			GameApp.GetInstance().PlayerPrefsSave();
		}
		else if (control.name == "Sound_On_Button" && eventType == 1)
		{
			GameApp.GetInstance().GetGameState().SoundOn = true;
			audioPlayer.PlayAudio("Button");
			GameApp.GetInstance().PlayerPrefsSave();
		}
		else if (control.name == "Share_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			string text = GameApp.GetInstance().GetGloabResourceConfig().shareHtm.text;
			Utils.ToSendMail(string.Empty, "Call Of Mini: Zombies", text);
		}
		else if (control.name == "Support_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			Utils.ToSendMail("support@trinitigame.com", "Call Of Mini: Zombies", string.Empty);
			Application.OpenURL("http://www.trinitigame.com/support?game=comz&version=3.0.1");
		}
		else if (control.name == "Review_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			Application.OpenURL("http://itunes.apple.com/us/app/call-of-mini-zombies/id431213733?mt=8");
		}
		else if (control.name == "Credits_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			credits_panel.transform.localPosition = new Vector3(0f, 0f, credits_panel.transform.localPosition.z);
		}
		else if (control.name == "Credits_Back_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			credits_panel.transform.localPosition = new Vector3(0f, -1000f, credits_panel.transform.localPosition.z);
		}
		else if (control.name == "Reset_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			msg_box.Show("Are You Sure You Want To Erase \nYour Progress And Start A New \nGame?", MsgBoxType.None);
		}
		else if (control.name == "Msg_Yes_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			msg_box.Hide();
			GameApp.GetInstance().GetGameState().ResetData();
			label_day.text = "DAY " + GameApp.GetInstance().GetGameState().LevelNum;
			label_money.text = "$" + GameApp.GetInstance().GetGameState().GetCash()
				.ToString("N0");
		}
		else if (control.name == "Msg_No_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			msg_box.Hide();
		}
		else if (control.name == "Sensitivity_Button" && eventType == 2)
		{
			float num = control.transform.localPosition.x - Sensitivity_Ori.transform.localPosition.x + 1f;
			GameApp.GetInstance().GetGameState().macos_sen = num;
			Debug.Log("delta_x:" + num);
			GameApp.GetInstance().PlayerPrefsSave();
		}
	}
}
