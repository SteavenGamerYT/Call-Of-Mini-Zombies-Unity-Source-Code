using UnityEngine;
using Zombie3D;

public class Step1Script : MonoBehaviour, ITutorialStep, ITutorialUIEvent
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
		GameObject gameObject = Object.Instantiate(GameApp.GetInstance().GetResourceConfig().halo, player.GetTransform().TransformPoint(Vector3.forward * 15f), Quaternion.Euler(90f, 0f, 0f));
		gameObject.name = "Halo";
		string text = "\nUse the left joystick to walk around. Go ahead and walk towards the circle of pulsating light.";
		text = text.ToUpper();
		GameDialog dialog = GameUIScript.GetGameUIScript().GetDialog();
		dialog.SetText(text);
		dialog.Show();
	}

	public void UpdateTutorialStep(float deltaTime, Player player)
	{
		guis.EnableTutorialOKButton(true);
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
