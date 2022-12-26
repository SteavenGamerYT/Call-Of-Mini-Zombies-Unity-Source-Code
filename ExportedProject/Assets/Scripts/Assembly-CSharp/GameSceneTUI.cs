using UnityEngine;
using Zombie3D;

public class GameSceneTUI : MonoBehaviour, TUIHandler
{
	private TUI m_tui;

	protected TUIInput[] input;

	public GameObject Msg_box;

	public GameObject Msg_box_option;

	public GameObject Indicator_panel;

	public GameObject Waiting_panel;

	public GameObject MapRect;

	protected GameScene mulit_scene;

	public GameObject map_show;

	public GameObject PlayerMark1;

	public GameObject PlayerMark2;

	public GameObject PlayerMark3;

	public GameObject PlayerMark4;

	protected NetworkObj net_com;

	public bool is_inited;

	public bool to_next_msg;

	public bool is_dead_msg;

	protected float rebirth_time = 10f;

	public bool is_rebirth_msg;

	public TUIMeshText Label_Time;

	private void Start()
	{
		m_tui = TUI.Instance("TUI");
		m_tui.SetHandler(this);
		m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
			.FadeIn();
		if (GameApp.GetInstance().GetGameState().endless_multiplayer)
		{
			net_com = GameApp.GetInstance().GetGameState().net_com;
			if ((bool)net_com)
			{
				net_com.reverse_heart_timeout_delegate = OnReverseHearTimeout;
				net_com.reverse_heart_renew_delegate = OnReverseHearRenew;
				net_com.reverse_heart_waiting_delegate = OnReverseHearWaiting;
				net_com.close_delegate = OnClosed;
				net_com.contecting_lost = OnContectingLost;
			}
		}
		is_inited = true;
	}

	public void Update()
	{
		input = TUIInputManager.GetInput();
		for (int i = 0; i < input.Length; i++)
		{
			m_tui.HandleInput(input[i]);
		}
		if (is_rebirth_msg)
		{
			rebirth_time -= Time.deltaTime;
			if (rebirth_time <= 0f)
			{
				is_rebirth_msg = false;
				rebirth_time = 0f;
				GameApp.GetInstance().GetGameScene().GetPlayer()
					.PlayerRealDead();
				HideRebirthMsg();
			}
			Label_Time.text = Mathf.Round(rebirth_time).ToString();
		}
	}

	private void OnDestroy()
	{
		if (net_com != null)
		{
			net_com.reverse_heart_timeout_delegate = null;
			net_com.reverse_heart_renew_delegate = null;
			net_com.reverse_heart_waiting_delegate = null;
			net_com.close_delegate = null;
			net_com.contecting_lost = null;
			net_com = null;
		}
	}

	public void AddMultiplayerMark(Player mPlayer, int index)
	{
		if (PlayerMark1.GetComponent<PlayerMark>().m_player == null && index == 0)
		{
			PlayerMark1.GetComponent<PlayerMark>().SetPlayer(mPlayer);
		}
		if (PlayerMark2.GetComponent<PlayerMark>().m_player == null && index == 1)
		{
			PlayerMark2.GetComponent<PlayerMark>().SetPlayer(mPlayer);
		}
		if (PlayerMark3.GetComponent<PlayerMark>().m_player == null && index == 2)
		{
			PlayerMark3.GetComponent<PlayerMark>().SetPlayer(mPlayer);
		}
		if (PlayerMark4.GetComponent<PlayerMark>().m_player == null && index == 3)
		{
			PlayerMark4.GetComponent<PlayerMark>().SetPlayer(mPlayer);
		}
	}

	public void RemoveMultiplayerMarkWithID(int id)
	{
		if (PlayerMark1.GetComponent<PlayerMark>().Mark_Player_ID == id)
		{
			PlayerMark1.GetComponent<PlayerMark>().RemoveMark();
		}
		else if (PlayerMark2.GetComponent<PlayerMark>().Mark_Player_ID == id)
		{
			PlayerMark2.GetComponent<PlayerMark>().RemoveMark();
		}
		else if (PlayerMark3.GetComponent<PlayerMark>().Mark_Player_ID == id)
		{
			PlayerMark3.GetComponent<PlayerMark>().RemoveMark();
		}
		else if (PlayerMark4.GetComponent<PlayerMark>().Mark_Player_ID == id)
		{
			PlayerMark4.GetComponent<PlayerMark>().RemoveMark();
		}
	}

	public void HandleEvent(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (control.name == "Msg_OK_Button" && eventType == 3)
		{
			switch (Msg_box.GetComponent<MsgBoxDelegate>().m_type)
			{
			case MsgBoxType.ContectingTimeout:
				Time.timeScale = 1f;
				SceneName.LoadLevel("MultiReportTUI");
				break;
			case MsgBoxType.MultiToturial:
				Msg_box.GetComponent<MsgBoxDelegate>().Hide();
				if (to_next_msg)
				{
					ShowTutorialMsgRescue(2);
				}
				if (is_dead_msg)
				{
					ShowRebirthMsg();
					is_dead_msg = false;
				}
				break;
			case MsgBoxType.ContectingLost:
				Time.timeScale = 1f;
				mulit_scene.TimeGameOver(0f);
				break;
			case MsgBoxType.JoinRommFailed:
				break;
			}
		}
		else if (control.name == "Msg_Yes_Button" && eventType == 3)
		{
			Debug.Log("Msg_Yes_Button");
			if (Msg_box_option != null)
			{
				Msg_box_option.GetComponent<MsgBoxDelegate>().Hide();
			}
			GameApp.GetInstance().GetGameScene().GetPlayer()
				.OnRebirth();
			GameApp.GetInstance().GetGameState().m_rebirth_packet_count--;
			Packet packet = CGUserRebirthPacket.MakePacket((uint)GameApp.GetInstance().GetGameScene().GetPlayer()
				.m_multi_id);
			GameApp.GetInstance().GetGameState().net_com.Send(packet);
		}
		else if (control.name == "Msg_No_Button" && eventType == 3)
		{
			GameApp.GetInstance().GetGameScene().GetPlayer()
				.PlayerRealDead();
			HideRebirthMsg();
		}
	}

	public void ShowRebirthMsg()
	{
		if (!ShowTutorialMsgDead() && Msg_box_option != null)
		{
			Msg_box_option.GetComponent<MsgBoxDelegate>().Show("USE A COMEBACK PACK?", MsgBoxType.Rebirth);
			is_rebirth_msg = true;
			rebirth_time = 10f;
		}
	}

	public void HideRebirthMsg()
	{
		if (Msg_box_option != null)
		{
			Msg_box_option.GetComponent<MsgBoxDelegate>().Hide();
			is_rebirth_msg = false;
		}
	}

	public void SetMultiScene(GameScene scene)
	{
		mulit_scene = scene;
	}

	public void OnReverseHearWaiting()
	{
		if (mulit_scene.GetGameExcute())
		{
			Waiting_panel.transform.localPosition = new Vector3(0f, 0f, Waiting_panel.transform.localPosition.z);
			Debug.Log("OnReverseHearWaiting...");
		}
	}

	public void OnReverseHearRenew()
	{
		if (mulit_scene.GetGameExcute())
		{
			Waiting_panel.transform.localPosition = new Vector3(0f, -5000f, Waiting_panel.transform.localPosition.z);
			Debug.Log("OnReverseHearRenew...");
		}
	}

	public void OnReverseHearTimeout()
	{
		if (mulit_scene.GetGameExcute())
		{
			mulit_scene.OnReverseHearTimeout();
			Waiting_panel.transform.localPosition = new Vector3(0f, -5000f, Waiting_panel.transform.localPosition.z);
			if (Msg_box_option != null)
			{
				Msg_box_option.GetComponent<MsgBoxDelegate>().Hide();
			}
			Msg_box.GetComponent<MsgBoxDelegate>().Show("YOU WERE DISCONNECTED.", MsgBoxType.ContectingTimeout);
		}
	}

	public void ShowTutorialMsg()
	{
		if (Msg_box_option != null)
		{
			Msg_box_option.GetComponent<MsgBoxDelegate>().Hide();
		}
		Debug.Log("-------------" + GameApp.GetInstance().GetGameState().multi_toturial);
		Msg_box.GetComponent<MsgBoxDelegate>().Show(MultiGameToturial.Msg_Content[GameApp.GetInstance().GetGameState().multi_toturial++], MsgBoxType.MultiToturial);
		Time.timeScale = 0f;
	}

	public void OnClosed()
	{
		Time.timeScale = 0f;
		Debug.Log("Game Scene OnClosed-----------");
		Waiting_panel.transform.localPosition = new Vector3(0f, -5000f, Waiting_panel.transform.localPosition.z);
		if (Msg_box_option != null)
		{
			Msg_box_option.GetComponent<MsgBoxDelegate>().Hide();
		}
		if (Msg_box != null)
		{
			Msg_box.GetComponent<MsgBoxDelegate>().Show("YOU WERE DISCONNECTED.", MsgBoxType.ContectingLost);
		}
	}

	public void OnContectingLost()
	{
		Time.timeScale = 0f;
		Waiting_panel.transform.localPosition = new Vector3(0f, -5000f, Waiting_panel.transform.localPosition.z);
		if (Msg_box_option != null)
		{
			Msg_box_option.GetComponent<MsgBoxDelegate>().Hide();
		}
		if (Msg_box != null)
		{
			Msg_box.GetComponent<MsgBoxDelegate>().Show("YOU WERE DISCONNECTED.", MsgBoxType.ContectingLost);
		}
	}

	public bool ShowTutorialMsgDead()
	{
		if (GameApp.GetInstance().GetGameState().multi_toturial_triger_game_dead == 1)
		{
			if (Msg_box_option != null)
			{
				Msg_box_option.GetComponent<MsgBoxDelegate>().Hide();
			}
			Msg_box.GetComponent<MsgBoxDelegate>().Show(MultiGameToturial.Msg_Content[5], MsgBoxType.MultiToturial);
			GameApp.GetInstance().GetGameState().multi_toturial_triger_game_dead = 0;
			GameApp.GetInstance().PlayerPrefsSave();
			is_dead_msg = true;
			return true;
		}
		return false;
	}

	public void ShowTutorialMsgRescue(int step)
	{
		if (GameApp.GetInstance().GetGameState().multi_toturial_triger_game_rescue != 1)
		{
			return;
		}
		switch (step)
		{
		case 1:
			if (Msg_box_option != null)
			{
				Msg_box_option.GetComponent<MsgBoxDelegate>().Hide();
			}
			Msg_box.GetComponent<MsgBoxDelegate>().Show(MultiGameToturial.Msg_Content[6], MsgBoxType.MultiToturial);
			to_next_msg = true;
			break;
		case 2:
			if (Msg_box_option != null)
			{
				Msg_box_option.GetComponent<MsgBoxDelegate>().Hide();
			}
			Msg_box.GetComponent<MsgBoxDelegate>().Show(MultiGameToturial.Msg_Content[7], MsgBoxType.MultiToturial);
			GameApp.GetInstance().GetGameState().multi_toturial_triger_game_rescue = 0;
			GameApp.GetInstance().PlayerPrefsSave();
			to_next_msg = false;
			break;
		}
	}
}
