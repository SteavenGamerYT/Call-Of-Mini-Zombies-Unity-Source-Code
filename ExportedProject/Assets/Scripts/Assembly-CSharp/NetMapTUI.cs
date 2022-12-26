using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using Zombie3D;

public class NetMapTUI : MonoBehaviour, TUIHandler
{
	private TUI m_tui;

	protected TUIInput[] input;

	public TUIButtonClick[] map_buttons;

	public TUIMeshSprite[] Map_States;

	public TUIMeshText label_money;

	protected AudioPlayer audioPlayer = new AudioPlayer();

	public GameObject NetModePanel;

	public string cur_scene_name;

	public GameObject Indicator_Panel;

	public GameObject Msg_Box_Panel;

	public TUIMeshSprite Map_icon;

	protected NetworkObj net_com;

	protected NetworkObj net_com_hall;

	protected string next_scene = string.Empty;

	public TUIButtonClickAni achi_button;

	public MapEffTrans map_eff_trans;

	protected bool is_effing;

	protected Hashtable maps_state = new Hashtable();

	protected float frash_map_time;

	protected bool net_com_inited;

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
		if (GameApp.GetInstance().GetGameState().last_scene == "MainMapTUI")
		{
			m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
				.fadeInColorBegin = new Color(0f, 0f, 0f, 1f);
		}
		else
		{
			m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
				.fadeInColorBegin = new Color(0f, 0f, 0f, 1f);
		}
		m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
			.FadeIn();
		GameObject[] array = GameObject.FindGameObjectsWithTag("Map_Button");
		map_buttons = new TUIButtonClick[array.Length];
		label_money.text = "$" + GameApp.GetInstance().GetGameState().GetCash()
			.ToString("N0");
		for (int i = 0; i < array.Length; i++)
		{
			map_buttons[i] = array[i].GetComponent<TUIButtonClick>();
		}
		Transform folderTrans = base.transform.Find("Audio");
		audioPlayer.AddAudio(folderTrans, "Button", true);
		audioPlayer.AddAudio(folderTrans, "Battle", true);
		audioPlayer.AddAudio(folderTrans, "UI_scan", true);
		GameApp.GetInstance().InitForMultiplay();
		if (Msg_Box_Panel != null && GameApp.GetInstance().GetGameState().multi_toturial_triger_map == 1)
		{
			Msg_Box_Panel.GetComponent<MsgBoxDelegate>().Show(MultiGameToturial.Msg_Content[1], MsgBoxType.MultiToturial);
			achi_button.PlayAnimation();
		}
		map_eff_trans.m_MapEffTransOut = OnMapEffOut;
		OpenClickPlugin.Hide();
		Resources.UnloadUnusedAssets();
		for (int j = 1; j < 9; j++)
		{
			maps_state[j] = 0;
		}
		UpdateMapStateShow();
		CheckNetWorkCom();
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
			frash_map_time += Time.deltaTime;
			if (frash_map_time >= 5f)
			{
				frash_map_time = 0f;
				GetMapsState();
			}
		}
	}

	private void OnMapEffOut()
	{
		SceneName.SaveSceneStatistics("MainMapTUI");
		m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
			.fadeOutColorEnd = new Color(0f, 0f, 0f, 1f);
		m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
			.FadeOut("MainMapTUI");
	}

	public void HandleEvent(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (control.name == "Back_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("UI_scan");
			is_effing = true;
			map_eff_trans.MapEffOut();
			TUIMeshSprite[] map_States = Map_States;
			TUIMeshSprite[] array = map_States;
			foreach (TUIMeshSprite tUIMeshSprite in array)
			{
				tUIMeshSprite.gameObject.active = false;
			}
		}
		else if (control.name == "Endless_Button" && eventType == 3)
		{
			Debug.Log("to main map.");
			audioPlayer.PlayAudio("Button");
			SceneName.SaveSceneStatistics("MainMapTUI");
			m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
				.fadeOutColorEnd = new Color(1f, 1f, 1f, 1f);
			m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
				.FadeOut("MainMapTUI");
		}
		else if (control.name == "Arena_Button" && eventType == 3)
		{
			Map_icon.frameName = "Map_icon_" + control.name;
			cur_scene_name = "Zombie3D_Arena";
			NetModePanel.transform.localPosition = new Vector3(0f, 0f, NetModePanel.transform.localPosition.z);
		}
		else if (control.name == "Church_Button" && eventType == 3)
		{
			Map_icon.frameName = "Map_icon_" + control.name;
			cur_scene_name = "Zombie3D_Church";
			NetModePanel.transform.localPosition = new Vector3(0f, 0f, NetModePanel.transform.localPosition.z);
		}
		else if (control.name == "Parking_Button" && eventType == 3)
		{
			Map_icon.frameName = "Map_icon_" + control.name;
			cur_scene_name = "Zombie3D_ParkingLot";
			NetModePanel.transform.localPosition = new Vector3(0f, 0f, NetModePanel.transform.localPosition.z);
		}
		else if (control.name == "PowerStation_Button" && eventType == 3)
		{
			Map_icon.frameName = "Map_icon_" + control.name;
			cur_scene_name = "Zombie3D_PowerStation";
			NetModePanel.transform.localPosition = new Vector3(0f, 0f, NetModePanel.transform.localPosition.z);
		}
		else if (control.name == "Recycle_Button" && eventType == 3)
		{
			Map_icon.frameName = "Map_icon_" + control.name;
			cur_scene_name = "Zombie3D_Recycle";
			NetModePanel.transform.localPosition = new Vector3(0f, 0f, NetModePanel.transform.localPosition.z);
		}
		else if (control.name == "Village2_Button" && eventType == 3)
		{
			Map_icon.frameName = "Map_icon_" + control.name;
			string text = "Zombie3D_Village2_3g";
			cur_scene_name = text;
			NetModePanel.transform.localPosition = new Vector3(0f, 0f, NetModePanel.transform.localPosition.z);
		}
		else if (control.name == "Village_Button" && eventType == 3)
		{
			Map_icon.frameName = "Map_icon_" + control.name;
			cur_scene_name = "Zombie3D_Village";
			NetModePanel.transform.localPosition = new Vector3(0f, 0f, NetModePanel.transform.localPosition.z);
		}
		else if (control.name == "Hospital_Button" && eventType == 3)
		{
			Map_icon.frameName = "Map_icon_" + control.name;
			cur_scene_name = "Zombie3D_Hospital";
			NetModePanel.transform.localPosition = new Vector3(0f, 0f, NetModePanel.transform.localPosition.z);
		}
		else if (control.name == "Shop_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			SceneName.SaveSceneStatistics("ShopMenuTUI");
			m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
				.fadeOutColorEnd = new Color(0f, 0f, 0f, 1f);
			m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
				.FadeOut("ShopMenuTUI");
			GameApp.GetInstance().GetGameState().last_map_to_shop = "net_map";
		}
		else if (control.name == "Option_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
		}
		else if (control.name == "Achivment_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			Debug.Log("Achivment_Button");
			m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
				.fadeOutTime = 0.35f;
			SceneName.SaveSceneStatistics("MultiAchievementTUI");
			m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
				.fadeOutColorEnd = new Color(0f, 0f, 0f, 1f);
			m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
				.FadeOut("MultiAchievementTUI");
		}
		else if (control.name == "Solo_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			GameApp.GetInstance().GetGameState().endless_level = true;
			GameApp.GetInstance().GetGameState().endless_multiplayer = false;
			GameApp.GetInstance().GetGameState().VS_mode = false;
			SceneName.SaveSceneStatistics(cur_scene_name);
			m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
				.fadeOutColorEnd = new Color(0f, 0f, 0f, 1f);
			m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
				.FadeOut(cur_scene_name);
			NetworkObj.DestroyNetCom();
		}
		else if (control.name == "Coop_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			GameApp.GetInstance().GetGameState().cur_net_map = cur_scene_name;
			next_scene = "MultiplayRoomTUI";
			CheckNetWorkCom();
			NetModePanel.transform.localPosition = new Vector3(0f, -2000f, NetModePanel.transform.localPosition.z);
		}
		else if (control.name == "Create_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			GameApp.GetInstance().GetGameState().cur_net_map = cur_scene_name;
			next_scene = "RoomOwnerTUI";
			CheckNetWorkCom();
			NetModePanel.transform.localPosition = new Vector3(0f, -2000f, NetModePanel.transform.localPosition.z);
		}
		else if (control.name == "Msg_OK_Button" && eventType == 3)
		{
			Msg_Box_Panel.GetComponent<MsgBoxDelegate>().Hide();
			GameApp.GetInstance().GetGameState().multi_toturial_triger_map = 0;
			GameApp.GetInstance().PlayerPrefsSave();
		}
		else if (control.name == "Close_Mode_Button" && eventType == 3)
		{
			NetModePanel.transform.localPosition = new Vector3(0f, -2000f, NetModePanel.transform.localPosition.z);
		}
	}

	public void ToNextScene()
	{
		if (next_scene == "MultiplayRoomTUI")
		{
			SceneName.SaveSceneStatistics(next_scene);
			m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
				.fadeOutColorEnd = new Color(0f, 0f, 0f, 1f);
			m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
				.FadeOut(next_scene);
		}
		else if (next_scene == "RoomOwnerTUI")
		{
			Packet packet = CGCreateRoomPacket.MakePacket((uint)SceneName.GetNetMapIndex(cur_scene_name), (long)(Time.time * 1000f), GameApp.GetInstance().GetGameState().nick_name, (uint)GameApp.GetInstance().GetGameState().Avatar, (uint)GameApp.GetInstance().GetGameState().LevelNum);
			net_com.Send(packet);
			Debug.Log("Ready Create map:" + cur_scene_name);
			Indicator_Panel.GetComponent<IndicatorTUI>().SetContent("CONNECTING...");
			Indicator_Panel.GetComponent<IndicatorTUI>().Show();
		}
	}

	public void GetMapsState()
	{
		if (NetworkObj.tcp_contected_status == NetworkObj.TCP_CONTECTED_STATUS.GameServer)
		{
			Packet packet = CGMapStatePacket.MakePacket();
			net_com.Send(packet);
		}
	}

	private void OnDestroy()
	{
		if (net_com != null)
		{
			net_com.connect_delegate = null;
			net_com.packet_delegate = null;
			net_com.close_delegate = null;
			net_com.contecting_timeout = null;
			net_com.contecting_lost = null;
			net_com.reverse_heart_timeout_delegate = null;
			net_com.reverse_heart_renew_delegate = null;
			net_com.reverse_heart_waiting_delegate = null;
			net_com = null;
		}
		if (net_com_hall != null)
		{
			net_com_hall.connect_delegate = null;
			net_com_hall.packet_delegate = null;
			net_com_hall.close_delegate = null;
			net_com_hall.contecting_timeout = null;
			net_com_hall.contecting_lost = null;
			net_com_hall = null;
		}
	}

	public void CheckNetWorkCom()
	{
		if (NetworkObj.tcp_contected_status == NetworkObj.TCP_CONTECTED_STATUS.None)
		{
			Debug.Log("NetWorkCom init...");
			GameScript.CheckNetWorkCom();
			net_com_inited = false;
		}
		if (!net_com_inited)
		{
			net_com = GameApp.GetInstance().GetGameState().net_com;
			net_com.connect_delegate = OnConnected;
			net_com.packet_delegate = OnPacket;
			net_com.close_delegate = OnClosed;
			net_com.contecting_timeout = OnContectingTimeout;
			net_com.contecting_lost = OnContectingLost;
			net_com.reverse_heart_timeout_delegate = OnReverseHearTimeout;
			net_com.reverse_heart_renew_delegate = OnReverseHearRenew;
			net_com.reverse_heart_waiting_delegate = OnReverseHearWaiting;
			net_com_hall = GameApp.GetInstance().GetGameState().net_com_hall;
			net_com_hall.connect_delegate = OnConnected;
			net_com_hall.packet_delegate = OnPacket;
			net_com_hall.close_delegate = OnClosed;
			net_com_hall.contecting_timeout = OnContectingTimeout;
			net_com_hall.contecting_lost = OnContectingLost;
			net_com_inited = true;
		}
		Indicator_Panel.GetComponent<IndicatorTUI>().SetContent("CONNECTING...");
		Indicator_Panel.GetComponent<IndicatorTUI>().Show();
		net_com.ClearNetUserArr();
		if (NetworkObj.tcp_contected_status == NetworkObj.TCP_CONTECTED_STATUS.None)
		{
			net_com_hall.GoHall();
		}
		else if (NetworkObj.tcp_contected_status == NetworkObj.TCP_CONTECTED_STATUS.GameServer)
		{
			ToNextScene();
		}
	}

	public void UpdateMapStateShow()
	{
		int num = 1;
		TUIMeshSprite[] map_States = Map_States;
		TUIMeshSprite[] array = map_States;
		foreach (TUIMeshSprite tUIMeshSprite in array)
		{
			if (tUIMeshSprite.gameObject.active)
			{
				if ((int)maps_state[num] == 0)
				{
					tUIMeshSprite.frameName = "map_state0";
				}
				else if ((int)maps_state[num] > 0 && (int)maps_state[num] <= 5)
				{
					tUIMeshSprite.frameName = "map_state1";
				}
				else if ((int)maps_state[num] > 5 && (int)maps_state[num] <= 20)
				{
					tUIMeshSprite.frameName = "map_state2";
				}
				else
				{
					tUIMeshSprite.frameName = "map_state3";
				}
			}
			num++;
		}
	}

	public void OnConnected()
	{
		if (NetworkObj.tcp_contected_status == NetworkObj.TCP_CONTECTED_STATUS.Hall)
		{
			Debug.Log("Hall Connected...");
			net_com_hall.GetHallList();
		}
		else if (NetworkObj.tcp_contected_status == NetworkObj.TCP_CONTECTED_STATUS.GameServer)
		{
			Indicator_Panel.GetComponent<IndicatorTUI>().Hide();
			Debug.Log("Game Server Connected...");
			UnityEngine.Object.Destroy(net_com_hall.gameObject);
			net_com_hall = null;
			Indicator_Panel.GetComponent<IndicatorTUI>().SetContent("CONNECTING...");
			Indicator_Panel.GetComponent<IndicatorTUI>().Show();
			GetMapsState();
		}
	}

	public void OnPacket(Packet packet)
	{
		uint val = 0u;
		if (!packet.WatchUInt32(ref val, 4))
		{
			return;
		}
		if (NetworkObj.tcp_contected_status == NetworkObj.TCP_CONTECTED_STATUS.Hall)
		{
			PROTOCOLS_HALL pROTOCOLS_HALL = (PROTOCOLS_HALL)val;
			if (pROTOCOLS_HALL == PROTOCOLS_HALL.HC_GAMESERVERLIST)
			{
				GetGameServerList(packet);
			}
		}
		else if (NetworkObj.tcp_contected_status == NetworkObj.TCP_CONTECTED_STATUS.GameServer)
		{
			switch (val)
			{
			case 1048576u:
				Debug.Log("CG_HEARTBEAT revceived!!");
				break;
			case 4099u:
				CreateRoom(packet);
				break;
			case 4110u:
				ProcessMapState(packet);
				break;
			}
		}
	}

	public void CreateRoom(Packet packet)
	{
		GCCreateRoomPacket gCCreateRoomPacket = new GCCreateRoomPacket();
		if (!gCCreateRoomPacket.ParserPacket(packet))
		{
			return;
		}
		if (Indicator_Panel != null)
		{
			Indicator_Panel.GetComponent<IndicatorTUI>().Hide();
		}
		if (gCCreateRoomPacket.m_iResult == 0)
		{
			float num = (Time.time - (float)gCCreateRoomPacket.m_lLocalTime / 1000f) / 2f;
			net_com.m_netUserInfo.SetNetUserInfo(true, GameApp.GetInstance().GetGameState().nick_name, (int)gCCreateRoomPacket.m_iRoomId, (int)gCCreateRoomPacket.m_iUserId, num, SceneName.GetNetMapIndex(cur_scene_name), 0, (int)GameApp.GetInstance().GetGameState().Avatar, GameApp.GetInstance().GetGameState().LevelNum);
			Debug.Log("Create map now! ping : " + num + "map id:" + SceneName.GetNetMapIndex(cur_scene_name));
			SceneName.SaveSceneStatistics("RoomOwnerTUI");
			m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
				.FadeOut("RoomOwnerTUI");
			if (net_com.m_netUserInfo.is_master)
			{
				net_com.ClearNetUserArr();
			}
		}
		else
		{
			Debug.Log("result error : " + gCCreateRoomPacket.m_iResult);
			if (Msg_Box_Panel != null)
			{
				Msg_Box_Panel.GetComponent<MsgBoxDelegate>().Show("Unable to create room, \nplease try again.".ToUpper(), MsgBoxType.CrateRommFailed);
			}
		}
	}

	public void ProcessMapState(Packet packet)
	{
		GCMapStatePacket gCMapStatePacket = new GCMapStatePacket();
		if (!gCMapStatePacket.ParserPacket(packet))
		{
			return;
		}
		if ((bool)Indicator_Panel)
		{
			Indicator_Panel.GetComponent<IndicatorTUI>().Hide();
		}
		int num = 0;
		for (int i = 1; i < 9; i++)
		{
			if (gCMapStatePacket.MapState.ContainsKey(i))
			{
				maps_state[i] = gCMapStatePacket.MapState[i];
			}
			else
			{
				maps_state[i] = 0;
			}
			num += (int)maps_state[i];
		}
		Debug.Log("ProcessMapState Room count:" + num);
		UpdateMapStateShow();
	}

	public void OnClosed()
	{
		NetworkObj.DestroyNetCom();
		if (Indicator_Panel != null)
		{
			Indicator_Panel.GetComponent<IndicatorTUI>().Hide();
		}
		if (Msg_Box_Panel != null)
		{
			Msg_Box_Panel.GetComponent<MsgBoxDelegate>().Show("YOU WERE DISCONNECTED.", MsgBoxType.ContectingLost);
		}
	}

	public void OnContectingLost()
	{
		UnityEngine.Object.Destroy(net_com.gameObject);
		if (net_com_hall != null)
		{
			UnityEngine.Object.Destroy(net_com_hall.gameObject);
		}
		if (Indicator_Panel != null)
		{
			Indicator_Panel.GetComponent<IndicatorTUI>().Hide();
		}
		if (Msg_Box_Panel != null)
		{
			Msg_Box_Panel.GetComponent<MsgBoxDelegate>().Show("YOU WERE DISCONNECTED.", MsgBoxType.ContectingLost);
		}
	}

	public void OnContectingTimeout()
	{
		Debug.Log("Server Connecting Timeount...");
		UnityEngine.Object.Destroy(net_com.gameObject);
		if (net_com_hall != null)
		{
			UnityEngine.Object.Destroy(net_com_hall.gameObject);
		}
		if (Indicator_Panel != null)
		{
			Indicator_Panel.GetComponent<IndicatorTUI>().Hide();
		}
		if (Msg_Box_Panel != null)
		{
			Msg_Box_Panel.GetComponent<MsgBoxDelegate>().Show("UNABLE TO CONNECT.", MsgBoxType.ContectingTimeout);
		}
	}

	public void GetGameServerList(Packet packet)
	{
		HCGameServerListPacket hCGameServerListPacket = new HCGameServerListPacket();
		if (!hCGameServerListPacket.ParserPacket(packet))
		{
			Debug.Log("HCGameServerListPacket ParserPacket error!!");
			return;
		}
		using (List<HCGameServerListPacket.GameServerInfo>.Enumerator enumerator = hCGameServerListPacket.m_GameServerList.GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				HCGameServerListPacket.GameServerInfo current = enumerator.Current;
				IPAddress iPAddress = new IPAddress(current.iServiceIP);
				Debug.LogError(current.iServiceIP);
				Debug.LogError(current.sServicePort);
				net_com.GoGameServer(iPAddress.ToString(), current.sServicePort);
			}
		}
	}

	public void OnReverseHearWaiting()
	{
		Debug.Log("OnReverseHearWaiting...");
	}

	public void OnReverseHearRenew()
	{
		if (Indicator_Panel != null)
		{
			Indicator_Panel.GetComponent<IndicatorTUI>().Hide();
			Debug.Log("OnReverseHearRenew...");
		}
	}

	public void OnReverseHearTimeout()
	{
		if (Indicator_Panel != null)
		{
			Indicator_Panel.GetComponent<IndicatorTUI>().Hide();
		}
		if (Msg_Box_Panel != null)
		{
			Msg_Box_Panel.GetComponent<MsgBoxDelegate>().Show("YOU WERE DISCONNECTED.", MsgBoxType.ContectingLost);
		}
	}
}
