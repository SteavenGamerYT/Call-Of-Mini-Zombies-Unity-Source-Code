using System.Collections.Generic;
using UnityEngine;

namespace Zombie3D
{
	public class UserStatistics
	{
		public const int Iap_count = 6;

		public int cash_cur;

		public int cash_loot;

		public int coop_count;

		public float play_time;

		public int achivment_count;

		public int cash_iap;

		public int day_count;

		public int application_count;

		public string last_scene = string.Empty;

		public int cash_spend;

		public int cash_spend_weapons;

		public int cash_spend_bullets;

		public string last_day_state = "null";

		public List<string> consume_list = new List<string>();

		public List<int> weapon_use_count = new List<int>();

		public List<int> avatar_use_count = new List<int>();

		public int[] Iap_buy_count = new int[6];

		public List<string> weapon_owner_list = new List<string>();

		public List<string> weapon_up_list = new List<string>();

		public List<string> bullets_list = new List<string>();

		public List<string> mission_failed_list = new List<string>();

		public List<string> mission_quit_list = new List<string>();

		public List<string> scene_enter_data = new List<string>();

		public void SetSingleWeaponUseCount(int index, int count)
		{
			if (index < weapon_use_count.Count)
			{
				weapon_use_count[index] = count;
			}
			else
			{
				Debug.Log("SetSingleWeaponUseCount Error!");
			}
		}
	}
}
