using UnityEngine;
using Zombie3D;

public class Step6Script : MonoBehaviour, ITutorialStep, ITutorialUIEvent
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
		player.InputController.EnableMoveInput = true;
		player.InputController.EnableTurningAround = true;
		player.InputController.EnableShootingInput = true;
		guis.EnableTutorialOKButton(false);
	}

	public void UpdateTutorialStep(float deltaTime, Player player)
	{
		GameObject gameObject = GameObject.Find("WoodBox");
		if (gameObject == null)
		{
			GameObject.Find("Arrow").SetActiveRecursively(false);
			ts.GoToNextStep();
		}
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
	}
}
