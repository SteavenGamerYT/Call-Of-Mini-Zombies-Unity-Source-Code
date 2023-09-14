using Zombie3D;

public class NetUserInfo
{
	public bool is_master;

	public string nick_name = string.Empty;

	public int cur_room_id = -1;

	public int user_id = -1;

	public float net_ping;

	public int cur_map_id = -1;

	public int room_index = -1;

	public int avatarType = -1;

	public int levelDays = -1;

	public Multiplayer multiplayer;

	public int mKill_count;

	public int mDeath_count;

	public int mCash_loot;

	public float mDamage_val;

	public bool is_submited;

	public void SetNetUserInfo(bool master, string nick, int roomid, int userid, float ping, int mapid, int roomindex, int avatar, int level)
	{
		is_master = master;
		nick_name = nick;
		cur_room_id = roomid;
		user_id = userid;
		net_ping = ping;
		cur_map_id = mapid;
		room_index = roomindex;
		avatarType = avatar;
		levelDays = level;
	}

	public void SetNetUserReportData(int Kill_count, int Death_count, int Cash_loot, float Damage_val)
	{
		mKill_count = Kill_count;
		mDeath_count = Death_count;
		mCash_loot = Cash_loot;
		mDamage_val = Damage_val;
	}
}
