using System.Collections;
using UnityEngine;
using Zombie3D;

public class MapUI : MonoBehaviour
{
	public UIManager m_UIManager;

	public MapUIPanel mapPanel;

	protected OptionsMenuUI optionsUI;

	protected AudioPlayer audioPlayer = new AudioPlayer();

	protected LoadingPanel loadingPanel;

	protected float startTime;

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
			GameApp.GetInstance().PlayerPrefsLoad();
			AudioSource component = gameObject.GetComponent<AudioSource>();
			component.mute = ((!GameApp.GetInstance().GetGameState().MusicOn) ? true : false);
			component.Play();
		}
	}

	private void Start()
	{
		loadingPanel = new LoadingPanel();
		loadingPanel.Show();
		startTime = Time.time;
		m_UIManager = base.gameObject.AddComponent<UIManager>();
		m_UIManager.SetParameter(8, 1, false);
		m_UIManager.CLEAR = true;
		Transform folderTrans = base.transform.Find("Audio");
		audioPlayer.AddAudio(folderTrans, "Button", true);
		audioPlayer.AddAudio(folderTrans, "Battle", true);
		GameApp.GetInstance().GetGameState().Achievement.SubmitAllToGameCenter();
		if (!GameApp.GetInstance().GetGameState().FromShopMenu)
		{
			m_UIManager.Add(loadingPanel);
		}
		StartCoroutine("Init");
	}

	private IEnumerator Init()
	{
		yield return 1;
		if (Time.time - startTime < 3f && !GameApp.GetInstance().GetGameState().FromShopMenu)
		{
			yield return new WaitForSeconds(3f - (Time.time - startTime));
		}
		FadeAnimationScript.GetInstance().FadeOutBlack();
		GameApp.GetInstance().GetGameState().FromShopMenu = false;
		mapPanel = new MapUIPanel();
		m_UIManager.Add(mapPanel);
		mapPanel.Start();
		mapPanel.Show();
		optionsUI = new OptionsMenuUI();
		m_UIManager.Add(optionsUI);
		UIResourceMgr.GetInstance().UnloadAllUIMaterials();
	}

	private void Update()
	{
		if (mapPanel != null)
		{
			mapPanel.Update();
		}
		UITouchInner[] array = ((!Application.isMobilePlatform) ? WindowsInputMgr.MockTouches() : iPhoneInputMgr.MockTouches());
		UITouchInner[] array2 = array;
		foreach (UITouchInner touch in array2)
		{
			if (!(m_UIManager != null) || m_UIManager.HandleInput(touch))
			{
			}
		}
	}

	public static MapUI GetInstance()
	{
		return GameObject.Find("MapUI").GetComponent<MapUI>();
	}

	public OptionsMenuUI GetOptionsMenuUI()
	{
		return optionsUI;
	}

	public MapUIPanel GetMapUI()
	{
		return mapPanel;
	}

	public AudioPlayer GetAudioPlayer()
	{
		return audioPlayer;
	}
}
