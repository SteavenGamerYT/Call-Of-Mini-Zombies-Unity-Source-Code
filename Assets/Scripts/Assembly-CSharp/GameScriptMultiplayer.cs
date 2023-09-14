using UnityEngine;
using Zombie3D;

public class GameScriptMultiplayer : MonoBehaviour
{
	protected float lastUpdateTime;

	protected float deltaTime;

	private void Start()
	{
		Debug.Log("start");
		GameApp.GetInstance().Init();
		GameApp.GetInstance().CreateScene();
		GameApp.GetInstance().AddMultiplayerComponents();
		lastUpdateTime = Time.time;
	}

	private void Update()
	{
		deltaTime += Time.deltaTime;
		GameApp.GetInstance().Loop(deltaTime);
		deltaTime = 0f;
	}
}
