using UnityEngine;

namespace Zombie3D
{
	public class PreyGoneState : EnemyState
	{
		public override void NextState(Enemy enemy, float deltaTime, Player player)
		{
			if (enemy.HP <= 0f)
			{
				enemy.OnDead();
				enemy.SetState(Enemy.DEAD_STATE);
			}
			enemy.Animate("Idle01", WrapMode.Loop);
			if (enemy.MoveToMucilage(deltaTime))
			{
				enemy.SetState(Enemy.IDLE_STATE);
				enemy.SetInPreyGone(false);
			}
		}
	}
}
