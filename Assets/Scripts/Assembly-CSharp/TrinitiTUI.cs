using UnityEngine;
using Zombie3D;

public class TrinitiTUI : MonoBehaviour, TUIHandler
{
	private TUI m_tui;

	protected TUIInput[] input;

	protected float startTime;

	private void Start()
	{
		OpenClickPlugin.Initialize("567D21BF-DA59-41F2-B7CC-9951F6187640");
		TapjoyPlugin.RequestConnect("a5e2b768-5c51-424c-aed2-62de204a8272", "gmupeHGvRpp8CXmfNsd9");
		startTime = Time.time;
		GameCenterPlugin.Login();
		if (Application.isEditor || Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.WindowsPlayer)
		{
			Application.runInBackground = true;
		}
		else
		{
			Application.runInBackground = false;
		}
	}

	private void Update()
	{
		input = TUIInputManager.GetInput();
		for (int i = 0; i < input.Length; i++)
		{
			m_tui.HandleInput(input[i]);
		}
		if (Time.time - startTime > 3f)
		{
			SceneName.LoadLevel("CoopAdTUI");
		}
	}

	public void HandleEvent(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
	}
}
