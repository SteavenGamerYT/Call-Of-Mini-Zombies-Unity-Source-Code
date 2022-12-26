using UnityEngine;
using Zombie3D;

public class Step9Script : MonoBehaviour, ITutorialStep, ITutorialUIEvent
{
	protected ITutorialGameUI guis;

	protected TutorialScript ts;

	protected Enemy enemy;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void StartStep(TutorialScript ts, Player player)
	{
		this.ts = ts;
		player.InputController.EnableMoveInput = false;
		player.InputController.EnableTurningAround = false;
		player.InputController.EnableShootingInput = false;
		player.InputController.CameraRotation = Vector2.zero;
		guis.EnableTutorialOKButton(true);
		string text = "\nCongrats, you've completed the call of mini bootcamp! Get ready to fight for survival, one day at a time.";
		text = text.ToUpper();
		GameDialog dialog = GameUIScript.GetGameUIScript().GetDialog();
		dialog.SetText(text);
		dialog.Show();
	}

	public void UpdateTutorialStep(float deltaTime, Player player)
	{
	}

	public void EndStep(Player player)
	{
	}

	public void SetGameGUI(ITutorialGameUI guis)
	{
		this.guis = guis;
	}

	public void OK(Player player)
	{
		GameApp.GetInstance().GetGameState().FirstTimeGame = false;
		GameApp.GetInstance().Save();
		SceneName.LoadLevel("Zombie3D_Arena");
	}
}
