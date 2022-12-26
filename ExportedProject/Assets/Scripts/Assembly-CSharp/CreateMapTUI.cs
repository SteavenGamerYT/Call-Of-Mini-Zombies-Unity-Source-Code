using UnityEngine;
using Zombie3D;

public class CreateMapTUI : MonoBehaviour, TUIHandler
{
	private TUI m_tui;

	protected TUIInput[] input;

	public GameObject Msg_box;

	public GameObject Indicator_panel;

	public GameObject Toturial_Mask;

	protected NetworkObj net_com;

	public MultiplayCreateMapUIPanel CreateMapUIPanel;

	private void Start()
	{
		m_tui = TUI.Instance("TUI");
		m_tui.SetHandler(this);
		m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
			.FadeIn();
		m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFadeEx>()
			.m_fadeout = OnSceneChange;
		net_com = GameApp.GetInstance().GetGameState().net_com;
		if ((bool)net_com)
		{
			net_com.reverse_heart_timeout_delegate = OnReverseHearTimeout;
			net_com.reverse_heart_renew_delegate = OnReverseHearRenew;
			net_com.reverse_heart_waiting_delegate = OnReverseHearWaiting;
		}
	}

	public bool CheckToturialInit()
	{
		if (GameApp.GetInstance().GetGameState().multi_toturial_triger == 1)
		{
			Msg_box.GetComponent<MsgBoxDelegate>().Show(MultiGameToturial.Msg_Content[GameApp.GetInstance().GetGameState().multi_toturial++], MsgBoxType.MultiToturial);
			return true;
		}
		return false;
	}

	public void OnSceneChange()
	{
		Debug.Log("OnSceneChange");
		if ((bool)net_com)
		{
			net_com.reverse_heart_timeout_delegate = null;
			net_com.reverse_heart_renew_delegate = null;
			net_com.reverse_heart_waiting_delegate = null;
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
		if (!(control.name == "Msg_OK_Button") || eventType != 3)
		{
			return;
		}
		MsgBoxType type = Msg_box.GetComponent<MsgBoxDelegate>().m_type;
		Msg_box.GetComponent<MsgBoxDelegate>().Hide();
		switch (type)
		{
		case MsgBoxType.ContectingTimeout:
			GameApp.GetInstance().GetGameState().FromShopMenu = true;
			SceneName.SaveSceneStatistics("EndlessModeTUI");
			m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
				.FadeOut("EndlessModeTUI");
			break;
		case MsgBoxType.MultiToturial:
			if (CreateMapUIPanel != null)
			{
				CreateMapUIPanel.Enable = true;
			}
			Toturial_Mask.transform.localPosition = new Vector3(0f, 0f, Toturial_Mask.transform.localPosition.z);
			break;
		case MsgBoxType.CrateRommFailed:
			GameApp.GetInstance().GetGameState().FromShopMenu = true;
			SceneName.SaveSceneStatistics("MultiplayRoomTUI");
			m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
				.FadeOut("MultiplayRoomTUI");
			break;
		case MsgBoxType.JoinRommFailed:
		case MsgBoxType.ContectingLost:
			break;
		}
	}

	public void OnToturialMapRight()
	{
		Toturial_Mask.transform.localPosition = new Vector3(0f, 1000f, Toturial_Mask.transform.localPosition.z);
	}

	public void OnToturialMapError()
	{
		Toturial_Mask.transform.localPosition = new Vector3(0f, 0f, Toturial_Mask.transform.localPosition.z);
	}

	public void OnReverseHearWaiting()
	{
		if (Indicator_panel != null)
		{
			Indicator_panel.GetComponent<IndicatorTUI>().SetContent("WAITING FOR SERVER.");
			Indicator_panel.GetComponent<IndicatorTUI>().Show();
		}
		Debug.Log("OnReverseHearWaiting...");
	}

	public void OnReverseHearRenew()
	{
		if (Indicator_panel != null)
		{
			Indicator_panel.GetComponent<IndicatorTUI>().Hide();
			Debug.Log("OnReverseHearRenew...");
		}
	}

	public void OnReverseHearTimeout()
	{
		if (Indicator_panel != null)
		{
			Indicator_panel.GetComponent<IndicatorTUI>().Hide();
		}
		if (Msg_box != null)
		{
			Msg_box.GetComponent<MsgBoxDelegate>().Show("YOU WERE DISCONNECTED.", MsgBoxType.ContectingTimeout);
		}
	}

	public void OnCreateRoomError(int result)
	{
		if (Msg_box != null)
		{
			Msg_box.GetComponent<MsgBoxDelegate>().Show("Unable to create room, \nplease try again.".ToUpper(), MsgBoxType.CrateRommFailed);
		}
	}
}
