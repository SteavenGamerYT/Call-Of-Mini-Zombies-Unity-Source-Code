using UnityEngine;
using Zombie3D;

public class RoomOwnerTUI : MonoBehaviour, TUIHandler
{
	private TUI m_tui;

	protected TUIInput[] input;

	public GameObject RoomOwner_Panel_Obj;

	public GameObject Msg_Box_Option_Panel;

	protected RoomOwnerPanel room_panel;

	public GameObject Msg_Box_Panel;

	public GameObject Start_Button;

	protected NetworkObj net_com;

	protected NetworkObj net_com_hall;

	public GameObject Indicator_Panel;

	public GameObject Toturial_Mask;

	public GameObject Titel_Label_ID;

	protected int kick_user_id = -1;

	protected TUIButtonClick tem_kick_button;

	public void Awake()
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
		net_com = GameApp.GetInstance().GetGameState().net_com;
		net_com.connect_delegate = OnConnected;
		net_com.packet_delegate = OnPacket;
		net_com.close_delegate = OnClosed;
		net_com.leave_room_delegate = OnLeaveRoom;
		net_com.leave_room_notity_delegate = OnLeaveRoomNotify;
		net_com.join_room_notity_delegate = OnJoinRoomNotify;
		net_com.kick_player_delegate = OnKickPlayer;
		net_com.kick_player_notify_delegate = OnKickPlayerNotify;
		net_com.destroy_delegate = OnDestroyRoom;
		net_com.destroy_notify_delegate = OnDestroyRoomNotify;
		net_com.be_kicked_delegate = OnBeKicked;
		net_com.contecting_lost = OnContectingLost;
		net_com.contecting_timeout = OnContectingTimeout;
		net_com.reverse_heart_timeout_delegate = OnReverseHearTimeout;
		net_com.reverse_heart_renew_delegate = OnReverseHearRenew;
		net_com.reverse_heart_waiting_delegate = OnReverseHearWaiting;
		if ((bool)RoomOwner_Panel_Obj)
		{
			room_panel = RoomOwner_Panel_Obj.GetComponent<RoomOwnerPanel>();
		}
	}

	public void Start()
	{
		m_tui = TUI.Instance("TUI");
		m_tui.SetHandler(this);
		m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
			.FadeIn();
		if ((bool)net_com && (bool)room_panel)
		{
			room_panel.RefrashClientCellShowData();
			room_panel.RefrashClientCellShow();
		}
		if ((bool)net_com && !net_com.m_netUserInfo.is_master)
		{
			Start_Button.transform.localPosition = new Vector3(0f, 1000f, Start_Button.transform.localPosition.z);
		}
		if ((bool)net_com && Titel_Label_ID != null)
		{
			Titel_Label_ID.GetComponent<TUIMeshText>().text = "ROOM " + net_com.m_netUserInfo.cur_room_id;
		}
		if ((bool)net_com && !net_com.m_netUserInfo.is_master)
		{
			Start_Button.transform.localPosition = new Vector3(0f, 1000f, Start_Button.transform.localPosition.z);
		}
		if (GameApp.GetInstance().GetGameState().multi_toturial_triger_room == 1 && (bool)net_com && !net_com.m_netUserInfo.is_master && (bool)Msg_Box_Panel)
		{
			Msg_Box_Panel.GetComponent<MsgBoxDelegate>().Show(MultiGameToturial.Msg_Content[3], MsgBoxType.MultiToturial);
			GameApp.GetInstance().GetGameState().multi_toturial_triger_room = 0;
			GameApp.GetInstance().PlayerPrefsSave();
		}
		if (GameApp.GetInstance().GetGameState().multi_toturial_triger_room_master == 1 && (bool)net_com && net_com.m_netUserInfo.is_master && (bool)Msg_Box_Panel)
		{
			Msg_Box_Panel.GetComponent<MsgBoxDelegate>().Show(MultiGameToturial.Msg_Content[4], MsgBoxType.MultiToturial);
			GameApp.GetInstance().GetGameState().multi_toturial_triger_room_master = 0;
			GameApp.GetInstance().PlayerPrefsSave();
		}
		OpenClickPlugin.Hide();
		Resources.UnloadUnusedAssets();
	}

	private void OnDestroy()
	{
		if (net_com != null)
		{
			net_com.connect_delegate = null;
			net_com.packet_delegate = null;
			net_com.close_delegate = null;
			net_com.leave_room_delegate = null;
			net_com.leave_room_notity_delegate = null;
			net_com.join_room_notity_delegate = null;
			net_com.kick_player_delegate = null;
			net_com.kick_player_notify_delegate = null;
			net_com.destroy_delegate = null;
			net_com.destroy_notify_delegate = null;
			net_com.be_kicked_delegate = null;
			net_com.contecting_lost = null;
			net_com.contecting_timeout = null;
			net_com.reverse_heart_timeout_delegate = null;
			net_com.reverse_heart_renew_delegate = null;
			net_com.reverse_heart_waiting_delegate = null;
			net_com = null;
		}
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
			Packet packet = null;
			packet = (net_com.m_netUserInfo.is_master ? CGDestroyRoomPacket.MakePacket() : CGLeaveRoomPacket.MakePacket());
			net_com.Send(packet);
		}
		else if (control.name == "Kick_Button" && eventType == 3)
		{
			tem_kick_button = control.GetComponent<TUIButtonClick>();
			kick_user_id = control.gameObject.transform.parent.gameObject.GetComponent<RoomCellData>().Room_cell_data.user_id;
			Debug.Log("Ready Kick User id:" + kick_user_id);
			Msg_Box_Option_Panel.GetComponent<MsgBoxDelegate>().Show("Kick this user from the \nroom?".ToUpper(), MsgBoxType.KickedOne);
		}
		else if (control.name == "Msg_No_Button" && eventType == 3)
		{
			Msg_Box_Option_Panel.GetComponent<MsgBoxDelegate>().Hide();
			kick_user_id = -1;
			tem_kick_button = null;
		}
		else if (control.name == "Msg_Yes_Button" && eventType == 3)
		{
			Msg_Box_Option_Panel.GetComponent<MsgBoxDelegate>().Hide();
			Packet packet2 = CGKickUserPacket.MakePacket((uint)kick_user_id);
			net_com.Send(packet2);
			tem_kick_button.enabled = false;
			tem_kick_button = null;
		}
		else if (control.name == "Panel_Back_Button" && eventType == 3)
		{
			GameApp.GetInstance().GetGameState().FromShopMenu = true;
			SceneName.SaveSceneStatistics("MultiplayRoomTUI");
			m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
				.FadeOut("MultiplayRoomTUI");
		}
		else if (control.name == "Msg_OK_Button" && eventType == 3)
		{
			MsgBoxType type = Msg_Box_Panel.GetComponent<MsgBoxDelegate>().m_type;
			Msg_Box_Panel.GetComponent<MsgBoxDelegate>().Hide();
			switch (type)
			{
			case MsgBoxType.ContectingTimeout:
			case MsgBoxType.ContectingLost:
				NetworkObj.DestroyNetCom();
				SceneName.SaveSceneStatistics("NetMapTUI");
				m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
					.FadeOut("NetMapTUI");
				break;
			case MsgBoxType.MultiToturial:
				if (GameApp.GetInstance().GetGameState().multi_toturial == 7)
				{
					Msg_Box_Panel.GetComponent<MsgBoxDelegate>().Show(MultiGameToturial.Msg_Content[GameApp.GetInstance().GetGameState().multi_toturial++], MsgBoxType.MultiToturial);
				}
				else if (GameApp.GetInstance().GetGameState().multi_toturial == 8)
				{
					net_com.PlayerJoinNotifyVirtual();
					Toturial_Mask.transform.localPosition = new Vector3(0f, 0f, Toturial_Mask.transform.localPosition.z);
					Start_Button.GetComponent<TUIButtonClickTextAni>().SetAnimationState(true);
				}
				break;
			case MsgBoxType.NotEnoughUser:
				break;
			default:
				if (net_com.m_netUserInfo.is_master)
				{
					SceneName.SaveSceneStatistics("NetMapTUI");
					m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
						.FadeOut("NetMapTUI");
				}
				else
				{
					SceneName.SaveSceneStatistics("MultiplayRoomTUI");
					m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
						.FadeOut("MultiplayRoomTUI");
				}
				break;
			}
		}
		else if (control.name == "Start_Button" && eventType == 3 && (bool)room_panel)
		{
			Debug.Log("count:" + room_panel.client_count);
			if (room_panel.client_count > 1)
			{
				Debug.Log("Ready Start Game");
				Packet packet3 = (packet3 = CGStartGamePacket.MakePacket());
				net_com.Send(packet3);
			}
			else if (Msg_Box_Panel != null)
			{
				Msg_Box_Panel.GetComponent<MsgBoxDelegate>().Show("You need at least 1 party \nmember to start the game.".ToUpper(), MsgBoxType.NotEnoughUser);
			}
		}
	}

	public void OnConnected()
	{
	}

	public void OnPacket(Packet packet)
	{
		uint val = 0u;
		if (packet.WatchUInt32(ref val, 4))
		{
			switch (val)
			{
			case 1048576u:
				Debug.Log("CG_HEARTBEAT revceived!!");
				break;
			case 4104u:
				Debug.Log("GC_STARTGAME revceived!!");
				OnStartGame(packet);
				break;
			}
		}
	}

	public void OnLeaveRoomNotify(int user_id)
	{
		if ((bool)room_panel)
		{
			room_panel.RefrashClientCellShowData();
			room_panel.RefrashClientCellShow();
		}
	}

	public void OnJoinRoomNotify(NetUserInfo user)
	{
		if ((bool)room_panel)
		{
			room_panel.RefrashClientCellShowData();
			room_panel.RefrashClientCellShow();
		}
	}

	public void OnKickPlayer(int user_id)
	{
		if ((bool)room_panel)
		{
			room_panel.RefrashClientCellShowData();
			room_panel.RefrashClientCellShow();
		}
	}

	public void OnKickPlayerNotify(int user_id)
	{
		if ((bool)room_panel)
		{
			room_panel.RefrashClientCellShowData();
			room_panel.RefrashClientCellShow();
		}
	}

	public void OnLeaveRoom()
	{
		SceneName.SaveSceneStatistics("MultiplayRoomTUI");
		m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
			.FadeOut("MultiplayRoomTUI");
	}

	public void OnDestroyRoom()
	{
		SceneName.SaveSceneStatistics("NetMapTUI");
		m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
			.FadeOut("NetMapTUI");
	}

	public void FadeToGameScene()
	{
		GameApp.GetInstance().GetGameState().endless_multiplayer = true;
		GameApp.GetInstance().GetGameState().endless_level = true;
		GameApp.GetInstance().GetGameState().VS_mode = false;
		Debug.Log("FadeToGameScene:" + net_com.m_netUserInfo.cur_map_id);
		string netMapName = SceneName.GetNetMapName(net_com.m_netUserInfo.cur_map_id);
		if (netMapName.Length > 0)
		{
			SceneName.SaveSceneStatistics(netMapName);
		}
	}

	public void OnStartGame(Packet packet)
	{
		GCStartGamePacket gCStartGamePacket = new GCStartGamePacket();
		if (gCStartGamePacket.ParserPacket(packet))
		{
			FadeToGameScene();
			Debug.Log("Start Game Now...");
		}
	}

	public void OnStartNotifyGame(Packet packet)
	{
		GCStartGameNotifyPacket gCStartGameNotifyPacket = new GCStartGameNotifyPacket();
		if (gCStartGameNotifyPacket.ParserPacket(packet))
		{
			FadeToGameScene();
			Debug.Log("Start Game Now...");
		}
	}

	public void OnDestroyRoomNotify()
	{
		if ((bool)Msg_Box_Panel)
		{
			Msg_Box_Panel.GetComponent<MsgBoxDelegate>().Show("Room full or closed, \ntry another one!".ToUpper(), MsgBoxType.RoomDestroyed);
		}
	}

	public void OnClosed()
	{
		Debug.Log("RoomOwnerTUI OnClosed-----------");
	}

	public void OnBeKicked()
	{
		if ((bool)Msg_Box_Panel)
		{
			Msg_Box_Panel.GetComponent<MsgBoxDelegate>().Show("YOU WERE REMOVED FROM \nTHE ROOM.", MsgBoxType.BeKicked);
		}
	}

	public void OnContectingLost()
	{
		if (Msg_Box_Panel != null)
		{
			Msg_Box_Panel.GetComponent<MsgBoxDelegate>().Show("YOU WERE DISCONNECTED.", MsgBoxType.ContectingLost);
		}
	}

	public void OnReverseHearWaiting()
	{
		if (Indicator_Panel != null)
		{
			Indicator_Panel.GetComponent<IndicatorTUI>().SetContent("WAITING FOR SERVER.");
			Indicator_Panel.GetComponent<IndicatorTUI>().Show();
		}
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
			Msg_Box_Panel.GetComponent<MsgBoxDelegate>().Show("YOU WERE DISCONNECTED.", MsgBoxType.ContectingTimeout);
		}
	}

	public void OnContectingTimeout()
	{
		Debug.Log("Server Connecting Timeount...");
		Indicator_Panel.GetComponent<IndicatorTUI>().Hide();
		Object.Destroy(net_com.gameObject);
		if (net_com_hall != null)
		{
			Object.Destroy(net_com_hall.gameObject);
		}
		if (Msg_Box_Panel != null)
		{
			Msg_Box_Panel.GetComponent<MsgBoxDelegate>().Show("UNABLE TO CONNECT.", MsgBoxType.ContectingTimeout);
		}
	}
}
