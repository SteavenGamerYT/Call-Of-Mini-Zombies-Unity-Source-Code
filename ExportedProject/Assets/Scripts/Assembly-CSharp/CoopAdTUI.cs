using UnityEngine;
using Zombie3D;

public class CoopAdTUI : MonoBehaviour, TUIHandler
{
	private TUI m_tui;

	protected TUIInput[] input;

	public void Start()
	{
		m_tui = TUI.Instance("TUI");
		m_tui.SetHandler(this);
		m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
			.FadeIn();
		PushNotification.ReSetNotifications();
		XAdManagerWrapper.SetVideoAdUrl("http://itunes.apple.com/us/app/call-of-mini-last-stand/id494829360?ls=1&mt=8");
		XAdManagerWrapper.ShowVideoAdLocal();
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
		if (control.name == "Skip_Button" && eventType == 3)
		{
			SceneName.SaveSceneStatistics("StartMenuUI");
			m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
				.FadeOut("StartMenuUI");
			control.gameObject.active = false;
		}
	}
}
