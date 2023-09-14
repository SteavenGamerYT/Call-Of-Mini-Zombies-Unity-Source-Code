using UnityEngine;
using Zombie3D;

public class Step5Script : MonoBehaviour, ITutorialStep, ITutorialUIEvent
{
	protected ITutorialGameUI guis;

	protected TutorialScript ts;

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
		string text = "\nNow you're ready to test your weapon. Hold the right joystick to shoot and aim at the glowing crate.";
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
		ts.GoToNextStep();
	}
}
