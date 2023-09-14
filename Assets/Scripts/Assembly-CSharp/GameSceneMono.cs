using UnityEngine;
using Zombie3D;

public class GameSceneMono : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
	}

	public void TimerTask(string method, float time)
	{
		Invoke(method, time);
	}

	public void GameOver()
	{
		GameMultiplayerScene gameMultiplayerScene = GameApp.GetInstance().GetGameScene() as GameMultiplayerScene;
		gameMultiplayerScene.OnGameOver(null, null, false);
	}
}
