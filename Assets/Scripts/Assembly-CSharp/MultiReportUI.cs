using System;
using UnityEngine;
using Zombie3D;

public class MultiReportUI : MonoBehaviour, TUIHandler
{
	private TUI m_tui;

	protected TUIInput[] input;

	public GameObject Label_Time;

	public GameObject Report_Scoll_Obj;

	public GameObject Scoll_Obj;

	public GameObject Report_Panel;

	public GameObject Achievement_Panel;

	public GameObject Content_Panel;

	public GameObject Achievement_Panel_Base;

	public GameObject Achievement_Panel_Scroll;

	public GameObject OK_Button;

	public GameObject Report_Button;

	public GameObject Msg_box;

	protected MultiReportData muliti_data;

	protected int finish_achievement_count;

	protected Vector3 button_ori_pos;

	public void Start()
	{
		m_tui = TUI.Instance("TUI");
		m_tui.SetHandler(this);
		m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
			.FadeIn();
		GameObject gameObject = GameObject.Find("MultiReportData");
		if ((bool)gameObject)
		{
			muliti_data = GameObject.Find("MultiReportData").GetComponent<MultiReportData>();
			SetLabelContent(str: new TimeSpan(0, 0, (int)muliti_data.play_time).ToString(), label: Label_Time);
			if (Achievement_Panel_Base != null)
			{
				Achievement_Panel_Base.GetComponent<AchimentReportPanelBase>().InitAchivments();
			}
			int num = 0;
			foreach (UserReportData user_report_datum in muliti_data.user_report_data)
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate(Resources.Load("Prefabs/TUI/ReportItem")) as GameObject;
				gameObject2.GetComponent<ReportItem>().SetContent(user_report_datum.name, user_report_datum.mKill_count, user_report_datum.mDeath_count, user_report_datum.mCash_loot, user_report_datum.mDamage_val);
				gameObject2.transform.parent = Report_Scoll_Obj.transform;
				gameObject2.transform.localPosition = new Vector3(0f, 35 - num * 40, -1f);
				if (num == 0)
				{
					gameObject2.GetComponent<ReportItem>().Win_Tip.active = true;
				}
				else
				{
					gameObject2.GetComponent<ReportItem>().Win_Tip.active = false;
				}
				num++;
			}
			muliti_data.CheckMultiAchievementFinishStatus();
			muliti_data.GiveReward();
			GameApp.GetInstance().Save();
			foreach (MultiAchievementCfg item in muliti_data.Achievement_Finished_Array)
			{
				if ((bool)Scoll_Obj)
				{
					Scoll_Obj.GetComponent<AchimentReportPanel>().AddAchievementToList(item);
				}
			}
			button_ori_pos = Report_Button.transform.localPosition;
			if (muliti_data.Achievement_Finished_Array.Count > 0)
			{
				Content_Panel.GetComponent<PanelParameter>().HidePanel();
				Report_Panel.GetComponent<PanelParameter>().HidePanel();
				Achievement_Panel.GetComponent<PanelParameter>().ShowPanel();
				OK_Button.transform.localPosition = new Vector3(button_ori_pos.x, 1000f, button_ori_pos.z);
				Report_Button.transform.localPosition = new Vector3(button_ori_pos.x, 1000f, button_ori_pos.z);
			}
			else
			{
				OK_Button.transform.localPosition = button_ori_pos;
				Report_Button.transform.localPosition = new Vector3(button_ori_pos.x, 1000f, button_ori_pos.z);
			}
		}
		OpenClickPlugin.Hide();
		Resources.UnloadUnusedAssets();
		NetworkObj.DestroyNetCom();
		GameApp.GetInstance().ClearScene();
		GC.Collect();
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
		if (control.name == "OK_Button" && eventType == 3)
		{
			SceneName.LoadLevel("NetMapTUI");
		}
		else if (control.name == "Report_Button" && eventType == 3)
		{
			Report_Button.transform.localPosition = new Vector3(button_ori_pos.x, 1000f, button_ori_pos.z);
			OK_Button.transform.localPosition = button_ori_pos;
			Report_Panel.GetComponent<PanelParameter>().ShowPanel();
			Achievement_Panel.GetComponent<PanelParameter>().HidePanel();
			Content_Panel.GetComponent<PanelParameter>().HidePanel();
		}
		else if (control.name == "Panel_Back_Button" && eventType == 3)
		{
			Content_Panel.GetComponent<PanelParameter>().HidePanel();
			Achievement_Panel.GetComponent<PanelParameter>().ShowPanel();
		}
		else if (control.name == "Achievement_Close_Button" && eventType == 3)
		{
			Achievement_Panel_Scroll.transform.localPosition = new Vector3(0f, 2000f, Achievement_Panel_Scroll.transform.localPosition.z);
			Report_Button.transform.localPosition = button_ori_pos;
			if (!(Achievement_Panel_Base != null))
			{
				return;
			}
			foreach (MultiAchievementCfg item in muliti_data.Achievement_Finished_Array)
			{
				Achievement_Panel_Base.GetComponent<AchimentReportPanelBase>().AddAchivmentItemAni(item);
			}
			Achievement_Panel_Base.GetComponent<AchimentReportPanelBase>().CheckAchivmentItems();
		}
		else if (control.name.StartsWith("Achivment_Button") && eventType == 3)
		{
			Debug.Log(control.name + " level:" + control.gameObject.GetComponent<AchievementData>().achievement_data.level);
			ShowAchivmentContent(control.GetComponent<AchievementData>().achievement_data);
		}
		else if (control.name == "Msg_OK_Button" && eventType == 3)
		{
			Msg_box.GetComponent<MsgBoxDelegate>().Hide();
			if (Msg_box.GetComponent<MsgBoxDelegate>().m_type == MsgBoxType.MultiToturial)
			{
				OK_Button.GetComponent<TUIButtonClickTextAni>().SetAnimationState(true);
			}
		}
	}

	public void SetReportTime(string str)
	{
		if ((bool)Label_Time)
		{
			TUIMeshText component = Label_Time.GetComponent<TUIMeshText>();
			if ((bool)component)
			{
				Debug.Log(str);
				component.text = str;
			}
		}
	}

	public void SetLabelContent(GameObject label, string str)
	{
		if ((bool)label)
		{
			TUIMeshText component = label.GetComponent<TUIMeshText>();
			component.text = str;
		}
	}

	public void ShowAchivmentContent(MultiAchievementCfg achi)
	{
		if ((bool)Content_Panel)
		{
			Content_Panel.transform.Find("PanelBK/IconBk").GetComponent<TUIMeshSprite>().frameName = achi.icon_bk;
			Content_Panel.transform.Find("PanelBK/Icon").GetComponent<TUIMeshSprite>().frameName = achi.icon;
			Content_Panel.transform.Find("PanelBK/TitleBk/Label_Title").GetComponent<TUIMeshText>().text = achi.name;
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
			Content_Panel.transform.Find("PanelBK/RewardBk/Label_Reward").GetComponent<TUIMeshText>().text = text;
			Content_Panel.transform.localPosition = new Vector3(0f, 0f, -10f);
		}
	}
}
