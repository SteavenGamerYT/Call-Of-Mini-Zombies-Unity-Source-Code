using System.Collections;
using UnityEngine;
using Zombie3D;

public class GameLoading : MonoBehaviour
{
	protected AsyncOperation async;

	private IEnumerator Start()
	{
		yield return 1;
		GameApp.GetInstance().GetGameState().endless_level = false;
		async = Application.LoadLevelAdditiveAsync("Zombie3D_Village2");
		yield return 1;
	}

	private void Update()
	{
		if (async != null && !async.isDone)
		{
			Debug.Log("porsess async: " + async.progress + Time.time);
		}
	}
}
