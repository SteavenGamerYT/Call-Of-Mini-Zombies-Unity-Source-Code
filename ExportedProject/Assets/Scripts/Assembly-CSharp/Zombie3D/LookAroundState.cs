using UnityEngine;

namespace Zombie3D
{
	public class LookAroundState : EnemyState
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
			enemy.Animate("Idle01", WrapMode.Loop);
			enemy.GetTransform().LookAt(player.GetTransform());
			if (!hunter.LookAroundTimOut())
			{
				return;
			}
			if (hunter.ReadyForJump())
			{
				int num = Random.Range(0, 100);
				if (num < 100)
				{
					hunter.StartJump();
					hunter.SetState(Hunter.JUMP_STATE);
				}
				else
				{
					hunter.SetState(Enemy.CATCHING_STATE);
				}
			}
			else
			{
				hunter.SetState(Enemy.CATCHING_STATE);
			}
		}
	}
}
