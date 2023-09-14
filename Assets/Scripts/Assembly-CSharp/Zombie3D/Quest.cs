namespace Zombie3D
{
	public abstract class Quest
	{
		protected bool questCompleted;

		protected QuestType questType;

		protected GameScene gameScene;

		public bool QuestCompleted
		{
			get
			{
				return questCompleted;
			}
		}

		public QuestType GetQuestType()
		{
			return questType;
		}

		public virtual void Init()
		{
			questCompleted = false;
			gameScene = GameApp.GetInstance().GetGameScene();
		}

		public virtual void DoLogic()
		{
		}

		public abstract string GetQuestInfo();
	}
}
