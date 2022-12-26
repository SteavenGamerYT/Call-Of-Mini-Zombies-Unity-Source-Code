using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zombie3D
{
	public class GameScene
	{
		protected Player player;

		protected BaseCameraScript camera;

		protected Hashtable enemyList;

		protected Quest quest;

		protected List<BombSpot> bombSpotList;

		protected GameObject[] woodboxList;

		protected Vector3[] path;

		public List<Player> m_multi_player_arr;

		public List<Player> m_player_set;

		public Enemy m_prey_enemy;

		protected ObjectPool hitBloodObjectPool = new ObjectPool();

		protected ObjectPool[] deadBodyObjectPool = new ObjectPool[8];

		protected ObjectPool[] enemyObjectPool = new ObjectPool[8];

		protected string sceneName;

		protected int infectionRate;

		protected int sceneIndex;

		protected int enemyNum;

		protected PlayingState playingState;

		protected float difficultyDamageFactor = 1f;

		protected float difficultyHpFactor = 1f;

		protected float difficultyCashDropFactor = 1f;

		protected int killed;

		protected int triggerCount;

		protected int enemyID;

		protected float spawnWoodBoxesTime = -30f;

		protected float lastPrintTime;

		protected GameObject m_woodboxs;

		public GameObject[] scene_points;

		protected bool is_game_excute;

		protected ArenaTriggerFromConfigScript arenaTrigger;

		protected ArenaTriggerEndlessScript arenaTrigger_endless;

		public float endless_start_time;

		public float endless_get_cash;

		public Weapon BonusWeapon { get; set; }

		public ArenaTriggerFromConfigScript ArenaTrigger
		{
			get
			{
				return arenaTrigger;
			}
			set
			{
				arenaTrigger = value;
			}
		}

		public ArenaTriggerEndlessScript ArenaTrigger_Endless
		{
			get
			{
				return arenaTrigger_endless;
			}
			set
			{
				arenaTrigger_endless = value;
			}
		}

		public PlayingState GamePlayingState
		{
			get
			{
				return playingState;
			}
			set
			{
				playingState = value;
			}
		}

		public int EnemyNum
		{
			get
			{
				return enemyNum;
			}
		}

		public int Killed
		{
			get
			{
				return killed;
			}
			set
			{
				killed = value;
			}
		}

		public int EnemyID
		{
			get
			{
				return enemyID;
			}
		}

		public ObjectPool HitBloodObjectPool
		{
			get
			{
				return hitBloodObjectPool;
			}
		}

		public float GetDifficultyDamageFactor
		{
			get
			{
				return difficultyDamageFactor;
			}
			set
			{
				difficultyDamageFactor = value;
			}
		}

		public float GetDifficultyCashDropFactor
		{
			get
			{
				return difficultyCashDropFactor;
			}
			set
			{
				difficultyCashDropFactor = value;
			}
		}

		public float GetDifficultyHpFactor
		{
			get
			{
				return difficultyHpFactor;
			}
			set
			{
				difficultyHpFactor = value;
			}
		}

		public virtual void Init(int index)
		{
			GameApp.GetInstance().DebugInfo = string.Empty;
			sceneIndex = index;
			sceneName = Application.loadedLevelName.Substring(9);
			CreateSceneData();
			hitBloodObjectPool.Init("HitBlood", GameApp.GetInstance().GetGameResourceConfig().hitBlood, 3, 0.4f);
			for (int i = 0; i < deadBodyObjectPool.Length; i++)
			{
				deadBodyObjectPool[i] = new ObjectPool();
				enemyObjectPool[i] = new ObjectPool();
			}
			deadBodyObjectPool[0].Init("DeadBody_Zombie", GameApp.GetInstance().GetEnemyResourceConfig().deadbody[0], 10, 2f);
			deadBodyObjectPool[1].Init("DeadBody_Nurse", GameApp.GetInstance().GetEnemyResourceConfig().deadbody[1], 5, 2f);
			deadBodyObjectPool[4].Init("DeadBody_Boomer", GameApp.GetInstance().GetEnemyResourceConfig().deadbody[4], 5, 2f);
			deadBodyObjectPool[5].Init("DeadBody_Swat", GameApp.GetInstance().GetEnemyResourceConfig().deadbody[5], 5, 2f);
			enemyObjectPool[0].Init("Zombies", GameApp.GetInstance().GetEnemyResourceConfig().enemy[0], 10, 0f);
			enemyObjectPool[1].Init("Nurses", GameApp.GetInstance().GetEnemyResourceConfig().enemy[1], 5, 0f);
			enemyObjectPool[2].Init("Tanks", GameApp.GetInstance().GetEnemyResourceConfig().enemy[2], 2, 0f);
			enemyObjectPool[3].Init("Hunters", GameApp.GetInstance().GetEnemyResourceConfig().enemy[3], 2, 0f);
			enemyObjectPool[4].Init("Boomers", GameApp.GetInstance().GetEnemyResourceConfig().enemy[4], 5, 0f);
			enemyObjectPool[5].Init("Swats", GameApp.GetInstance().GetEnemyResourceConfig().enemy[5], 5, 0f);
			enemyObjectPool[6].Init("Dogs", GameApp.GetInstance().GetEnemyResourceConfig().enemy[6], 5, 0f);
			enemyObjectPool[7].Init("Polices", GameApp.GetInstance().GetEnemyResourceConfig().enemy[7], 5, 0f);
			camera = GameObject.Find("Main Camera").GetComponent<TPSSimpleCameraScript>();
			if (camera == null)
			{
				camera = GameObject.Find("Main Camera").GetComponent<TopWatchingCameraScript>();
			}
			else if (!camera.enabled)
			{
				camera = GameObject.Find("Main Camera").GetComponent<TopWatchingCameraScript>();
			}
			player = new Player();
			player.Init();
			camera.Init();
			enemyList = new Hashtable();
			playingState = PlayingState.GamePlaying;
			GameApp.GetInstance().GetGameState().Achievement.CheckAchievemnet_NeverGiveUp();
			enemyNum = 0;
			killed = 0;
			triggerCount = 0;
			enemyID = 0;
			Color[] array = new Color[8]
			{
				Color.white,
				Color.red,
				Color.blue,
				Color.yellow,
				Color.magenta,
				Color.gray,
				Color.grey,
				Color.cyan
			};
			int num = Random.Range(0, array.Length);
			RenderSettings.ambientLight = array[num];
			m_woodboxs = GameObject.Find("WoodBoxes");
			scene_points = GameObject.FindGameObjectsWithTag("WayPoint");
			if (GameApp.GetInstance().GetGameState().endless_level)
			{
				m_woodboxs.SetActiveRecursively(false);
				endless_start_time = Time.time;
				endless_get_cash = 0f;
			}
		}

		public void AddNetworkComponents()
		{
		}

		public Player GetPlayer()
		{
			return player;
		}

		public BaseCameraScript GetCamera()
		{
			return camera;
		}

		public Hashtable GetEnemies()
		{
			return enemyList;
		}

		public Enemy GetEnemyByID(string enemyID)
		{
			return (Enemy)enemyList[enemyID];
		}

		public int GetNextTriggerID()
		{
			triggerCount++;
			return triggerCount;
		}

		public int GetNextEnemyID()
		{
			enemyID++;
			return enemyID;
		}

		public int IncreaseKills()
		{
			killed++;
			return killed;
		}

		public ObjectPool GetDeadBodyPool(EnemyType eType)
		{
			return deadBodyObjectPool[(int)eType];
		}

		public ObjectPool GetEnemyPool(EnemyType eType)
		{
			return enemyObjectPool[(int)eType];
		}

		public void CalculateDifficultyFactor(int wave)
		{
			if (GameApp.GetInstance().GetGameState().endless_level)
			{
				int num = (wave - 1) / 3;
				float endless_a = GameApp.GetInstance().GetGameConfig().endless_a1;
				float endless_a2 = GameApp.GetInstance().GetGameConfig().endless_a2;
				float endless_b = GameApp.GetInstance().GetGameConfig().endless_b1;
				float endless_b2 = GameApp.GetInstance().GetGameConfig().endless_b2;
				difficultyCashDropFactor = GameApp.GetInstance().GetGameConfig().endless_cash + (float)num * 0.05f;
				difficultyHpFactor = endless_a * (float)wave + endless_b;
				difficultyDamageFactor = endless_a2 * (float)wave + endless_b2;
				return;
			}
			if (wave > 85)
			{
				wave = 85;
			}
			int num2 = (wave - 1) / 3;
			difficultyDamageFactor = 1f + (float)num2 * 0.1f + (float)(wave / 5) * 0.2f;
			difficultyHpFactor = 1f;
			for (int i = 0; i < num2; i++)
			{
				difficultyHpFactor *= 1.3f;
			}
			difficultyCashDropFactor = 1f + (float)num2 * 0.05f;
		}

		public void ModifyEnemyNum(int num)
		{
			enemyNum += num;
		}

		public int GetSceneIndex()
		{
			return sceneIndex;
		}

		public int GetInfectionRate()
		{
			return infectionRate;
		}

		public string GetSceneName()
		{
			return sceneName;
		}

		public virtual void DoLogic(float deltaTime)
		{
			player.DoLogic(deltaTime);
			object[] array = new object[enemyList.Count];
			enemyList.Keys.CopyTo(array, 0);
			for (int i = 0; i < array.Length; i++)
			{
				Enemy enemy = enemyList[array[i]] as Enemy;
				enemy.DoLogic(deltaTime);
			}
			hitBloodObjectPool.AutoDestruct();
			deadBodyObjectPool[0].AutoDestruct();
			deadBodyObjectPool[1].AutoDestruct();
			deadBodyObjectPool[4].AutoDestruct();
			deadBodyObjectPool[5].AutoDestruct();
		}

		public void CreateSceneData()
		{
			GameObject[] array = GameObject.FindGameObjectsWithTag("Path");
			path = new Vector3[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				path[i] = array[i].transform.position;
			}
			woodboxList = GameObject.FindGameObjectsWithTag("WoodBox");
		}

		public void ClearAllSceneResources()
		{
			player = null;
			enemyList.Clear();
			woodboxList = null;
			path = null;
			camera = null;
		}

		public void CreateBombSpots()
		{
			bombSpotList = new List<BombSpot>();
			GameObject[] array = GameObject.FindGameObjectsWithTag("BombSpot");
			List<int> list = new List<int>();
			for (int i = 0; i < array.Length; i++)
			{
				list.Add(i);
			}
			if (list.Count <= 0)
			{
				return;
			}
			int index = Random.Range(0, list.Count);
			int num = list[index];
			list.Remove(num);
			index = Random.Range(0, list.Count - 1);
			int num2 = list[index];
			list.Remove(num2);
			index = Random.Range(0, list.Count - 2);
			int num3 = list[index];
			for (int j = 0; j < array.Length; j++)
			{
				if (j != num && j != num2 && j != num3)
				{
					Object.Destroy(array[j]);
					continue;
				}
				BombSpot bombSpot = new BombSpot();
				bombSpot.bombSpotObj = array[j];
				array[j].active = true;
				bombSpot.Init();
				bombSpotList.Add(bombSpot);
			}
		}

		public List<BombSpot> GetBombSpots()
		{
			return bombSpotList;
		}

		public Vector3[] GetPath()
		{
			return path;
		}

		public Quest GetQuest()
		{
			return quest;
		}

		public GameObject[] GetWoodBoxes()
		{
			return woodboxList;
		}

		public void RefreshWoodBoxes()
		{
			Object.Instantiate(GameApp.GetInstance().GetResourceConfig().woodBoxes);
			woodboxList = GameObject.FindGameObjectsWithTag("WoodBox");
			spawnWoodBoxesTime = Time.time;
		}

		public virtual void OnMultiPlayerDead(Player mPlayer)
		{
		}

		public virtual void CheckMultiGameOver()
		{
		}

		public virtual bool GetGameExcute()
		{
			return is_game_excute;
		}

		public virtual void TimeGameOver(float time)
		{
			Debug.Log("Base scene TimeGameOver.");
		}

		public virtual void OnReverseHearTimeout()
		{
			Debug.Log("Base scene OnReverseHearTimeout.");
		}
	}
}
