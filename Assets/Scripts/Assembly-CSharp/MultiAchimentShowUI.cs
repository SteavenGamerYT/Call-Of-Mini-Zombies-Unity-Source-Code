using UnityEngine;
using Zombie3D;

public class MultiAchimentShowUI : MonoBehaviour, TUIHandler
{
	private TUI m_tui;

	protected TUIInput[] input;

	public GameObject Content_Panel;

	public GameObject Msg_Box_Panel;

	public GameObject Back_button;

	public GameObject panel_back_button;

	public void Start()
	{
		m_tui = TUI.Instance("TUI");
		m_tui.SetHandler(this);
		m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
			.FadeIn();
		if (Msg_Box_Panel != null && GameApp.GetInstance().GetGameState().multi_toturial_triger_achi == 1)
		{
			Msg_Box_Panel.GetComponent<MsgBoxDelegate>().Show(MultiGameToturial.Msg_Content[8], MsgBoxType.MultiToturial);
			GameApp.GetInstance().GetGameState().multi_toturial_triger_achi = 0;
			GameApp.GetInstance().Save();
		}
		OpenClickPlugin.Hide();
		NetworkObj.DestroyNetCom();
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
			SceneName.LoadLevel("NetMapTUI");
		}
		else if (control.name.StartsWith("Achivment_Button") && eventType == 3)
		{
			Debug.Log(control.name + " level:" + control.gameObject.GetComponent<AchievementData>().achievement_data.level);
			ShowAchivmentContent(control.GetComponent<AchievementData>().achievement_data);
		}
		else if (control.name == "Panel_Back_Button" && eventType == 3)
		{
			Content_Panel.transform.localPosition = new Vector3(0f, 500f, -10f);
			Back_button.active = true;
		}
		else
		{
			if (!(control.name == "Msg_OK_Button") || eventType != 3)
			{
				return;
			}
			MsgBoxType type = Msg_Box_Panel.GetComponent<MsgBoxDelegate>().m_type;
			Msg_Box_Panel.GetComponent<MsgBoxDelegate>().Hide();
			if (type == MsgBoxType.MultiToturial)
			{
				if (GameApp.GetInstance().GetGameState().multi_toturial == 15)
				{
					Back_button.GetComponent<TUIButtonClick>().enabled = false;
				}
				else if (GameApp.GetInstance().GetGameState().multi_toturial == 16)
				{
					Back_button.GetComponent<TUIButtonClick>().enabled = true;
					Debug.Log("Mulit Tutorial Finish!");
					GameApp.GetInstance().GetGameState().multi_toturial_triger = 0;
					GameApp.GetInstance().GetGameState().multi_toturial = 0;
					GameApp.GetInstance().PlayerPrefsSave();
				}
			}
		}
	}

	public void ShowAchivmentContent(MultiAchievementCfg achi)
	{
		if ((bool)Content_Panel)
		{
			Content_Panel.transform.Find("PanelBK/IconBk").GetComponent<TUIMeshSprite>().frameName = achi.icon_bk;
			Content_Panel.transform.Find("PanelBK/Icon").GetComponent<TUIMeshSprite>().frameName = achi.icon;
			Content_Panel.transform.Find("PanelBK/Label_Title").GetComponent<TUIMeshText>().text = achi.name;
			string content = achi.content;
			content = content.Replace("\\n", "\n");
			Content_Panel.transform.Find("PanelBK/Label_Content").GetComponent<TUIMeshText>().text = content;
			string text = string.Empty;
			if (achi.reward_cash > 0)
			{
				text = "REWARD: $" + achi.reward_cash;
			}
			if (achi.reward_avata != AvatarType.None)
			{
				text = text + "    REWARD: " + achi.reward_avata.ToString().ToUpper();
			}
			if (achi.reward_weapon != "none")
			{
				text = text + "     REWARD: UNLOCK " + achi.reward_weapon.ToString();
			}
			Content_Panel.transform.Find("PanelBK/Label_Reward").GetComponent<TUIMeshText>().text = text;
			Content_Panel.transform.localPosition = new Vector3(0f, 0f, -10f);
			Back_button.active = false;
		}
	}
}
