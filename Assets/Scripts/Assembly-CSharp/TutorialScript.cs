using System.Collections;
using UnityEngine;
using Zombie3D;

public class TutorialScript : MonoBehaviour
{
	protected ITutorialStep tutorialStep;

	protected ITutorialStep[] steps;

	protected ITutorialGameUI guis;

	protected int currentStepNum;

	protected Player player;

	private IEnumerator Start()
	{
		yield return 0;
		currentStepNum = 0;
		player = GameApp.GetInstance().GetGameScene().GetPlayer();
		guis = GameObject.Find("SceneGUI").GetComponent<GameUIScript>();
		steps = new ITutorialStep[9];
		steps[0] = GetComponent<Step1Script>();
		((ITutorialUIEvent)steps[0]).SetGameGUI(guis);
		steps[1] = GetComponent<Step2Script>();
		((ITutorialUIEvent)steps[1]).SetGameGUI(guis);
		steps[2] = GetComponent<Step3Script>();
		((ITutorialUIEvent)steps[2]).SetGameGUI(guis);
		steps[3] = GetComponent<Step4Script>();
		((ITutorialUIEvent)steps[3]).SetGameGUI(guis);
		steps[4] = GetComponent<Step5Script>();
		((ITutorialUIEvent)steps[4]).SetGameGUI(guis);
		steps[5] = GetComponent<Step6Script>();
		((ITutorialUIEvent)steps[5]).SetGameGUI(guis);
		steps[6] = GetComponent<Step7Script>();
		((ITutorialUIEvent)steps[6]).SetGameGUI(guis);
		steps[7] = GetComponent<Step8Script>();
		((ITutorialUIEvent)steps[7]).SetGameGUI(guis);
		steps[8] = GetComponent<Step9Script>();
		((ITutorialUIEvent)steps[8]).SetGameGUI(guis);
		tutorialStep = steps[currentStepNum];
		guis.SetTutorialUIEvent((ITutorialUIEvent)tutorialStep);
		tutorialStep.StartStep(this, player);
	}

	public void GoToNextStep()
	{
		currentStepNum++;
		if (currentStepNum < steps.Length)
		{
			tutorialStep.EndStep(player);
			tutorialStep = steps[currentStepNum];
			guis.SetTutorialUIEvent((ITutorialUIEvent)tutorialStep);
			tutorialStep.StartStep(this, player);
		}
	}

	private void Update()
	{
		if (player != null)
		{
			tutorialStep.UpdateTutorialStep(Time.deltaTime, player);
		}
	}
}
