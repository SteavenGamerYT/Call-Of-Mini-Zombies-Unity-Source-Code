namespace Zombie3D
{
	public class BoomerExplodeState : EnemyState
	{
		public override void NextState(Enemy enemy, float deltaTime, Player player)
		{
			if (enemy.HP <= 0f)
			{
				enemy.OnDead();
				enemy.SetState(Enemy.DEAD_STATE);
				return;
			}
			Boomer boomer = enemy as Boomer;
			if (boomer.IsAnimationPlayedPercentage("Attack01", 1f))
			{
				boomer.OnAttack();
			}
		}
	}
}
