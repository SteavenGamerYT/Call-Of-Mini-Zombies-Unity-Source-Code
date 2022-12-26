using System.Text.RegularExpressions;
using UnityEngine;
using Zombie3D;

public class NickNameTUI : MonoBehaviour, TUIHandler
{
	private TUI m_tui;

	protected TUIInput[] input;

	public GameObject Msg_box;

	public GameObject Indicator_Panel;

	public GameObject Editor_Panel;

	public GameObject OK_Button;

	protected Regex myRex;

	private void Awake()
	{
		GameScript.CheckMenuResourceConfig();
		GameScript.CheckGlobalResourceConfig();
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
		if ((bool)Msg_box)
		{
			Msg_box.transform.localPosition = new Vector3(0f, 0f, Msg_box.transform.localPosition.z);
			Msg_box.GetComponent<MsgBoxDelegate>().SetMsgContent(MultiGameToturial.Msg_Content[0], MsgBoxType.MultiToturial);
			Editor_Panel.GetComponent<TUITextField>().Hide();
		}
		else
		{
			Editor_Panel.GetComponent<TUITextField>().Show();
		}
		myRex = new Regex("^[A-Za-z0-9]+$");
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
		if (control.name == "Msg_OK_Button" && eventType == 3)
		{
			MsgBoxType type = Msg_box.GetComponent<MsgBoxDelegate>().m_type;
			Msg_box.GetComponent<MsgBoxDelegate>().Hide();
			switch (type)
			{
			case MsgBoxType.ContectingTimeout:
			case MsgBoxType.ContectingLost:
				SceneName.SaveSceneStatistics("MainMapTUI");
				m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
					.FadeOut("MainMapTUI");
				break;
			case MsgBoxType.None:
			case MsgBoxType.MultiToturial:
				Editor_Panel.GetComponent<TUITextField>().Show();
				break;
			case MsgBoxType.BeKicked:
			case MsgBoxType.RoomDestroyed:
			case MsgBoxType.OnClosed:
			case MsgBoxType.JoinRommFailed:
				break;
			}
		}
		else
		{
			if (!(control.name == "OK") || eventType != 3)
			{
				return;
			}
			Match match = myRex.Match(Editor_Panel.GetComponent<TUITextField>().GetText());
			Editor_Panel.GetComponent<TUITextField>().Hide();
			if (Editor_Panel.GetComponent<TUITextField>().GetText().Length > 0 && match.Success)
			{
				GameApp.GetInstance().GetGameState().multiplay_named = 1;
				GameApp.GetInstance().GetGameState().nick_name = Editor_Panel.GetComponent<TUITextField>().GetText();
				GameApp.GetInstance().Save();
				if (GameApp.GetInstance().GetGameState().multiname_to_coop)
				{
					SceneName.SaveSceneStatistics("NetMapTUI");
					m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
						.FadeOut("NetMapTUI");
				}
				else
				{
					SceneName.SaveSceneStatistics("NetMapVSTUI");
					m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
						.FadeOut("NetMapVSTUI");
				}
			}
			else if ((bool)Msg_box)
			{
				Msg_box.transform.localPosition = new Vector3(0f, 0f, Msg_box.transform.localPosition.z);
				Msg_box.GetComponent<MsgBoxDelegate>().SetMsgContent("Name Error! Try again~", MsgBoxType.None);
				Editor_Panel.GetComponent<TUITextField>().ResetText();
			}
		}
	}
}
