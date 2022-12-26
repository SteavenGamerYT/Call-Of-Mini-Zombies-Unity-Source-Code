using UnityEngine;

public class TestTUI : MonoBehaviour, TUIHandler
{
	private TUI m_tui;

	protected TUIInput[] input;

	public GameObject Scoll_Obj;

	public GameObject OK_Button;

	public GameObject Msg_box;

	public void Start()
	{
		m_tui = TUI.Instance("TUI");
		m_tui.SetHandler(this);
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
			Debug.Log(Scoll_Obj.GetComponent<ScaleScroller>().GetSelectedButton().name);
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
}
