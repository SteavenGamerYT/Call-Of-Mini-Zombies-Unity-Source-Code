public class UserReportData
{
	public int user_id;

	public string name;

	public int mKill_count;

	public int mDeath_count;

	public int mCash_loot;

	public float mDamage_val;

	public void SetNetUserReportData(int Kill_count, int Death_count, int Cash_loot, float Damage_val)
	{
		mKill_count = Kill_count;
		mDeath_count = Death_count;
		mCash_loot = Cash_loot;
		mDamage_val = Damage_val;
	}
}
