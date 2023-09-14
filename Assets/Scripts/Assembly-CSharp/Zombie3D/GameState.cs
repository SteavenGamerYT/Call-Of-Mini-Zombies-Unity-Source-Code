using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using LitJson;
using UnityEngine;

namespace Zombie3D
{
	public class GameState
	{
		public string last_scene = string.Empty;

		public UserStatistics user_statistics = new UserStatistics();

		protected int cash;

		protected int score;

		protected int[] infectionRate;

		protected List<Weapon> weaponList;

		protected AvatarState[] avatarData;

		protected GameConfig gConfig;

		protected bool inited;

		protected bool weaponsInited;

		protected int[] rectToWeaponMap = new int[3];

		public int multi_enemy_death_elite_count;

		public int multi_enemy_death_normal_count;

		public int[] MultiAchievementData;

		public bool hunting_level;

		public bool VS_mode;

		public int endless_rank_max;

		public int review_count;

		public int update_welcome_view;

		public int survival_welcome_view;

		public int is_auto_aim;

		public float Game_ver;

		public int Hunting_val;

		public int loot_cash;

		public int bullet_comsume;

		public int multi_toturial;

		public int multi_toturial_triger;

		public int multi_toturial_triger_achi = 1;

		public int multi_toturial_triger_map = 1;

		public int multi_toturial_triger_hall = 1;

		public int multi_toturial_triger_room = 1;

		public int multi_toturial_triger_game_dead = 1;

		public int multi_toturial_triger_game_rescue = 1;

		public int multi_toturial_triger_room_master = 1;

		public int vs_toturial_triger_map = 1;

		public int vs_toturial_triger_hall = 1;

		public int vs_toturial_triger_password = 1;

		public int vs_toturial_triger_joinRoom = 1;

		public int vs_toturial_triger_roomOwner = 1;

		public int vs_toturial_triger_dead = 1;

		public int vs_toturial_triger_vs = 1;

		public int First_Statistic = 1;

		public int m_rebirth_packet_count = 1;

		public int m_rescue_packet_count = 1;

		public float TotalTime;

		public int TotalMoneyGet;

		public string First_Statistic_Time = "null";

		public string last_map_to_shop = string.Empty;

		public string cur_net_map = string.Empty;

		protected int cur_endless_rank;

		public bool multiname_to_coop = true;

		public int endless_cash_max;

		public int endless_kill_max;

		public int endless_time_max;

		public int cur_endless_time;

		public int multiplay_named;

		public string nick_name;

		public NetworkObj net_com;

		public NetworkObj net_com_hall;

		protected bool fromShopMenu;

		public PaperUIEnterStatus PaperMenuStatus;

		protected bool is_app_launch = true;

		public float macos_sen = 1f;

		public int first_vs_mode = 1;

		protected string path = Application.persistentDataPath;

		public int LevelNum { get; set; }

		public AchievementState Achievement { get; set; }

		public float MenuMusicTime { get; set; }

		public bool FirstTimeGame { get; set; }

		public AvatarType Avatar { get; set; }

		public bool MusicOn { get; set; }

		public bool SoundOn { get; set; }

		public bool PaperModelShow { get; set; }

		public int ArmorLevel { get; set; }

		public bool AlreadyCountered { get; set; }

		public bool AlreadyPopReview { get; set; }

		public bool endless_level { get; set; }

		public bool endless_multiplayer { get; set; }

		public int Cur_endless_rank
		{
			get
			{
				return cur_endless_rank;
			}
			set
			{
				cur_endless_rank = value;
			}
		}

		public bool FromShopMenu
		{
			get
			{
				return fromShopMenu;
			}
			set
			{
				fromShopMenu = value;
			}
		}

		public GameState()
		{
			inited = false;
			weaponList = new List<Weapon>();
			Achievement = new AchievementState();
			AlreadyCountered = false;
			AlreadyPopReview = false;
		}

		public bool GetInited()
		{
			return inited;
		}

		public void AddScore(int scoreAdd)
		{
			score += scoreAdd;
		}

		public int GetScore()
		{
			return score;
		}

		public AvatarState GetAvatarData(AvatarType aType)
		{
			return avatarData[(int)aType];
		}

		public int GetAvatarNum()
		{
			return avatarData.Length;
		}

		public int[] GetRectToWeaponMap()
		{
			return rectToWeaponMap;
		}

		public bool GotAllWeapons()
		{
			for (int i = 0; i < weaponList.Count; i++)
			{
				if (weaponList[i].Exist != WeaponExistState.Owned)
				{
					return false;
				}
			}
			return true;
		}

		public void EnableAvatar(AvatarType aType)
		{
			avatarData[(int)aType] = AvatarState.Avaliable;
			Achievement.GotNewAvatar();
		}

		public void LoadData(BinaryReader br)
		{
			Debug.Log("Load");
			if (!inited)
			{
				Init();
			}
			InitWeapons();
			cash = br.ReadInt32();
			score = br.ReadInt32();
			LevelNum = br.ReadInt32();
			int num = br.ReadInt32();
			Debug.Log("weapon counts = " + num + " cfg counts = " + weaponList.Count);
			for (int i = 0; i < num; i++)
			{
				int wType = br.ReadInt32();
				string text = br.ReadString();
				switch (text)
				{
				case "Winchester 1200":
					text = "Winchester-1200";
					break;
				case "Remington 870":
					text = "Remington-870";
					break;
				case "XM 1014":
					text = "XM-1014";
					break;
				}
				Weapon weapon = null;
				for (int j = 0; j < weaponList.Count; j++)
				{
					if (weaponList[j].Name == text)
					{
						weapon = weaponList[j];
						break;
					}
				}
				if (weapon == null)
				{
					weapon = WeaponFactory.GetInstance().CreateWeapon((WeaponType)wType, false);
					weapon.Name = text;
					weaponList.Add(weapon);
				}
				weapon.WConf = gConfig.GetWeaponConfig(weapon.Name);
				weapon.LoadConfig();
				weapon.BulletCount = br.ReadInt32();
				weapon.Damage = (float)br.ReadDouble();
				weapon.AttackFrequency = (float)br.ReadDouble();
				weapon.Accuracy = (float)br.ReadDouble();
				weapon.DamageLevel = br.ReadInt32();
				weapon.FrequencyLevel = br.ReadInt32();
				weapon.AccuracyLevel = br.ReadInt32();
				weapon.IsSelectedForBattle = br.ReadBoolean();
				weapon.Exist = (WeaponExistState)br.ReadInt32();
			}
			if (LevelNum >= 18)
			{
				for (int k = 0; k < weaponList.Count; k++)
				{
					if (weaponList[k].Name == "LightSword" && weaponList[k].Exist == WeaponExistState.Locked)
					{
						weaponList[k].Exist = WeaponExistState.Unlocked;
						Debug.Log("this is unlock sword");
						break;
					}
				}
			}
			if (LevelNum >= 22)
			{
				for (int l = 0; l < weaponList.Count; l++)
				{
					if (weaponList[l].Name == "M32" && weaponList[l].Exist == WeaponExistState.Locked)
					{
						weaponList[l].Exist = WeaponExistState.Unlocked;
						Debug.Log("this is unlock m32");
						break;
					}
				}
			}
			for (int m = 0; m < rectToWeaponMap.Length; m++)
			{
				rectToWeaponMap[m] = br.ReadInt32();
			}
			if (num == 13)
			{
				for (int n = 0; n < 8; n++)
				{
					avatarData[n] = (AvatarState)br.ReadInt32();
				}
				Debug.Log("old version ");
			}
			else
			{
				int num2 = br.ReadInt32();
				Debug.Log("new version " + num2);
				for (int num3 = 0; num3 < num2; num3++)
				{
					avatarData[num3] = (AvatarState)br.ReadInt32();
				}
			}
			Avatar = (AvatarType)br.ReadInt32();
			ArmorLevel = br.ReadInt32();
			FirstTimeGame = br.ReadBoolean();
			Achievement.Load(br);
		}

		public int GetWeaponIndex(Weapon w)
		{
			for (int i = 0; i < weaponList.Count; i++)
			{
				if (weaponList[i] == w)
				{
					return i;
				}
			}
			return 0;
		}

		public string GetWeaponNameByIndex(int index)
		{
			string result = "null";
			if (index < weaponList.Count)
			{
				result = weaponList[index].Name;
			}
			return result;
		}

		public static string GameSaveStringEncipher(string str, int key)
		{
			char[] array = str.ToCharArray();
			char[] array2 = str.ToCharArray();
			char[] array3 = new char[2];
			for (int i = 0; i < array.Length; i++)
			{
				char c = array[i];
				array3[0] = c;
				string s = new string(array3);
				int num = char.ConvertToUtf32(s, 0);
				num ^= key;
				array2[i] = char.ConvertFromUtf32(num)[0];
			}
			return new string(array2);
		}

		public void LoadUserStatistics()
		{
			string text = Utils.GetIOSYear().ToString("D4") + Utils.GetIOSMonth().ToString("D2") + Utils.GetIOSDay().ToString("D2");
			string text2 = "statistics" + text + ".sta";
			if (File.Exists(path + text2))
			{
				Configure configure = new Configure();
				Stream stream = File.Open(path + text2, FileMode.Open);
				BinaryReader binaryReader = new BinaryReader(stream);
				string str = binaryReader.ReadString();
				binaryReader.Close();
				stream.Close();
				str = GameSaveStringEncipher(str, 28);
				configure.Load(str);
				if (configure.GetSingle("Statistics", "CashLoot") != null)
				{
					user_statistics.cash_loot = int.Parse(configure.GetSingle("Statistics", "CashLoot"));
				}
				user_statistics.coop_count = int.Parse(configure.GetSingle("Statistics", "CoopCount"));
				user_statistics.play_time = float.Parse(configure.GetSingle("Statistics", "PlayTime"));
				user_statistics.achivment_count = int.Parse(configure.GetSingle("Statistics", "AchivmentCount"));
				user_statistics.cash_iap = int.Parse(configure.GetSingle("Statistics", "CashIap"));
				user_statistics.day_count = int.Parse(configure.GetSingle("Statistics", "DayCount"));
				user_statistics.application_count = int.Parse(configure.GetSingle("Statistics", "AppCount"));
				user_statistics.cash_spend = int.Parse(configure.GetSingle("Statistics", "CashSpend"));
				user_statistics.cash_spend_weapons = int.Parse(configure.GetSingle("Statistics", "CashSpendWeapons"));
				user_statistics.cash_spend_bullets = int.Parse(configure.GetSingle("Statistics", "CashSpendBullets"));
				user_statistics.last_day_state = configure.GetSingle("Statistics", "LastDayState");
				int num = 0;
				num = int.Parse(configure.GetSingle("Statistics", "ConsumeCount"));
				string empty = string.Empty;
				user_statistics.consume_list.Clear();
				for (int i = 0; i < num; i++)
				{
					empty = configure.GetArray("Statistics", "ConsumeContent", i);
					user_statistics.consume_list.Add(empty);
				}
				int num2 = 0;
				num2 = int.Parse(configure.GetSingle("Statistics", "WeaponUseDataCount"));
				for (int j = 0; j < num2; j++)
				{
					int value = int.Parse(configure.GetArray("Statistics", "WeaponUseData", j));
					user_statistics.weapon_use_count[j] = value;
				}
				int num3 = 0;
				num3 = int.Parse(configure.GetSingle("Statistics", "AvatarUseDataCount"));
				for (int k = 0; k < num3; k++)
				{
					int value2 = int.Parse(configure.GetArray("Statistics", "AvatarUseData", k));
					user_statistics.avatar_use_count[k] = value2;
				}
				for (int l = 0; l < user_statistics.Iap_buy_count.Length; l++)
				{
					int num4 = int.Parse(configure.GetArray("Statistics", "IapBuyCount", l));
					user_statistics.Iap_buy_count[l] = num4;
				}
				int num5 = 0;
				num5 = int.Parse(configure.GetSingle("Statistics", "WeaponUpListCount"));
				string empty2 = string.Empty;
				user_statistics.weapon_up_list.Clear();
				for (int m = 0; m < num5; m++)
				{
					empty2 = configure.GetArray("Statistics", "WeaponUpContent", m);
					user_statistics.weapon_up_list.Add(empty2);
				}
				int num6 = 0;
				num6 = int.Parse(configure.GetSingle("Statistics", "BulletsBuyCount"));
				string empty3 = string.Empty;
				user_statistics.bullets_list.Clear();
				for (int n = 0; n < num6; n++)
				{
					empty3 = configure.GetArray("Statistics", "BulletsBuyContent", n);
					user_statistics.bullets_list.Add(empty3);
				}
				int num7 = 0;
				num7 = int.Parse(configure.GetSingle("Statistics", "MissionFailedCount"));
				string empty4 = string.Empty;
				user_statistics.mission_failed_list.Clear();
				for (int num8 = 0; num8 < num7; num8++)
				{
					empty4 = configure.GetArray("Statistics", "MissionFailedContent", num8);
					user_statistics.mission_failed_list.Add(empty4);
				}
				int num9 = 0;
				num9 = int.Parse(configure.GetSingle("Statistics", "MissionQuitCount"));
				string empty5 = string.Empty;
				user_statistics.mission_quit_list.Clear();
				for (int num10 = 0; num10 < num9; num10++)
				{
					empty5 = configure.GetArray("Statistics", "MissionQuitContent", num10);
					user_statistics.mission_quit_list.Add(empty5);
				}
				int num11 = 0;
				num11 = int.Parse(configure.GetSingle("Statistics", "SceneEnterCount"));
				string empty6 = string.Empty;
				user_statistics.scene_enter_data.Clear();
				for (int num12 = 0; num12 < num11; num12++)
				{
					empty6 = configure.GetArray("Statistics", "SceneEnterContent", num12);
					user_statistics.scene_enter_data.Add(empty6);
				}
			}
		}

		public string SaveSceneStatistics()
		{
			Configure configure = new Configure();
			configure.AddSection("Statistics", string.Empty, string.Empty);
			configure.AddValueSingle("Statistics", "CashLoot", user_statistics.cash_loot.ToString(), string.Empty, string.Empty);
			configure.AddValueSingle("Statistics", "CoopCount", user_statistics.coop_count.ToString(), string.Empty, string.Empty);
			configure.AddValueSingle("Statistics", "PlayTime", user_statistics.play_time.ToString(), string.Empty, string.Empty);
			configure.AddValueSingle("Statistics", "AchivmentCount", user_statistics.achivment_count.ToString(), string.Empty, string.Empty);
			configure.AddValueSingle("Statistics", "CashIap", user_statistics.cash_iap.ToString(), string.Empty, string.Empty);
			configure.AddValueSingle("Statistics", "DayCount", user_statistics.day_count.ToString(), string.Empty, string.Empty);
			configure.AddValueSingle("Statistics", "CashSpend", user_statistics.cash_spend.ToString(), string.Empty, string.Empty);
			configure.AddValueSingle("Statistics", "LastScene", user_statistics.last_scene, string.Empty, string.Empty);
			configure.AddValueSingle("Statistics", "CashSpendWeapons", user_statistics.cash_spend_weapons.ToString(), string.Empty, string.Empty);
			configure.AddValueSingle("Statistics", "CashSpendBullets", user_statistics.cash_spend_bullets.ToString(), string.Empty, string.Empty);
			configure.AddValueSingle("Statistics", "LastDayState", user_statistics.last_day_state, string.Empty, string.Empty);
			if (is_app_launch)
			{
				user_statistics.application_count++;
				is_app_launch = false;
			}
			user_statistics.application_count += Utils.GetAppAcctiveTimes();
			configure.AddValueSingle("Statistics", "AppCount", user_statistics.application_count.ToString(), string.Empty, string.Empty);
			configure.AddValueSingle("Statistics", "ConsumeCount", user_statistics.consume_list.Count.ToString(), string.Empty, string.Empty);
			StringLine stringLine = new StringLine();
			foreach (string item in user_statistics.consume_list)
			{
				stringLine.AddString(item);
			}
			configure.AddValueArray("Statistics", "ConsumeContent", stringLine.content, string.Empty, string.Empty);
			configure.AddValueSingle("Statistics", "WeaponUseDataCount", user_statistics.weapon_use_count.Count.ToString(), string.Empty, string.Empty);
			StringLine stringLine2 = new StringLine();
			for (int i = 0; i < user_statistics.weapon_use_count.Count; i++)
			{
				stringLine2.AddString(user_statistics.weapon_use_count[i].ToString());
			}
			configure.AddValueArray("Statistics", "WeaponUseData", stringLine2.content, string.Empty, string.Empty);
			configure.AddValueSingle("Statistics", "AvatarUseDataCount", avatarData.Length.ToString(), string.Empty, string.Empty);
			StringLine stringLine3 = new StringLine();
			for (int j = 0; j < avatarData.Length; j++)
			{
				stringLine3.AddString(user_statistics.avatar_use_count[j].ToString());
			}
			configure.AddValueArray("Statistics", "AvatarUseData", stringLine3.content, string.Empty, string.Empty);
			StringLine stringLine4 = new StringLine();
			for (int k = 0; k < user_statistics.Iap_buy_count.Length; k++)
			{
				stringLine4.AddString(user_statistics.Iap_buy_count[k].ToString());
			}
			configure.AddValueArray("Statistics", "IapBuyCount", stringLine4.content, string.Empty, string.Empty);
			user_statistics.weapon_owner_list.Clear();
			foreach (Weapon weapon in weaponList)
			{
				if (weapon.Exist == WeaponExistState.Owned)
				{
					user_statistics.weapon_owner_list.Add(weapon.Name);
				}
			}
			StringLine stringLine5 = new StringLine();
			foreach (string item2 in user_statistics.weapon_owner_list)
			{
				stringLine5.AddString(item2);
			}
			configure.AddValueArray("Statistics", "WeaponOwnerList", stringLine5.content, string.Empty, string.Empty);
			configure.AddValueSingle("Statistics", "WeaponOwnerCount", user_statistics.weapon_owner_list.Count.ToString(), string.Empty, string.Empty);
			StringLine stringLine6 = new StringLine();
			foreach (string item3 in user_statistics.weapon_up_list)
			{
				stringLine6.AddString(item3);
			}
			configure.AddValueArray("Statistics", "WeaponUpContent", stringLine6.content, string.Empty, string.Empty);
			configure.AddValueSingle("Statistics", "WeaponUpListCount", user_statistics.weapon_up_list.Count.ToString(), string.Empty, string.Empty);
			StringLine stringLine7 = new StringLine();
			foreach (string item4 in user_statistics.bullets_list)
			{
				stringLine7.AddString(item4);
			}
			configure.AddValueArray("Statistics", "BulletsBuyContent", stringLine7.content, string.Empty, string.Empty);
			configure.AddValueSingle("Statistics", "BulletsBuyCount", user_statistics.bullets_list.Count.ToString(), string.Empty, string.Empty);
			StringLine stringLine8 = new StringLine();
			foreach (string item5 in user_statistics.mission_failed_list)
			{
				stringLine8.AddString(item5);
			}
			configure.AddValueArray("Statistics", "MissionFailedContent", stringLine8.content, string.Empty, string.Empty);
			configure.AddValueSingle("Statistics", "MissionFailedCount", user_statistics.mission_failed_list.Count.ToString(), string.Empty, string.Empty);
			StringLine stringLine9 = new StringLine();
			foreach (string item6 in user_statistics.mission_quit_list)
			{
				stringLine9.AddString(item6);
			}
			configure.AddValueArray("Statistics", "MissionQuitContent", stringLine9.content, string.Empty, string.Empty);
			configure.AddValueSingle("Statistics", "MissionQuitCount", user_statistics.mission_quit_list.Count.ToString(), string.Empty, string.Empty);
			StringLine stringLine10 = new StringLine();
			foreach (string scene_enter_datum in user_statistics.scene_enter_data)
			{
				stringLine10.AddString(scene_enter_datum);
			}
			configure.AddValueArray("Statistics", "SceneEnterContent", stringLine10.content, string.Empty, string.Empty);
			configure.AddValueSingle("Statistics", "SceneEnterCount", user_statistics.scene_enter_data.Count.ToString(), string.Empty, string.Empty);
			string str = configure.Save();
			str = GameSaveStringEncipher(str, 28);
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			string text = Utils.GetIOSYear().ToString("D4") + Utils.GetIOSMonth().ToString("D2") + Utils.GetIOSDay().ToString("D2");
			string text2 = "statistics" + text + ".sta";
			Stream stream = File.Open(path + text2, FileMode.Create);
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			binaryWriter.Write(str);
			binaryWriter.Close();
			stream.Close();
			return text2;
		}

		public void SaveUserStatistics()
		{
			string text = SaveSceneStatistics();
			if (!CheckStatisticsTable(text))
			{
				Debug.Log("AddStatisticsItem..." + text);
				AddStatisticsItem(text);
			}
			CheckStatisticsUpload();
		}

		public bool CheckStatisticsTable(string file_name)
		{
			string text = "statisticsTable.sta";
			if (File.Exists(path + text))
			{
				Configure configure = new Configure();
				Stream stream = File.Open(path + text, FileMode.Open);
				BinaryReader binaryReader = new BinaryReader(stream);
				string str = binaryReader.ReadString();
				binaryReader.Close();
				stream.Close();
				str = GameSaveStringEncipher(str, 28);
				configure.Load(str);
				int num = int.Parse(configure.GetSingle("Table", "Count"));
				for (int i = 0; i < num; i++)
				{
					string array = configure.GetArray("Table", "StatisticsList", i);
					if (array == file_name)
					{
						return true;
					}
				}
			}
			return false;
		}

		public void AddStatisticsItem(string file_name)
		{
			string text = "statisticsTable.sta";
			Configure configure = new Configure();
			List<string> list = new List<string>();
			if (File.Exists(path + text))
			{
				Stream stream = File.Open(path + text, FileMode.Open);
				BinaryReader binaryReader = new BinaryReader(stream);
				string str = binaryReader.ReadString();
				binaryReader.Close();
				stream.Close();
				str = GameSaveStringEncipher(str, 28);
				configure.Load(str);
				int num = int.Parse(configure.GetSingle("Table", "Count"));
				for (int i = 0; i < num; i++)
				{
					string array = configure.GetArray("Table", "StatisticsList", i);
					list.Add(array);
				}
				list.Add(file_name);
			}
			else
			{
				list.Add(file_name);
			}
			SaveStatisticsList(list);
		}

		public void SaveStatisticsList(List<string> data)
		{
			Configure configure = new Configure();
			configure.AddSection("Table", string.Empty, string.Empty);
			configure.AddValueSingle("Table", "Count", data.Count.ToString(), string.Empty, string.Empty);
			StringLine stringLine = new StringLine();
			foreach (string datum in data)
			{
				stringLine.AddString(datum);
			}
			configure.AddValueArray("Table", "StatisticsList", stringLine.content, string.Empty, string.Empty);
			string str = configure.Save();
			str = GameSaveStringEncipher(str, 28);
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			string text = "statisticsTable.sta";
			Stream stream = File.Open(path + text, FileMode.Create);
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			binaryWriter.Write(str);
			binaryWriter.Close();
			stream.Close();
		}

		public void CheckStatisticsUpload()
		{
			string text = "statisticsTable.sta";
			if (!File.Exists(path + text))
			{
				return;
			}
			string text2 = Utils.GetIOSYear().ToString("D4") + Utils.GetIOSMonth().ToString("D2") + Utils.GetIOSDay().ToString("D2");
			string text3 = "statistics" + text2 + ".sta";
			Configure configure = new Configure();
			Stream stream = File.Open(path + text, FileMode.Open);
			BinaryReader binaryReader = new BinaryReader(stream);
			string str = binaryReader.ReadString();
			binaryReader.Close();
			stream.Close();
			str = GameSaveStringEncipher(str, 28);
			configure.Load(str);
			int num = int.Parse(configure.GetSingle("Table", "Count"));
			for (int i = 0; i < num; i++)
			{
				string array = configure.GetArray("Table", "StatisticsList", i);
				if (array != text3)
				{
					UploadStatistics(array);
					break;
				}
			}
		}

		public void RemoveStatisticsItem(string file_name)
		{
			string text = "statisticsTable.sta";
			List<string> list = new List<string>();
			Configure configure = new Configure();
			if (File.Exists(path + text))
			{
				Stream stream = File.Open(path + text, FileMode.Open);
				BinaryReader binaryReader = new BinaryReader(stream);
				string str = binaryReader.ReadString();
				binaryReader.Close();
				stream.Close();
				str = GameSaveStringEncipher(str, 28);
				configure.Load(str);
				int num = int.Parse(configure.GetSingle("Table", "Count"));
				for (int i = 0; i < num; i++)
				{
					string array = configure.GetArray("Table", "StatisticsList", i);
					if (file_name != array)
					{
						list.Add(array);
					}
				}
			}
			if (File.Exists(path + file_name))
			{
				File.Delete(path + file_name);
				Debug.Log("File.Delete:" + path + file_name);
			}
			SaveStatisticsList(list);
		}

		public void UploadStatistics(string file_name)
		{
			if (!File.Exists(path + file_name))
			{
				return;
			}
			Configure configure = new Configure();
			Stream stream = File.Open(path + file_name, FileMode.Open);
			BinaryReader binaryReader = new BinaryReader(stream);
			string str = binaryReader.ReadString();
			binaryReader.Close();
			stream.Close();
			str = GameSaveStringEncipher(str, 28);
			configure.Load(str);
			Hashtable hashtable = new Hashtable();
			hashtable["FirstStatisticsTime"] = First_Statistic_Time;
			Debug.Log("FirstStatisticsTime:" + First_Statistic_Time);
			hashtable["UserId"] = Utils.GetMacAddr();
			hashtable["IsJailbreak"] = Utils.IsJailbreak();
			hashtable["CurDay"] = LevelNum;
			hashtable["CashCurrent"] = cash;
			hashtable["TotalTime"] = (int)TotalTime;
			hashtable["TotalMoneyGet"] = TotalMoneyGet;
			int num = int.Parse(configure.GetSingle("Statistics", "CashLoot"));
			hashtable["CashLoot"] = num;
			int num2 = int.Parse(configure.GetSingle("Statistics", "CoopCount"));
			hashtable["CoopCount"] = num2;
			int num3 = (int)float.Parse(configure.GetSingle("Statistics", "PlayTime"));
			hashtable["PlayTime"] = num3;
			int num4 = int.Parse(configure.GetSingle("Statistics", "AchivmentCount"));
			hashtable["AchivmentCount"] = num4;
			int num5 = int.Parse(configure.GetSingle("Statistics", "CashIap"));
			hashtable["CashIap"] = num5;
			int num6 = int.Parse(configure.GetSingle("Statistics", "DayCount"));
			hashtable["DayCount"] = num6;
			int num7 = int.Parse(configure.GetSingle("Statistics", "CashSpend"));
			hashtable["CashSpend"] = num7;
			int num8 = int.Parse(configure.GetSingle("Statistics", "CashSpendWeapons"));
			hashtable["CashSpendWeapons"] = num8;
			int num9 = int.Parse(configure.GetSingle("Statistics", "CashSpendBullets"));
			hashtable["CashSpendBullets"] = num9;
			int num10 = int.Parse(configure.GetSingle("Statistics", "AppCount"));
			hashtable["AppCount"] = num10;
			string single = configure.GetSingle("Statistics", "LastDayState");
			hashtable["LastDayState"] = single;
			int num11 = 0;
			num11 = int.Parse(configure.GetSingle("Statistics", "ConsumeCount"));
			string empty = string.Empty;
			for (int i = 0; i < num11; i++)
			{
				empty = configure.GetArray("Statistics", "ConsumeContent", i);
				hashtable["ConsumeContent" + i] = empty;
			}
			int num12 = 0;
			num12 = int.Parse(configure.GetSingle("Statistics", "WeaponUseDataCount"));
			int num13 = 0;
			for (int j = 0; j < num12; j++)
			{
				num13 = int.Parse(configure.GetArray("Statistics", "WeaponUseData", j));
				hashtable["WeaponUseData_" + GetWeaponNameByIndex(j)] = num13;
			}
			int num14 = 0;
			num14 = int.Parse(configure.GetSingle("Statistics", "AvatarUseDataCount"));
			int num15 = 0;
			for (int k = 0; k < num14; k++)
			{
				num15 = int.Parse(configure.GetArray("Statistics", "AvatarUseData", k));
				hashtable["AvatarUseData_" + (AvatarType)k] = num15;
			}
			hashtable["Cur_Avatar"] = Avatar.ToString();
			for (int l = 0; l < rectToWeaponMap.Length; l++)
			{
				if (rectToWeaponMap[l] > -1)
				{
					hashtable["Cur_Weapon_" + l] = GetWeaponNameByIndex(rectToWeaponMap[l]);
				}
			}
			for (int m = 0; m < user_statistics.Iap_buy_count.Length; m++)
			{
				hashtable["Iap_Buy_Count_" + m] = user_statistics.Iap_buy_count[m];
			}
			int num16 = int.Parse(configure.GetSingle("Statistics", "WeaponOwnerCount"));
			for (int n = 0; n < num16; n++)
			{
				string array = configure.GetArray("Statistics", "WeaponOwnerList", n);
				hashtable["WeaponOwner_" + n] = array;
			}
			int num17 = int.Parse(configure.GetSingle("Statistics", "WeaponUpListCount"));
			for (int num18 = 0; num18 < num17; num18++)
			{
				string array2 = configure.GetArray("Statistics", "WeaponUpContent", num18);
				if (hashtable.ContainsKey("WeaponUp_" + array2))
				{
					hashtable["WeaponUp_" + array2] = (int)hashtable["WeaponUp_" + array2] + 1;
				}
				else
				{
					hashtable["WeaponUp_" + array2] = 1;
				}
			}
			int num19 = int.Parse(configure.GetSingle("Statistics", "BulletsBuyCount"));
			for (int num20 = 0; num20 < num19; num20++)
			{
				string array3 = configure.GetArray("Statistics", "BulletsBuyContent", num20);
				if (hashtable.ContainsKey("BulletsBuy_" + array3))
				{
					hashtable["BulletsBuy_" + array3] = (int)hashtable["BulletsBuy_" + array3] + 1;
				}
				else
				{
					hashtable["BulletsBuy_" + array3] = 1;
				}
			}
			int num21 = int.Parse(configure.GetSingle("Statistics", "MissionFailedCount"));
			for (int num22 = 0; num22 < num21; num22++)
			{
				string array4 = configure.GetArray("Statistics", "MissionFailedContent", num22);
				if (hashtable.ContainsKey("MissionFailed_" + array4))
				{
					hashtable["MissionFailed_" + array4] = (int)hashtable["MissionFailed_" + array4] + 1;
				}
				else
				{
					hashtable["MissionFailed_" + array4] = 1;
				}
			}
			int num23 = int.Parse(configure.GetSingle("Statistics", "MissionQuitCount"));
			for (int num24 = 0; num24 < num23; num24++)
			{
				string array5 = configure.GetArray("Statistics", "MissionQuitContent", num24);
				if (hashtable.ContainsKey("MissionQuit_" + array5))
				{
					hashtable["MissionQuit_" + array5] = (int)hashtable["MissionQuit_" + array5] + 1;
				}
				else
				{
					hashtable["MissionQuit_" + array5] = 1;
				}
			}
			int num25 = int.Parse(configure.GetSingle("Statistics", "SceneEnterCount"));
			for (int num26 = 0; num26 < num25; num26++)
			{
				string array6 = configure.GetArray("Statistics", "SceneEnterContent", num26);
				if (hashtable.ContainsKey("SceneEnter_" + array6))
				{
					hashtable["SceneEnter_" + array6] = (int)hashtable["SceneEnter_" + array6] + 1;
				}
				else
				{
					hashtable["SceneEnter_" + array6] = 1;
				}
			}
			hashtable["DataTime"] = file_name.Substring(10, 8);
			string value = JsonMapper.ToJson(hashtable);
			string text = file_name.Substring(10, 8);
			Debug.Log("upload file date:" + text);
			Hashtable hashtable2 = new Hashtable();
			hashtable2["gamename"] = "callofminizombies";
			hashtable2["data"] = value;
			string text2 = JsonMapper.ToJson(hashtable2);
		}

		public void OnResponse(int task_id, string param, int code, string response)
		{
			Debug.Log("OnResponse:" + response);
			JsonData jsonData = JsonMapper.ToObject(response);
			string text = jsonData["code"].ToString();
			Debug.Log("OnResponse code:" + text);
			if (text == "0")
			{
				RemoveStatisticsItem(param);
			}
			else
			{
				Debug.Log("OnResponse code error:" + text);
			}
		}

		public void OnRequestTimeout(int task_id, string param)
		{
			Debug.Log("OnRequestTimeout file:" + param);
		}

		public void LoadDataNew()
		{
			if (!inited)
			{
				Init();
			}
			InitWeapons();
			Configure configure = new Configure();
			if (File.Exists(path + "CallMini.save"))
			{
				Stream stream = File.Open(path + "CallMini.save", FileMode.Open);
				BinaryReader binaryReader = new BinaryReader(stream);
				string str = binaryReader.ReadString();
				binaryReader.Close();
				stream.Close();
				str = GameSaveStringEncipher(str, 71);
				configure.Load(str);
			}
			cash = int.Parse(configure.GetSingle("Save", "Cash"));
			score = int.Parse(configure.GetSingle("Save", "Score"));
			LevelNum = int.Parse(configure.GetSingle("Save", "LevelNum"));
			int num = int.Parse(configure.GetSingle("Save", "WeaponListCount"));
			for (int i = 0; i < num; i++)
			{
				int wType = int.Parse(configure.GetArray2("Save", "WeaponsCfg", i, 0));
				string array = configure.GetArray2("Save", "WeaponsCfg", i, 1);
				Weapon weapon = null;
				for (int j = 0; j < weaponList.Count; j++)
				{
					if (weaponList[j].Name == array)
					{
						weapon = weaponList[j];
						break;
					}
				}
				if (weapon == null)
				{
					weapon = WeaponFactory.GetInstance().CreateWeapon((WeaponType)wType, false);
					weapon.Name = array;
					weaponList.Add(weapon);
				}
				weapon.WConf = gConfig.GetWeaponConfig(weapon.Name);
				weapon.LoadConfig();
				weapon.BulletCount = int.Parse(configure.GetArray2("Save", "WeaponsCfg", i, 2));
				weapon.Damage = (float)double.Parse(configure.GetArray2("Save", "WeaponsCfg", i, 3));
				weapon.AttackFrequency = (float)double.Parse(configure.GetArray2("Save", "WeaponsCfg", i, 4));
				weapon.Accuracy = (float)double.Parse(configure.GetArray2("Save", "WeaponsCfg", i, 5));
				weapon.DamageLevel = int.Parse(configure.GetArray2("Save", "WeaponsCfg", i, 6));
				weapon.FrequencyLevel = int.Parse(configure.GetArray2("Save", "WeaponsCfg", i, 7));
				weapon.AccuracyLevel = int.Parse(configure.GetArray2("Save", "WeaponsCfg", i, 8));
				weapon.IsSelectedForBattle = bool.Parse(configure.GetArray2("Save", "WeaponsCfg", i, 9));
				weapon.Exist = (WeaponExistState)int.Parse(configure.GetArray2("Save", "WeaponsCfg", i, 10));
			}
			if (LevelNum >= 18)
			{
				for (int k = 0; k < weaponList.Count; k++)
				{
					if (weaponList[k].Name == "LightSword" && weaponList[k].Exist == WeaponExistState.Locked)
					{
						weaponList[k].Exist = WeaponExistState.Unlocked;
						break;
					}
				}
			}
			if (LevelNum >= 22)
			{
				for (int l = 0; l < weaponList.Count; l++)
				{
					if (weaponList[l].Name == "M32" && weaponList[l].Exist == WeaponExistState.Locked)
					{
						weaponList[l].Exist = WeaponExistState.Unlocked;
						break;
					}
				}
			}
			bool flag = true;
			for (int m = 0; m < rectToWeaponMap.Length; m++)
			{
				rectToWeaponMap[m] = int.Parse(configure.GetArray("Save", "RectToWeaponMap", m));
				if (rectToWeaponMap[m] != -1)
				{
					flag = false;
				}
			}
			if (flag)
			{
				rectToWeaponMap[0] = 0;
			}
			int num2 = int.Parse(configure.GetSingle("Save", "AvatarDataLength"));
			for (int n = 0; n < num2; n++)
			{
				avatarData[n] = (AvatarState)int.Parse(configure.GetArray("Save", "avatarData", n));
			}
			if (configure.GetSingle("Save", "MultiAchievement") != null)
			{
				for (int num3 = 0; num3 < 54; num3++)
				{
					MultiAchievementData[num3] = int.Parse(configure.GetArray("Save", "MultiAchievementData", num3));
				}
			}
			Avatar = (AvatarType)int.Parse(configure.GetSingle("Save", "AvatarType"));
			ArmorLevel = int.Parse(configure.GetSingle("Save", "ArmorLevel"));
			FirstTimeGame = bool.Parse(configure.GetSingle("Save", "FirstTimeGame"));
			if (configure.GetSingle("Save", "TotalTime") != null)
			{
				TotalTime = float.Parse(configure.GetSingle("Save", "TotalTime"));
			}
			if (configure.GetSingle("Save", "RebirthPacketCount") != null)
			{
				m_rebirth_packet_count = int.Parse(configure.GetSingle("Save", "RebirthPacketCount"));
			}
			if (configure.GetSingle("Save", "RescuePacketCount") != null)
			{
				m_rescue_packet_count = int.Parse(configure.GetSingle("Save", "RescuePacketCount"));
			}
			if (configure.GetSingle("Save", "TotalMoneyGet") != null)
			{
				TotalMoneyGet = int.Parse(configure.GetSingle("Save", "TotalMoneyGet"));
			}
			if (configure.GetSingle("Save", "MultiEnemyCount") != null)
			{
				multi_enemy_death_elite_count = int.Parse(configure.GetSingle("Save", "MultiEliteCount"));
				multi_enemy_death_normal_count = int.Parse(configure.GetSingle("Save", "MultiNormalCount"));
			}
			if (configure.GetSingle("Save", "First_Statistic_Time") != null)
			{
				First_Statistic_Time = configure.GetSingle("Save", "First_Statistic_Time");
			}
			Achievement.LoadNew(configure);
		}

		public void SaveMusicState()
		{
			string key = "Music";
			PlayerPrefs.SetInt(key, Convert.ToInt32(MusicOn));
			key = "Sound";
			PlayerPrefs.SetInt(key, Convert.ToInt32(SoundOn));
		}

		public void SaveDataTest()
		{
			if (!inited)
			{
				Init();
			}
			InitWeapons();
			Configure configure = new Configure();
			configure.AddSection("Save", string.Empty, string.Empty);
			configure.AddValueSingle("Save", "Cash", cash.ToString(), string.Empty, string.Empty);
			configure.AddValueSingle("Save", "Score", score.ToString(), string.Empty, string.Empty);
			configure.AddValueSingle("Save", "LevelNum", LevelNum.ToString(), string.Empty, string.Empty);
			configure.AddValueSingle("Save", "WeaponListCount", weaponList.Count.ToString(), string.Empty, string.Empty);
			ArrayList arrayList = new ArrayList();
			for (int i = 0; i < weaponList.Count; i++)
			{
				StringLine stringLine = new StringLine();
				stringLine.AddString(((int)weaponList[i].GetWeaponType()).ToString());
				stringLine.AddString(weaponList[i].Name);
				stringLine.AddString(weaponList[i].BulletCount.ToString());
				stringLine.AddString(((double)weaponList[i].Damage).ToString());
				stringLine.AddString(((double)weaponList[i].AttackFrequency).ToString());
				stringLine.AddString(((double)weaponList[i].Accuracy).ToString());
				stringLine.AddString(weaponList[i].DamageLevel.ToString());
				stringLine.AddString(weaponList[i].FrequencyLevel.ToString());
				stringLine.AddString(weaponList[i].AccuracyLevel.ToString());
				stringLine.AddString(weaponList[i].IsSelectedForBattle.ToString());
				stringLine.AddString(((int)weaponList[i].Exist).ToString());
				arrayList.Add(stringLine.content);
			}
			configure.AddValueArray2("Save", "WeaponsCfg", arrayList, string.Empty, string.Empty);
			StringLine stringLine2 = new StringLine();
			for (int j = 0; j < rectToWeaponMap.Length; j++)
			{
				stringLine2.AddString(rectToWeaponMap[j].ToString());
			}
			configure.AddValueArray("Save", "RectToWeaponMap", stringLine2.content, string.Empty, string.Empty);
			StringLine stringLine3 = new StringLine();
			configure.AddValueSingle("Save", "AvatarDataLength", avatarData.Length.ToString(), string.Empty, string.Empty);
			for (int k = 0; k < avatarData.Length; k++)
			{
				int num = (int)avatarData[k];
				stringLine3.AddString(num.ToString());
			}
			configure.AddValueArray("Save", "avatarData", stringLine3.content, string.Empty, string.Empty);
			configure.AddValueSingle("Save", "MultiAchievement", "1", string.Empty, string.Empty);
			StringLine stringLine4 = new StringLine();
			for (int l = 0; l < MultiAchievementData.Length; l++)
			{
				stringLine4.AddString(MultiAchievementData[l].ToString());
			}
			configure.AddValueArray("Save", "MultiAchievementData", stringLine4.content, string.Empty, string.Empty);
			configure.AddValueSingle("Save", "AvatarType", ((int)Avatar).ToString(), string.Empty, string.Empty);
			configure.AddValueSingle("Save", "ArmorLevel", ArmorLevel.ToString(), string.Empty, string.Empty);
			configure.AddValueSingle("Save", "FirstTimeGame", FirstTimeGame.ToString(), string.Empty, string.Empty);
			configure.AddValueSingle("Save", "MultiEnemyCount", "1", string.Empty, string.Empty);
			configure.AddValueSingle("Save", "MultiEliteCount", multi_enemy_death_elite_count.ToString(), string.Empty, string.Empty);
			configure.AddValueSingle("Save", "MultiNormalCount", multi_enemy_death_normal_count.ToString(), string.Empty, string.Empty);
			configure.AddValueSingle("Save", "TotalTime", TotalTime.ToString(), string.Empty, string.Empty);
			configure.AddValueSingle("Save", "TotalMoneyGet", TotalMoneyGet.ToString(), string.Empty, string.Empty);
			configure.AddValueSingle("Save", "RebirthPacketCount", m_rebirth_packet_count.ToString(), string.Empty, string.Empty);
			configure.AddValueSingle("Save", "RescuePacketCount", m_rescue_packet_count.ToString(), string.Empty, string.Empty);
			if (First_Statistic == 1)
			{
				First_Statistic_Time = Utils.GetIOSYear().ToString("D4") + "-" + Utils.GetIOSMonth().ToString("D2") + "-" + Utils.GetIOSDay().ToString("D2") + "-" + Utils.GetIOSHour().ToString("D2") + "-" + Utils.GetIOSMin().ToString("D2") + "-" + Utils.GetIOSSec().ToString("D2");
				First_Statistic = 0;
				configure.AddValueSingle("Save", "First_Statistic_Time", First_Statistic_Time, string.Empty, string.Empty);
			}
			Achievement.SaveNew(configure);
			string value = configure.Save();
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			Stream stream = File.Open(path + "TestSave.save", FileMode.Create);
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			binaryWriter.Write(value);
			binaryWriter.Close();
			stream.Close();
		}

		public void SaveDataNew()
		{
			if (!inited)
			{
				Init();
			}
			InitWeapons();
			Configure configure = new Configure();
			configure.AddSection("Save", string.Empty, string.Empty);
			configure.AddValueSingle("Save", "Cash", cash.ToString(), string.Empty, string.Empty);
			configure.AddValueSingle("Save", "Score", score.ToString(), string.Empty, string.Empty);
			configure.AddValueSingle("Save", "LevelNum", LevelNum.ToString(), string.Empty, string.Empty);
			configure.AddValueSingle("Save", "WeaponListCount", weaponList.Count.ToString(), string.Empty, string.Empty);
			ArrayList arrayList = new ArrayList();
			for (int i = 0; i < weaponList.Count; i++)
			{
				StringLine stringLine = new StringLine();
				stringLine.AddString(((int)weaponList[i].GetWeaponType()).ToString());
				stringLine.AddString(weaponList[i].Name);
				stringLine.AddString(weaponList[i].BulletCount.ToString());
				stringLine.AddString(((double)weaponList[i].Damage).ToString());
				stringLine.AddString(((double)weaponList[i].AttackFrequency).ToString());
				stringLine.AddString(((double)weaponList[i].Accuracy).ToString());
				stringLine.AddString(weaponList[i].DamageLevel.ToString());
				stringLine.AddString(weaponList[i].FrequencyLevel.ToString());
				stringLine.AddString(weaponList[i].AccuracyLevel.ToString());
				stringLine.AddString(weaponList[i].IsSelectedForBattle.ToString());
				stringLine.AddString(((int)weaponList[i].Exist).ToString());
				arrayList.Add(stringLine.content);
			}
			configure.AddValueArray2("Save", "WeaponsCfg", arrayList, string.Empty, string.Empty);
			StringLine stringLine2 = new StringLine();
			for (int j = 0; j < rectToWeaponMap.Length; j++)
			{
				stringLine2.AddString(rectToWeaponMap[j].ToString());
			}
			configure.AddValueArray("Save", "RectToWeaponMap", stringLine2.content, string.Empty, string.Empty);
			StringLine stringLine3 = new StringLine();
			configure.AddValueSingle("Save", "AvatarDataLength", avatarData.Length.ToString(), string.Empty, string.Empty);
			for (int k = 0; k < avatarData.Length; k++)
			{
				int num = (int)avatarData[k];
				stringLine3.AddString(num.ToString());
			}
			configure.AddValueArray("Save", "avatarData", stringLine3.content, string.Empty, string.Empty);
			configure.AddValueSingle("Save", "MultiAchievement", "1", string.Empty, string.Empty);
			StringLine stringLine4 = new StringLine();
			for (int l = 0; l < MultiAchievementData.Length; l++)
			{
				stringLine4.AddString(MultiAchievementData[l].ToString());
			}
			configure.AddValueArray("Save", "MultiAchievementData", stringLine4.content, string.Empty, string.Empty);
			configure.AddValueSingle("Save", "AvatarType", ((int)Avatar).ToString(), string.Empty, string.Empty);
			configure.AddValueSingle("Save", "ArmorLevel", ArmorLevel.ToString(), string.Empty, string.Empty);
			configure.AddValueSingle("Save", "FirstTimeGame", FirstTimeGame.ToString(), string.Empty, string.Empty);
			configure.AddValueSingle("Save", "MultiEnemyCount", "1", string.Empty, string.Empty);
			configure.AddValueSingle("Save", "MultiEliteCount", multi_enemy_death_elite_count.ToString(), string.Empty, string.Empty);
			configure.AddValueSingle("Save", "MultiNormalCount", multi_enemy_death_normal_count.ToString(), string.Empty, string.Empty);
			configure.AddValueSingle("Save", "TotalTime", TotalTime.ToString(), string.Empty, string.Empty);
			configure.AddValueSingle("Save", "TotalMoneyGet", TotalMoneyGet.ToString(), string.Empty, string.Empty);
			configure.AddValueSingle("Save", "RebirthPacketCount", m_rebirth_packet_count.ToString(), string.Empty, string.Empty);
			configure.AddValueSingle("Save", "RescuePacketCount", m_rescue_packet_count.ToString(), string.Empty, string.Empty);
			if (First_Statistic == 1)
			{
				First_Statistic_Time = Utils.GetIOSYear().ToString("D4") + "-" + Utils.GetIOSMonth().ToString("D2") + "-" + Utils.GetIOSDay().ToString("D2") + "-" + Utils.GetIOSHour().ToString("D2") + "-" + Utils.GetIOSMin().ToString("D2") + "-" + Utils.GetIOSSec().ToString("D2");
				First_Statistic = 0;
			}
			configure.AddValueSingle("Save", "First_Statistic_Time", First_Statistic_Time, string.Empty, string.Empty);
			Achievement.SaveNew(configure);
			string str = configure.Save();
			str = GameSaveStringEncipher(str, 71);
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			Stream stream = File.Open(path + "CallMini.save", FileMode.Create);
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			binaryWriter.Write(str);
			binaryWriter.Close();
			stream.Close();
		}

		public void ClearState()
		{
			inited = false;
			weaponsInited = false;
			Init();
		}

		public void Init()
		{
			if (!inited)
			{
				gConfig = GameApp.GetInstance().GetGameConfig();
				cash = gConfig.globalConf.startMoney;
				SoundOn = true;
				MusicOn = true;
				infectionRate = new int[4];
				for (int i = 0; i < 4; i++)
				{
					infectionRate[i] = 100;
				}
				infectionRate[2] = 100;
				Avatar = AvatarType.Human;
				avatarData = new AvatarState[12];
				for (int j = 0; j < avatarData.Length; j++)
				{
					avatarData[j] = AvatarState.ToBuy;
					user_statistics.avatar_use_count.Add(0);
				}
				avatarData[0] = AvatarState.Avaliable;
				MultiAchievementData = new int[54];
				for (int k = 0; k < MultiAchievementData.Length; k++)
				{
					MultiAchievementData[k] = 0;
				}
				FirstTimeGame = true;
				ArmorLevel = 0;
				LevelNum = 1;
				inited = true;
			}
		}

		public Weapon InitMultiWeapon(int index)
		{
			List<WeaponConfig> weapons = gConfig.GetWeapons();
			Weapon result = null;
			int num = 0;
			foreach (WeaponConfig item in weapons)
			{
				if (num == index)
				{
					string name = item.name;
					result = WeaponFactory.GetInstance().CreateWeapon(item.wType, true);
					result.Exist = item.startEquip;
					result.Name = name;
					result.WConf = item;
					result.LoadConfig();
					return result;
				}
				num++;
			}
			return result;
		}

		public void InitWeapons()
		{
			if (weaponsInited)
			{
				return;
			}
			weaponList.Clear();
			for (int i = 0; i < rectToWeaponMap.Length; i++)
			{
				rectToWeaponMap[i] = -1;
			}
			List<WeaponConfig> weapons = gConfig.GetWeapons();
			user_statistics.weapon_use_count.Clear();
			int num = 0;
			foreach (WeaponConfig item in weapons)
			{
				string name = item.name;
				Weapon weapon = WeaponFactory.GetInstance().CreateWeapon(item.wType, false);
				weapon.weapon_index = num;
				if (item.name == "MP5")
				{
					weapon.IsSelectedForBattle = true;
					rectToWeaponMap[0] = num;
				}
				if (item.name == "Chainsaw")
				{
					weapon.IsSelectedForBattle = true;
					rectToWeaponMap[1] = num;
				}
				weapon.Exist = item.startEquip;
				weapon.Name = name;
				weapon.WConf = item;
				weapon.LoadConfig();
				weaponList.Add(weapon);
				user_statistics.weapon_use_count.Add(0);
				num++;
			}
			weaponsInited = true;
		}

		public int GetArmorPrice()
		{
			float num = gConfig.playerConf.upgradeArmorPrice;
			for (int i = 0; i < ArmorLevel; i++)
			{
				num += num * gConfig.playerConf.upPriceFactor;
			}
			return (int)num;
		}

		public void DeliverIAPItem(IAPName iapName)
		{
			switch (iapName)
			{
			case IAPName.Cash1D:
				AddCash(62500);
				user_statistics.cash_iap += 62500;
				user_statistics.Iap_buy_count[0]++;
				break;
			case IAPName.Cash5D:
				AddCash(350000);
				user_statistics.cash_iap += 350000;
				user_statistics.Iap_buy_count[1]++;
				break;
			case IAPName.Cash10D:
				AddCash(760000);
				user_statistics.cash_iap += 760000;
				user_statistics.Iap_buy_count[2]++;
				break;
			case IAPName.Cash20D:
				AddCash(1660000);
				user_statistics.cash_iap += 1660000;
				user_statistics.Iap_buy_count[3]++;
				break;
			case IAPName.Cash50D:
				AddCash(4160000);
				user_statistics.cash_iap += 4160000;
				user_statistics.Iap_buy_count[4]++;
				break;
			case IAPName.Cash100D:
				AddCash(9160000);
				user_statistics.cash_iap += 9160000;
				user_statistics.Iap_buy_count[5]++;
				break;
			}
			GameApp.GetInstance().Save();
		}

		public bool IsWeaponOwned(WeaponType wType)
		{
			for (int i = 0; i < weaponList.Count; i++)
			{
				if (weaponList[i].GetWeaponType() == wType && weaponList[i].Exist == WeaponExistState.Owned)
				{
					return true;
				}
			}
			return false;
		}

		public List<Weapon> GetBattleWeapons()
		{
			List<Weapon> list = new List<Weapon>();
			for (int i = 0; i < rectToWeaponMap.Length; i++)
			{
				if (rectToWeaponMap[i] != -1)
				{
					list.Add(weaponList[rectToWeaponMap[i]]);
				}
			}
			return list;
		}

		public void StatisticWeapons()
		{
			for (int i = 0; i < rectToWeaponMap.Length; i++)
			{
				if (rectToWeaponMap[i] != -1)
				{
					List<int> weapon_use_count;
					List<int> list = (weapon_use_count = user_statistics.weapon_use_count);
					int index;
					int index2 = (index = rectToWeaponMap[i]);
					index = weapon_use_count[index];
					list[index2] = index + 1;
				}
			}
		}

		public int GetWeaponByOwnedIndex(int index)
		{
			int num = 0;
			for (int i = 0; i < weaponList.Count; i++)
			{
				Weapon weapon = weaponList[i];
				if (weapon.Exist == WeaponExistState.Owned)
				{
					if (num == index)
					{
						return i;
					}
					num++;
				}
			}
			return -1;
		}

		public void AddCash(int cashGot)
		{
			cash += cashGot;
			cash = Mathf.Clamp(cash, 0, 99999999);
			Achievement.CheckAchievemnet_RichMan(cash);
		}

		public void AddCashForRecord(int cashGot)
		{
			if (!GameApp.GetInstance().GetGameState().endless_multiplayer)
			{
				loot_cash += cashGot;
				AddCash(cashGot);
				user_statistics.cash_loot += cashGot;
				TotalMoneyGet += cashGot;
			}
		}

		public void AddCashForReport(int cashGot)
		{
			loot_cash += cashGot;
			AddCash(cashGot);
			user_statistics.cash_loot += cashGot;
			TotalMoneyGet += cashGot;
		}

		public void LoseCash(int cashSpend)
		{
			cash -= cashSpend;
		}

		public int GetCash()
		{
			return cash;
		}

		public List<Weapon> GetWeapons()
		{
			return weaponList;
		}

		public WeaponBuyStatus BuyWeapon(Weapon w, int price)
		{
			int weaponIndex = GetWeaponIndex(w);
			if (weaponList[weaponIndex].Exist == WeaponExistState.Unlocked)
			{
				if (cash >= price)
				{
					weaponList[weaponIndex].Exist = WeaponExistState.Owned;
					LoseCash(price);
					GameApp.GetInstance().GetGameState().user_statistics.cash_spend_weapons += price;
					GameApp.GetInstance().GetGameState().user_statistics.consume_list.Add("Buy_Weapon_" + w.Name);
					Achievement.GotNewWeapon();
					return WeaponBuyStatus.Succeed;
				}
				Debug.Log("Not Enough Cash!");
				return WeaponBuyStatus.NotEnoughCash;
			}
			Debug.Log("Weapon is Not Available!");
			return WeaponBuyStatus.Locked;
		}

		public PacketBuyStatus BuyRescue(int price)
		{
			if (cash >= price)
			{
				if (m_rescue_packet_count < 10)
				{
					LoseCash(price);
					m_rescue_packet_count++;
					return PacketBuyStatus.Succeed;
				}
				return PacketBuyStatus.Maxed;
			}
			return PacketBuyStatus.NotEnoughCash;
		}

		public PacketBuyStatus BuyRebirth(int price)
		{
			if (cash >= price)
			{
				if (m_rebirth_packet_count < 5)
				{
					LoseCash(price);
					m_rebirth_packet_count++;
					return PacketBuyStatus.Succeed;
				}
				return PacketBuyStatus.Maxed;
			}
			return PacketBuyStatus.NotEnoughCash;
		}

		public bool BuyAvatar(AvatarType aType, int price)
		{
			if (avatarData[(int)aType] == AvatarState.ToBuy && cash >= price)
			{
				avatarData[(int)aType] = AvatarState.Avaliable;
				LoseCash(price);
				GameApp.GetInstance().GetGameState().user_statistics.consume_list.Add("Buy_Avata_" + aType);
				Achievement.GotNewAvatar();
				return true;
			}
			return false;
		}

		public void UnlockAvatar(AvatarType aType)
		{
			if (avatarData[(int)aType] == AvatarState.ToBuy)
			{
				avatarData[(int)aType] = AvatarState.Avaliable;
				Achievement.GotNewAvatar();
			}
		}

		public WeaponType RandomWeaponAlreadyHave()
		{
			int count = weaponList.Count;
			int index = UnityEngine.Random.Range(0, count);
			if (count != 0)
			{
				return weaponList[index].GetWeaponType();
			}
			return WeaponType.AssaultRifle;
		}

		public WeaponType RandomBattleWeapons()
		{
			List<Weapon> list = new List<Weapon>();
			foreach (Weapon weapon in weaponList)
			{
				if (weapon.IsSelectedForBattle && weapon.GetWeaponType() != WeaponType.Saw && weapon.GetWeaponType() != WeaponType.Sword)
				{
					list.Add(weapon);
				}
			}
			int count = list.Count;
			int index = UnityEngine.Random.Range(0, count);
			if (count != 0)
			{
				return list[index].GetWeaponType();
			}
			return WeaponType.AssaultRifle;
		}

		public bool BuyBullets(Weapon w, int bulletsNum, int price)
		{
			if (cash >= price)
			{
				w.AddBullets(bulletsNum);
				LoseCash(price);
				GameApp.GetInstance().GetGameState().user_statistics.cash_spend_bullets += price;
				return true;
			}
			return false;
		}

		public bool UpgradeWeapon(Weapon w, float power, float frequency, float accuracy, int price)
		{
			if (cash >= price)
			{
				w.Upgrade(power, frequency, accuracy);
				LoseCash(price);
				GameApp.GetInstance().GetGameState().user_statistics.cash_spend_weapons += price;
				return true;
			}
			return false;
		}

		public bool UpgradeArmor(int price)
		{
			if (cash >= price && ArmorLevel < gConfig.playerConf.maxArmorLevel)
			{
				ArmorLevel++;
				LoseCash(price);
				GameApp.GetInstance().GetGameState().user_statistics.cash_spend_weapons += price;
				return true;
			}
			return false;
		}

		public void ResetData()
		{
			ClearState();
			InitWeapons();
			Achievement.ResetData();
			GameApp.GetInstance().PlayerPrefsReset();
			GameApp.GetInstance().Save();
		}
	}
}
