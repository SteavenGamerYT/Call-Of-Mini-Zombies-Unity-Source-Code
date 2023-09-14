namespace Zombie3D
{
	public class VSPlayerReport
	{
		public string nick_name;

		public int kill_cout;

		public int death_count;

		public int loot_cash;

		public int combo_kill;

		public bool isMyself;

		public void Init(string name, bool myself)
		{
			nick_name = name;
			kill_cout = 0;
			death_count = 0;
			loot_cash = 0;
			combo_kill = 0;
			isMyself = myself;
		}

		public static VSPlayerReport CreateReport(string name, bool myself)
		{
			VSPlayerReport vSPlayerReport = new VSPlayerReport();
			vSPlayerReport.Init(name, myself);
			return vSPlayerReport;
		}
	}
}
