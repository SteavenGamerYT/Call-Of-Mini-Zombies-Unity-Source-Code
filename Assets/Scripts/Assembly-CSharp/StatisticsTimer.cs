using UnityEngine;
using Zombie3D;

public class StatisticsTimer : MonoBehaviour
{
	private void Update()
	{
		GameApp.GetInstance().GetGameState().user_statistics.play_time += Time.deltaTime;
		GameApp.GetInstance().GetGameState().TotalTime += Time.deltaTime;
	}
}
