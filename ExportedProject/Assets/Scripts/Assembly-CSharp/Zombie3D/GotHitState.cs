using UnityEngine;

namespace Zombie3D
{
	public class GotHitState : EnemyState
	{
		public override void NextState(Enemy enemy, float deltaTime, Player player)
		{
			if (enemy.HP <= 0f)
			{
				enemy.OnDead();
				enemy.SetState(Enemy.DEAD_STATE);
				return;
			}
			enemy.Animate("Damage", WrapMode.Once);
			if (enemy.GotHitAnimationEnds())
			{
				enemy.SetState(Enemy.IDLE_STATE);
			}
		}
	}
}
