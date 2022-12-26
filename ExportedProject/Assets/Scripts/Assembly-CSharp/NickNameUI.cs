using System.Collections;
using UnityEngine;
using Zombie3D;

public class NickNameUI : MonoBehaviour
{
	public UIManager m_UIManager;

	protected NickNameUIPanel nick_name_panel;

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
			audioSource.bypassEffects = true;
			audioSource.rolloffMode = AudioRolloffMode.Linear;
			audioSource.mute = !GameApp.GetInstance().GetGameState().MusicOn;
			audioSource.Play();
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
		if (!GameApp.GetInstance().GetGameState().FromShopMenu)
		{
			m_UIManager.Add(loadingPanel);
		}
		StartCoroutine("Init");
	}

	private IEnumerator Init()
	{
		yield return 1;
		if (Time.time - startTime < 1.5f)
		{
			yield return new WaitForSeconds(1.5f - (Time.time - startTime));
		}
		FadeAnimationScript.GetInstance().FadeOutBlack();
		nick_name_panel = new NickNameUIPanel();
		nick_name_panel.Start();
		nick_name_panel.Show();
		m_UIManager.Add(nick_name_panel);
		UIResourceMgr.GetInstance().UnloadAllUIMaterials();
	}

	private void Update()
	{
		if (nick_name_panel != null)
		{
			nick_name_panel.Update();
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
}
