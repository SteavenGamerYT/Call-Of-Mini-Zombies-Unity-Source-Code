using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zombie3D
{
	public class GameMultiplayerScene : GameScene
	{
		public List<UserReportData> user_report_data = new List<UserReportData>();

		protected NetworkObj net_com;

		public int MultiMonsterWave;

		public int[] multi_enemy_death_type_count;

		public int[] multi_elite_enemy_death_type_count;

		public int[] multi_combo_enemy_death_type_count;

		public int[] multi_combo_enemy_death_type_count_temp;

		public int multi_enemy_death_elite_count;

		public int multi_enemy_death_normal_count;

		public int multi_enemy_death_elite_count_now;

		public int multi_enemy_death_normal_count_now;

		public float player_injured_val;

		public int[] multi_enemy_death_weapontype_count;

		public float multi_god_time;

		public float multi_survive_time;

		public int loot_item;

		protected EnemyType last_kill_enemy_type = EnemyType.E_NONE;

		protected CallbackFunc game_over_call_back;

		public GameObject Multi_Tutorial_Triger;

		public GameSceneTUI game_tui;

		protected float report_data_time;

		protected float game_over_check_time;

		public bool is_game_over;

		public override bool GetGameExcute()
		{
			return is_game_excute;
		}

		public override void Init(int index)
		{
			net_com = GameApp.GetInstance().GetGameState().net_com;
			net_com.packet_delegate = OnPacket;
			net_com.someone_birth_delegate = OnSomeoneBirth;
			net_com.leave_room_notity_delegate = OnSomeoneLeave;
			net_com.leave_room_delegate = OnLeaveRoom;
			game_over_call_back = OnGameOver;
			GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load("Prefabs/TUI/MultiGameTUI")) as GameObject;
			game_tui = gameObject.transform.Find("SceneTUI").GetComponent<GameSceneTUI>();
			game_tui.SetMultiScene(this);
			is_game_excute = true;
			m_multi_player_arr = new List<Player>();
			m_player_set = new List<Player>();
			base.Init(index);
			m_multi_player_arr.Add(player);
			m_player_set.Add(player);
			for (int i = 0; i < 4; i++)
			{
				if (net_com.netUserInfo_array[i] != null && net_com.netUserInfo_array[i].multiplayer != null)
				{
					net_com.netUserInfo_array[i].multiplayer.nick_name = net_com.netUserInfo_array[i].nick_name;
					net_com.netUserInfo_array[i].multiplayer.Init();
					m_multi_player_arr.Add(net_com.netUserInfo_array[i].multiplayer);
					m_player_set.Add(net_com.netUserInfo_array[i].multiplayer);
				}
			}
			multi_enemy_death_type_count = new int[8];
			for (int j = 0; j < 8; j++)
			{
				multi_enemy_death_type_count[j] = 0;
			}
			multi_elite_enemy_death_type_count = new int[8];
			for (int k = 0; k < 8; k++)
			{
				multi_elite_enemy_death_type_count[k] = 0;
			}
			multi_enemy_death_weapontype_count = new int[12];
			for (int l = 0; l < 12; l++)
			{
				multi_enemy_death_weapontype_count[l] = 0;
			}
			multi_combo_enemy_death_type_count = new int[8];
			for (int m = 0; m < 8; m++)
			{
				multi_combo_enemy_death_type_count[m] = 0;
			}
			multi_combo_enemy_death_type_count_temp = new int[8];
			for (int n = 0; n < 8; n++)
			{
				multi_combo_enemy_death_type_count_temp[n] = 0;
			}
			GameApp.GetInstance().GetGameState().loot_cash = 0;
			GameApp.GetInstance().GetGameState().bullet_comsume = 0;
			if (GameApp.GetInstance().GetGameState().multi_toturial_triger == 1)
			{
				net_com.SomeoneBirthNotifyVirtual();
				Multi_Tutorial_Triger = GameObject.Find("MultiTutorialTriger");
				Multi_Tutorial_Triger.GetComponent<MultiTutorialTriger>().game_scnen = this;
				Multi_Tutorial_Triger.GetComponent<MultiTutorialTriger>().LocalPlayer = player;
			}
			multi_enemy_death_elite_count = GameApp.GetInstance().GetGameState().multi_enemy_death_elite_count;
			multi_enemy_death_normal_count = GameApp.GetInstance().GetGameState().multi_enemy_death_normal_count;
			GameApp.GetInstance().GetGameState().user_statistics.coop_count++;
		}

		public override void DoLogic(float deltaTime)
		{
			if (!is_game_excute)
			{
				return;
			}
			player.DoLogic(deltaTime);
			if (!is_game_over)
			{
				object[] array = new object[enemyList.Count];
				enemyList.Keys.CopyTo(array, 0);
				for (int i = 0; i < array.Length; i++)
				{
					Enemy enemy = enemyList[array[i]] as Enemy;
					enemy.DoLogic(deltaTime);
				}
			}
			hitBloodObjectPool.AutoDestruct();
			deadBodyObjectPool[0].AutoDestruct();
			deadBodyObjectPool[1].AutoDestruct();
			deadBodyObjectPool[4].AutoDestruct();
			deadBodyObjectPool[5].AutoDestruct();
			for (int j = 0; j < 4; j++)
			{
				if (net_com.netUserInfo_array[j] != null && net_com.netUserInfo_array[j].multiplayer != null)
				{
					net_com.netUserInfo_array[j].multiplayer.DoLogic(deltaTime);
				}
			}
			report_data_time += deltaTime;
			if (report_data_time >= 1f)
			{
				Packet packet = CGUserDataReportPacket.MakePacket((uint)net_com.m_netUserInfo.user_id, (uint)(multi_enemy_death_elite_count_now + multi_enemy_death_normal_count_now), (uint)player.m_death_count, (uint)GameApp.GetInstance().GetGameState().loot_cash, (long)player_injured_val * 1000);
				net_com.Send(packet);
			}
			game_over_check_time += deltaTime;
			if (game_over_check_time >= 5f)
			{
				game_over_check_time = 0f;
				CheckMultiGameOver();
			}
		}

		public void OnPacket(Packet packet)
		{
			if (!is_game_excute)
			{
				return;
			}
			uint val = 0u;
			if (packet.WatchUInt32(ref val, 4))
			{
				switch (val)
				{
				case 69891u:
					OnUserActionNotify(packet);
					break;
				case 69890u:
					OnUserStatusNotify(packet);
					break;
				case 69892u:
					OnUserChangeWeaponNotify(packet);
					break;
				case 69893u:
					OnUserSniperFireNotify(packet);
					break;
				case 69894u:
					OnEnemyBirthNotify(packet);
					break;
				case 69895u:
					OnEnemyStatusNotify(packet);
					break;
				case 69896u:
					OnEnemyGotHitNotify(packet);
					break;
				case 4106u:
					OnEnemyDead(packet);
					break;
				case 4362u:
					OnEnemyDeadNotify(packet);
					break;
				case 69901u:
					OnEnemyRemoveNotify(packet);
					break;
				case 69897u:
					OnEnemyLootNotify(packet);
					break;
				case 69898u:
					OnUserInjuryed(packet);
					break;
				case 69899u:
					OnEnemyChangeTarget(packet);
					break;
				case 69900u:
					OnUserRebirth(packet);
					break;
				case 4363u:
					OnMasterChange(packet);
					break;
				case 4108u:
					OnUserDoRebirth(packet);
					break;
				case 4364u:
					OnUserDoRebirthNotity(packet);
					break;
				case 69902u:
					OnUserReportDataNotity(packet);
					break;
				case 69903u:
					OnUserRealDead(packet);
					break;
				}
			}
		}

		public void OnSomeoneBirth(Player player)
		{
			player.Init();
			m_multi_player_arr.Add(player);
			m_player_set.Add(player);
			if (GameApp.GetInstance().GetGameState().multi_toturial_triger == 1)
			{
				Multi_Tutorial_Triger = GameObject.Find("MultiTutorialTriger");
				Multi_Tutorial_Triger.GetComponent<MultiTutorialTriger>().AIPlayer = player;
			}
		}

		public void OnUserStatusNotify(Packet packet)
		{
			GCUserStatusNotifyPacket gCUserStatusNotifyPacket = new GCUserStatusNotifyPacket();
			if (!gCUserStatusNotifyPacket.ParserPacket(packet))
			{
				Debug.Log("OnUserStatusNotify ParserPacket Error!!!");
				return;
			}
			for (int i = 0; i < 4; i++)
			{
				if (net_com.netUserInfo_array[i] != null && net_com.netUserInfo_array[i].multiplayer != null && net_com.netUserInfo_array[i].user_id == gCUserStatusNotifyPacket.m_iUserId)
				{
					float ping = (float)gCUserStatusNotifyPacket.m_iPingTime / 1000f;
					net_com.netUserInfo_array[i].multiplayer.SetNetUserStatus(gCUserStatusNotifyPacket.m_direct, gCUserStatusNotifyPacket.m_Rotation, gCUserStatusNotifyPacket.m_Position, ping);
					break;
				}
			}
		}

		public void OnUserChangeWeaponNotify(Packet packet)
		{
			GCUserChangeWeaponNotifyPacket gCUserChangeWeaponNotifyPacket = new GCUserChangeWeaponNotifyPacket();
			if (!gCUserChangeWeaponNotifyPacket.ParserPacket(packet))
			{
				Debug.Log("OnUserChangeWeaponNotify ParserPacket Error!!!");
				return;
			}
			for (int i = 0; i < 4; i++)
			{
				if (net_com.netUserInfo_array[i] != null && net_com.netUserInfo_array[i].multiplayer != null && net_com.netUserInfo_array[i].user_id == gCUserChangeWeaponNotifyPacket.m_iUserId)
				{
					net_com.netUserInfo_array[i].multiplayer.ChangeWeaponWithindex((int)gCUserChangeWeaponNotifyPacket.m_iWeaponIndex);
					break;
				}
			}
		}

		public void OnUserSniperFireNotify(Packet packet)
		{
			GCUserSniperFireNotifyPacket gCUserSniperFireNotifyPacket = new GCUserSniperFireNotifyPacket();
			if (!gCUserSniperFireNotifyPacket.ParserPacket(packet))
			{
				return;
			}
			for (int i = 0; i < 4; i++)
			{
				if (net_com.netUserInfo_array[i] != null && net_com.netUserInfo_array[i].multiplayer != null && net_com.netUserInfo_array[i].user_id == gCUserSniperFireNotifyPacket.m_iUserId)
				{
					if (net_com.netUserInfo_array[i].multiplayer.GetWeapon().GetWeaponType() == WeaponType.Sniper)
					{
						MultiSniper multiSniper = net_com.netUserInfo_array[i].multiplayer.GetWeapon() as MultiSniper;
						multiSniper.AddMultiTarget(gCUserSniperFireNotifyPacket.m_Position);
						net_com.netUserInfo_array[i].multiplayer.OnMultiSniperFire();
					}
					break;
				}
			}
		}

		public void OnUserActionNotify(Packet packet)
		{
			GCUserActionNotifyPacket gCUserActionNotifyPacket = new GCUserActionNotifyPacket();
			if (!gCUserActionNotifyPacket.ParserPacket(packet))
			{
				Debug.Log("OnUserActionNotify ParserPacket Error!!!");
				return;
			}
			for (int i = 0; i < 4; i++)
			{
				if (net_com.netUserInfo_array[i] != null && net_com.netUserInfo_array[i].multiplayer != null && net_com.netUserInfo_array[i].user_id == gCUserActionNotifyPacket.m_iUserId)
				{
					net_com.netUserInfo_array[i].multiplayer.SetStateWithType((PlayerStateType)gCUserActionNotifyPacket.m_iAction);
					break;
				}
			}
		}

		public void OnEnemyBirthNotify(Packet packet)
		{
			GCEnemyBirthNotifyPacket gCEnemyBirthNotifyPacket = new GCEnemyBirthNotifyPacket();
			if (!gCEnemyBirthNotifyPacket.ParserPacket(packet))
			{
				Debug.Log("OnEnemyBirthNotify ParserPacket Error!!!");
				return;
			}
			if (gCEnemyBirthNotifyPacket.m_enemy_wave > MultiMonsterWave)
			{
				MultiMonsterWave = (int)gCEnemyBirthNotifyPacket.m_enemy_wave;
				CalculateDifficultyFactor(MultiMonsterWave);
			}
			GameObject gameObject = null;
			GameObject gameObject2 = null;
			if (gCEnemyBirthNotifyPacket.m_isElite == 1)
			{
				gameObject = GameApp.GetInstance().GetEnemyResourceConfig().enemy_elite[gCEnemyBirthNotifyPacket.m_enemy_type];
				gameObject2 = (gameObject2 = UnityEngine.Object.Instantiate(gameObject, gCEnemyBirthNotifyPacket.m_Position, Quaternion.Euler(0f, 0f, 0f)));
			}
			else
			{
				gameObject = GameApp.GetInstance().GetEnemyResourceConfig().enemy[gCEnemyBirthNotifyPacket.m_enemy_type];
				gameObject2 = GetEnemyPool((EnemyType)gCEnemyBirthNotifyPacket.m_enemy_type).CreateObject(gCEnemyBirthNotifyPacket.m_Position, Quaternion.Euler(0f, 0f, 0f));
				gameObject2.layer = 9;
			}
			if (gCEnemyBirthNotifyPacket.m_isGrave == 1)
			{
				UnityEngine.Object.Instantiate(GameApp.GetInstance().GetGameResourceConfig().graveRock, gCEnemyBirthNotifyPacket.m_Position + Vector3.down * 0.3f, Quaternion.identity);
			}
			enemyID = (int)gCEnemyBirthNotifyPacket.m_enemy_Id;
			gameObject2.name = "E_" + enemyID;
			Enemy enemy = null;
			switch (gCEnemyBirthNotifyPacket.m_enemy_type)
			{
			case 0u:
				enemy = new Zombie();
				break;
			case 1u:
				enemy = new Nurse();
				break;
			case 2u:
				enemy = new Tank();
				gameObject2.transform.Translate(Vector3.up * 2f);
				break;
			case 3u:
				enemy = new Hunter();
				break;
			case 4u:
				enemy = new Boomer();
				break;
			case 5u:
				enemy = new Swat();
				break;
			case 6u:
				enemy = new Dog();
				break;
			case 7u:
				enemy = new Police();
				break;
			}
			enemy.IsElite = gCEnemyBirthNotifyPacket.m_isElite == 1;
			enemy.m_isBoss = gCEnemyBirthNotifyPacket.m_isBoss == 1;
			enemy.Init(gameObject2);
			enemy.EnemyType = (EnemyType)gCEnemyBirthNotifyPacket.m_enemy_type;
			enemy.Name = gameObject2.name;
			for (int i = 0; i < 4; i++)
			{
				if (net_com.netUserInfo_array[i] != null && net_com.netUserInfo_array[i].multiplayer != null && net_com.netUserInfo_array[i].user_id == gCEnemyBirthNotifyPacket.m_target_id)
				{
					enemy.TargetPlayer = net_com.netUserInfo_array[i].multiplayer;
					break;
				}
			}
			if (gCEnemyBirthNotifyPacket.m_isGrave == 1)
			{
				enemy.SetInGrave(true);
			}
			GetEnemies().Add(gameObject2.name, enemy);
		}

		public void OnEnemyStatusNotify(Packet packet)
		{
			GCEnemyStatusNotifyPacket gCEnemyStatusNotifyPacket = new GCEnemyStatusNotifyPacket();
			if (!gCEnemyStatusNotifyPacket.ParserPacket(packet))
			{
				Debug.Log("OnEnemyStatusNotify ParserPacket Error!!!");
				return;
			}
			Enemy enemyByID = GetEnemyByID(gCEnemyStatusNotifyPacket.m_enemyID);
			if (enemyByID != null)
			{
				enemyByID.SetNetEnemyStatus(gCEnemyStatusNotifyPacket.m_Direction, gCEnemyStatusNotifyPacket.m_Rotation, gCEnemyStatusNotifyPacket.m_Position);
			}
		}

		public void OnEnemyGotHitNotify(Packet packet)
		{
			GCEnemyGotHitNotifyPacket gCEnemyGotHitNotifyPacket = new GCEnemyGotHitNotifyPacket();
			if (!gCEnemyGotHitNotifyPacket.ParserPacket(packet))
			{
				Debug.Log("OnEnemyGotHitNotify ParserPacket Error!!!");
				return;
			}
			Enemy enemyByID = GetEnemyByID(gCEnemyGotHitNotifyPacket.m_enemyID);
			if (enemyByID != null)
			{
				enemyByID.OnMultiHit((float)gCEnemyGotHitNotifyPacket.m_iDamage / 1000f, (WeaponType)gCEnemyGotHitNotifyPacket.m_weapon_type, (int)gCEnemyGotHitNotifyPacket.m_critical_attack);
			}
		}

		public void OnEnemyDead(Packet packet)
		{
			GCEnemyDeadPacket gCEnemyDeadPacket = new GCEnemyDeadPacket();
			if (!gCEnemyDeadPacket.ParserPacket(packet))
			{
				Debug.Log("OnEnemyDead ParserPacket Error!!!");
			}
			else if (gCEnemyDeadPacket.m_iResult == 0)
			{
				int monsterLootCash = GetMonsterLootCash((EnemyType)gCEnemyDeadPacket.m_enemy_type);
				GameApp.GetInstance().GetGameState().AddCashForReport((int)((float)monsterLootCash * base.GetDifficultyCashDropFactor));
				if (gCEnemyDeadPacket.bElite == 1)
				{
					multi_enemy_death_elite_count++;
					multi_elite_enemy_death_type_count[gCEnemyDeadPacket.m_enemy_type]++;
					multi_enemy_death_elite_count_now++;
				}
				else
				{
					multi_enemy_death_normal_count++;
					multi_enemy_death_normal_count_now++;
				}
				multi_enemy_death_type_count[gCEnemyDeadPacket.m_enemy_type]++;
				multi_enemy_death_weapontype_count[gCEnemyDeadPacket.weapon_type]++;
				if (last_kill_enemy_type == EnemyType.E_NONE)
				{
					last_kill_enemy_type = (EnemyType)gCEnemyDeadPacket.m_enemy_type;
					multi_combo_enemy_death_type_count_temp[gCEnemyDeadPacket.m_enemy_type] = 1;
					multi_combo_enemy_death_type_count[gCEnemyDeadPacket.m_enemy_type] = 1;
				}
				else if (last_kill_enemy_type == (EnemyType)gCEnemyDeadPacket.m_enemy_type)
				{
					multi_combo_enemy_death_type_count_temp[gCEnemyDeadPacket.m_enemy_type]++;
					if (multi_combo_enemy_death_type_count_temp[gCEnemyDeadPacket.m_enemy_type] > multi_combo_enemy_death_type_count[gCEnemyDeadPacket.m_enemy_type])
					{
						multi_combo_enemy_death_type_count[gCEnemyDeadPacket.m_enemy_type] = multi_combo_enemy_death_type_count_temp[gCEnemyDeadPacket.m_enemy_type];
					}
				}
				else
				{
					multi_combo_enemy_death_type_count_temp[(int)last_kill_enemy_type] = 0;
					last_kill_enemy_type = (EnemyType)gCEnemyDeadPacket.m_enemy_type;
				}
			}
			else
			{
				Debug.Log("OnEnemyDead Result : " + gCEnemyDeadPacket.m_iResult);
			}
		}

		public void OnEnemyDeadNotify(Packet packet)
		{
			GCEnemyDeadNotifyPacket gCEnemyDeadNotifyPacket = new GCEnemyDeadNotifyPacket();
			if (!gCEnemyDeadNotifyPacket.ParserPacket(packet))
			{
				Debug.Log("OnEnemyDeadNotify ParserPacket Error!!!");
				return;
			}
			Enemy enemyByID = GetEnemyByID(gCEnemyDeadNotifyPacket.enemy_id);
			if (enemyByID != null && enemyByID.GetState() != Enemy.DEAD_STATE)
			{
				enemyByID.OnDead();
				enemyByID.SetState(Enemy.DEAD_STATE);
			}
		}

		public void OnEnemyRemoveNotify(Packet packet)
		{
			GCEnemyRemoveNotifyPacket gCEnemyRemoveNotifyPacket = new GCEnemyRemoveNotifyPacket();
			if (!gCEnemyRemoveNotifyPacket.ParserPacket(packet))
			{
				Debug.Log("OnEnemyRemoveNotify ParserPacket Error!!!");
				return;
			}
			Enemy enemyByID = GetEnemyByID(gCEnemyRemoveNotifyPacket.m_enemyID);
			if (enemyByID != null && enemyByID.GetState() != Enemy.DEAD_STATE)
			{
				enemyByID.OnDead();
				enemyByID.SetState(Enemy.DEAD_STATE);
			}
		}

		public void OnEnemyLootNotify(Packet packet)
		{
			GCEnemyLootNotifyPacket gCEnemyLootNotifyPacket = new GCEnemyLootNotifyPacket();
			if (!gCEnemyLootNotifyPacket.ParserPacket(packet))
			{
				Debug.Log("OnEnemyLootNotify ParserPacket Error!!!");
			}
			else
			{
				LootManagerScript.LootSpawnItem((ItemType)gCEnemyLootNotifyPacket.item_type, gCEnemyLootNotifyPacket.m_Position);
			}
		}

		public void OnUserInjuryed(Packet packet)
		{
			GCUserInjuryNotifyPacket gCUserInjuryNotifyPacket = new GCUserInjuryNotifyPacket();
			if (!gCUserInjuryNotifyPacket.ParserPacket(packet))
			{
				Debug.Log("OnUserInjuryed ParserPacket Error!!!");
				return;
			}
			for (int i = 0; i < 4; i++)
			{
				if (net_com.netUserInfo_array[i] != null && net_com.netUserInfo_array[i].multiplayer != null && net_com.netUserInfo_array[i].user_id == gCUserInjuryNotifyPacket.m_iUserId)
				{
					net_com.netUserInfo_array[i].multiplayer.OnMultiInjury((float)gCUserInjuryNotifyPacket.m_iInjury_val / 1000f, (float)gCUserInjuryNotifyPacket.m_total_hp_val / 1000f, (float)gCUserInjuryNotifyPacket.m_cur_hp_val / 1000f);
					break;
				}
			}
		}

		public void OnEnemyChangeTarget(Packet packet)
		{
			GCEnemyChangeTargetNotifyPacket gCEnemyChangeTargetNotifyPacket = new GCEnemyChangeTargetNotifyPacket();
			if (!gCEnemyChangeTargetNotifyPacket.ParserPacket(packet))
			{
				Debug.Log("OnEnemyChangeTarget ParserPacket Error!!!");
				return;
			}
			Enemy enemyByID = GetEnemyByID(gCEnemyChangeTargetNotifyPacket.m_enemyID);
			if (enemyByID == null)
			{
				return;
			}
			foreach (Player item in m_multi_player_arr)
			{
				if (item.m_multi_id == gCEnemyChangeTargetNotifyPacket.target_id)
				{
					enemyByID.SetPlayer(item);
					break;
				}
			}
		}

		public void OnUserRebirth(Packet packet)
		{
			GCUserRebirthNotifyPacket gCUserRebirthNotifyPacket = new GCUserRebirthNotifyPacket();
			if (!gCUserRebirthNotifyPacket.ParserPacket(packet))
			{
				return;
			}
			Debug.Log("GCUserRebirthNotifyPacket ***" + gCUserRebirthNotifyPacket.m_iUserId);
			for (int i = 0; i < 4; i++)
			{
				if (net_com.netUserInfo_array[i] != null && net_com.netUserInfo_array[i].multiplayer != null && net_com.netUserInfo_array[i].user_id == gCUserRebirthNotifyPacket.m_iUserId)
				{
					net_com.netUserInfo_array[i].multiplayer.OnRebirth();
					break;
				}
			}
		}

		public void OnMasterChange(Packet packet)
		{
			GCMasterChangePacket gCMasterChangePacket = new GCMasterChangePacket();
			if (!gCMasterChangePacket.ParserPacket(packet))
			{
				Debug.Log("OnMasterChange ParserPacket Error!!!");
				return;
			}
			net_com.m_netUserInfo.is_master = true;
			if (base.ArenaTrigger_Endless != null)
			{
				base.ArenaTrigger_Endless.SetWaveNum(MultiMonsterWave);
			}
			Enemy enemy = null;
			IDictionaryEnumerator enumerator = enemyList.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					enemy = (Enemy)((DictionaryEntry)enumerator.Current).Value;
					enemy.SetTargetWithMultiplayer();
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
			Debug.Log("OnMasterChange!!!");
		}

		public void OnUserDoRebirth(Packet packet)
		{
			GCUserDoRebirthPacket gCUserDoRebirthPacket = new GCUserDoRebirthPacket();
			if (gCUserDoRebirthPacket.ParserPacket(packet))
			{
				Debug.Log("OnUserDoRebirth..." + gCUserDoRebirthPacket.rebirth_user_id);
				if (gCUserDoRebirthPacket.m_iResult == 0)
				{
					player.m_life_packet_count--;
					GameUIScript.GetGameUIScript().SetLifePacketCount();
				}
				else
				{
					player.m_life_packet_count_temp++;
				}
			}
		}

		public void OnUserDoRebirthNotity(Packet packet)
		{
			Debug.Log("OnUserDoRebirthNotity...");
			GCUserDoRebirthNotifyPacket gCUserDoRebirthNotifyPacket = new GCUserDoRebirthNotifyPacket();
			if (!gCUserDoRebirthNotifyPacket.ParserPacket(packet))
			{
				return;
			}
			for (int i = 0; i < 4; i++)
			{
				if (net_com.netUserInfo_array[i] != null && net_com.netUserInfo_array[i].multiplayer != null && net_com.netUserInfo_array[i].user_id == gCUserDoRebirthNotifyPacket.action_user_id)
				{
					net_com.netUserInfo_array[i].multiplayer.m_life_packet_count--;
					net_com.netUserInfo_array[i].multiplayer.m_life_packet_count_temp--;
					break;
				}
			}
			if (gCUserDoRebirthNotifyPacket.rebirth_user_id == player.m_multi_id)
			{
				player.OnRebirth();
				return;
			}
			for (int j = 0; j < 4; j++)
			{
				if (net_com.netUserInfo_array[j] != null && net_com.netUserInfo_array[j].multiplayer != null && net_com.netUserInfo_array[j].user_id == gCUserDoRebirthNotifyPacket.rebirth_user_id)
				{
					net_com.netUserInfo_array[j].multiplayer.OnRebirth();
					break;
				}
			}
		}

		public void OnUserReportDataNotity(Packet packet)
		{
			GCUserDataReportNotifyPacket gCUserDataReportNotifyPacket = new GCUserDataReportNotifyPacket();
			if (!gCUserDataReportNotifyPacket.ParserPacket(packet))
			{
				return;
			}
			bool flag = false;
			foreach (UserReportData user_report_datum in user_report_data)
			{
				if (user_report_datum.user_id == gCUserDataReportNotifyPacket.m_iUserId)
				{
					user_report_datum.SetNetUserReportData((int)gCUserDataReportNotifyPacket.mKill_count, (int)gCUserDataReportNotifyPacket.mDeath_count, (int)gCUserDataReportNotifyPacket.mCash_loot, (float)gCUserDataReportNotifyPacket.mDamage_val / 1000f);
					flag = true;
					break;
				}
			}
			if (flag)
			{
				return;
			}
			UserReportData userReportData = new UserReportData();
			userReportData.user_id = (int)gCUserDataReportNotifyPacket.m_iUserId;
			for (int i = 0; i < 4; i++)
			{
				if (net_com.netUserInfo_array[i] != null && net_com.netUserInfo_array[i].multiplayer != null && userReportData.user_id == net_com.netUserInfo_array[i].user_id)
				{
					userReportData.name = net_com.netUserInfo_array[i].nick_name;
					break;
				}
			}
			userReportData.SetNetUserReportData((int)gCUserDataReportNotifyPacket.mKill_count, (int)gCUserDataReportNotifyPacket.mDeath_count, (int)gCUserDataReportNotifyPacket.mCash_loot, (float)gCUserDataReportNotifyPacket.mDamage_val / 1000f);
			user_report_data.Add(userReportData);
		}

		public void OnUserRealDead(Packet packet)
		{
			GCGameOverNotifyPacket gCGameOverNotifyPacket = new GCGameOverNotifyPacket();
			if (!gCGameOverNotifyPacket.ParserPacket(packet))
			{
				return;
			}
			Debug.Log("OnUserRealDead...");
			foreach (Player item in m_player_set)
			{
				if (item.m_multi_id == gCGameOverNotifyPacket.m_iUserId)
				{
					item.PlayerRealDead();
					break;
				}
			}
		}

		public void OnSomeoneLeave(int user_id)
		{
			if (!is_game_excute)
			{
				return;
			}
			Debug.Log("Mutiplayer leave room : " + user_id);
			if ((bool)GameUIScript.GetGameUIScript())
			{
				GameUIScript.GetGameUIScript().RemoveMultiHpBar(user_id);
			}
			game_tui.RemoveMultiplayerMarkWithID(user_id);
			Player player = null;
			foreach (Player item in m_multi_player_arr)
			{
				if (item.m_multi_id == user_id)
				{
					player = item;
					break;
				}
			}
			if (player != null)
			{
				m_multi_player_arr.Remove(player);
				m_player_set.Remove(player);
			}
			else
			{
				GameObject gameObject = GameObject.Find("Multiplayer" + user_id);
				if (gameObject != null)
				{
					player = gameObject.GetComponent<PlayerShell>().m_player;
				}
			}
			if (player != null)
			{
				Enemy enemy = null;
				IDictionaryEnumerator enumerator2 = enemyList.GetEnumerator();
				try
				{
					while (enumerator2.MoveNext())
					{
						enemy = (Enemy)((DictionaryEntry)enumerator2.Current).Value;
						if (enemy.TargetPlayer != null && enemy.TargetPlayer.m_multi_id == user_id)
						{
							enemy.SetPlayer(null);
							enemy.SetState(Enemy.IDLE_STATE);
						}
						enemy.SetTargetWithMultiplayer();
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
				player.is_real_dead = true;
				UnityEngine.Object.Destroy(player.PlayerObject);
				player = null;
			}
			OnMultiPlayerDead(null);
			CheckMultiGameOver();
		}

		public void ResetEnemyTarget()
		{
			if (m_multi_player_arr.Count > 1)
			{
				return;
			}
			Enemy enemy = null;
			IDictionaryEnumerator enumerator = enemyList.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					enemy = (Enemy)((DictionaryEntry)enumerator.Current).Value;
					enemy.SetTargetWithMultiplayer();
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

		public void OnLeaveRoom()
		{
			Debug.Log("Scnen OnLeaveRoom...");
		}

		public override void OnMultiPlayerDead(Player mPlayer)
		{
			m_multi_player_arr.Remove(mPlayer);
		}

		public void GameOverDataOp()
		{
			if (is_game_excute)
			{
				Debug.Log("Scnen OnGameOver...");
				GameApp.GetInstance().GetGameState().Achievement.LoseGame();
				camera.GetComponent<AudioSource>().Stop();
				camera.loseAudio.Play();
				camera.loseAudio.mute = !GameApp.GetInstance().GetGameState().SoundOn;
				GameObject gameObject = UnityEngine.Object.Instantiate(GameApp.GetInstance().GetGameResourceConfig().Multi_ReportData, Vector3.zero, Quaternion.identity);
				UnityEngine.Object.DontDestroyOnLoad(gameObject);
				gameObject.name = "MultiReportData";
				MultiReportData component = gameObject.GetComponent<MultiReportData>();
				component.SetEnemyDeathType(multi_enemy_death_type_count);
				component.SetEliteEnemyDeathType(multi_elite_enemy_death_type_count);
				component.SetComboEnemyDeathType(multi_combo_enemy_death_type_count);
				component.multi_enemy_death_elite_count = multi_enemy_death_elite_count;
				component.multi_enemy_death_normal_count = multi_enemy_death_normal_count;
				component.multi_enemy_death_elite_count_now = multi_enemy_death_elite_count_now;
				component.multi_enemy_death_normal_count_now = multi_enemy_death_normal_count_now;
				component.player_injured_val = player_injured_val;
				component.multi_god_time = multi_god_time;
				component.loot_item = loot_item;
				component.SetWeaponDeathType(multi_enemy_death_weapontype_count);
				component.play_time = multi_survive_time;
				component.loot_money = GameApp.GetInstance().GetGameState().loot_cash;
				component.bullet_consume = GameApp.GetInstance().GetGameState().bullet_comsume;
				UserReportData userReportData = new UserReportData();
				userReportData.user_id = net_com.m_netUserInfo.user_id;
				userReportData.name = net_com.m_netUserInfo.nick_name;
				userReportData.SetNetUserReportData(multi_enemy_death_elite_count_now + multi_enemy_death_normal_count_now, player.m_death_count, GameApp.GetInstance().GetGameState().loot_cash, player_injured_val);
				user_report_data.Add(userReportData);
				while (user_report_data.Count > 0)
				{
					UserReportData item = FindMaxOneReportData();
					user_report_data.Remove(item);
					component.user_report_data.Add(item);
				}
				GameApp.GetInstance().GetGameState().m_rescue_packet_count = player.m_life_packet_count;
			}
		}

		public void OnGameOver(object param, object attach, bool bFinish)
		{
			GameOverDataOp();
			is_game_excute = false;
			Packet packet = CGLeaveRoomPacket.MakePacket();
			GameApp.GetInstance().GetGameState().net_com.Send(packet);
			net_com.reverse_heart_timeout_delegate = null;
			net_com.reverse_heart_renew_delegate = null;
			net_com.reverse_heart_waiting_delegate = null;
			net_com.close_delegate = null;
			net_com.contecting_lost = null;
			net_com.packet_delegate = null;
			net_com.someone_birth_delegate = null;
			net_com.leave_room_notity_delegate = null;
			net_com.leave_room_delegate = null;
			GameApp.GetInstance().GetGameScene().GetCamera()
				.loseAudio.mute = true;
			SceneName.LoadLevel("MultiReportTUI");
		}

		public UserReportData FindMaxOneReportData()
		{
			int num = -9999;
			int num2 = 0;
			UserReportData result = null;
			foreach (UserReportData user_report_datum in user_report_data)
			{
				num2 = user_report_datum.mKill_count - user_report_datum.mDeath_count * 5 + (int)user_report_datum.mDamage_val + user_report_datum.mCash_loot;
				if (num2 > num)
				{
					result = user_report_datum;
					num = num2;
				}
			}
			return result;
		}

		public override void CheckMultiGameOver()
		{
			if (is_game_over)
			{
				return;
			}
			int num = 0;
			foreach (Player item in m_player_set)
			{
				if (item.is_real_dead && item.GetPlayerState() == item.DEAD_STATE)
				{
					num++;
				}
			}
			if ((num == m_player_set.Count) ? true : false)
			{
				is_game_over = true;
				GameUIScript component = GameObject.Find("SceneGUI").GetComponent<GameUIScript>();
				component.GetPanel(1).Show();
				GameApp.GetInstance().GetGameScene().GetCamera()
					.GetComponent<AudioSource>()
					.Stop();
				GameApp.GetInstance().GetGameScene().GetCamera()
					.loseAudio.Play();
				GameApp.GetInstance().GetGameScene().GetCamera()
					.loseAudio.mute = !GameApp.GetInstance().GetGameState().SoundOn;
				TimeGameOver(5f);
			}
		}

		public override void TimeGameOver(float time)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load("Prefabs/GameSceneMono")) as GameObject;
			gameObject.GetComponent<GameSceneMono>().TimerTask("GameOver", time);
		}

		public int GetMonsterLootCash(EnemyType enemyType)
		{
			int result = 0;
			GameConfig gameConfig = GameApp.GetInstance().GetGameConfig();
			string text = string.Empty;
			switch (enemyType)
			{
			case EnemyType.E_ZOMBIE:
				text = "Zombie";
				break;
			case EnemyType.E_TANK:
				text = "Tank";
				break;
			case EnemyType.E_SWAT:
				text = "Swat";
				break;
			case EnemyType.E_POLICE:
				text = "Police";
				break;
			case EnemyType.E_NURSE:
				text = "Nurse";
				break;
			case EnemyType.E_HUNTER:
				text = "Hunter";
				break;
			case EnemyType.E_DOG:
				text = "Dog";
				break;
			case EnemyType.E_BOOMER:
				text = "Boomer";
				break;
			}
			if (text != string.Empty)
			{
				result = gameConfig.GetMonsterConfig(text).lootCash;
			}
			return result;
		}

		public override void OnReverseHearTimeout()
		{
			GameOverDataOp();
			is_game_excute = false;
			if ((bool)net_com)
			{
				UnityEngine.Object.Destroy(net_com.gameObject);
			}
			Time.timeScale = 0f;
		}
	}
}
