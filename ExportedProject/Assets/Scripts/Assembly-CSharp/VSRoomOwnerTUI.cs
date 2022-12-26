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

public class VSRoomOwnerTUI : MonoBehaviour, TUIHandler
{
	private TUI m_tui;

	protected TUIInput[] input;

	public GameObject RoomOwner_Panel_Obj;

	protected VSRoomOwnerPanel room_panel;

	public GameObject Msg_Box_Panel;

	public GameObject Start_Button;

	public GameObject Indicator_Panel;

	public GameObject Toturial_Mask;

	public GameObject Titel_Label_ID;

	private bool kicked_room;

	private SmartFox smartFox;

	private bool time_inited;

	private int owner_id = -1;

	private void Awake()
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
		if ((bool)RoomOwner_Panel_Obj)
		{
			room_panel = RoomOwner_Panel_Obj.GetComponent<VSRoomOwnerPanel>();
		}
		if (SmartFoxConnection.IsInitialized)
		{
			smartFox = SmartFoxConnection.Connection;
			smartFox.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
			smartFox.AddLogListener(LogLevel.DEBUG, OnDebugMessage);
			smartFox.AddEventListener(SFSEvent.USER_ENTER_ROOM, OnUserEnterRoom);
			smartFox.AddEventListener(SFSEvent.USER_EXIT_ROOM, OnUserExitRoom);
			smartFox.AddEventListener(SFSEvent.OBJECT_MESSAGE, OnObjectMessage);
			smartFox.AddEventListener(SFSEvent.ROOM_VARIABLES_UPDATE, OnRoomVarsUpdate);
			smartFox.AddEventListener(SFSEvent.ROOM_NAME_CHANGE, OnRoomNameChange);
			smartFox.AddEventListener(SFSEvent.ROOM_NAME_CHANGE_ERROR, OnRoomNameChangeError);
			smartFox.AddEventListener(SFSEvent.USER_VARIABLES_UPDATE, OnUserVarsUpdate);
			smartFox.AddEventListener(SFSEvent.EXTENSION_RESPONSE, OnExtensionResponse);
			SFSHeartBeat.Instance.reverse_heart_timeout_delegate = OnReverseHearTimeout;
			SFSHeartBeat.Instance.reverse_heart_renew_delegate = OnReverseHearRenew;
			SFSHeartBeat.Instance.reverse_heart_waiting_delegate = OnReverseHearWaiting;
			owner_id = smartFox.LastJoinedRoom.GetVariable("OwnerId").GetIntValue();
			if (owner_id == smartFox.MySelf.Id)
			{
				SmartFoxConnection.is_server = true;
				RoomSetting();
				Debug.Log("this is server.~~");
			}
			else
			{
				SmartFoxConnection.is_server = false;
			}
		}
		else
		{
			Debug.Log("smartFox init error!");
		}
	}

	private IEnumerator Start()
	{
		m_tui = TUI.Instance("TUI");
		m_tui.SetHandler(this);
		m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
			.FadeIn();
		m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFadeEx>()
			.m_fadeout = OnSceneChange;
		if ((bool)room_panel)
		{
			room_panel.RefrashClientCellShowData();
			room_panel.RefrashClientCellShow();
		}
		RefrashStartButton();
		OpenClickPlugin.Hide();
		Resources.UnloadUnusedAssets();
		List<UserVariable> userVars = new List<UserVariable>();
		SFSObject dataObj = new SFSObject();
		dataObj.PutInt("avatarType", (int)GameApp.GetInstance().GetGameState().Avatar);
		dataObj.PutBool("InRoom", true);
		userVars.Add(new SFSUserVariable("RoomState", dataObj));
		smartFox.Send(new SetUserVariablesRequest(userVars));
		yield return 1;
		if (SmartFoxConnection.is_server && Msg_Box_Panel != null && GameApp.GetInstance().GetGameState().vs_toturial_triger_roomOwner == 1)
		{
			Msg_Box_Panel.GetComponent<MsgBoxDelegate>().Show(VSGameToturial.Msg_Content[4], MsgBoxType.MultiToturial);
		}
		if (!SmartFoxConnection.is_server && Msg_Box_Panel != null && GameApp.GetInstance().GetGameState().vs_toturial_triger_joinRoom == 1)
		{
			Msg_Box_Panel.GetComponent<MsgBoxDelegate>().Show(VSGameToturial.Msg_Content[3], MsgBoxType.VSPasswordToturial);
		}
		if (smartFox.LastJoinedRoom.ContainsVariable("GameStarted") && smartFox.LastJoinedRoom.GetVariable("GameStarted").GetBoolValue())
		{
			Debug.Log("Game Started!");
			LoadGameLevel(GameApp.GetInstance().GetGameState().cur_net_map);
		}
	}

	public void OnSceneChange()
	{
	}

	private void FixedUpdate()
	{
		smartFox.ProcessEvents();
	}

	private void Update()
	{
		input = TUIInputManager.GetInput();
		for (int i = 0; i < input.Length; i++)
		{
			m_tui.HandleInput(input[i]);
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

	private void RefrashStartButton()
	{
		if (SmartFoxConnection.is_server && time_inited)
		{
			Start_Button.active = true;
		}
		else
		{
			Start_Button.active = false;
		}
	}

	public void HandleEvent(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (control.name == "Back_Button" && eventType == 3)
		{
			SetRoomStateForLeaveRoom();
		}
		else if (control.name == "Kick_Button" && eventType == 3)
		{
			ISFSObject iSFSObject = new SFSObject();
			iSFSObject.PutBool("kick", true);
			List<User> list = new List<User>();
			list.Add(control.transform.parent.GetComponent<VSRoomCellData>().sfs_user);
			smartFox.Send(new ObjectMessageRequest(iSFSObject, smartFox.LastJoinedRoom, list));
		}
		else
		{
			if (control.name == "Panel_Back_Button" && eventType == 3)
			{
				return;
			}
			if (control.name == "Msg_OK_Button" && eventType == 3)
			{
				MsgBoxType type = Msg_Box_Panel.GetComponent<MsgBoxDelegate>().m_type;
				Msg_Box_Panel.GetComponent<MsgBoxDelegate>().Hide();
				switch (type)
				{
				case MsgBoxType.BeKicked:
				case MsgBoxType.RoomDestroyed:
					SceneName.SaveSceneStatistics("VSRoomTUI");
					m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
						.FadeOut("VSRoomTUI");
					break;
				case MsgBoxType.ContectingTimeout:
				case MsgBoxType.ContectingLost:
					SceneName.SaveSceneStatistics("NetMapVSTUI");
					m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
						.FadeOut("NetMapVSTUI");
					break;
				case MsgBoxType.MultiToturial:
					GameApp.GetInstance().GetGameState().vs_toturial_triger_roomOwner = 0;
					GameApp.GetInstance().PlayerPrefsSave();
					break;
				case MsgBoxType.VSPasswordToturial:
					GameApp.GetInstance().GetGameState().vs_toturial_triger_joinRoom = 0;
					GameApp.GetInstance().PlayerPrefsSave();
					break;
				case MsgBoxType.OnClosed:
				case MsgBoxType.JoinRommFailed:
				case MsgBoxType.CrateRommFailed:
				case MsgBoxType.NotEnoughUser:
				case MsgBoxType.Rebirth:
				case MsgBoxType.KickedOne:
					break;
				}
			}
			else if (control.name == "Start_Button" && eventType == 3 && (bool)room_panel)
			{
				Debug.Log("count:" + room_panel.client_count);
				if (smartFox.LastJoinedRoom.UserCount > 1)
				{
					string[] array = smartFox.LastJoinedRoom.Name.Split('|');
					smartFox.Send(new ChangeRoomNameRequest(smartFox.LastJoinedRoom, array[0] + "|" + array[1] + "|close|" + smartFox.LastJoinedRoom.Id));
				}
				else if (Msg_Box_Panel != null)
				{
					Msg_Box_Panel.GetComponent<MsgBoxDelegate>().Show("You need at least 1 party \nmember to start the game.".ToUpper(), MsgBoxType.NotEnoughUser);
				}
			}
		}
	}

	private void LeaveRoom()
	{
		smartFox.Send(new LeaveRoomRequest());
		SmartFoxConnection.UnregisterSFSSceneCallbacks();
		if (kicked_room)
		{
			if ((bool)Msg_Box_Panel)
			{
				Msg_Box_Panel.GetComponent<MsgBoxDelegate>().Show("YOU WERE REMOVED FROM \nTHE ROOM.", MsgBoxType.BeKicked);
			}
		}
		else
		{
			SceneName.SaveSceneStatistics("VSRoomTUI");
			m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
				.FadeOut("VSRoomTUI");
		}
	}

	private void SetRoomStateForLeaveRoom()
	{
		List<UserVariable> list = new List<UserVariable>();
		SFSObject sFSObject = new SFSObject();
		sFSObject.PutInt("avatarType", (int)GameApp.GetInstance().GetGameState().Avatar);
		sFSObject.PutBool("InRoom", false);
		list.Add(new SFSUserVariable("RoomState", sFSObject));
		smartFox.Send(new SetUserVariablesRequest(list));
	}

	private void OnDebugMessage(BaseEvent evt)
	{
		string text = (string)evt.Params["message"];
		Debug.Log("[SFS DEBUG] " + text);
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

	private void OnConnectionLost(BaseEvent evt)
	{
		Debug.Log("Connection was lost, Reason: " + (string)evt.Params["reason"]);
		SmartFoxConnection.UnregisterSFSSceneCallbacks();
		ConnectFailed();
	}

	private void OnUserEnterRoom(BaseEvent evt)
	{
		Room room = (Room)evt.Params["room"];
		User user = (User)evt.Params["user"];
		Debug.Log("User: " + user.Name + " has just joined Room: " + room.Name);
		if ((bool)room_panel)
		{
			room_panel.RefrashClientCellShowData();
			room_panel.RefrashClientCellShow();
		}
	}

	private void OnUserExitRoom(BaseEvent evt)
	{
		Room room = (Room)evt.Params["room"];
		User user = (User)evt.Params["user"];
		Debug.Log("User: " + user.Name + " has just left Room: " + room.Name);
		if (user.Id == owner_id && user.Id != smartFox.MySelf.Id)
		{
			smartFox.Send(new LeaveRoomRequest());
			SmartFoxConnection.UnregisterSFSSceneCallbacks();
			if ((bool)Msg_Box_Panel)
			{
				Msg_Box_Panel.GetComponent<MsgBoxDelegate>().Show("Room full or closed, \ntry another one!".ToUpper(), MsgBoxType.RoomDestroyed);
			}
			return;
		}
		smartFox.LastJoinedRoom.UserList.Remove(user);
		smartFox.RoomManager.RemoveUser(user);
		if ((bool)room_panel)
		{
			room_panel.RefrashClientCellShowData();
			room_panel.RefrashClientCellShow();
		}
	}

	private void OnObjectMessage(BaseEvent evt)
	{
		ISFSObject iSFSObject = (SFSObject)evt.Params["message"];
		User user = (User)evt.Params["sender"];
		if (iSFSObject.ContainsKey("kick") && iSFSObject.GetBool("kick"))
		{
			Debug.Log("Be Kick!");
			kicked_room = true;
			SetRoomStateForLeaveRoom();
		}
	}

	private void OnRoomVarsUpdate(BaseEvent evt)
	{
		ArrayList arrayList = (ArrayList)evt.Params["changedVars"];
		Room room = (Room)evt.Params["room"];
		if (arrayList.Contains("GameStartTime"))
		{
			Debug.Log("Game Start Time:" + room.GetVariable("GameStartTime").GetDoubleValue());
		}
		if (arrayList.Contains("GameStarted"))
		{
			if (room.GetVariable("GameStarted").GetBoolValue())
			{
				Debug.Log("Game Starts!");
				LoadGameLevel(GameApp.GetInstance().GetGameState().cur_net_map);
			}
			else
			{
				Debug.Log("Game was Stopped!");
			}
		}
	}

	private void LoadGameLevel(string level)
	{
		GameApp.GetInstance().GetGameState().endless_level = false;
		GameApp.GetInstance().GetGameState().endless_multiplayer = false;
		GameApp.GetInstance().GetGameState().hunting_level = false;
		GameApp.GetInstance().GetGameState().VS_mode = true;
		if (level == "Zombie3D_Village2")
		{
			level = "Zombie3D_Village2_3g";
		}
		SceneName.SaveSceneStatistics(level);
		m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
			.FadeOut(level);
	}

	private void OnRoomNameChange(BaseEvent evt)
	{
		if (((Room)evt.Params["room"]).Id == smartFox.LastJoinedRoom.Id && SmartFoxConnection.is_server)
		{
			Debug.Log("Room was renamed successfully: " + (Room)evt.Params["room"]);
			Debug.Log("Room old name was: " + (string)evt.Params["oldName"]);
			List<RoomVariable> list = new List<RoomVariable>();
			SFSRoomVariable sFSRoomVariable = new SFSRoomVariable("GameStarted", true);
			sFSRoomVariable.IsPrivate = true;
			sFSRoomVariable.IsPersistent = true;
			list.Add(sFSRoomVariable);
			sFSRoomVariable = new SFSRoomVariable("GameStartTime", TimeManager.Instance.NetworkTime);
			sFSRoomVariable.IsPrivate = true;
			sFSRoomVariable.IsPersistent = true;
			list.Add(sFSRoomVariable);
			smartFox.Send(new SetRoomVariablesRequest(list, smartFox.LastJoinedRoom));
		}
	}

	private void OnRoomNameChangeError(BaseEvent evt)
	{
		Debug.Log("Room name change failed: " + (string)evt.Params["errorMessage"]);
	}

	private void OnUserVarsUpdate(BaseEvent evt)
	{
		ArrayList arrayList = (ArrayList)evt.Params["changedVars"];
		User user = (User)evt.Params["user"];
		if (user == smartFox.MySelf)
		{
			if (arrayList.Contains("RoomState"))
			{
				SFSObject sFSObject = user.GetVariable("RoomState").GetSFSObjectValue() as SFSObject;
				if (!sFSObject.GetBool("InRoom"))
				{
					LeaveRoom();
				}
			}
		}
		else if ((bool)room_panel)
		{
			room_panel.RefrashClientCellShowData();
			room_panel.RefrashClientCellShow();
		}
	}

	private void OnExtensionResponse(BaseEvent evt)
	{
		try
		{
			string text = (string)evt.Params["cmd"];
			ISFSObject iSFSObject = (SFSObject)evt.Params["params"];
			if (text == "Zone.Common.Keepalive")
			{
				HandleServerTime(iSFSObject);
			}
			else if (text == "Room.Common.RoomSetting" && iSFSObject.GetInt("code") == 0)
			{
				Debug.Log("Room.Common.RoomSetting success.");
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
		if (!time_inited)
		{
			time_inited = true;
			RefrashStartButton();
		}
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

	public void RoomSetting()
	{
		Debug.Log("send Room.Common.RoomSetting");
		SFSObject sFSObject = new SFSObject();
		sFSObject.PutUtfString("autoRemoveMode", "WHEN_EMPTY");
		smartFox.Send(new ExtensionRequest("Room.Common.RoomSetting", sFSObject, smartFox.LastJoinedRoom));
	}
}
