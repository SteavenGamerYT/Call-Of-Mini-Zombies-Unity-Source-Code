using UnityEngine;

namespace Zombie3D
{
	public class JumpState : EnemyState
	{
		public override void NextState(Enemy enemy, float deltaTime, Player player)
		{
			if (enemy.HP <= 0f)
			{
				enemy.OnDead();
				enemy.SetState(Enemy.DEAD_STATE);
				return;
			}
			Hunter hunter = enemy as Hunter;
			if (hunter != null)
			{
				if (hunter.Jump(deltaTime))
				{
					hunter.SetState(Enemy.IDLE_STATE);
				}
				else if (!hunter.JumpEnded)
				{
					hunter.Animate("JumpIdle01", WrapMode.Loop);
				}
			}
		}

		public override void OnHit(Enemy enemy)
		{
		}
	}
}
