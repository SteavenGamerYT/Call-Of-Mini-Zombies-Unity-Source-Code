using UnityEngine;

namespace Zombie3D
{
	public class AttackState : EnemyState
	{
		public override void NextState(Enemy enemy, float deltaTime, Player player)
		{
			if (enemy.HP <= 0f)
			{
				enemy.OnDead();
				enemy.SetState(Enemy.DEAD_STATE);
				return;
			}
			if (enemy.CouldMakeNextAttack())
			{
				enemy.OnAttack();
			}
			else if (enemy.AttackAnimationEnds())
			{
				enemy.Animate("Idle01", WrapMode.Loop);
			}
			enemy.CheckHit();
			if (enemy.SqrDistanceFromPlayer >= enemy.AttackRange * enemy.AttackRange && enemy.AttackAnimationEnds())
			{
				enemy.SetState(Enemy.CATCHING_STATE);
			}
		}
	}
}
