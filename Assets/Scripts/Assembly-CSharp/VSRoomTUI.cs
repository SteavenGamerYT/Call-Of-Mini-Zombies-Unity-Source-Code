using System;
using System.Collections;
using System.Collections.Generic;
using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using Sfs2X.Entities.Variables;
using Sfs2X.Logging;
using Sfs2X.Requests;
using UnityEngine;
using Zombie3D;

public class VSRoomTUI : MonoBehaviour, TUIHandler
{
	protected const int room_max_page = 20;

	private TUI m_tui;

	protected TUIInput[] input;

	public GameObject Room_Panel_Obj;

	public GameObject Msg_Box_Panel;

	public GameObject Indicator_Panel;

	protected int cur_room_page;

	protected float last_refrash_page_time;

	public GameObject Toturial_Mask;

	public GameObject Page_Show;

	public GameObject Page_Up;

	public GameObject Page_Down;

	public GameObject Create_Button;

	public GameObject CreatePasswordPanel;

	public GameObject JoinPasswordPanel;

	public TUIMeshSprite password_enabel;

	private SmartFox smartFox;

	private Room cur_room;

	public static readonly string ExtName = "TrinitiExtension";

	public static readonly string ExtClass = "com.trinitigame.sfs.RoomExtension";

	public void Awake()
	{
		GameScript.CheckMenuResourceConfig();
		GameScript.CheckGlobalResourceConfig();
		if (GameObject.Find("Music") == null)
		{
			GameApp.GetInstance().InitForMenu();
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
		if (SmartFoxConnection.IsInitialized)
		{
			smartFox = SmartFoxConnection.Connection;
			smartFox.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
			smartFox.AddLogListener(LogLevel.DEBUG, OnDebugMessage);
			smartFox.AddEventListener(SFSEvent.ROOM_ADD, OnRoomAdd);
			smartFox.AddEventListener(SFSEvent.ROOM_CREATION_ERROR, OnRoomCreationError);
			smartFox.AddEventListener(SFSEvent.ROOM_VARIABLES_UPDATE, OnRoomVarsUpdate);
			smartFox.AddEventListener(SFSEvent.ROOM_JOIN, OnJoinRoom);
			smartFox.AddEventListener(SFSEvent.ROOM_JOIN_ERROR, OnJoinRoomError);
			smartFox.AddEventListener(SFSEvent.ROOM_GROUP_UNSUBSCRIBE, OnUnsubscribeRoomGroup);
			smartFox.AddEventListener(SFSEvent.EXTENSION_RESPONSE, OnExtensionResponse);
			SFSHeartBeat.Instance.reverse_heart_timeout_delegate = OnReverseHearTimeout;
			SFSHeartBeat.Instance.reverse_heart_renew_delegate = OnReverseHearRenew;
			SFSHeartBeat.Instance.reverse_heart_waiting_delegate = OnReverseHearWaiting;
		}
		else
		{
			Debug.Log("smartFox init error!");
		}
	}

	public void Start()
	{
		m_tui = TUI.Instance("TUI");
		m_tui.SetHandler(this);
		m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFadeEx>()
			.m_fadeout = OnSceneChange;
		m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
			.FadeIn();
		cur_room_page = 0;
		last_refrash_page_time = 0f;
		OpenClickPlugin.Hide();
		Resources.UnloadUnusedAssets();
		if (smartFox != null)
		{
			RefrashRoomList();
		}
		TUITextField component = CreatePasswordPanel.GetComponent<TUITextField>();
		component.style.overflow.left *= component.ResetResolution();
		component.style.padding.right *= component.ResetResolution();
		component.style.padding.left = component.style.padding.right + -component.style.overflow.left;
		component = JoinPasswordPanel.GetComponent<TUITextField>();
		component.style.overflow.left *= component.ResetResolution();
		component.style.overflow.right *= component.ResetResolution();
		component.style.padding.right *= component.ResetResolution();
		component.style.padding.left *= component.ResetResolution();
		CreatePasswordPanel.GetComponent<TUITextField>().callback = OnTextFieldActive;
		if (Msg_Box_Panel != null && GameApp.GetInstance().GetGameState().vs_toturial_triger_hall == 1)
		{
			Msg_Box_Panel.GetComponent<MsgBoxDelegate>().Show(VSGameToturial.Msg_Content[1], MsgBoxType.MultiToturial);
		}
	}

	private void FixedUpdate()
	{
		if (smartFox != null)
		{
			smartFox.ProcessEvents();
		}
	}

	public void OnSceneChange()
	{
		Debug.Log("OnSceneChange");
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

	private void OnTextFieldActive()
	{
		if (CreatePasswordPanel.GetComponent<TUITextField>().GetText().Length > 0)
		{
			password_enabel.frameName = "mima3";
		}
		else
		{
			password_enabel.frameName = "mima2";
		}
	}

	public void Update()
	{
		input = TUIInputManager.GetInput();
		for (int i = 0; i < input.Length; i++)
		{
			m_tui.HandleInput(input[i]);
		}
		last_refrash_page_time += Time.deltaTime;
		if (last_refrash_page_time >= 5f)
		{
			last_refrash_page_time = 0f;
			RefrashRoomList();
		}
	}

	public void HandleEvent(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (control.name == "Back_Button" && eventType == 3)
		{
			SceneName.SaveSceneStatistics("NetMapVSTUI");
			m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
				.FadeOut("NetMapVSTUI");
		}
		else if (control.name == "Msg_OK_Button" && eventType == 3)
		{
			MsgBoxType type = Msg_Box_Panel.GetComponent<MsgBoxDelegate>().m_type;
			Msg_Box_Panel.GetComponent<MsgBoxDelegate>().Hide();
			switch (type)
			{
			case MsgBoxType.ContectingTimeout:
			case MsgBoxType.ContectingLost:
				SceneName.SaveSceneStatistics("NetMapVSTUI");
				m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
					.FadeOut("NetMapVSTUI");
				break;
			case MsgBoxType.MultiToturial:
				GameApp.GetInstance().GetGameState().vs_toturial_triger_hall = 0;
				GameApp.GetInstance().PlayerPrefsSave();
				break;
			case MsgBoxType.VSPasswordToturial:
				GameApp.GetInstance().GetGameState().vs_toturial_triger_password = 0;
				GameApp.GetInstance().PlayerPrefsSave();
				CreatePasswordPanel.transform.localPosition = new Vector3(0f, 0f, JoinPasswordPanel.transform.localPosition.z);
				break;
			}
		}
		else if (control.name == "Page_Down" && eventType == 3)
		{
			cur_room_page++;
			RefrashRoomList();
		}
		else if (control.name == "Page_Up" && eventType == 3)
		{
			cur_room_page--;
			if (cur_room_page < 0)
			{
				cur_room_page = 0;
			}
			RefrashRoomList();
		}
		else if (control.name == "ReFrash_Button" && eventType == 3)
		{
			RefrashRoomList();
		}
		else if (control.name.StartsWith("R_Button_") && eventType == 3)
		{
			cur_room = control.transform.parent.GetComponent<SFSRoomData>().Room_data;
			string id = cur_room.Name;
			if (control.transform.parent.GetComponent<SFSRoomData>().Room_data.IsPasswordProtected)
			{
				JoinPasswordPanel.GetComponent<TUITextField>().ResetText();
				JoinPasswordPanel.transform.localPosition = new Vector3(0f, 0f, CreatePasswordPanel.transform.localPosition.z);
			}
			else
			{
				smartFox.Send(new JoinRoomRequest(id));
			}
		}
		else if (control.name.StartsWith("Create_Button") && eventType == 3)
		{
			if (GameApp.GetInstance().GetGameState().vs_toturial_triger_password == 1)
			{
				Msg_Box_Panel.GetComponent<MsgBoxDelegate>().Show(VSGameToturial.Msg_Content[2], MsgBoxType.VSPasswordToturial);
			}
			else
			{
				CreatePasswordPanel.transform.localPosition = new Vector3(0f, 0f, JoinPasswordPanel.transform.localPosition.z);
			}
		}
		else if (control.name.StartsWith("Create_Passwork_OK_Button") && eventType == 3)
		{
			CreatePasswordPanel.transform.localPosition = new Vector3(0f, 4000f, CreatePasswordPanel.transform.localPosition.z);
			StartServer(CreatePasswordPanel.GetComponent<TUITextField>().GetText());
			control.gameObject.active = false;
		}
		else if (control.name.StartsWith("Join_Passwork_OK_Button") && eventType == 3)
		{
			if (cur_room != null)
			{
				JoinPasswordPanel.transform.localPosition = new Vector3(0f, 5000f, JoinPasswordPanel.transform.localPosition.z);
				string id2 = cur_room.Name;
				smartFox.Send(new JoinRoomRequest(id2, JoinPasswordPanel.GetComponent<TUITextField>().GetText()));
			}
		}
		else if (control.name.StartsWith("Create_Passwork_Close_Button") && eventType == 3)
		{
			CreatePasswordPanel.transform.localPosition = new Vector3(0f, 4000f, CreatePasswordPanel.transform.localPosition.z);
		}
		else if (control.name.StartsWith("Join_Passwork_Close_Button") && eventType == 3)
		{
			JoinPasswordPanel.transform.localPosition = new Vector3(0f, 5000f, JoinPasswordPanel.transform.localPosition.z);
		}
	}

	public void StartServer(string password)
	{
		string text = smartFox.MySelf.Name + "|open";
		List<RoomVariable> list = new List<RoomVariable>();
		SFSRoomVariable item = new SFSRoomVariable("OwnerId", smartFox.MySelf.Id);
		list.Add(item);
		item = new SFSRoomVariable("GameStarted", false);
		list.Add(item);
		item = new SFSRoomVariable("GameStartTime", TimeManager.Instance.NetworkTime);
		list.Add(item);
		RoomPermissions roomPermissions = new RoomPermissions();
		roomPermissions.AllowNameChange = true;
		roomPermissions.AllowPasswordStateChange = true;
		roomPermissions.AllowPublicMessages = true;
		roomPermissions.AllowResizing = true;
		RoomSettings roomSettings = new RoomSettings(text);
		roomSettings.MaxUsers = 8;
		roomSettings.GroupId = GameApp.GetInstance().GetGameState().cur_net_map;
		roomSettings.Variables = list;
		roomSettings.Permissions = roomPermissions;
		roomSettings.Extension = new RoomExtension(ExtName, ExtClass);
		if (password != string.Empty)
		{
			roomSettings.Password = password;
		}
		smartFox.Send(new CreateRoomRequest(roomSettings, true, smartFox.LastJoinedRoom));
	}

	public void RefrashRoomList()
	{
		if (smartFox == null)
		{
			return;
		}
		Debug.Log("RefrashRoomList...");
		if ((bool)Room_Panel_Obj)
		{
			Room_Panel_Obj.GetComponent<MultiRoomPanel>().ResetRoomCells();
		}
		List<Room> roomListFromGroup = smartFox.GetRoomListFromGroup(GameApp.GetInstance().GetGameState().cur_net_map);
		int num = cur_room_page * 20;
		int num2 = num + 20;
		int num3 = 0;
		foreach (Room item in roomListFromGroup)
		{
			if ((bool)Room_Panel_Obj && num3 >= num && num3 < num2)
			{
				Room_Panel_Obj.GetComponent<MultiRoomPanel>().AddSFSRoom(item);
			}
			num3++;
			if (num3 > num2)
			{
				break;
			}
		}
		if (cur_room_page <= 0)
		{
			Page_Up.active = false;
			Page_Down.active = roomListFromGroup.Count > 20;
		}
		else
		{
			Page_Up.active = true;
			Page_Down.active = ((roomListFromGroup.Count > 20 * (cur_room_page + 1)) ? true : false);
		}
	}

	private void ConnectFailed()
	{
		Debug.Log("ConnectFailed");
		if (Indicator_Panel != null)
		{
			Indicator_Panel.GetComponent<IndicatorTUI>().Hide();
		}
		if (Msg_Box_Panel != null)
		{
			Msg_Box_Panel.GetComponent<MsgBoxDelegate>().Show("UNABLE TO CONNECT.", MsgBoxType.ContectingTimeout);
		}
	}

	public void OnConnectionLost(BaseEvent evt)
	{
		Debug.Log("Connection was lost, Reason: " + (string)evt.Params["reason"]);
		SmartFoxConnection.UnregisterSFSSceneCallbacks();
		ConnectFailed();
	}

	public void OnDebugMessage(BaseEvent evt)
	{
		string text = (string)evt.Params["message"];
		Debug.Log("[SFS DEBUG] " + text);
	}

	private void OnRoomAdd(BaseEvent evt)
	{
		Debug.Log("A new Room was added: " + (Room)evt.Params["room"]);
	}

	private void OnRoomCreationError(BaseEvent evt)
	{
		Debug.Log("An error occurred while attempting to create the Room: " + (string)evt.Params["errorMessage"]);
	}

	private void OnRoomVarsUpdate(BaseEvent evt)
	{
		ArrayList arrayList = (ArrayList)evt.Params["changedVars"];
		Room room = (Room)evt.Params["room"];
		if (arrayList.Contains("gameStarted"))
		{
			if (room.GetVariable("gameStarted").GetBoolValue())
			{
				Debug.Log("Game Starts!");
			}
			else
			{
				Debug.Log("Game was Stopped!");
			}
		}
		Debug.Log("room name:" + room.Name);
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
			content = "Room full or closed, \ntry another one!";
		}
		Msg_Box_Panel.GetComponent<MsgBoxDelegate>().Show(content, MsgBoxType.JoinRommFailed);
	}

	private void OnUnsubscribeRoomGroup(BaseEvent evt)
	{
		Debug.Log("Group Removed: " + (string)evt.Params["groupId"]);
		SceneName.SaveSceneStatistics("NetMapVSTUI");
		m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
			.FadeOut("NetMapVSTUI");
	}

	private void OnExtensionResponse(BaseEvent evt)
	{
		try
		{
			string text = (string)evt.Params["cmd"];
			ISFSObject dt = (SFSObject)evt.Params["params"];
			if (text == "Zone.Common.Keepalive")
			{
				HandleServerTime(dt);
			}
		}
		catch (Exception ex)
		{
			Debug.Log("Exception handling response: " + ex.Message + " >>> " + ex.StackTrace);
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
		SFSHeartBeat.DestroyInstanceObj();
	}
}
