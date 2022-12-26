using System;
using System.Collections;
using System.Collections.Generic;
using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using UnityEngine;
using Zombie3D;

public class NetMapVSTUI : MonoBehaviour, TUIHandler
{
	private TUI m_tui;

	protected TUIInput[] input;

	public TUIButtonClick[] map_buttons;

	public TUIMeshSprite[] Map_States;

	public TUIMeshText label_money;

	protected AudioPlayer audioPlayer = new AudioPlayer();

	public GameObject NetModePanel;

	public string cur_scene_name = string.Empty;

	public GameObject Indicator_Panel;

	public GameObject Msg_Box_Panel;

	public TUIMeshSprite Map_icon;

	public MapEffTrans map_eff_trans;

	protected bool is_effing;

	protected Hashtable maps_state = new Hashtable();

	protected float frash_map_time;

	protected string next_scene = string.Empty;

	protected bool to_shop;

	protected List<string> map_group_list = new List<string>();

	private SmartFox smartFox;

	public string serverName = "127.0.0.1";

	public int serverPort = 9933;

	public string zone = "BasicExamples";

	public bool debug = true;

	private string username = string.Empty;

	private bool sfs_inited;

	private float sfs_init_time;

	private string m_authChallenge = string.Empty;

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
		if (GameObject.Find("SFSTimeManager") == null)
		{
			GameObject gameObject2 = UnityEngine.Object.Instantiate(Resources.Load("Prefabs/SFSTimeManager")) as GameObject;
			gameObject2.name = "SFSTimeManager";
			UnityEngine.Object.DontDestroyOnLoad(gameObject2);
		}
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
		map_eff_trans.m_MapEffTransOut = OnMapEffOut;
		OpenClickPlugin.Hide();
		Resources.UnloadUnusedAssets();
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			username = GameApp.GetInstance().GetGameState().nick_name + "|" + Utils.GetMacAddr();
		}
		else if (Application.platform == RuntimePlatform.Android)
		{
			username = GameApp.GetInstance().GetGameState().nick_name + "|" + SystemInfo.deviceUniqueIdentifier;
		}
		else
		{
			username = GameApp.GetInstance().GetGameState().nick_name + "|" + Utils.GetIOSDay() + Utils.GetIOSHour() + Utils.GetIOSMin() + Utils.GetIOSSec();
		}
		if (SmartFoxConnection.IsInitialized)
		{
			smartFox = SmartFoxConnection.Connection;
		}
		else
		{
			smartFox = new SmartFox(debug);
		}
		smartFox.AddEventListener(SFSEvent.CONNECTION, OnConnection);
		smartFox.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
		smartFox.AddEventListener(SFSEvent.LOGIN, OnLogin);
		smartFox.AddEventListener(SFSEvent.LOGOUT, OnLogout);
		smartFox.AddEventListener(SFSEvent.UDP_INIT, OnUdpInit);
		smartFox.AddEventListener(SFSEvent.EXTENSION_RESPONSE, OnExtensionResponse);
		smartFox.AddEventListener(SFSEvent.ROOM_JOIN, OnJoinRoom);
		smartFox.AddEventListener(SFSEvent.ROOM_JOIN_ERROR, OnJoinRoomError);
		if (!smartFox.IsConnected)
		{
			Debug.Log("connecting server...");
			smartFox.Connect(serverName, serverPort);
			Indicator_Panel.GetComponent<IndicatorTUI>().SetContent("CONNECTING...");
			Indicator_Panel.GetComponent<IndicatorTUI>().Show();
			sfs_inited = false;
		}
		else
		{
			SmartFoxConnection.Connection = smartFox;
			Debug.Log("Already connected, Sending login request user name:" + username);
			smartFox.Send(new LoginRequest(username, string.Empty, zone));
			sfs_inited = true;
		}
		ClearMapState();
		UpdateMapStateShow();
		if (GameApp.GetInstance().GetGameState().first_vs_mode == 1)
		{
			GameApp.GetInstance().GetGameState().first_vs_mode = 0;
			GameApp.GetInstance().PlayerPrefsSave();
		}
		List<string> list = new List<string>();
		for (int j = 1; j < 9; j++)
		{
			if (j == 7)
			{
				list.Add("Zombie3D_Village2");
			}
			else
			{
				list.Add(SceneName.GetNetMapName(j));
			}
		}
		map_group_list = RandomSortList(list);
	}

	private void FixedUpdate()
	{
		if (!sfs_inited)
		{
			sfs_init_time += Time.deltaTime;
			if (sfs_init_time >= 15f)
			{
				Debug.Log("Connect time out.");
				ConnectServerFailed();
				SmartFoxConnection.UnregisterSFSSceneCallbacks();
			}
		}
		smartFox.ProcessEvents();
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

	private void OnDestroy()
	{
		SmartFoxConnection.UnregisterSFSSceneCallbacks();
		if (SFSHeartBeat.Instance != null)
		{
			SFSHeartBeat.Instance.reverse_heart_timeout_delegate = null;
			SFSHeartBeat.Instance.reverse_heart_renew_delegate = null;
			SFSHeartBeat.Instance.reverse_heart_waiting_delegate = null;
		}
	}

	private void OnMapEffOut()
	{
		SmartFoxConnection.UnregisterSFSSceneCallbacks();
		SmartFoxConnection.Disconnect();
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
			smartFox.Send(new LogoutRequest());
		}
		else if (control.name == "Arena_Button" && eventType == 3)
		{
			Map_icon.frameName = "Map_icon_" + control.name;
			cur_scene_name = "Zombie3D_Arena";
			SwitchMapGroupRoom();
		}
		else if (control.name == "Church_Button" && eventType == 3)
		{
			Map_icon.frameName = "Map_icon_" + control.name;
			cur_scene_name = "Zombie3D_Church";
			SwitchMapGroupRoom();
		}
		else if (control.name == "Parking_Button" && eventType == 3)
		{
			Map_icon.frameName = "Map_icon_" + control.name;
			cur_scene_name = "Zombie3D_ParkingLot";
			SwitchMapGroupRoom();
		}
		else if (control.name == "PowerStation_Button" && eventType == 3)
		{
			Map_icon.frameName = "Map_icon_" + control.name;
			cur_scene_name = "Zombie3D_PowerStation";
			SwitchMapGroupRoom();
		}
		else if (control.name == "Recycle_Button" && eventType == 3)
		{
			Map_icon.frameName = "Map_icon_" + control.name;
			cur_scene_name = "Zombie3D_Recycle";
			SwitchMapGroupRoom();
		}
		else if (control.name == "Village2_Button" && eventType == 3)
		{
			Map_icon.frameName = "Map_icon_" + control.name;
			string text = "Zombie3D_Village2";
			cur_scene_name = text;
			SwitchMapGroupRoom();
		}
		else if (control.name == "Village_Button" && eventType == 3)
		{
			Map_icon.frameName = "Map_icon_" + control.name;
			cur_scene_name = "Zombie3D_Village";
			SwitchMapGroupRoom();
		}
		else if (control.name == "Hospital_Button" && eventType == 3)
		{
			Map_icon.frameName = "Map_icon_" + control.name;
			cur_scene_name = "Zombie3D_Hospital";
			SwitchMapGroupRoom();
		}
		else if (control.name == "Shop_Button" && eventType == 3)
		{
			to_shop = true;
			smartFox.Send(new LogoutRequest());
		}
		else if (control.name == "Quick_Button" && eventType == 3)
		{
			QuickMach();
		}
		else
		{
			if (!(control.name == "Msg_OK_Button") || eventType != 3)
			{
				return;
			}
			Msg_Box_Panel.GetComponent<MsgBoxDelegate>().Hide();
			if (Msg_Box_Panel.GetComponent<MsgBoxDelegate>().m_type == MsgBoxType.MultiToturial)
			{
				GameApp.GetInstance().GetGameState().vs_toturial_triger_map = 0;
			}
			else if (Msg_Box_Panel.GetComponent<MsgBoxDelegate>().m_type != MsgBoxType.JoinRommFailed)
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
			GameApp.GetInstance().PlayerPrefsSave();
		}
	}

	public void SwitchMapGroupRoom()
	{
		if (cur_scene_name != string.Empty)
		{
			int count = smartFox.GetRoomListFromGroup(cur_scene_name).Count;
			Debug.Log(cur_scene_name + " room count:" + count);
			GameApp.GetInstance().GetGameState().cur_net_map = cur_scene_name;
			SceneName.SaveSceneStatistics("VSRoomTUI");
			m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
				.fadeOutColorEnd = new Color(0f, 0f, 0f, 1f);
			m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
				.FadeOut("VSRoomTUI");
		}
		else
		{
			Debug.Log("Error map info.");
		}
	}

	public void GetMapsState()
	{
		if (!smartFox.IsConnected)
		{
			return;
		}
		string empty = string.Empty;
		for (int i = 1; i < 9; i++)
		{
			empty = SceneName.GetNetMapName(i);
			if (empty == "Zombie3D_Village2_3g")
			{
				empty = "Zombie3D_Village2";
			}
			maps_state[i] = smartFox.GetRoomListFromGroup(empty).Count;
		}
		UpdateMapStateShow();
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

	private void ClearMapState()
	{
		for (int i = 1; i < 9; i++)
		{
			maps_state[i] = 0;
		}
	}

	private void ProcessMapState(string map_name)
	{
		int netMapIndex = SceneName.GetNetMapIndex(map_name);
		if (maps_state.ContainsKey(netMapIndex))
		{
			maps_state[netMapIndex] = (int)maps_state[netMapIndex] + 1;
		}
		else
		{
			maps_state[netMapIndex] = 1;
		}
	}

	private void ConnectServerFailed()
	{
		if (Indicator_Panel != null)
		{
			Indicator_Panel.GetComponent<IndicatorTUI>().Hide();
		}
		if (Msg_Box_Panel != null)
		{
			Msg_Box_Panel.GetComponent<MsgBoxDelegate>().Show("UNABLE TO CONNECT.", MsgBoxType.ContectingTimeout);
		}
		sfs_inited = true;
	}

	public void OnConnection(BaseEvent evt)
	{
		bool flag = (bool)evt.Params["success"];
		Debug.Log("On Connection callback got: " + flag);
		if (flag)
		{
			SmartFoxConnection.Connection = smartFox;
			Debug.Log("Sending login request user name:" + username);
			smartFox.Send(new LoginRequest(username, string.Empty, zone));
			sfs_inited = true;
		}
		else
		{
			ConnectServerFailed();
		}
	}

	public void OnConnectionLost(BaseEvent evt)
	{
		Debug.Log("Connection was lost, Reason: " + (string)evt.Params["reason"]);
		SmartFoxConnection.UnregisterSFSSceneCallbacks();
		if ((string)evt.Params["reason"] == "manual")
		{
			if (to_shop)
			{
				audioPlayer.PlayAudio("Button");
				SceneName.SaveSceneStatistics("ShopMenuTUI");
				m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
					.fadeOutColorEnd = new Color(0f, 0f, 0f, 1f);
				m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
					.FadeOut("ShopMenuTUI");
				GameApp.GetInstance().GetGameState().last_map_to_shop = "vs_net_map";
				return;
			}
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
		else
		{
			ConnectServerFailed();
		}
	}

	private void OnLogout(BaseEvent evt)
	{
		SmartFoxConnection.UnregisterSFSSceneCallbacks();
		if (to_shop)
		{
			audioPlayer.PlayAudio("Button");
			SceneName.SaveSceneStatistics("ShopMenuTUI");
			m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
				.fadeOutColorEnd = new Color(0f, 0f, 0f, 1f);
			m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
				.FadeOut("ShopMenuTUI");
			GameApp.GetInstance().GetGameState().last_map_to_shop = "vs_net_map";
			return;
		}
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

	public void OnDebugMessage(BaseEvent evt)
	{
		string text = (string)evt.Params["message"];
		Debug.Log("[SFS DEBUG] " + text);
	}

	private void OnLogin(BaseEvent evt)
	{
		Debug.Log("Login ok my name: " + ((User)evt.Params["user"]).Name);
		GetMapsState();
		smartFox.InitUDP(serverName, serverPort);
	}

	private void OnLoginError(BaseEvent evt)
	{
		Debug.Log("Login Failed. Reason: " + (string)evt.Params["errorMessage"]);
		ConnectServerFailed();
	}

	private void OnUdpInit(BaseEvent evt)
	{
		if (!(bool)evt.Params["success"])
		{
			Debug.Log("Sorry, UDP is not available. Initialization failed");
			ConnectServerFailed();
			return;
		}
		Debug.Log("UDPInit is OK.");
		if (Application.platform == RuntimePlatform.Android)
		{
			OnAuthResponse(0);
		}
		else
		{
			AuthChallenge();
		}
	}

	private void OnExtensionResponse(BaseEvent evt)
	{
		string text = (string)evt.Params["cmd"];
		SFSObject sFSObject = (SFSObject)evt.Params["params"];
		switch (text)
		{
		case "Zone.Common.AuthChallenge":
			m_authChallenge = sFSObject.GetUtfString("authChallenge");
			m_authChallenge = SFSAuth.Encode(m_authChallenge);
			Debug.Log(m_authChallenge);
			AuthResponse();
			break;
		case "Zone.Common.AuthResponse":
		{
			int @int = sFSObject.GetInt("code");
			OnAuthResponse(@int);
			break;
		}
		case "Zone.Common.Keepalive":
			HandleServerTime(sFSObject);
			break;
		}
	}

	private void AuthChallenge()
	{
		smartFox.Send(new ExtensionRequest("Zone.Common.AuthChallenge", new SFSObject()));
	}

	private void AuthResponse()
	{
		SFSObject sFSObject = new SFSObject();
		sFSObject.PutUtfString("authResponse", m_authChallenge);
		smartFox.Send(new ExtensionRequest("Zone.Common.AuthResponse", sFSObject));
	}

	private void OnAuthResponse(int code)
	{
		Debug.Log("Zone.Common.AuthResponse:" + code);
		if (Indicator_Panel != null)
		{
			Indicator_Panel.GetComponent<IndicatorTUI>().Hide();
		}
		if (Msg_Box_Panel != null && GameApp.GetInstance().GetGameState().vs_toturial_triger_map == 1)
		{
			Msg_Box_Panel.GetComponent<MsgBoxDelegate>().Show(VSGameToturial.Msg_Content[0], MsgBoxType.MultiToturial);
		}
		if (code == 0)
		{
			TimeManager.Instance.Init();
			SFSHeartBeat.Instance.Init();
			SFSHeartBeat.Instance.reverse_heart_timeout_delegate = OnReverseHearTimeout;
			SFSHeartBeat.Instance.reverse_heart_renew_delegate = OnReverseHearRenew;
			SFSHeartBeat.Instance.reverse_heart_waiting_delegate = OnReverseHearWaiting;
		}
		else
		{
			Debug.Log("AuthResponse error code.");
		}
	}

	private void HandleServerTime(ISFSObject dt)
	{
		long @long = dt.GetLong("serverTime");
		TimeManager.Instance.Synchronize(Convert.ToDouble(@long));
	}

	public void OnReverseHearWaiting()
	{
		if (Indicator_Panel != null)
		{
			Indicator_Panel.GetComponent<IndicatorTUI>().SetContent("WAITING FOR SERVER.");
			Indicator_Panel.GetComponent<IndicatorTUI>().Show();
		}
	}

	public void OnReverseHearRenew()
	{
		if (Indicator_Panel != null)
		{
			Indicator_Panel.GetComponent<IndicatorTUI>().Hide();
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
			Msg_Box_Panel.GetComponent<MsgBoxDelegate>().Show("YOU WERE DISCONNECTED.", MsgBoxType.ContectingTimeout);
		}
		SmartFoxConnection.UnregisterSFSSceneCallbacks();
		SmartFoxConnection.Disconnect();
	}

	public List<T> RandomSortList<T>(List<T> ListT)
	{
		System.Random random = new System.Random();
		List<T> list = new List<T>();
		foreach (T item in ListT)
		{
			list.Insert(random.Next(list.Count), item);
		}
		return list;
	}

	private void QuickMach()
	{
		List<Room> list = null;
		foreach (string item in map_group_list)
		{
			list = smartFox.GetRoomListFromGroup(item);
			foreach (Room item2 in list)
			{
				if (!item2.IsPasswordProtected && item2.UserCount < 8)
				{
					cur_scene_name = item;
					int count = smartFox.GetRoomListFromGroup(cur_scene_name).Count;
					GameApp.GetInstance().GetGameState().cur_net_map = cur_scene_name;
					Debug.Log("Random jion room:" + item2.Name);
					smartFox.Send(new JoinRoomRequest(item2.Name));
					return;
				}
			}
		}
		Msg_Box_Panel.GetComponent<MsgBoxDelegate>().Show("No match found! Please \ntry again.", MsgBoxType.JoinRommFailed);
	}

	private void OnJoinRoom(BaseEvent evt)
	{
		Debug.Log("Room joined successfully: " + (Room)evt.Params["room"]);
		SceneName.SaveSceneStatistics("VSRoomOwnerTUI");
		m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
			.FadeOut("VSRoomOwnerTUI");
	}

	private void OnJoinRoomError(BaseEvent evt)
	{
		Debug.Log("Join Room failure: " + (string)evt.Params["errorMessage"] + " errorCode:" + (short)evt.Params["errorCode"]);
		string content = string.Empty;
		if ((short)evt.Params["errorCode"] == 21)
		{
			content = "INCORRECT PASSWORD";
		}
		else if ((short)evt.Params["errorCode"] == 20)
		{
			content = "No match found! Please \ntry again.";
		}
		Msg_Box_Panel.GetComponent<MsgBoxDelegate>().Show(content, MsgBoxType.JoinRommFailed);
	}
}
