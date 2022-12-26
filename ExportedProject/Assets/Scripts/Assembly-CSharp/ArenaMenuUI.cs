using System.Collections;
using UnityEngine;
using Zombie3D;

public class ArenaMenuUI : MonoBehaviour, UIHandler
{
	protected Rect[] buttonRect;

	public UIManager m_UIManager;

	protected float screenRatioX;

	protected float screenRatioY;

	private ArenaMenuPanel arenaMenuPanel;

	private OptionsMenuUI optionPanel;

	protected UIPanel[] panels = new UIPanel[5];

	protected AudioPlayer audioPlayer = new AudioPlayer();

	protected LoadingPanel loadingPanel;

	protected bool init;

	protected bool setAudioTime;

	protected float startTime;

	public UIPanel GetPanel(int panelID)
	{
		return panels[panelID];
	}

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
			audioSource.bypassEffects = true;
			audioSource.rolloffMode = AudioRolloffMode.Linear;
			audioSource.Play();
		}
	}

	private void Start()
	{
		OpenClickPlugin.Hide();
		loadingPanel = new LoadingPanel();
		loadingPanel.Show();
		m_UIManager = base.gameObject.AddComponent<UIManager>();
		m_UIManager.SetParameter(8, 1, false);
		m_UIManager.SetUIHandler(this);
		m_UIManager.CLEAR = true;
		m_UIManager.Add(loadingPanel);
		startTime = Time.time;
		StartCoroutine("Init");
	}

	private IEnumerator Init()
	{
		yield return 1;
		UIResourceMgr.GetInstance().LoadAllUIMaterials();
		if (Time.time - startTime < 1.5f)
		{
			yield return new WaitForSeconds(1.5f - (Time.time - startTime));
		}
		GameApp.GetInstance().ClearScene();
		panels[0] = new ArenaMenuPanel();
		panels[1] = new WeaponUpgradeUI();
		panels[2] = new EquipmentUI();
		panels[3] = new AvatarUI();
		panels[4] = new ShopUI();
		for (int i = 0; i < 5; i++)
		{
			m_UIManager.Add(panels[i]);
		}
		panels[0].Show();
		Transform audioFolderTrans = base.transform.Find("Audio");
		audioPlayer.AddAudio(audioFolderTrans, "Button", true);
		audioPlayer.AddAudio(audioFolderTrans, "Upgrade", true);
		audioPlayer.AddAudio(audioFolderTrans, "Battle", true);
		GameApp.GetInstance().GetGameState().Achievement.SubmitAllToGameCenter();
		UIResourceMgr.GetInstance().UnloadAllUIMaterials();
	}

	private void Update()
	{
		if (Camera.main != null && !setAudioTime)
		{
			setAudioTime = true;
		}
		ArenaMenuPanel arenaMenuPanel = panels[0] as ArenaMenuPanel;
		if (arenaMenuPanel != null)
		{
			arenaMenuPanel.Update();
		}
		for (int i = 0; i < 5; i++)
		{
			if (panels[i] != null)
			{
				panels[i].UpdateLogic();
			}
		}
		ShopUI shopUI = panels[4] as ShopUI;
		if (shopUI != null)
		{
			shopUI.GetPurchaseStatus();
			shopUI.GetPhotoSaveStatus();
		}
		if (ReviewDialogUI.GetInstance().IsVisible())
		{
			return;
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

	public static ArenaMenuUI GetInstance()
	{
		return GameObject.Find("ArenaMenuUI").GetComponent<ArenaMenuUI>();
	}

	public AudioPlayer GetAudioPlayer()
	{
		return audioPlayer;
	}

	public void HandleEvent(UIControl control, int command, float wparam, float lparam)
	{
	}
}
