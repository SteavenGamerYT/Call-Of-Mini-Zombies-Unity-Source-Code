using System.Collections.Generic;
using UnityEngine;
using Zombie3D;

public class VSReportUI : MonoBehaviour, TUIHandler
{
	private TUI m_tui;

	protected TUIInput[] input;

	public GameObject ReportPanel;

	public GameObject Report_Scroll;

	public GameObject Report_Scroller;

	protected VSReprotData report_data;

	protected List<VSPlayerReport> sorted_report_data = new List<VSPlayerReport>();

	protected List<VSPlayerReport> unsorted_report_data = new List<VSPlayerReport>();

	private void Start()
	{
		m_tui = TUI.Instance("TUI");
		m_tui.SetHandler(this);
		m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
			.FadeIn();
		if (GameObject.Find("NetviewObj") != null)
		{
			GameObject obj = GameObject.Find("NetviewObj");
			Object.Destroy(obj);
		}
		if ((bool)GameObject.Find("VSReprotObj"))
		{
			GameObject gameObject = GameObject.Find("VSReprotObj");
			report_data = gameObject.GetComponent<VSReprotData>();
			foreach (VSPlayerReport value in report_data.player_reports.Values)
			{
				unsorted_report_data.Add(value);
			}
			while (unsorted_report_data.Count > 0)
			{
				VSPlayerReport item = FindMaxOneReportData();
				unsorted_report_data.Remove(item);
				sorted_report_data.Add(item);
			}
			int num = 0;
			GameObject gameObject2 = null;
			foreach (VSPlayerReport sorted_report_datum in sorted_report_data)
			{
				gameObject2 = Object.Instantiate(Resources.Load("Prefabs/TUI/VSReportItem")) as GameObject;
				gameObject2.transform.parent = Report_Scroll.transform;
				gameObject2.transform.localPosition = new Vector3(0f, 44 - num * 20, gameObject2.transform.localPosition.z);
				num++;
				GameObject gameObject3 = gameObject2.transform.Find("name").gameObject;
				GameObject gameObject4 = gameObject2.transform.Find("kill_count").gameObject;
				GameObject gameObject5 = gameObject2.transform.Find("death_count").gameObject;
				GameObject gameObject6 = gameObject2.transform.Find("cash").gameObject;
				GameObject gameObject7 = gameObject2.transform.Find("combo").gameObject;
				gameObject3.GetComponent<TUIMeshText>().text = sorted_report_datum.nick_name;
				gameObject4.GetComponent<TUIMeshText>().text = sorted_report_datum.kill_cout.ToString();
				gameObject5.GetComponent<TUIMeshText>().text = sorted_report_datum.death_count.ToString();
				gameObject6.GetComponent<TUIMeshText>().text = "$" + sorted_report_datum.loot_cash;
				gameObject7.GetComponent<TUIMeshText>().text = sorted_report_datum.combo_kill.ToString();
				if (sorted_report_datum.isMyself)
				{
					gameObject3.GetComponent<TUIMeshText>().color = Color.yellow;
					gameObject4.GetComponent<TUIMeshText>().color = Color.yellow;
					gameObject5.GetComponent<TUIMeshText>().color = Color.yellow;
					gameObject6.GetComponent<TUIMeshText>().color = Color.yellow;
					gameObject7.GetComponent<TUIMeshText>().color = Color.yellow;
				}
			}
			if (num > 8)
			{
				Report_Scroller.GetComponent<TUIScroll>().rangeYMax = (num - 8) * 20;
				Report_Scroller.GetComponent<TUIScroll>().borderYMax = (num - 8) * 20;
			}
			Object.Destroy(gameObject);
		}
		else
		{
			Debug.Log("No find report data obj.");
		}
		SmartFoxConnection.UnregisterSFSSceneCallbacks();
		SmartFoxConnection.Disconnect();
		SFSHeartBeat.DestroyInstanceObj();
		OpenClickPlugin.Hide();
	}

	private void Update()
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
			SceneName.LoadLevel("NetMapVSTUI");
		}
	}

	public VSPlayerReport FindMaxOneReportData()
	{
		VSPlayerReport result = null;
		int num = -9999;
		foreach (VSPlayerReport unsorted_report_datum in unsorted_report_data)
		{
			if (unsorted_report_datum.kill_cout > num)
			{
				result = unsorted_report_datum;
				num = unsorted_report_datum.kill_cout;
			}
		}
		return result;
	}
}
