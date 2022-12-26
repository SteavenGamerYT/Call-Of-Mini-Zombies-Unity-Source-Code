namespace Zombie3D
{
	public class PatrolState : EnemyState
	{
		public override void NextState(Enemy enemy, float deltaTime, Player player)
		{
			if (enemy.HP <= 0f)
			{
				enemy.OnDead();
				enemy.SetState(Enemy.DEAD_STATE);
			}
			else
			{
				enemy.Patrol(deltaTime);
			}
		}
	}
}
