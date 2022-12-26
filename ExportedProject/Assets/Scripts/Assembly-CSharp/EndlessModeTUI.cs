using UnityEngine;
using Zombie3D;

public class EndlessModeTUI : MonoBehaviour, TUIHandler
{
	private TUI m_tui;

	protected TUIInput[] input;

	public GameObject coop_button;

	public void Start()
	{
		m_tui = TUI.Instance("TUI");
		m_tui.SetHandler(this);
		m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
			.FadeIn();
		if (GameApp.GetInstance().GetGameState().multi_toturial_triger == 1 && coop_button != null)
		{
			coop_button.GetComponent<TUIButtonClickAni>().SetAnimationState(true);
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
			GameApp.GetInstance().GetGameState().FromShopMenu = true;
			SceneName.SaveSceneStatistics("MainMapTUI");
			m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
				.FadeOut("MainMapTUI");
		}
		else if (control.name == "Single_Button" && eventType == 3)
		{
			GameApp.GetInstance().GetGameState().FromShopMenu = true;
			SceneName.SaveSceneStatistics("EndlessMenuUI");
			m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
				.FadeOut("EndlessMenuUI");
		}
		else if (control.name == "Multi_Button" && eventType == 3)
		{
			GameApp.GetInstance().GetGameState().FromShopMenu = true;
			if (GameApp.GetInstance().GetGameState().multiplay_named == 0)
			{
				SceneName.SaveSceneStatistics("NickNameTUI");
				m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
					.FadeOut("NickNameTUI");
			}
			else
			{
				SceneName.SaveSceneStatistics("MultiplayRoomTUI");
				m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
					.FadeOut("MultiplayRoomTUI");
			}
		}
	}
}
