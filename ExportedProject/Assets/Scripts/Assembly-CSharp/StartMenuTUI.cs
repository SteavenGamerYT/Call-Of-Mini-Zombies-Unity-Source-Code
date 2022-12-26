using UnityEngine;
using Zombie3D;

public class StartMenuTUI : MonoBehaviour, TUIHandler
{
	private TUI m_tui;

	protected TUIInput[] input;

	protected AudioPlayer audio_player;

	protected AudioInfo button_audio;

	protected bool m_is_fade_out = true;

	protected float image_a = 1f;

	public TUIMeshSprite background_tip;

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
		m_tui = TUI.Instance("TUI");
		m_tui.SetHandler(this);
		m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
			.FadeIn();
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
		if (m_is_fade_out)
		{
			image_a -= Time.deltaTime * 2f;
			if (image_a <= 0f)
			{
				image_a = 0f;
				m_is_fade_out = false;
			}
			background_tip.color = new Color(1f, 1f, 1f, image_a);
		}
		else
		{
			image_a += Time.deltaTime * 2f;
			if (image_a >= 1f)
			{
				image_a = 1f;
				m_is_fade_out = true;
			}
			background_tip.color = new Color(1f, 1f, 1f, image_a);
		}
		input = TUIInputManager.GetInput();
		for (int i = 0; i < input.Length; i++)
		{
			m_tui.HandleInput(input[i]);
		}
	}

	public void HandleEvent(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (control.name == "Start_Button" && eventType == 3)
		{
			XAdManagerWrapper.HideImageAd();
			GameApp.GetInstance().Load();
			SceneName.SaveSceneStatistics("MainMapTUI");
			m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
				.FadeOut("MainMapTUI");
			control.gameObject.active = false;
		}
	}
}
