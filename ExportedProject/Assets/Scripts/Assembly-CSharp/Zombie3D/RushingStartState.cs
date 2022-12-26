using UnityEngine;

namespace Zombie3D
{
	public class RushingStartState : EnemyState
	{
		public override void NextState(Enemy enemy, float deltaTime, Player player)
		{
			if (enemy.HP <= 0f)
			{
				enemy.OnDead();
				enemy.SetState(Enemy.DEAD_STATE);
				return;
			}
			Tank tank = enemy as Tank;
			if (tank != null)
			{
				tank.Audio.PlayAudio("Shout");
				if (tank.IsAnimationPlayedPercentage("Rush01", 1f))
				{
					tank.SetState(Tank.RUSHING_STATE);
					tank.Animate("Rush02", WrapMode.Loop);
				}
			}
		}

		public override void OnHit(Enemy enemy)
		{
		}
	}
}
