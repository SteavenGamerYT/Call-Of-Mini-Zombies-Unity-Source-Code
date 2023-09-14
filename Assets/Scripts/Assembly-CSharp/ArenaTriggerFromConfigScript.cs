using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zombie3D;

public class ArenaTriggerFromConfigScript : MonoBehaviour
{
	protected Player player;

	protected int waveNum;

	protected GameParametersXML paramXML;

	protected GameScene gameScene;

	protected GameObject[] doors;

	protected int currentSpawnIndex;

	protected float waveStartTime;

	protected float waveEndTime;

	protected int currentDoorIndex;

	protected int doorCount;

	protected bool levelClear;

	private SpawnConfig spawnConfigInfo;

	public int WaveNum
	{
		get
		{
			return waveNum;
		}
	}

	private IEnumerator Start()
	{
		while (GameApp.GetInstance().GetGameState().VS_mode)
		{
			yield return 1;
		}
		while (GameApp.GetInstance().GetGameState().endless_level)
		{
			yield return 0;
		}
		yield return 0;
		while (!GameUIScript.GetGameUIScript()._finishInit)
		{
			yield return 1;
		}
		gameScene = GameApp.GetInstance().GetGameScene();
		GameApp.GetInstance().GetGameScene().ArenaTrigger = this;
		doors = GameObject.FindGameObjectsWithTag("Door");
		doorCount = doors.Length;
		waveStartTime = Time.time;
		player = GameApp.GetInstance().GetGameScene().GetPlayer();
		paramXML = new GameParametersXML();
		if (Application.isEditor || Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.Android)
		{
			spawnConfigInfo = paramXML.Load(null, GameApp.GetInstance().GetGameState().LevelNum, false);
		}
		else
		{
			spawnConfigInfo = paramXML.Load("/", GameApp.GetInstance().GetGameState().LevelNum, false);
		}
		int limit = GameApp.GetInstance().GetGameConfig().globalConf.enemyLimit;
		waveNum = 0;
		int roundNum = 0;
		gameScene.ArenaTrigger = this;
		if (GameApp.GetInstance().GetGameState().hunting_level)
		{
			Debug.Log("SpawnPreyOne:");
			int num = GameApp.GetInstance().GetGameState().Hunting_val - 2;
			bool old_one = true;
			bool isHellFirer = false;
			if (GameApp.GetInstance().GetGameState().Hunting_val == 0)
			{
				isHellFirer = true;
				num = 9;
			}
			else if (GameApp.GetInstance().GetGameState().Hunting_val == 1)
			{
				num = UnityEngine.Random.Range(0, 7);
				old_one = false;
				GameApp.GetInstance().GetGameState().Hunting_val = num + 2;
				GameApp.GetInstance().PlayerPrefsSave();
			}
			Enemy enemy2 = SpawnPreyOne(old_one, (EnemyType)num, isHellFirer);
			GameUIScript.GetGameUIScript().InitHuntingTimeText(enemy2);
		}
		Debug.Log("wave num:" + spawnConfigInfo.Waves.Count);
		foreach (Wave wave in spawnConfigInfo.Waves)
		{
			bool tem_go = ((GameApp.GetInstance().GetGameScene().GetEnemies()
				.Count == 0) ? true : false);
			while (!tem_go && Time.time - waveStartTime < 1800f)
			{
				yield return 0;
				switch (GameApp.GetInstance().GetGameScene().GetEnemies()
					.Count)
				{
				case 0:
					tem_go = true;
					break;
				case 1:
				{
					IEnumerator enumerator2 = GameApp.GetInstance().GetGameScene().GetEnemies()
						.Values.GetEnumerator();
					try
					{
						while (enumerator2.MoveNext())
						{
							Enemy enemy3 = (Enemy)enumerator2.Current;
							if (enemy3.m_isPrey)
							{
								tem_go = true;
							}
						}
					}
					finally
					{
						IDisposable disposable;
						if ((disposable = enumerator2 as IDisposable) != null)
						{
							disposable.Dispose();
						}
					}
					break;
				}
				}
			}
			waveNum++;
			waveStartTime = Time.time;
			gameScene.CalculateDifficultyFactor(GameApp.GetInstance().GetGameState().LevelNum);
			Debug.Log("Wave " + waveNum);
			GameApp.GetInstance().GetGameScene().BonusWeapon = null;
			GameApp.GetInstance().DebugInfo = string.Empty;
			yield return new WaitForSeconds(wave.intermission);
			foreach (Round round in wave.Rounds)
			{
				roundNum++;
				yield return new WaitForSeconds(round.intermission);
				foreach (EnemyInfo enemyInfo in round.EnemyInfos)
				{
					EnemyType enemyType = enemyInfo.EType;
					int spawnNum = enemyInfo.Count;
					SpawnFromType from = enemyInfo.From;
					GameObject enemyPrefab3 = null;
					Transform grave = null;
					if (from == SpawnFromType.Grave)
					{
						grave = CalculateGravePosition(player.GetTransform());
					}
					for (int i = 0; i < spawnNum; i++)
					{
						bool bElite3 = false;
						bElite3 = EliteSpawn(enemyType, spawnNum, i);
						enemyPrefab3 = (bElite3 ? GameApp.GetInstance().GetEnemyResourceConfig().enemy_elite[(int)enemyType] : GameApp.GetInstance().GetEnemyResourceConfig().enemy[(int)enemyType]);
						for (int enemyLeft = GameApp.GetInstance().GetGameScene().GetEnemies()
							.Count; enemyLeft >= limit; enemyLeft = GameApp.GetInstance().GetGameScene().GetEnemies()
							.Count)
						{
							yield return 0;
						}
						Vector3 spawnPosition = Vector3.zero;
						switch (from)
						{
						case SpawnFromType.Door:
							spawnPosition = doors[currentDoorIndex].transform.position;
							currentDoorIndex++;
							if (currentDoorIndex == doorCount)
							{
								currentDoorIndex = 0;
							}
							break;
						case SpawnFromType.Grave:
						{
							float x = UnityEngine.Random.Range((0f - grave.localScale.x) / 2f, grave.localScale.x / 2f);
							float z = UnityEngine.Random.Range((0f - grave.localScale.z) / 2f, grave.localScale.z / 2f);
							spawnPosition = grave.position + new Vector3(x, 0f, z);
							UnityEngine.Object.Instantiate(GameApp.GetInstance().GetResourceConfig().graveRock, spawnPosition + Vector3.down * 0.3f, Quaternion.identity);
							break;
						}
						}
						GameObject currentEnemy3 = null;
						if (bElite3)
						{
							currentEnemy3 = UnityEngine.Object.Instantiate(enemyPrefab3, spawnPosition, Quaternion.Euler(0f, 0f, 0f));
						}
						else
						{
							currentEnemy3 = gameScene.GetEnemyPool(enemyType).CreateObject(spawnPosition, Quaternion.Euler(0f, 0f, 0f));
							currentEnemy3.layer = 9;
						}
						currentEnemy3.name = "E_" + GameApp.GetInstance().GetGameScene().GetNextEnemyID();
						Enemy enemy = null;
						switch (enemyType)
						{
						case EnemyType.E_ZOMBIE:
							enemy = new Zombie();
							break;
						case EnemyType.E_NURSE:
							enemy = new Nurse();
							break;
						case EnemyType.E_TANK:
							enemy = new Tank();
							currentEnemy3.transform.Translate(Vector3.up * 2f);
							break;
						case EnemyType.E_HUNTER:
							enemy = new Hunter();
							break;
						case EnemyType.E_BOOMER:
							enemy = new Boomer();
							break;
						case EnemyType.E_SWAT:
							enemy = new Swat();
							break;
						case EnemyType.E_DOG:
							enemy = new Dog();
							break;
						case EnemyType.E_POLICE:
							enemy = new Police();
							break;
						}
						enemy.IsElite = bElite3;
						enemy.Init(currentEnemy3);
						enemy.EnemyType = enemyType;
						enemy.Name = currentEnemy3.name;
						if (from == SpawnFromType.Grave)
						{
							enemy.SetInGrave(true);
						}
						GameApp.GetInstance().GetGameScene().GetEnemies()
							.Add(currentEnemy3.name, enemy);
						yield return new WaitForSeconds(0.3f);
					}
				}
			}
		}
		int enemyCount = GameApp.GetInstance().GetGameScene().GetEnemies()
			.Count;
		while (enemyCount > 0)
		{
			enemyCount = GameApp.GetInstance().GetGameScene().GetEnemies()
				.Count;
			yield return 0;
		}
		GameApp.GetInstance().GetGameScene().GamePlayingState = PlayingState.GameWin;
		List<Weapon> weaponList = GameApp.GetInstance().GetGameState().GetWeapons();
		GameConfig gConfig = GameApp.GetInstance().GetGameConfig();
		WeaponConfig wConf = gConfig.GetUnLockWeapon(GameApp.GetInstance().GetGameState().LevelNum);
		if (wConf != null)
		{
			foreach (Weapon item in weaponList)
			{
				if (item.Name == wConf.name && item.Exist == WeaponExistState.Locked)
				{
					item.Exist = WeaponExistState.Unlocked;
					GameApp.GetInstance().GetGameScene().BonusWeapon = item;
					break;
				}
			}
		}
		else
		{
			GameApp.GetInstance().GetGameScene().BonusWeapon = null;
		}
		GameApp.GetInstance().GetGameState().Achievement.CheckAchievemnet_Survivior(GameApp.GetInstance().GetGameState().LevelNum);
		GameApp.GetInstance().GetGameState().Achievement.CheckAchievemnet_LastSurvivior(GameApp.GetInstance().GetGameState().LevelNum);
		GameApp.GetInstance().GetGameState().Achievement.SubmitScore(GameApp.GetInstance().GetGameState().LevelNum);
		GameApp.GetInstance().GetGameState().LevelNum++;
		GameApp.GetInstance().GetGameState().user_statistics.day_count++;
		GameApp.GetInstance().GetGameState().user_statistics.last_day_state = "Day_" + GameApp.GetInstance().GetGameState().LevelNum + "_Finished";
		GameApp.GetInstance().Save();
		OpenClickPlugin.Show(false);
		yield return new WaitForSeconds(4f);
		if (gameScene.BonusWeapon != null)
		{
			GameUIScript.GetGameUIScript().ClearLevelInfo();
			GameUIScript.GetGameUIScript().GetPanel(2).Show();
			yield return new WaitForSeconds(4f);
		}
		FadeAnimationScript.GetInstance().StartFade(new Color(0f, 0f, 0f, 0f), new Color(0f, 0f, 0f, 1f), 1f);
		yield return new WaitForSeconds(1.5f);
		UIResourceMgr.GetInstance().UnloadAllUIMaterials();
		OpenClickPlugin.Hide();
		SceneName.LoadLevel("MainMapTUI");
	}

	public bool EliteSpawn(EnemyType eType, int spawnNum, int index)
	{
		bool result = false;
		switch (eType)
		{
		case EnemyType.E_ZOMBIE:
			result = ((spawnNum < 5) ? Math.RandomRate(5f) : (index % 5 == 4));
			break;
		case EnemyType.E_NURSE:
			result = ((spawnNum < 4) ? Math.RandomRate(10f) : (index % 4 == 3));
			break;
		case EnemyType.E_BOOMER:
			result = Math.RandomRate(10f);
			break;
		case EnemyType.E_SWAT:
			result = Math.RandomRate(15f);
			break;
		case EnemyType.E_HUNTER:
			if (GameApp.GetInstance().GetGameState().LevelNum > 15)
			{
				result = Math.RandomRate(30f);
			}
			break;
		case EnemyType.E_TANK:
			if (GameApp.GetInstance().GetGameState().LevelNum > 20)
			{
				result = Math.RandomRate(50f);
			}
			break;
		case EnemyType.E_DOG:
			result = Math.RandomRate(15f);
			break;
		case EnemyType.E_POLICE:
			result = Math.RandomRate(15f);
			break;
		}
		return result;
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

	private void Update()
	{
	}

	public Enemy SpawnPreyOne(bool old_one, EnemyType type, bool isHellFirer)
	{
		if (type == EnemyType.E_BOOMER)
		{
			type = EnemyType.E_HUNTER;
		}
		GameObject gameObject = null;
		bool flag = false;
		if (UnityEngine.Random.Range(0, 10) > 5)
		{
			flag = true;
		}
		if (!isHellFirer)
		{
			gameObject = (flag ? GameApp.GetInstance().GetEnemyResourceConfig().enemy_elite[(int)type] : GameApp.GetInstance().GetEnemyResourceConfig().enemy[(int)type]);
		}
		else
		{
			GameObject gameObject2 = UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Avata/HellFirer")) as GameObject;
			AvataConfigScript component = gameObject2.GetComponent<AvataConfigScript>();
			gameObject = component.Avata_Instance;
		}
		Vector3 zero = Vector3.zero;
		zero = doors[0].transform.position;
		GameObject gameObject3 = null;
		if (flag || isHellFirer)
		{
			gameObject3 = UnityEngine.Object.Instantiate(gameObject, zero, Quaternion.Euler(0f, 0f, 0f));
		}
		else
		{
			gameObject3 = GameApp.GetInstance().GetGameScene().GetEnemyPool(type)
				.CreateObject(zero, Quaternion.Euler(0f, 0f, 0f));
			gameObject3.layer = 9;
		}
		gameObject3.name = "E_" + GameApp.GetInstance().GetGameScene().GetNextEnemyID();
		int num = (int)type;
		Enemy enemy = null;
		switch (num)
		{
		case 0:
			enemy = new Zombie();
			break;
		case 1:
			enemy = new Nurse();
			break;
		case 2:
			enemy = new Tank();
			gameObject3.transform.Translate(Vector3.up * 2f);
			break;
		case 3:
			enemy = new Hunter();
			break;
		case 5:
			enemy = new Swat();
			break;
		case 6:
			enemy = new Dog();
			break;
		case 7:
			enemy = new Police();
			break;
		case 9:
			enemy = new HellFirer();
			break;
		}
		enemy.m_isPrey = true;
		enemy.IsElite = flag;
		enemy.Init(gameObject3);
		enemy.EnemyType = type;
		enemy.Name = gameObject3.name;
		GameApp.GetInstance().GetGameScene().GetEnemies()
			.Add(gameObject3.name, enemy);
		return enemy;
	}

	private void OnDrawGizmos()
	{
		if (gameScene != null)
		{
			Vector3[] path = gameScene.GetPath();
			if (path != null && path.Length > 0)
			{
				Vector3 from = path[path.Length - 1];
				Vector3[] array = path;
				Vector3[] array2 = array;
				foreach (Vector3 vector in array2)
				{
					Gizmos.color = Color.white;
					Gizmos.DrawSphere(vector, 0.1f);
					Gizmos.DrawLine(from, vector);
					from = vector;
				}
			}
		}
		if (gameScene == null || gameScene.GetEnemies() == null)
		{
			return;
		}
		IEnumerator enumerator = gameScene.GetEnemies().Values.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Enemy enemy = (Enemy)enumerator.Current;
				if (enemy.LastTarget != Vector3.zero)
				{
					Gizmos.color = Color.blue;
					Gizmos.DrawSphere(enemy.LastTarget, 0.3f);
					Gizmos.DrawLine(enemy.ray.origin, enemy.rayhit.point);
				}
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = enumerator as IDisposable) != null)
			{
				disposable.Dispose();
			}
		}
	}
}
