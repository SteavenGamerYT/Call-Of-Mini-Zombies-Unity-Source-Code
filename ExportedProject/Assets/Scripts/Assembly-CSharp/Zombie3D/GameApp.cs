using System;
using System.IO;
using UnityEngine;

namespace Zombie3D
{
	public class GameApp
	{
		protected static GameApp instance;

		protected ResourceConfigScript resourceConfig;

		protected GameConfig gameConfig = new GameConfig();

		protected GameState gameState = new GameState();

		protected GameScene scene;

		protected GameScript script;

		protected ResourceConfigScript _GamerRsourceConfig;

		protected EnemyConfigScript _EnemyResourceConfig;

		protected MenuConfigScript _MenuResourceConfig;

		protected GloabConfigScript _GloabResourceConfig;

		protected string path = Application.persistentDataPath;

		public DeviceOrientation PreviousOrientation { get; set; }

		public string DebugInfo { get; set; }

		protected GameApp()
		{
		}

		public static GameApp GetInstance()
		{
			if (instance == null)
			{
				instance = new GameApp();
				instance.PreviousOrientation = DeviceOrientation.Portrait;
			}
			return instance;
		}

		public void PlayerPrefsSave()
		{
			string empty = string.Empty;
			if (GetInstance().GetGameState().Cur_endless_rank > GetInstance().GetGameState().endless_rank_max)
			{
				empty = "Endless_rank_max";
				PlayerPrefs.SetInt(empty, GetInstance().GetGameState().Cur_endless_rank);
			}
			empty = "Review_count";
			PlayerPrefs.SetInt(empty, GetInstance().GetGameState().review_count);
			empty = "Survival_Welcome_View";
			PlayerPrefs.SetInt(empty, GetInstance().GetGameState().survival_welcome_view);
			empty = "Auto_aim";
			PlayerPrefs.SetInt(empty, GetInstance().GetGameState().is_auto_aim);
			empty = "Hunting_Val";
			PlayerPrefs.SetInt(empty, GetInstance().GetGameState().Hunting_val);
			empty = "Endless_kill_max";
			PlayerPrefs.SetInt(empty, GetInstance().GetGameState().endless_kill_max);
			empty = "Endless_time_max";
			PlayerPrefs.SetInt(empty, GetInstance().GetGameState().endless_time_max);
			empty = "Endless_cash_max";
			PlayerPrefs.SetInt(empty, GetInstance().GetGameState().endless_cash_max);
			empty = "update_welcome_view";
			PlayerPrefs.SetInt(empty, GetInstance().GetGameState().update_welcome_view);
			empty = "Music";
			PlayerPrefs.SetInt(empty, Convert.ToInt32(GetInstance().GetGameState().MusicOn));
			empty = "Sound";
			PlayerPrefs.SetInt(empty, Convert.ToInt32(GetInstance().GetGameState().SoundOn));
			empty = "Paper_Model_Show";
			PlayerPrefs.SetInt(empty, Convert.ToInt32(GetInstance().GetGameState().PaperModelShow));
			empty = "Multiplay_Named";
			PlayerPrefs.SetInt(empty, GetInstance().GetGameState().multiplay_named);
			empty = "Nick_Name";
			PlayerPrefs.SetString(empty, GetInstance().GetGameState().nick_name);
			empty = "First_Statistic";
			PlayerPrefs.SetInt(empty, GetInstance().GetGameState().First_Statistic);
			PlayerPrefs.SetInt(string.Empty, 1);
			empty = "Multiplay_Tutorial_Achi";
			PlayerPrefs.SetInt(empty, GetInstance().GetGameState().multi_toturial_triger_achi);
			empty = "Multiplay_Tutorial_Map";
			PlayerPrefs.SetInt(empty, GetInstance().GetGameState().multi_toturial_triger_map);
			empty = "Multiplay_Tutorial_Hall";
			PlayerPrefs.SetInt(empty, GetInstance().GetGameState().multi_toturial_triger_hall);
			empty = "Multiplay_Tutorial_Room";
			PlayerPrefs.SetInt(empty, GetInstance().GetGameState().multi_toturial_triger_room);
			empty = "Multiplay_Tutorial_Game";
			PlayerPrefs.SetInt(empty, GetInstance().GetGameState().multi_toturial_triger_game_dead);
			empty = "Multiplay_Tutorial_Game_Rescue";
			PlayerPrefs.SetInt(empty, GetInstance().GetGameState().multi_toturial_triger_game_rescue);
			empty = "Multiplay_Tutorial_Room_Master";
			PlayerPrefs.SetInt(empty, GetInstance().GetGameState().multi_toturial_triger_room_master);
			empty = "MacOs_Sensitivity";
			PlayerPrefs.SetFloat(empty, GetInstance().GetGameState().macos_sen);
			empty = "First_Vs_Mode";
			PlayerPrefs.SetInt(empty, GetInstance().GetGameState().first_vs_mode);
			empty = "VS_Tutorial_Map";
			PlayerPrefs.SetInt(empty, GetInstance().GetGameState().vs_toturial_triger_map);
			empty = "VS_Tutorial_Hall";
			PlayerPrefs.SetInt(empty, GetInstance().GetGameState().vs_toturial_triger_hall);
			empty = "VS_Tutorial_Password";
			PlayerPrefs.SetInt(empty, GetInstance().GetGameState().vs_toturial_triger_password);
			empty = "VS_Tutorial_JoinRoom";
			PlayerPrefs.SetInt(empty, GetInstance().GetGameState().vs_toturial_triger_joinRoom);
			empty = "VS_Tutorial_RoomOwner";
			PlayerPrefs.SetInt(empty, GetInstance().GetGameState().vs_toturial_triger_roomOwner);
			empty = "VS_Tutorial_Dead";
			PlayerPrefs.SetInt(empty, GetInstance().GetGameState().vs_toturial_triger_dead);
			empty = "VS_Tutorial_VS";
			PlayerPrefs.SetInt(empty, GetInstance().GetGameState().vs_toturial_triger_vs);
		}

		public void PlayerPrefsReset()
		{
			GetInstance().GetGameState().endless_rank_max = 0;
			PlayerPrefs.SetInt("Endless_rank_max", 0);
			GetInstance().GetGameState().review_count = 0;
			PlayerPrefs.SetInt("Review_count", 0);
			GetInstance().GetGameState().survival_welcome_view = 1;
			PlayerPrefs.SetInt("Survival_Welcome_View", 1);
			GetInstance().GetGameState().Hunting_val = 0;
			PlayerPrefs.SetInt("Hunting_Val", 0);
			GetInstance().GetGameState().is_auto_aim = 1;
			PlayerPrefs.SetInt("Auto_aim", 1);
			GetInstance().GetGameState().MusicOn = true;
			PlayerPrefs.SetInt("Music", 1);
			GetInstance().GetGameState().SoundOn = true;
			PlayerPrefs.SetInt("Sound", 1);
			GetInstance().GetGameState().endless_kill_max = 0;
			PlayerPrefs.SetInt("Endless_kill_max", 0);
			GetInstance().GetGameState().endless_time_max = 0;
			PlayerPrefs.SetInt("Endless_time_max", 0);
			GetInstance().GetGameState().endless_cash_max = 0;
			PlayerPrefs.SetInt("Endless_cash_max", 0);
			GetInstance().GetGameState().multiplay_named = 0;
			PlayerPrefs.SetInt("Multiplay_Named", 0);
			GetInstance().GetGameState().nick_name = "None";
			PlayerPrefs.SetString("Nick_Name", "None");
			GetInstance().GetGameState().PaperModelShow = false;
			PlayerPrefs.SetInt("Paper_Model_Show", 0);
			GetInstance().GetGameState().multi_toturial_triger_achi = 1;
			PlayerPrefs.SetInt("Multiplay_Tutorial_Achi", 1);
			GetInstance().GetGameState().multi_toturial_triger_map = 1;
			PlayerPrefs.SetInt("Multiplay_Tutorial_Map", 1);
			GetInstance().GetGameState().multi_toturial_triger_hall = 1;
			PlayerPrefs.SetInt("Multiplay_Tutorial_Hall", 1);
			GetInstance().GetGameState().multi_toturial_triger_room = 1;
			PlayerPrefs.SetInt("Multiplay_Tutorial_Room", 1);
			GetInstance().GetGameState().multi_toturial_triger_game_dead = 1;
			PlayerPrefs.SetInt("Multiplay_Tutorial_Game", 1);
			GetInstance().GetGameState().multi_toturial_triger_game_rescue = 1;
			PlayerPrefs.SetInt("Multiplay_Tutorial_Game_Rescue", 1);
			GetInstance().GetGameState().multi_toturial_triger_room_master = 1;
			PlayerPrefs.SetInt("Multiplay_Tutorial_Room_Master", 1);
			GetInstance().GetGameState().macos_sen = 1f;
			PlayerPrefs.SetFloat("MacOs_Sensitivity", 1f);
			GetInstance().GetGameState().first_vs_mode = 1;
			PlayerPrefs.SetInt("First_Vs_Mode", 1);
			GetInstance().GetGameState().vs_toturial_triger_map = 1;
			PlayerPrefs.SetInt("VS_Tutorial_Map", 1);
			GetInstance().GetGameState().vs_toturial_triger_hall = 1;
			PlayerPrefs.SetInt("VS_Tutorial_Hall", 1);
			GetInstance().GetGameState().vs_toturial_triger_password = 1;
			PlayerPrefs.SetInt("VS_Tutorial_Password", 1);
			GetInstance().GetGameState().vs_toturial_triger_joinRoom = 1;
			PlayerPrefs.SetInt("VS_Tutorial_JoinRoom", 1);
			GetInstance().GetGameState().vs_toturial_triger_roomOwner = 1;
			PlayerPrefs.SetInt("VS_Tutorial_RoomOwner", 1);
			GetInstance().GetGameState().vs_toturial_triger_dead = 1;
			PlayerPrefs.SetInt("VS_Tutorial_Dead", 1);
			GetInstance().GetGameState().vs_toturial_triger_vs = 1;
			PlayerPrefs.SetInt("VS_Tutorial_VS", 1);
		}

		public void PlayerPrefsLoad()
		{
			if (PlayerPrefs.HasKey("ver"))
			{
				GetInstance().GetGameState().Game_ver = PlayerPrefs.GetFloat("ver");
				if (!PlayerPrefs.HasKey("update_welcome_view"))
				{
					GetInstance().GetGameState().update_welcome_view = 1;
					PlayerPrefs.SetInt("update_welcome_view", 1);
				}
				if (GetInstance().GetGameState().Game_ver != 2.01f)
				{
					GetInstance().GetGameState().update_welcome_view = 1;
					PlayerPrefs.SetInt("update_welcome_view", 1);
					GetInstance().GetGameState().Game_ver = 2.01f;
					PlayerPrefs.SetFloat("ver", 2.01f);
				}
				else
				{
					GetInstance().GetGameState().update_welcome_view = PlayerPrefs.GetInt("update_welcome_view");
				}
			}
			else
			{
				GetInstance().GetGameState().Game_ver = 2.01f;
				PlayerPrefs.SetFloat("ver", 2.01f);
				GetInstance().GetGameState().update_welcome_view = 1;
				PlayerPrefs.SetInt("update_welcome_view", 1);
			}
			if (PlayerPrefs.HasKey("Endless_rank_max"))
			{
				GetInstance().GetGameState().endless_rank_max = PlayerPrefs.GetInt("Endless_rank_max");
			}
			else
			{
				GetInstance().GetGameState().endless_rank_max = 0;
				PlayerPrefs.SetInt("Endless_rank_max", 0);
			}
			if (PlayerPrefs.HasKey("Review_count"))
			{
				GetInstance().GetGameState().review_count = PlayerPrefs.GetInt("Review_count");
			}
			else
			{
				GetInstance().GetGameState().review_count = 0;
				PlayerPrefs.SetInt("Review_count", 0);
			}
			if (PlayerPrefs.HasKey("Survival_Welcome_View"))
			{
				GetInstance().GetGameState().survival_welcome_view = PlayerPrefs.GetInt("Survival_Welcome_View");
			}
			else
			{
				GetInstance().GetGameState().survival_welcome_view = 1;
				PlayerPrefs.SetInt("Survival_Welcome_View", 1);
			}
			if (PlayerPrefs.HasKey("Hunting_Val"))
			{
				GetInstance().GetGameState().Hunting_val = PlayerPrefs.GetInt("Hunting_Val");
			}
			else
			{
				GetInstance().GetGameState().Hunting_val = 0;
				PlayerPrefs.SetInt("Hunting_Val", 0);
			}
			if (PlayerPrefs.HasKey("Auto_aim"))
			{
				GetInstance().GetGameState().is_auto_aim = PlayerPrefs.GetInt("Auto_aim");
			}
			else
			{
				GetInstance().GetGameState().is_auto_aim = 1;
				PlayerPrefs.SetInt("Auto_aim", 1);
			}
			if (PlayerPrefs.HasKey("Music"))
			{
				GetInstance().GetGameState().MusicOn = Convert.ToBoolean(PlayerPrefs.GetInt("Music"));
			}
			else
			{
				GetInstance().GetGameState().MusicOn = true;
				PlayerPrefs.SetInt("Music", 1);
			}
			if (PlayerPrefs.HasKey("Sound"))
			{
				GetInstance().GetGameState().SoundOn = Convert.ToBoolean(PlayerPrefs.GetInt("Sound"));
			}
			else
			{
				GetInstance().GetGameState().SoundOn = true;
				PlayerPrefs.SetInt("Sound", 1);
			}
			if (PlayerPrefs.HasKey("Endless_kill_max"))
			{
				GetInstance().GetGameState().endless_kill_max = PlayerPrefs.GetInt("Endless_kill_max");
			}
			else
			{
				GetInstance().GetGameState().endless_kill_max = 0;
				PlayerPrefs.SetInt("Endless_kill_max", 0);
			}
			if (PlayerPrefs.HasKey("Endless_time_max"))
			{
				GetInstance().GetGameState().endless_time_max = PlayerPrefs.GetInt("Endless_time_max");
			}
			else
			{
				GetInstance().GetGameState().endless_time_max = 0;
				PlayerPrefs.SetInt("Endless_time_max", 0);
			}
			if (PlayerPrefs.HasKey("Endless_cash_max"))
			{
				GetInstance().GetGameState().endless_cash_max = PlayerPrefs.GetInt("Endless_cash_max");
			}
			else
			{
				GetInstance().GetGameState().endless_cash_max = 0;
				PlayerPrefs.SetInt("Endless_cash_max", 0);
			}
			if (PlayerPrefs.HasKey("Multiplay_Named"))
			{
				GetInstance().GetGameState().multiplay_named = PlayerPrefs.GetInt("Multiplay_Named");
			}
			else
			{
				GetInstance().GetGameState().multiplay_named = 0;
				PlayerPrefs.SetInt("Multiplay_Named", 0);
			}
			if (PlayerPrefs.HasKey("Nick_Name"))
			{
				GetInstance().GetGameState().nick_name = PlayerPrefs.GetString("Nick_Name");
			}
			else
			{
				GetInstance().GetGameState().nick_name = "None";
				PlayerPrefs.SetString("Nick_Name", "None");
			}
			if (PlayerPrefs.HasKey("Paper_Model_Show"))
			{
				GetInstance().GetGameState().PaperModelShow = Convert.ToBoolean(PlayerPrefs.GetInt("Paper_Model_Show"));
			}
			else
			{
				GetInstance().GetGameState().PaperModelShow = false;
				PlayerPrefs.SetInt("Paper_Model_Show", 0);
			}
			if (PlayerPrefs.HasKey("First_Statistic"))
			{
				GetInstance().GetGameState().First_Statistic = PlayerPrefs.GetInt("First_Statistic");
			}
			else
			{
				GetInstance().GetGameState().First_Statistic = 1;
				PlayerPrefs.SetInt("First_Statistic", 1);
			}
			if (PlayerPrefs.HasKey("Multiplay_Tutorial_Achi"))
			{
				GetInstance().GetGameState().multi_toturial_triger_achi = PlayerPrefs.GetInt("Multiplay_Tutorial_Achi");
			}
			else
			{
				GetInstance().GetGameState().multi_toturial_triger_achi = 1;
				PlayerPrefs.SetInt("Multiplay_Tutorial_Achi", 1);
			}
			if (PlayerPrefs.HasKey("Multiplay_Tutorial_Map"))
			{
				GetInstance().GetGameState().multi_toturial_triger_map = PlayerPrefs.GetInt("Multiplay_Tutorial_Map");
			}
			else
			{
				GetInstance().GetGameState().multi_toturial_triger_map = 1;
				PlayerPrefs.SetInt("Multiplay_Tutorial_Map", 1);
			}
			if (PlayerPrefs.HasKey("Multiplay_Tutorial_Hall"))
			{
				GetInstance().GetGameState().multi_toturial_triger_hall = PlayerPrefs.GetInt("Multiplay_Tutorial_Hall");
			}
			else
			{
				GetInstance().GetGameState().multi_toturial_triger_hall = 1;
				PlayerPrefs.SetInt("Multiplay_Tutorial_Hall", 1);
			}
			if (PlayerPrefs.HasKey("Multiplay_Tutorial_Room"))
			{
				GetInstance().GetGameState().multi_toturial_triger_room = PlayerPrefs.GetInt("Multiplay_Tutorial_Room");
			}
			else
			{
				GetInstance().GetGameState().multi_toturial_triger_room = 1;
				PlayerPrefs.SetInt("Multiplay_Tutorial_Room", 1);
			}
			if (PlayerPrefs.HasKey("Multiplay_Tutorial_Game"))
			{
				GetInstance().GetGameState().multi_toturial_triger_game_dead = PlayerPrefs.GetInt("Multiplay_Tutorial_Game");
			}
			else
			{
				GetInstance().GetGameState().multi_toturial_triger_game_dead = 1;
				PlayerPrefs.SetInt("Multiplay_Tutorial_Game", 1);
			}
			if (PlayerPrefs.HasKey("Multiplay_Tutorial_Game_Rescue"))
			{
				GetInstance().GetGameState().multi_toturial_triger_game_rescue = PlayerPrefs.GetInt("Multiplay_Tutorial_Game_Rescue");
			}
			else
			{
				GetInstance().GetGameState().multi_toturial_triger_game_rescue = 1;
				PlayerPrefs.SetInt("Multiplay_Tutorial_Game_Rescue", 1);
			}
			if (PlayerPrefs.HasKey("Multiplay_Tutorial_Room_Master"))
			{
				GetInstance().GetGameState().multi_toturial_triger_room_master = PlayerPrefs.GetInt("Multiplay_Tutorial_Room_Master");
			}
			else
			{
				GetInstance().GetGameState().multi_toturial_triger_room_master = 1;
				PlayerPrefs.SetInt("Multiplay_Tutorial_Room_Master", 1);
			}
			if (PlayerPrefs.HasKey("MacOs_Sensitivity"))
			{
				GetInstance().GetGameState().macos_sen = PlayerPrefs.GetFloat("MacOs_Sensitivity");
			}
			else
			{
				GetInstance().GetGameState().macos_sen = 1f;
				PlayerPrefs.SetFloat("MacOs_Sensitivity", 1f);
			}
			if (PlayerPrefs.HasKey("First_Vs_Mode"))
			{
				GetInstance().GetGameState().first_vs_mode = PlayerPrefs.GetInt("First_Vs_Mode");
			}
			else
			{
				GetInstance().GetGameState().first_vs_mode = 1;
				PlayerPrefs.SetInt("First_Vs_Mode", 1);
			}
			if (PlayerPrefs.HasKey("VS_Tutorial_Map"))
			{
				GetInstance().GetGameState().vs_toturial_triger_map = PlayerPrefs.GetInt("VS_Tutorial_Map");
			}
			else
			{
				GetInstance().GetGameState().vs_toturial_triger_map = 1;
				PlayerPrefs.SetInt("VS_Tutorial_Map", 1);
			}
			if (PlayerPrefs.HasKey("VS_Tutorial_Hall"))
			{
				GetInstance().GetGameState().vs_toturial_triger_hall = PlayerPrefs.GetInt("VS_Tutorial_Hall");
			}
			else
			{
				GetInstance().GetGameState().vs_toturial_triger_hall = 1;
				PlayerPrefs.SetInt("VS_Tutorial_Hall", 1);
			}
			if (PlayerPrefs.HasKey("VS_Tutorial_Password"))
			{
				GetInstance().GetGameState().vs_toturial_triger_password = PlayerPrefs.GetInt("VS_Tutorial_Password");
			}
			else
			{
				GetInstance().GetGameState().vs_toturial_triger_password = 1;
				PlayerPrefs.SetInt("VS_Tutorial_Password", 1);
			}
			if (PlayerPrefs.HasKey("VS_Tutorial_JoinRoom"))
			{
				GetInstance().GetGameState().vs_toturial_triger_joinRoom = PlayerPrefs.GetInt("VS_Tutorial_JoinRoom");
			}
			else
			{
				GetInstance().GetGameState().vs_toturial_triger_joinRoom = 1;
				PlayerPrefs.SetInt("VS_Tutorial_JoinRoom", 1);
			}
			if (PlayerPrefs.HasKey("VS_Tutorial_RoomOwner"))
			{
				GetInstance().GetGameState().vs_toturial_triger_roomOwner = PlayerPrefs.GetInt("VS_Tutorial_RoomOwner");
			}
			else
			{
				GetInstance().GetGameState().vs_toturial_triger_roomOwner = 1;
				PlayerPrefs.SetInt("VS_Tutorial_RoomOwner", 1);
			}
			if (PlayerPrefs.HasKey("VS_Tutorial_Dead"))
			{
				GetInstance().GetGameState().vs_toturial_triger_dead = PlayerPrefs.GetInt("VS_Tutorial_Dead");
			}
			else
			{
				GetInstance().GetGameState().vs_toturial_triger_dead = 1;
				PlayerPrefs.SetInt("VS_Tutorial_Dead", 1);
			}
			if (PlayerPrefs.HasKey("VS_Tutorial_VS"))
			{
				GetInstance().GetGameState().vs_toturial_triger_vs = PlayerPrefs.GetInt("VS_Tutorial_VS");
				return;
			}
			GetInstance().GetGameState().vs_toturial_triger_vs = 1;
			PlayerPrefs.SetInt("VS_Tutorial_VS", 1);
		}

		public void Save()
		{
			bool vS_mode = gameState.VS_mode;
			gameState.VS_mode = false;
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			gameState.SaveDataNew();
			PlayerPrefsSave();
			gameState.SaveUserStatistics();
			gameState.VS_mode = vS_mode;
		}

		public bool Load()
		{
			PlayerPrefsLoad();
			if (File.Exists(path + "CallMini.save"))
			{
				gameState.LoadDataNew();
				gameState.LoadUserStatistics();
				return true;
			}
			if (File.Exists(path + "MySavedGame.game"))
			{
				Stream stream = File.Open(path + "MySavedGame.game", FileMode.Open);
				BinaryReader binaryReader = new BinaryReader(stream);
				gameState.LoadData(binaryReader);
				binaryReader.Close();
				stream.Close();
				gameState.SaveDataNew();
				gameState.LoadUserStatistics();
				return true;
			}
			Save();
			gameState.LoadUserStatistics();
			return false;
		}

		public void Init()
		{
			LoadResource();
			LoadConfig();
			InitGameState();
		}

		public void InitForMenu()
		{
			LoadResourceMenu();
			LoadConfig();
			InitGameState();
		}

		public void InitForMultiplay()
		{
			LoadMultiAchievementConf();
		}

		public void LoadResource()
		{
			_GamerRsourceConfig = GameObject.Find("GameResourceConfig").GetComponent<ResourceConfigScript>();
			_EnemyResourceConfig = GameObject.Find("EnemyResourceConfig").GetComponent<EnemyConfigScript>();
			_GloabResourceConfig = GameObject.Find("GlobalResourceConfig").GetComponent<GloabConfigScript>();
		}

		public void LoadResourceMenu()
		{
			_GloabResourceConfig = GameObject.Find("GlobalResourceConfig").GetComponent<GloabConfigScript>();
			_MenuResourceConfig = GameObject.Find("MenuResourceConfig").GetComponent<MenuConfigScript>();
		}

		public void LoadConfig()
		{
			if (gameConfig.monsterConfTable.Count == 0)
			{
				if (Application.isEditor || Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.Android)
				{
					gameConfig.LoadFromXML(null);
				}
				else
				{
					gameConfig.LoadFromXML("/");
				}
			}
		}

		public void LoadMultiAchievementConf()
		{
			if (gameConfig.Multi_AchievementConfTable.Count == 0)
			{
				if (Application.isEditor || Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.Android)
				{
					gameConfig.LoadMultiAchievementFromXML(null);
				}
				else
				{
					gameConfig.LoadMultiAchievementFromXML("/");
				}
				Debug.Log("MultiAchievement Count:" + gameConfig.Multi_AchievementConfTable.Count);
			}
		}

		public void InitGameState()
		{
			gameState.Init();
		}

		public void CreateScene()
		{
			script = GameObject.Find("GameApp").GetComponent<GameScript>();
			if (GetInstance().GetGameState().endless_multiplayer)
			{
				scene = new GameMultiplayerScene();
			}
			else if (GetInstance().GetGameState().VS_mode)
			{
				scene = new GameVSScene();
			}
			else
			{
				scene = new GameScene();
			}
			scene.Init(Application.loadedLevel - 1);
		}

		public void AddMultiplayerComponents()
		{
			scene.AddNetworkComponents();
		}

		public void ClearScene()
		{
			scene = null;
		}

		public void Loop(float deltaTime)
		{
			if (scene != null)
			{
				scene.DoLogic(deltaTime);
			}
		}

		public ResourceConfigScript GetGameResourceConfig()
		{
			return _GamerRsourceConfig;
		}

		public EnemyConfigScript GetEnemyResourceConfig()
		{
			return _EnemyResourceConfig;
		}

		public MenuConfigScript GetMenuResourceConfig()
		{
			return _MenuResourceConfig;
		}

		public GloabConfigScript GetGloabResourceConfig()
		{
			return _GloabResourceConfig;
		}

		public ResourceConfigScript GetResourceConfig()
		{
			return _GamerRsourceConfig;
		}

		public GameScene GetGameScene()
		{
			return scene;
		}

		public GameState GetGameState()
		{
			return gameState;
		}

		public GameConfig GetGameConfig()
		{
			return gameConfig;
		}
	}
}
