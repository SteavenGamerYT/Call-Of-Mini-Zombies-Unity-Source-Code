using System;
using System.Collections;
using UnityEngine;
using Zombie3D;

public class ArenaTriggerEndlessScript : MonoBehaviour
{
	protected Player player;

	protected int waveNum;

	protected GameParametersXML paramXML;

	protected GameScene gameScene;

	protected GameObject[] doors;

	protected int currentSpawnIndex;

	protected float waveStartTime;

	protected int currentDoorIndex;

	protected int doorCount;

	protected bool levelClear;

	private SpawnConfig spawnConfigInfo;

	private SpawnConfig spawnConfigInfo_boss;

	protected int isGrave;

	protected int uBElite;

	public int WaveNum
	{
		get
		{
			return waveNum;
		}
	}

	public void SetWaveNum(int wave)
	{
		waveNum = wave;
	}

	private IEnumerator Start()
	{
		while (GameApp.GetInstance().GetGameState().VS_mode)
		{
			yield return 1;
		}
		while (!GameApp.GetInstance().GetGameState().endless_level)
		{
			yield return 0;
		}
		yield return 0;
		while (!GameUIScript.GetGameUIScript()._finishInit)
		{
			yield return 1;
		}
		gameScene = GameApp.GetInstance().GetGameScene();
		gameScene.ArenaTrigger_Endless = this;
		doors = GameObject.FindGameObjectsWithTag("Door");
		doorCount = doors.Length;
		waveStartTime = Time.time;
		player = GameApp.GetInstance().GetGameScene().GetPlayer();
		paramXML = new GameParametersXML();
		if (Application.isEditor || Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.Android)
		{
			spawnConfigInfo = paramXML.Load(null, 0, true);
			spawnConfigInfo_boss = paramXML.Load(null, 1, true);
		}
		else
		{
			spawnConfigInfo = paramXML.Load("/", 0, true);
			spawnConfigInfo_boss = paramXML.Load("/", 1, true);
		}
		int limit = GameApp.GetInstance().GetGameConfig().globalConf.enemyLimit;
		waveNum = 0;
		int roundNum = 0;
		while (GameApp.GetInstance().GetGameState().endless_multiplayer && !GameApp.GetInstance().GetGameState().net_com.m_netUserInfo.is_master)
		{
			yield return 0;
		}
		while (true)
		{
			foreach (Wave wave in spawnConfigInfo.Waves)
			{
				int enemyLeft2 = GameApp.GetInstance().GetGameScene().GetEnemies()
					.Count;
				while (enemyLeft2 > 0 && Time.time - waveStartTime < 1800f)
				{
					yield return 0;
					enemyLeft2 = GameApp.GetInstance().GetGameScene().GetEnemies()
						.Count;
				}
				GameApp.GetInstance().GetGameState().Cur_endless_rank = waveNum;
				waveNum++;
				waveStartTime = Time.time;
				gameScene.CalculateDifficultyFactor(waveNum);
				Debug.Log("Now endless wave : " + waveNum);
				GameApp.GetInstance().GetGameScene().BonusWeapon = null;
				GameApp.GetInstance().DebugInfo = string.Empty;
				yield return new WaitForSeconds(wave.intermission);
				foreach (Round round in wave.Rounds)
				{
					roundNum++;
					yield return new WaitForSeconds(round.intermission);
					foreach (EnemyInfo enemyInfo2 in round.EnemyInfos)
					{
						EnemyType enemyType2 = enemyInfo2.EType;
						int spawnNum2 = enemyInfo2.Count + (int)(0.2f * (float)waveNum);
						SpawnFromType from2 = enemyInfo2.From;
						GameObject enemyPrefab3 = null;
						Transform grave2 = null;
						if (from2 == SpawnFromType.Grave)
						{
							grave2 = CalculateGravePosition(player.GetTransform());
						}
						for (int j = 0; j < spawnNum2; j++)
						{
							bool bElite3 = false;
							bElite3 = EliteSpawn(enemyType2, spawnNum2, j);
							enemyPrefab3 = (bElite3 ? GameApp.GetInstance().GetEnemyResourceConfig().enemy_elite[(int)enemyType2] : GameApp.GetInstance().GetEnemyResourceConfig().enemy[(int)enemyType2]);
							for (enemyLeft2 = GameApp.GetInstance().GetGameScene().GetEnemies()
								.Count; enemyLeft2 >= limit; enemyLeft2 = GameApp.GetInstance().GetGameScene().GetEnemies()
								.Count)
							{
								yield return 0;
							}
							Vector3 spawnPosition2 = Vector3.zero;
							switch (from2)
							{
							case SpawnFromType.Door:
								spawnPosition2 = doors[currentDoorIndex].transform.position;
								currentDoorIndex++;
								if (currentDoorIndex == doorCount)
								{
									currentDoorIndex = 0;
								}
								break;
							case SpawnFromType.Grave:
							{
								float x = UnityEngine.Random.Range((0f - grave2.localScale.x) / 2f, grave2.localScale.x / 2f);
								float z = UnityEngine.Random.Range((0f - grave2.localScale.z) / 2f, grave2.localScale.z / 2f);
								spawnPosition2 = grave2.position + new Vector3(x, 0f, z);
								UnityEngine.Object.Instantiate(GameApp.GetInstance().GetGameResourceConfig().graveRock, spawnPosition2 + Vector3.down * 0.3f, Quaternion.identity);
								break;
							}
							}
							GameObject currentEnemy5 = null;
							if (bElite3)
							{
								currentEnemy5 = UnityEngine.Object.Instantiate(enemyPrefab3, spawnPosition2, Quaternion.identity);
							}
							else
							{
								currentEnemy5 = gameScene.GetEnemyPool(enemyType2).CreateObject(spawnPosition2, Quaternion.identity);
								currentEnemy5.layer = 9;
							}
							int enemyID2 = GameApp.GetInstance().GetGameScene().GetNextEnemyID();
							currentEnemy5.name = "E_" + enemyID2;
							Enemy enemy2 = null;
							switch (enemyType2)
							{
							case EnemyType.E_ZOMBIE:
								enemy2 = new Zombie();
								break;
							case EnemyType.E_NURSE:
								enemy2 = new Nurse();
								break;
							case EnemyType.E_TANK:
								enemy2 = new Tank();
								currentEnemy5.transform.Translate(Vector3.up * 2f);
								break;
							case EnemyType.E_HUNTER:
								enemy2 = new Hunter();
								break;
							case EnemyType.E_BOOMER:
								enemy2 = new Boomer();
								break;
							case EnemyType.E_SWAT:
								enemy2 = new Swat();
								break;
							case EnemyType.E_DOG:
								enemy2 = new Dog();
								break;
							case EnemyType.E_POLICE:
								enemy2 = new Police();
								break;
							}
							enemy2.IsElite = bElite3;
							enemy2.Init(currentEnemy5);
							enemy2.EnemyType = enemyType2;
							enemy2.Name = currentEnemy5.name;
							isGrave = ((from2 == SpawnFromType.Grave) ? 1 : 0);
							uBElite = (bElite3 ? 1 : 0);
							if (from2 == SpawnFromType.Grave)
							{
								enemy2.SetInGrave(true);
							}
							GameApp.GetInstance().GetGameScene().GetEnemies()
								.Add(currentEnemy5.name, enemy2);
							if (GameApp.GetInstance().GetGameState().endless_multiplayer && GameApp.GetInstance().GetGameState().net_com.m_netUserInfo.is_master)
							{
								Packet packet = CGEnemyBirthPacket.MakePacket((uint)waveNum, (uint)enemyID2, (uint)enemyType2, (uint)uBElite, (uint)isGrave, 0u, spawnPosition2, (uint)enemy2.TargetPlayer.m_multi_id);
								GameApp.GetInstance().GetGameState().net_com.Send(packet);
							}
							yield return new WaitForSeconds(0.3f);
						}
					}
				}
				if (waveNum % 3 != 0)
				{
					continue;
				}
				Debug.Log("boss!!");
				int index_boss = UnityEngine.Random.Range(0, spawnConfigInfo_boss.Waves[0].Rounds.Count);
				Round round_boss = spawnConfigInfo_boss.Waves[0].Rounds[index_boss];
				foreach (EnemyInfo enemyInfo in round_boss.EnemyInfos)
				{
					EnemyType enemyType = enemyInfo.EType;
					int spawnNum = enemyInfo.Count;
					SpawnFromType from = enemyInfo.From;
					GameObject boss_enemyPrefab3 = null;
					Transform grave = null;
					if (from == SpawnFromType.Grave)
					{
						grave = CalculateGravePosition(player.GetTransform());
					}
					for (int i = 0; i < spawnNum; i++)
					{
						bool bElite = true;
						boss_enemyPrefab3 = GameApp.GetInstance().GetEnemyResourceConfig().enemy_elite[(int)enemyType];
						for (enemyLeft2 = GameApp.GetInstance().GetGameScene().GetEnemies()
							.Count; enemyLeft2 >= limit; enemyLeft2 = GameApp.GetInstance().GetGameScene().GetEnemies()
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
							float x2 = UnityEngine.Random.Range((0f - grave.localScale.x) / 2f, grave.localScale.x / 2f);
							float z2 = UnityEngine.Random.Range((0f - grave.localScale.z) / 2f, grave.localScale.z / 2f);
							spawnPosition = grave.position + new Vector3(x2, 0f, z2);
							UnityEngine.Object.Instantiate(GameApp.GetInstance().GetGameResourceConfig().graveRock, spawnPosition + Vector3.down * 0.3f, Quaternion.identity);
							break;
						}
						}
						GameObject currentEnemy3 = null;
						if (bElite)
						{
							currentEnemy3 = UnityEngine.Object.Instantiate(boss_enemyPrefab3, spawnPosition, Quaternion.Euler(0f, 0f, 0f));
						}
						else
						{
							currentEnemy3 = gameScene.GetEnemyPool(enemyType).CreateObject(spawnPosition, Quaternion.Euler(0f, 0f, 0f));
							currentEnemy3.layer = 9;
						}
						int enemyID = GameApp.GetInstance().GetGameScene().GetNextEnemyID();
						currentEnemy3.name = "E_" + enemyID;
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
						enemy.IsElite = bElite;
						enemy.m_isBoss = true;
						enemy.Init(currentEnemy3);
						enemy.EnemyType = enemyType;
						enemy.Name = currentEnemy3.name;
						if (from == SpawnFromType.Grave)
						{
							enemy.SetInGrave(true);
						}
						GameApp.GetInstance().GetGameScene().GetEnemies()
							.Add(currentEnemy3.name, enemy);
						if (GameApp.GetInstance().GetGameState().endless_multiplayer && GameApp.GetInstance().GetGameState().net_com.m_netUserInfo.is_master)
						{
							Packet packet2 = CGEnemyBirthPacket.MakePacket((uint)waveNum, (uint)enemyID, (uint)enemyType, (uint)uBElite, (uint)isGrave, 1u, spawnPosition, (uint)enemy.TargetPlayer.m_multi_id);
							GameApp.GetInstance().GetGameState().net_com.Send(packet2);
						}
						yield return new WaitForSeconds(0.3f);
					}
				}
			}
		}
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
			result = Math.RandomRate(30f);
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
