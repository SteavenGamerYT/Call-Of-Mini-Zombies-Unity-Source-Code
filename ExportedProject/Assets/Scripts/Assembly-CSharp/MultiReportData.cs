using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zombie3D;

public class MultiReportData : MonoBehaviour
{
	public int loot_money;

	public float play_time;

	public int loot_item;

	public int bullet_consume;

	public int[] multi_enemy_death_type_count;

	public int[] multi_elite_enemy_death_type_count;

	public int[] multi_combo_enemy_death_type_count;

	public int multi_enemy_death_elite_count;

	public int multi_enemy_death_normal_count;

	public int multi_enemy_death_elite_count_now;

	public int multi_enemy_death_normal_count_now;

	public float player_injured_val;

	public int[] multi_enemy_death_weapontype_count;

	public float multi_god_time;

	public List<MultiAchievementCfg> Achievement_Finished_Array = new List<MultiAchievementCfg>();

	public List<UserReportData> user_report_data = new List<UserReportData>();

	public void SetEnemyDeathType(int[] array)
	{
		multi_enemy_death_type_count = new int[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			multi_enemy_death_type_count[i] = array[i];
		}
	}

	public void SetEliteEnemyDeathType(int[] array)
	{
		multi_elite_enemy_death_type_count = new int[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			multi_elite_enemy_death_type_count[i] = array[i];
		}
	}

	public void SetComboEnemyDeathType(int[] array)
	{
		multi_combo_enemy_death_type_count = new int[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			multi_combo_enemy_death_type_count[i] = array[i];
		}
	}

	public void SetWeaponDeathType(int[] array)
	{
		multi_enemy_death_weapontype_count = new int[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			multi_enemy_death_weapontype_count[i] = array[i];
		}
	}

	public void CheckMultiAchievementFinishStatus()
	{
		MultiAchievementCfg multiAchievementCfg = null;
		IEnumerator enumerator = GameApp.GetInstance().GetGameConfig().Multi_AchievementConfTable.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				MultiAchievementCfg multiAchievementCfg2 = (MultiAchievementCfg)enumerator.Current;
				multiAchievementCfg = null;
				bool flag = true;
				switch (multiAchievementCfg2.type)
				{
				case MultiAchievementType.A_Damage:
					if ((int)player_injured_val >= (int)multiAchievementCfg2.total_damage)
					{
						multiAchievementCfg = multiAchievementCfg2;
					}
					break;
				case MultiAchievementType.A_Weapon_Kill:
					if (multi_enemy_death_weapontype_count[(int)multiAchievementCfg2.weapon_type_kill.type] >= multiAchievementCfg2.weapon_type_kill.count)
					{
						multiAchievementCfg = multiAchievementCfg2;
					}
					break;
				case MultiAchievementType.A_Combo_Kill:
					foreach (MultiAchievementCfg.MonsterTypeKill item in multiAchievementCfg2.monster_type_kill)
					{
						if (multi_combo_enemy_death_type_count[(int)item.type] < item.count)
						{
							flag = false;
							break;
						}
					}
					if (flag)
					{
						multiAchievementCfg = multiAchievementCfg2;
					}
					break;
				case MultiAchievementType.A_Multi_Kill:
					foreach (MultiAchievementCfg.MonsterTypeKill item2 in multiAchievementCfg2.monster_type_kill)
					{
						if (multi_enemy_death_type_count[(int)item2.type] < item2.count)
						{
							flag = false;
							break;
						}
					}
					if (flag)
					{
						multiAchievementCfg = multiAchievementCfg2;
					}
					break;
				case MultiAchievementType.A_Total_Kill:
					if (multi_enemy_death_normal_count + multi_enemy_death_elite_count >= multiAchievementCfg2.total_kill)
					{
						multiAchievementCfg = multiAchievementCfg2;
					}
					break;
				case MultiAchievementType.A_Elite_Total_Kill:
					if (multi_enemy_death_elite_count >= multiAchievementCfg2.total_kill)
					{
						multiAchievementCfg = multiAchievementCfg2;
					}
					break;
				case MultiAchievementType.A_Elit_Type_Kill:
					if (multi_enemy_death_elite_count_now >= multiAchievementCfg2.total_kill)
					{
						multiAchievementCfg = multiAchievementCfg2;
					}
					break;
				case MultiAchievementType.A_Type_Kill:
					foreach (MultiAchievementCfg.MonsterTypeKill item3 in multiAchievementCfg2.monster_type_kill)
					{
						if (multi_enemy_death_type_count[(int)item3.type] < item3.count)
						{
							flag = false;
							break;
						}
					}
					if (flag)
					{
						multiAchievementCfg = multiAchievementCfg2;
					}
					break;
				case MultiAchievementType.A_Time:
					if (play_time >= multiAchievementCfg2.battle_time)
					{
						multiAchievementCfg = multiAchievementCfg2;
					}
					break;
				case MultiAchievementType.A_GodTime:
					if (multi_god_time >= multiAchievementCfg2.battle_time)
					{
						multiAchievementCfg = multiAchievementCfg2;
					}
					break;
				}
				if (multiAchievementCfg != null && GameApp.GetInstance().GetGameState().MultiAchievementData[multiAchievementCfg.m_index] == 0)
				{
					GameApp.GetInstance().GetGameState().MultiAchievementData[multiAchievementCfg.m_index] = 1;
					multiAchievementCfg.finish = 1;
					Achievement_Finished_Array.Add(multiAchievementCfg);
					GameApp.GetInstance().GetGameState().user_statistics.achivment_count++;
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
		GameApp.GetInstance().GetGameState().multi_enemy_death_elite_count = multi_enemy_death_elite_count;
		GameApp.GetInstance().GetGameState().multi_enemy_death_normal_count = multi_enemy_death_normal_count;
	}

	public void GiveReward()
	{
		if (Achievement_Finished_Array.Count == 0)
		{
			return;
		}
		List<string> list = new List<string>();
		foreach (MultiAchievementCfg item in Achievement_Finished_Array)
		{
			if (item.reward_cash > 0)
			{
				GameApp.GetInstance().GetGameState().AddCash(item.reward_cash);
			}
			if (item.reward_avata != AvatarType.None)
			{
				GameApp.GetInstance().GetGameState().UnlockAvatar(item.reward_avata);
			}
			if (item.reward_weapon != null)
			{
				list.Add(item.reward_weapon);
			}
		}
		List<Weapon> weapons = GameApp.GetInstance().GetGameState().GetWeapons();
		if (list.Count > 0)
		{
			foreach (string item2 in list)
			{
				foreach (Weapon item3 in weapons)
				{
					if (item3.Name == item2 && item3.Exist == WeaponExistState.Locked)
					{
						item3.Exist = WeaponExistState.Unlocked;
						GameApp.GetInstance().GetGameScene().BonusWeapon = item3;
					}
				}
			}
			return;
		}
		GameApp.GetInstance().GetGameScene().BonusWeapon = null;
	}
}
