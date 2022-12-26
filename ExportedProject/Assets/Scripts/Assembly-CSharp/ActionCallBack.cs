using UnityEngine;
using Zombie3D;

public class ActionCallBack : MonoBehaviour
{
	public GameObject actor;

	public GameObject leftFoot;

	public GameObject rightFoot;

	protected ResourceConfigScript rConfig;

	protected Vector3 position;

	protected Vector3 rot;

	protected bool isSnowScene;

	private void Start()
	{
		rConfig = (rConfig = GameApp.GetInstance().GetResourceConfig());
		if (Application.loadedLevelName == "Zombie3D_Village")
		{
			isSnowScene = true;
		}
	}

	private void Update()
	{
	}

	public void OnFootTouchFloor(int isLeft)
	{
		if (isSnowScene)
		{
			if (isLeft == 1)
			{
				position = leftFoot.transform.position;
			}
			else
			{
				position = rightFoot.transform.position;
			}
			rot = actor.transform.rotation.eulerAngles;
			rot.x = 270f;
			rot.y += 180f;
			Object.Instantiate(rConfig.foot_print, position, Quaternion.Euler(rot));
		}
	}
}
