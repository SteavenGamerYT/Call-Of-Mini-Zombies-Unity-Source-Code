using UnityEngine;

namespace Zombie3D
{
	public class SurvivalQuest : Quest
	{
		protected int enemyKilled;

		protected float survivalTime;

		protected float startedTime;

		protected int timeSurvive;

		public override void Init()
		{
			base.Init();
			questType = QuestType.KillAll;
			startedTime = Time.time;
		}

		public override void DoLogic()
		{
			base.DoLogic();
		}

		public override string GetQuestInfo()
		{
			return string.Format("{0:00}", timeSurvive / 60) + ":" + string.Format("{0:00}", timeSurvive % 60);
		}
	}
}
