using System.Collections.Generic;
using UnityEngine;
using Zombie3D;

public class MultiRoomTUI : MonoBehaviour, TUIHandler
{
	private TUI m_tui;

	protected TUIInput[] input;

	public GameObject Room_Panel_Obj;

	public GameObject Msg_Box_Panel;

	public GameObject Indicator_Panel;

	protected int cur_room_page;

	protected NetworkObj net_com;

	protected float last_refrash_page_time;

	public GameObject Toturial_Mask;

	public GameObject Page_Show;

	public GameObject Page_Up;

	public GameObject Page_Down;

	public void Awake()
	{
		GameScript.CheckMenuResourceConfig();
		GameScript.CheckGlobalResourceConfig();
		GameScript.CheckNetWorkCom();
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

	public void Start()
	{
		m_tui = TUI.Instance("TUI");
		m_tui.SetHandler(this);
		m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
			.FadeIn();
		net_com = GameApp.GetInstance().GetGameState().net_com;
		net_com.connect_delegate = OnConnected;
		net_com.packet_delegate = OnPacket;
		net_com.close_delegate = OnClosed;
		net_com.join_room_delegate = OnJoinRoom;
		net_com.join_room_error_delegate = OnJoinRoomError;
		net_com.contecting_timeout = OnContectingTimeout;
		net_com.contecting_lost = OnContectingLost;
		net_com.reverse_heart_timeout_delegate = OnReverseHearTimeout;
		net_com.reverse_heart_renew_delegate = OnReverseHearRenew;
		net_com.reverse_heart_waiting_delegate = OnReverseHearWaiting;
		net_com.ClearNetUserArr();
		cur_room_page = 0;
		GetRoomPage(cur_room_page);
		if (Msg_Box_Panel != null && GameApp.GetInstance().GetGameState().multi_toturial_triger_hall == 1)
		{
			Msg_Box_Panel.GetComponent<MsgBoxDelegate>().Show(MultiGameToturial.Msg_Content[2], MsgBoxType.MultiToturial);
		}
		last_refrash_page_time = 0f;
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
			net_com.join_room_delegate = null;
			net_com.join_room_error_delegate = null;
			net_com.contecting_timeout = null;
			net_com.contecting_lost = null;
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
		if (NetworkObj.tcp_contected_status == NetworkObj.TCP_CONTECTED_STATUS.GameServer)
		{
			last_refrash_page_time += Time.deltaTime;
			if (last_refrash_page_time >= 3f)
			{
				GetRoomPage(cur_room_page);
				last_refrash_page_time = 0f;
			}
		}
	}

	public void HandleEvent(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (control.name == "Back_Button" && eventType == 3)
		{
			SceneName.SaveSceneStatistics("NetMapTUI");
			m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
				.FadeOut("NetMapTUI");
		}
		else if (control.name == "Msg_OK_Button" && eventType == 3)
		{
			MsgBoxType type = Msg_Box_Panel.GetComponent<MsgBoxDelegate>().m_type;
			Msg_Box_Panel.GetComponent<MsgBoxDelegate>().Hide();
			switch (type)
			{
			case MsgBoxType.ContectingTimeout:
				SceneName.SaveSceneStatistics("NetMapTUI");
				m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
					.FadeOut("NetMapTUI");
				NetworkObj.DestroyNetCom();
				break;
			case MsgBoxType.ContectingLost:
				SceneName.SaveSceneStatistics("NetMapTUI");
				m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
					.FadeOut("NetMapTUI");
				NetworkObj.DestroyNetCom();
				break;
			case MsgBoxType.MultiToturial:
				GameApp.GetInstance().GetGameState().multi_toturial_triger_hall = 0;
				GameApp.GetInstance().PlayerPrefsSave();
				break;
			case MsgBoxType.JoinRommFailed:
				break;
			}
		}
		else if (control.name == "Page_Down" && eventType == 3)
		{
			GetRoomPage(cur_room_page + 1);
		}
		else if (control.name == "Toturial_Page_Down" && eventType == 3)
		{
			GetRoomPage(cur_room_page + 1);
			if (Msg_Box_Panel != null && GameApp.GetInstance().GetGameState().multi_toturial_triger == 1)
			{
				Msg_Box_Panel.GetComponent<MsgBoxDelegate>().Show(MultiGameToturial.Msg_Content[GameApp.GetInstance().GetGameState().multi_toturial++], MsgBoxType.MultiToturial);
			}
		}
		else if (control.name == "Page_Up" && eventType == 3)
		{
			GetRoomPage(cur_room_page - 1);
		}
		else if (control.name == "ReFrash_Button" && eventType == 3)
		{
			GetRoomPage(cur_room_page);
		}
		else if (control.name == "Toturial_ReFrash_Button" && eventType == 3)
		{
			GetRoomPage(cur_room_page);
			if (Msg_Box_Panel != null && GameApp.GetInstance().GetGameState().multi_toturial_triger == 1)
			{
				Msg_Box_Panel.GetComponent<MsgBoxDelegate>().Show(MultiGameToturial.Msg_Content[GameApp.GetInstance().GetGameState().multi_toturial++], MsgBoxType.MultiToturial);
			}
		}
		else if (control.name.StartsWith("R_Button_") && eventType == 3)
		{
			uint iRoomId = control.gameObject.transform.parent.GetComponent<RoomInfoData>().Room_info_data.m_iRoomId;
			Packet packet = CGJoinRoomPacket.MakePacket(iRoomId, (long)(Time.time * 1000f), GameApp.GetInstance().GetGameState().nick_name, (uint)GameApp.GetInstance().GetGameState().Avatar, (uint)GameApp.GetInstance().GetGameState().LevelNum);
			net_com.Send(packet);
			Debug.Log("Ready join room:" + iRoomId);
		}
	}

	public void OnJoinRoom()
	{
		Debug.Log("join room now...");
		SceneName.SaveSceneStatistics("RoomOwnerTUI");
		m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
			.FadeOut("RoomOwnerTUI");
		if (net_com.m_netUserInfo.is_master)
		{
			net_com.ClearNetUserArr();
		}
	}

	public void OnJoinRoomError(uint error_info)
	{
		if (Msg_Box_Panel != null)
		{
			string content = string.Empty;
			switch (error_info)
			{
			case 1u:
				content = "Room full or closed, \ntry another one!";
				break;
			case 2u:
				content = "Room full or closed, \ntry another one!";
				break;
			case 3u:
				content = "Room unavailable, try \nanother one!";
				break;
			}
			Msg_Box_Panel.GetComponent<MsgBoxDelegate>().Show(content, MsgBoxType.JoinRommFailed);
			GetRoomPage(cur_room_page);
		}
	}

	public void OnConnected()
	{
		Debug.Log("MultiRoomTUI OnConnected");
	}

	public void OnContectingTimeout()
	{
		Debug.Log("Server Connecting Timeount...");
		Indicator_Panel.GetComponent<IndicatorTUI>().Hide();
		Object.Destroy(net_com.gameObject);
		if (Msg_Box_Panel != null)
		{
			Msg_Box_Panel.GetComponent<MsgBoxDelegate>().Show("UNABLE TO CONNECT.", MsgBoxType.ContectingTimeout);
		}
	}

	public void GetRoomPage(int page)
	{
		if (page < 0)
		{
			page = 0;
		}
		Packet packet = CGRoomListPacket.MakePacket((uint)page, (uint)SceneName.GetNetMapIndex(GameApp.GetInstance().GetGameState().cur_net_map));
		net_com.Send(packet);
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
			case 4098u:
				GetRoomList(packet);
				break;
			}
		}
	}

	public void GetRoomList(Packet packet)
	{
		GCRoomListPacket gCRoomListPacket = new GCRoomListPacket();
		if (!gCRoomListPacket.ParserPacket(packet))
		{
			Debug.Log("GCRoomListPacket ParserPacket error!!");
			return;
		}
		cur_room_page = (int)gCRoomListPacket.m_iCurpage;
		if ((bool)Page_Show)
		{
			Page_Show.GetComponent<TUIMeshText>().text = "PAGE " + (cur_room_page + 1);
		}
		Page_Up.active = true;
		Page_Down.active = true;
		if (gCRoomListPacket.m_iCurpage == 0)
		{
			Page_Up.active = false;
		}
		if (gCRoomListPacket.m_iCurpage == gCRoomListPacket.m_pagesum)
		{
			Page_Down.active = false;
		}
		if ((bool)Room_Panel_Obj)
		{
			Room_Panel_Obj.GetComponent<MultiRoomPanel>().ResetRoomCells();
		}
		List<GCRoomListPacket.RoomInfo> out_list = new List<GCRoomListPacket.RoomInfo>();
		SortRoomInfoList(ref out_list, gCRoomListPacket.m_vRoomList);
		foreach (GCRoomListPacket.RoomInfo item in out_list)
		{
			if ((bool)Room_Panel_Obj)
			{
				Room_Panel_Obj.GetComponent<MultiRoomPanel>().AddRoomInfo(item);
			}
		}
		if ((bool)Room_Panel_Obj)
		{
			Room_Panel_Obj.transform.localPosition = new Vector3(Room_Panel_Obj.transform.localPosition.x, 0f, Room_Panel_Obj.transform.localPosition.z);
		}
	}

	public void SortRoomInfoList(ref List<GCRoomListPacket.RoomInfo> out_list, List<GCRoomListPacket.RoomInfo> in_list)
	{
		List<GCRoomListPacket.RoomInfo> list = new List<GCRoomListPacket.RoomInfo>();
		List<GCRoomListPacket.RoomInfo> list2 = new List<GCRoomListPacket.RoomInfo>();
		foreach (GCRoomListPacket.RoomInfo item in in_list)
		{
			if (item.m_room_status != 0)
			{
				list2.Add(item);
			}
			else
			{
				list.Add(item);
			}
		}
		foreach (GCRoomListPacket.RoomInfo item2 in list)
		{
			out_list.Add(item2);
		}
		foreach (GCRoomListPacket.RoomInfo item3 in list2)
		{
			out_list.Add(item3);
		}
	}

	public void OnClosed()
	{
	}

	public void OnContectingLost()
	{
		Object.Destroy(net_com.gameObject);
		if (Msg_Box_Panel != null)
		{
			Msg_Box_Panel.GetComponent<MsgBoxDelegate>().Show("YOU WERE DISCONNECTED.", MsgBoxType.ContectingLost);
		}
		if (Indicator_Panel != null)
		{
			Indicator_Panel.GetComponent<IndicatorTUI>().Hide();
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
}
