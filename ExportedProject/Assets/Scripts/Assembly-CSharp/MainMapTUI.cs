using System;
using System.Collections.Generic;
using UnityEngine;
using Zombie3D;

public class MainMapTUI : MonoBehaviour, TUIHandler
{
	private TUI m_tui;

	protected TUIInput[] input;

	public TUIButtonClick[] map_buttons;

	public TUIButtonClick first_map_button;

	public TUIMeshText label_day;

	public TUIMeshText label_money;

	protected AudioPlayer audioPlayer = new AudioPlayer();

	protected string prey_scene_name = string.Empty;

	public List<TUIButtonClick> map_buttons_active = new List<TUIButtonClick>();

	public TUIMeshSpriteAnimation coop_ani;

	public TUIMeshSpriteAnimation vs_ani;

	public MapEffTrans map_eff_trans;

	protected bool is_effing;

	public bool vs_mode;

	public TUIMeshSprite map_mode_bk;

	public TUIMeshSprite map_mode_eff;

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
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
			gameObject.transform.position = new Vector3(0f, 1f, -10f);
			AudioSource audioSource = gameObject.AddComponent<AudioSource>();
			audioSource.clip = GameApp.GetInstance().GetMenuResourceConfig().menuAudio;
			audioSource.loop = true;
			audioSource.bypassEffects = true;
			audioSource.rolloffMode = AudioRolloffMode.Linear;
			audioSource.mute = !GameApp.GetInstance().GetGameState().MusicOn;
			audioSource.Play();
		}
		GameApp.GetInstance().ClearScene();
		GC.Collect();
	}

	private void Start()
	{
		m_tui = TUI.Instance("TUI");
		m_tui.SetHandler(this);
		if (GameApp.GetInstance().GetGameState().last_scene == "NetMapTUI")
		{
			m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
				.fadeInColorBegin = new Color(0f, 0f, 0f, 1f);
		}
		else
		{
			m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
				.fadeInColorBegin = new Color(0f, 0f, 0f, 0f);
		}
		m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
			.FadeIn();
		GameObject[] array = GameObject.FindGameObjectsWithTag("Map_Button");
		map_buttons = new TUIButtonClick[array.Length];
		label_day.text = "DAY " + GameApp.GetInstance().GetGameState().LevelNum;
		label_money.text = "$" + GameApp.GetInstance().GetGameState().GetCash()
			.ToString("N0");
		for (int i = 0; i < array.Length; i++)
		{
			map_buttons[i] = array[i].GetComponent<TUIButtonClick>();
			map_buttons[i].disabled = true;
		}
		TUIButtonClick tUIButtonClick = null;
		if (GameApp.GetInstance().GetGameState().LevelNum == 1)
		{
			tUIButtonClick = first_map_button;
			map_buttons_active.Add(tUIButtonClick);
		}
		else
		{
			for (int j = 0; j < 3; j++)
			{
				tUIButtonClick = map_buttons[UnityEngine.Random.Range(0, array.Length)];
				if (!map_buttons_active.Contains(tUIButtonClick))
				{
					map_buttons_active.Add(tUIButtonClick);
				}
			}
		}
		GameObject gameObject = null;
		int num = 0;
		if (GameApp.GetInstance().GetGameState().LevelNum >= 20)
		{
			num = UnityEngine.Random.Range(0, map_buttons_active.Count);
			prey_scene_name = map_buttons_active[num].gameObject.name;
		}
		foreach (TUIButtonClick item in map_buttons_active)
		{
			item.disabled = false;
			gameObject = ((item.gameObject.name == prey_scene_name) ? (UnityEngine.Object.Instantiate(Resources.Load("Prefabs/TUI/Prey_Tip")) as GameObject) : (UnityEngine.Object.Instantiate(Resources.Load("Prefabs/TUI/Active_Tip")) as GameObject));
			gameObject.transform.parent = label_day.transform.parent;
			gameObject.transform.localPosition = new Vector3(item.transform.localPosition.x, item.transform.localPosition.y, -0.5f);
		}
		Transform folderTrans = base.transform.Find("Audio");
		audioPlayer.AddAudio(folderTrans, "Button", true);
		audioPlayer.AddAudio(folderTrans, "Battle", true);
		audioPlayer.AddAudio(folderTrans, "UI_scan", true);
		coop_ani.is_animate = false;
		coop_ani.gameObject.active = false;
		if (GameApp.GetInstance().GetGameState().multiplay_named == 0)
		{
			coop_ani.is_animate = true;
			coop_ani.gameObject.active = true;
		}
		vs_ani.is_animate = false;
		vs_ani.gameObject.active = false;
		if (GameApp.GetInstance().GetGameState().first_vs_mode == 1)
		{
			vs_ani.is_animate = true;
			vs_ani.gameObject.active = true;
		}
		map_eff_trans.m_MapEffTransIn = OnMapEffIn;
		OpenClickPlugin.Hide();
		Resources.UnloadUnusedAssets();
		NetworkObj.DestroyNetCom();
		if (GameObject.Find("NetviewObj") != null)
		{
			GameObject obj = GameObject.Find("NetviewObj");
			UnityEngine.Object.Destroy(obj);
		}
		if (GameObject.Find("StatisticsTimer") == null)
		{
			GameObject gameObject2 = UnityEngine.Object.Instantiate(Resources.Load("Prefabs/StatisticsTimer")) as GameObject;
			gameObject2.name = "StatisticsTimer";
			UnityEngine.Object.DontDestroyOnLoad(gameObject2);
		}
		SFSHeartBeat.DestroyInstanceObj();
		GC.Collect();
	}

	private void OnMapEffIn()
	{
		m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
			.fadeOutColorEnd = new Color(0f, 0f, 0f, 1f);
		if (vs_mode)
		{
			if (GameApp.GetInstance().GetGameState().multiplay_named == 0)
			{
				SceneName.SaveSceneStatistics("NickNameTUI");
				m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
					.FadeOut("NickNameTUI");
				GameApp.GetInstance().GetGameState().multiname_to_coop = false;
			}
			else
			{
				SceneName.SaveSceneStatistics("NetMapVSTUI");
				m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
					.FadeOut("NetMapVSTUI");
			}
		}
		else if (GameApp.GetInstance().GetGameState().multiplay_named == 0)
		{
			SceneName.SaveSceneStatistics("NickNameTUI");
			m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
				.FadeOut("NickNameTUI");
			GameApp.GetInstance().GetGameState().multiname_to_coop = true;
		}
		else
		{
			SceneName.SaveSceneStatistics("NetMapTUI");
			m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
				.FadeOut("NetMapTUI");
		}
	}

	private void OnMapEffOut()
	{
	}

	private void Update()
	{
		if (!is_effing)
		{
			input = TUIInputManager.GetInput();
			for (int i = 0; i < input.Length; i++)
			{
				m_tui.HandleInput(input[i]);
			}
		}
	}

	public void HandleEvent(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (control.name == "Back_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			GameApp.GetInstance().GetGameState().FromShopMenu = true;
			m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
				.fadeOutTime = 0.35f;
			SceneName.SaveSceneStatistics("StartMenuUI");
			m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
				.fadeOutColorEnd = new Color(0f, 0f, 0f, 1f);
			m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
				.FadeOut("StartMenuUI");
		}
		else if (control.name == "Endless_Button" && eventType == 3)
		{
			m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
				.fadeOutTime = 0.35f;
			audioPlayer.PlayAudio("UI_scan");
			is_effing = true;
			map_eff_trans.MapEffIn();
			if (map_mode_bk != null)
			{
				map_mode_bk.frameName = "net_map";
			}
			if (map_mode_eff != null)
			{
				map_mode_eff.frameName = "z14";
			}
		}
		else if (control.name == "VS_Button" && eventType == 3)
		{
			m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
				.fadeOutTime = 0.35f;
			audioPlayer.PlayAudio("UI_scan");
			is_effing = true;
			vs_mode = true;
			map_eff_trans.MapEffIn();
			if (map_mode_bk != null)
			{
				map_mode_bk.frameName = "VSMap";
			}
			if (map_mode_eff != null)
			{
				map_mode_eff.frameName = "z14_1";
			}
		}
		else if (control.name == "Arena_Button" && eventType == 3)
		{
			m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
				.fadeOutTime = 1.4f;
			audioPlayer.PlayAudio("Battle");
			GameApp.GetInstance().GetGameState().endless_level = false;
			GameApp.GetInstance().GetGameState().endless_multiplayer = false;
			GameApp.GetInstance().GetGameState().hunting_level = ((control.name == prey_scene_name) ? true : false);
			GameApp.GetInstance().GetGameState().VS_mode = false;
			m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
				.fadeOutColorEnd = new Color(0f, 0f, 0f, 1f);
			if (GameApp.GetInstance().GetGameState().FirstTimeGame)
			{
				SceneName.SaveSceneStatistics("Zombie3D_Tutorial");
				m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
					.FadeOut("Zombie3D_Tutorial");
			}
			else
			{
				SceneName.SaveSceneStatistics("Zombie3D_Arena");
				m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
					.FadeOut("Zombie3D_Arena");
			}
		}
		else if (control.name == "Church_Button" && eventType == 3)
		{
			m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
				.fadeOutTime = 1.4f;
			audioPlayer.PlayAudio("Battle");
			Debug.Log("Zombie3D_Church");
			GameApp.GetInstance().GetGameState().endless_level = false;
			GameApp.GetInstance().GetGameState().endless_multiplayer = false;
			GameApp.GetInstance().GetGameState().hunting_level = ((control.name == prey_scene_name) ? true : false);
			GameApp.GetInstance().GetGameState().VS_mode = false;
			SceneName.SaveSceneStatistics("Zombie3D_Church");
			m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
				.fadeOutColorEnd = new Color(0f, 0f, 0f, 1f);
			m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
				.FadeOut("Zombie3D_Church");
		}
		else if (control.name == "Parking_Button" && eventType == 3)
		{
			m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
				.fadeOutTime = 1.4f;
			audioPlayer.PlayAudio("Battle");
			Debug.Log("Zombie3D_ParkingLot");
			GameApp.GetInstance().GetGameState().endless_level = false;
			GameApp.GetInstance().GetGameState().endless_multiplayer = false;
			GameApp.GetInstance().GetGameState().hunting_level = ((control.name == prey_scene_name) ? true : false);
			GameApp.GetInstance().GetGameState().VS_mode = false;
			SceneName.SaveSceneStatistics("Zombie3D_ParkingLot");
			m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
				.fadeOutColorEnd = new Color(0f, 0f, 0f, 1f);
			m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
				.FadeOut("Zombie3D_ParkingLot");
		}
		else if (control.name == "PowerStation_Button" && eventType == 3)
		{
			m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
				.fadeOutTime = 1.4f;
			audioPlayer.PlayAudio("Battle");
			Debug.Log("Zombie3D_PowerStation");
			GameApp.GetInstance().GetGameState().endless_level = false;
			GameApp.GetInstance().GetGameState().endless_multiplayer = false;
			GameApp.GetInstance().GetGameState().hunting_level = ((control.name == prey_scene_name) ? true : false);
			GameApp.GetInstance().GetGameState().VS_mode = false;
			SceneName.SaveSceneStatistics("Zombie3D_PowerStation");
			m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
				.fadeOutColorEnd = new Color(0f, 0f, 0f, 1f);
			m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
				.FadeOut("Zombie3D_PowerStation");
		}
		else if (control.name == "Recycle_Button" && eventType == 3)
		{
			m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
				.fadeOutTime = 1.4f;
			audioPlayer.PlayAudio("Battle");
			Debug.Log("Zombie3D_Recycle");
			GameApp.GetInstance().GetGameState().endless_level = false;
			GameApp.GetInstance().GetGameState().endless_multiplayer = false;
			GameApp.GetInstance().GetGameState().hunting_level = ((control.name == prey_scene_name) ? true : false);
			GameApp.GetInstance().GetGameState().VS_mode = false;
			SceneName.SaveSceneStatistics("Zombie3D_Recycle");
			m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
				.fadeOutColorEnd = new Color(0f, 0f, 0f, 1f);
			m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
				.FadeOut("Zombie3D_Recycle");
		}
		else if (control.name == "Village2_Button" && eventType == 3)
		{
			m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
				.fadeOutTime = 1.4f;
			audioPlayer.PlayAudio("Battle");
			string text = "Zombie3D_Village2_3g";
			Debug.Log(text);
			GameApp.GetInstance().GetGameState().endless_level = false;
			GameApp.GetInstance().GetGameState().endless_multiplayer = false;
			GameApp.GetInstance().GetGameState().hunting_level = ((control.name == prey_scene_name) ? true : false);
			GameApp.GetInstance().GetGameState().VS_mode = false;
			SceneName.SaveSceneStatistics(text);
			m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
				.fadeOutColorEnd = new Color(0f, 0f, 0f, 1f);
			m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
				.FadeOut(text);
		}
		else if (control.name == "Village_Button" && eventType == 3)
		{
			m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
				.fadeOutTime = 1.4f;
			audioPlayer.PlayAudio("Battle");
			Debug.Log("Zombie3D_Village");
			GameApp.GetInstance().GetGameState().endless_level = false;
			GameApp.GetInstance().GetGameState().endless_multiplayer = false;
			GameApp.GetInstance().GetGameState().hunting_level = ((control.name == prey_scene_name) ? true : false);
			GameApp.GetInstance().GetGameState().VS_mode = false;
			SceneName.SaveSceneStatistics("Zombie3D_Village");
			m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
				.fadeOutColorEnd = new Color(0f, 0f, 0f, 1f);
			m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
				.FadeOut("Zombie3D_Village");
		}
		else if (control.name == "Hospital_Button" && eventType == 3)
		{
			m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
				.fadeOutTime = 1.4f;
			audioPlayer.PlayAudio("Battle");
			Debug.Log("Zombie3D_Hospital");
			GameApp.GetInstance().GetGameState().endless_level = false;
			GameApp.GetInstance().GetGameState().endless_multiplayer = false;
			GameApp.GetInstance().GetGameState().hunting_level = ((control.name == prey_scene_name) ? true : false);
			GameApp.GetInstance().GetGameState().VS_mode = false;
			SceneName.SaveSceneStatistics("Zombie3D_Hospital");
			m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
				.fadeOutColorEnd = new Color(0f, 0f, 0f, 1f);
			m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
				.FadeOut("Zombie3D_Hospital");
		}
		else if (control.name == "Shop_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
				.fadeOutTime = 0.35f;
			SceneName.SaveSceneStatistics("ShopMenuTUI");
			m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
				.fadeOutColorEnd = new Color(0f, 0f, 0f, 1f);
			m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
				.FadeOut("ShopMenuTUI");
			GameApp.GetInstance().GetGameState().last_map_to_shop = "main_map";
		}
		else if (control.name == "Option_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			Debug.Log("Option_Button");
			m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
				.fadeOutTime = 0.35f;
			SceneName.SaveSceneStatistics("OptionTUI");
			m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
				.fadeOutColorEnd = new Color(0f, 0f, 0f, 1f);
			m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
				.FadeOut("OptionTUI");
		}
		else if (control.name == "Paper_Model_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			Debug.Log("Paper_Model_Button");
			m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
				.fadeOutTime = 0.35f;
			SceneName.SaveSceneStatistics("PaperJoyShowUI");
			m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
				.fadeOutColorEnd = new Color(0f, 0f, 0f, 1f);
			m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
				.FadeOut("PaperJoyShowUI");
		}
	}
}
