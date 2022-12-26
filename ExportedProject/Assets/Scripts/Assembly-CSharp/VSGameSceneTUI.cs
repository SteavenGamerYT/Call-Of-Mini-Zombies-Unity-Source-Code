using System;
using UnityEngine;
using Zombie3D;

public class VSGameSceneTUI : MonoBehaviour, TUIHandler
{
	private TUI m_tui;

	protected TUIInput[] input;

	public GameObject Msg_box;

	public GameObject Msg_box_option;

	public GameObject Indicator_panel;

	public GameObject Waiting_panel;

	protected GameScene mulit_scene;

	public bool is_inited;

	public TUIMeshText Label_Time;

	public TUIMeshText Mission_Label_Time;

	public TUIMeshText Label_kill_count;

	public TUIMeshText Label_death_count;

	public GameMessagePanel message_panel;

	public double mission_start_time;

	public float mission_total_time = 300f;

	public float mission_cur_time;

	public VsSeatState vs_seat_state;

	private bool mission_is_over;

	private GameObject Combo_Label;

	private void Start()
	{
		m_tui = TUI.Instance("TUI");
		m_tui.SetHandler(this);
		m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
			.FadeIn();
		SFSHeartBeat.Instance.reverse_heart_timeout_delegate = OnReverseHearTimeout;
		SFSHeartBeat.Instance.reverse_heart_renew_delegate = OnReverseHearRenew;
		SFSHeartBeat.Instance.reverse_heart_waiting_delegate = OnReverseHearWaiting;
		is_inited = true;
	}

	private void Update()
	{
		if (!mission_is_over)
		{
			if (TimeManager.Instance.IsSynchronized())
			{
				mission_cur_time = mission_total_time - (float)(TimeManager.Instance.NetworkTime - mission_start_time) / 1000f;
			}
			if (mission_cur_time <= 0f)
			{
				mission_cur_time = 0f;
				mission_is_over = true;
				Debug.Log("Time out and game over.");
				GameVSScene gameVSScene = mulit_scene as GameVSScene;
				gameVSScene.QuitGameForDisconnect();
			}
		}
		if (Mission_Label_Time != null)
		{
			int seconds = (int)mission_cur_time;
			TimeSpan timeSpan = new TimeSpan(0, 0, seconds);
			Mission_Label_Time.text = timeSpan.ToString();
		}
		input = TUIInputManager.GetInput();
		for (int i = 0; i < input.Length; i++)
		{
			m_tui.HandleInput(input[i]);
		}
	}

	private void OnDestroy()
	{
		if (SFSHeartBeat.Instance != null)
		{
			SFSHeartBeat.Instance.reverse_heart_timeout_delegate = null;
			SFSHeartBeat.Instance.reverse_heart_renew_delegate = null;
			SFSHeartBeat.Instance.reverse_heart_waiting_delegate = null;
		}
	}

	public void HandleEvent(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (control.name == "Msg_OK_Button" && eventType == 3)
		{
			switch (Msg_box.GetComponent<MsgBoxDelegate>().m_type)
			{
			case MsgBoxType.ContectingTimeout:
			case MsgBoxType.ContectingLost:
			{
				Time.timeScale = 1f;
				GameVSScene gameVSScene = mulit_scene as GameVSScene;
				gameVSScene.QuitGameForDisconnect();
				break;
			}
			case MsgBoxType.MultiToturial:
				Msg_box.GetComponent<MsgBoxDelegate>().Hide();
				GameApp.GetInstance().GetGameState().vs_toturial_triger_dead = 0;
				GameApp.GetInstance().PlayerPrefsSave();
				break;
			case MsgBoxType.JoinRommFailed:
				break;
			}
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
			Waiting_panel.transform.localPosition = new Vector3(0f, -5000f, Waiting_panel.transform.localPosition.z);
			if (Msg_box_option != null)
			{
				Msg_box_option.GetComponent<MsgBoxDelegate>().Hide();
			}
			Msg_box.GetComponent<MsgBoxDelegate>().Show("YOU WERE DISCONNECTED.", MsgBoxType.ContectingTimeout);
			mission_is_over = true;
			GameVSScene gameVSScene = mulit_scene as GameVSScene;
			gameVSScene.SetGameExcute(false);
			SmartFoxConnection.UnregisterSFSSceneCallbacks();
			SmartFoxConnection.Disconnect();
			SFSHeartBeat.DestroyInstanceObj();
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

	public void ShowTutorialMsgDead()
	{
		if (GameApp.GetInstance().GetGameState().vs_toturial_triger_dead == 1)
		{
			Msg_box.GetComponent<MsgBoxDelegate>().Show(VSGameToturial.Msg_Content[5], MsgBoxType.MultiToturial);
		}
	}

	public void HideTutorialMsgDead()
	{
		Msg_box.GetComponent<MsgBoxDelegate>().Hide();
		GameApp.GetInstance().GetGameState().vs_toturial_triger_dead = 0;
		GameApp.GetInstance().PlayerPrefsSave();
	}

	public void SetKillCountLabel(int count)
	{
		if (Label_kill_count != null)
		{
			Label_kill_count.text = "KILLS:" + count;
		}
	}

	public void SetDeathCountLabel(int count)
	{
		if (Label_death_count != null)
		{
			Label_death_count.text = "DEATHS:" + count;
		}
	}

	public void SetComboCountLabel(int count)
	{
		if (Combo_Label != null)
		{
			UnityEngine.Object.Destroy(Combo_Label);
			Combo_Label = null;
		}
		if (Label_kill_count != null)
		{
			string text = string.Empty;
			switch (count)
			{
			case 1:
				text = "First Blood";
				break;
			case 2:
				text = "Double Kill";
				break;
			case 3:
				text = "Triple Kill";
				break;
			case 4:
				text = "Quadra Kill";
				break;
			case 5:
				text = "Mega Kill";
				break;
			case 6:
				text = "Ultra Kill";
				break;
			case 7:
				text = "Monster Kill";
				break;
			case 8:
				text = "Overkill";
				break;
			}
			Combo_Label = UnityEngine.Object.Instantiate(Resources.Load("Prefabs/TUI/Combo_Text_Eff")) as GameObject;
			Combo_Label.transform.parent = Label_kill_count.transform.parent;
			Combo_Label.transform.localPosition = new Vector3(0f, 80f, -17f);
			Combo_Label.GetComponent<TUIMeshText>().text = text;
		}
	}
}
