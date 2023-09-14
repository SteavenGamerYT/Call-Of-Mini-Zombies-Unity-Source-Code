namespace Zombie3D
{
	public class KillAllQuest : Quest
	{
		protected int enemyLeft;

		public override void Init()
		{
			base.Init();
			questType = QuestType.KillAll;
		}

		public override void DoLogic()
		{
			base.DoLogic();
			enemyLeft = gameScene.EnemyNum;
			if (enemyLeft == 0)
			{
				questCompleted = true;
			}
		}

		public override string GetQuestInfo()
		{
			string empty = string.Empty;
			empty = ((enemyLeft < 10) ? enemyLeft.ToString() : "???");
			string result = "Mission: Kill Them All  " + empty;
			if (questCompleted)
			{
				result = "Mission Complete";
			}
			return result;
		}
	}
}
