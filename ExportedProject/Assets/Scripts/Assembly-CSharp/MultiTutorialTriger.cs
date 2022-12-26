using System.Collections;
using UnityEngine;
using Zombie3D;

public class MultiTutorialTriger : MonoBehaviour
{
	public GameMultiplayerScene game_scnen;

	public Player LocalPlayer;

	public Player AIPlayer;

	public bool isEnd;

	private IEnumerator Start()
	{
		while (game_scnen == null || LocalPlayer == null || AIPlayer == null)
		{
			yield return 1;
		}
		Debug.Log("MultiTutorial Start~~");
		yield return new WaitForSeconds(1f);
		EnemyInfo enemyInfo = new EnemyInfo
		{
			Count = 1,
			EType = EnemyType.E_ZOMBIE,
			From = SpawnFromType.Grave
		};
		EnemyType enemyType = enemyInfo.EType;
		int spawnNum = enemyInfo.Count;
		SpawnFromType from = enemyInfo.From;
		GameObject enemyPrefab3 = null;
		Transform grave = null;
		if (from == SpawnFromType.Grave)
		{
			grave = CalculateGravePosition(AIPlayer.GetTransform());
		}
		bool bElite = false;
		enemyPrefab3 = GameApp.GetInstance().GetEnemyResourceConfig().enemy[(int)enemyType];
		Vector3 spawnPosition3 = Vector3.zero;
		float rndX = Random.Range(-2, 2);
		float rndZ = Random.Range(-2, 2);
		spawnPosition3 = grave.position + new Vector3(rndX, 0f, rndZ);
		Object.Instantiate(GameApp.GetInstance().GetResourceConfig().graveRock, spawnPosition3 + Vector3.down * 0.3f, Quaternion.identity);
		GameObject currentEnemy3 = null;
		currentEnemy3 = game_scnen.GetEnemyPool(enemyType).CreateObject(spawnPosition3, Quaternion.Euler(0f, 0f, 0f));
		currentEnemy3.layer = 9;
		currentEnemy3.name = "E_" + GameApp.GetInstance().GetGameScene().GetNextEnemyID();
		Enemy enemy = new ZombieTutorial();
		enemy.IsElite = bElite;
		enemy.Init(currentEnemy3);
		enemy.EnemyType = enemyType;
		enemy.Name = currentEnemy3.name;
		enemy.TargetPlayer = AIPlayer;
		enemy.SetInGrave(true);
		GameApp.GetInstance().GetGameScene().GetEnemies()
			.Add(currentEnemy3.name, enemy);
		while (!isEnd)
		{
			yield return 1;
		}
		yield return new WaitForSeconds(1f);
		game_scnen.game_tui.ShowTutorialMsg();
	}

	private void Update()
	{
	}

	public Transform CalculateGravePosition(Transform playerTrans)
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("Grave");
		GameObject gameObject = null;
		float num = 99999f;
		GameObject[] array2 = array;
		GameObject[] array3 = array2;
		foreach (GameObject gameObject2 in array3)
		{
			float sqrMagnitude = (playerTrans.position - gameObject2.transform.position).sqrMagnitude;
			if (sqrMagnitude < num)
			{
				gameObject = gameObject2;
				num = sqrMagnitude;
			}
		}
		return gameObject.transform;
	}
}
