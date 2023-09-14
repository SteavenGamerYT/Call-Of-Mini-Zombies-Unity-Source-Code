using System.Collections.Generic;
using Zombie3D;

public class MultiAchievementCfg
{
	public class MonsterTypeKill
	{
		public EnemyType type;

		public int count;
	}

	public class WeaponTypeKill
	{
		public WeaponType type;

		public int count;
	}

	public MultiAchievementType type;

	public int m_index = -1;

	public int finish;

	public string icon_bk;

	public string icon;

	public string name;

	public string content;

	public string m_class;

	public int level;

	public float battle_time;

	public float total_damage;

	public int total_kill;

	public WeaponTypeKill weapon_type_kill;

	public List<MonsterTypeKill> monster_type_kill = new List<MonsterTypeKill>();

	public int reward_cash;

	public AvatarType reward_avata = AvatarType.None;

	public string reward_weapon;

	public void SetTypeWith(string typeStr)
	{
		switch (typeStr)
		{
		case "Damage":
			type = MultiAchievementType.A_Damage;
			break;
		case "WeaponKill":
			type = MultiAchievementType.A_Weapon_Kill;
			break;
		case "ComboKill":
			type = MultiAchievementType.A_Combo_Kill;
			break;
		case "MultiKill":
			type = MultiAchievementType.A_Multi_Kill;
			break;
		case "TotalKill":
			type = MultiAchievementType.A_Total_Kill;
			break;
		case "EliteTotalKill":
			type = MultiAchievementType.A_Elite_Total_Kill;
			break;
		case "EliteTypeKill":
			type = MultiAchievementType.A_Elit_Type_Kill;
			break;
		case "TypeKill":
			type = MultiAchievementType.A_Type_Kill;
			break;
		case "Time":
			type = MultiAchievementType.A_Time;
			break;
		case "GodTime":
			type = MultiAchievementType.A_GodTime;
			break;
		}
	}
}
