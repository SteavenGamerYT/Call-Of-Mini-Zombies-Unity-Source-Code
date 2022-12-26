using UnityEngine;

namespace Zombie3D
{
	public abstract class PlayerState
	{
		protected PlayerStateType state_type;

		public PlayerStateType GetStateType()
		{
			return state_type;
		}

		public virtual void DoStateLogic(Player player, float deltaTime)
		{
		}

		public virtual void OnHit(Player player, float damage)
		{
			if (player.HP <= 0f)
			{
				player.StopFire();
				player.OnDead();
				player.SetState(player.DEAD_STATE);
			}
			else if (player.CouldGetAnotherHit())
			{
				player.CreateScreenBlood(damage);
				if (!GameApp.GetInstance().GetGameState().VS_mode && player.GetWeapon().GetWeaponType() != WeaponType.Saw)
				{
					player.Animate("Damage01", WrapMode.Once);
					player.StopFire();
					player.SetState(player.GOTHIT_STATE);
				}
			}
		}

		public virtual void MultiOnHit(Player player, float damage)
		{
			if (player.HP <= 0f)
			{
				player.StopFire();
				player.OnDead();
				player.SetState(player.DEAD_STATE);
			}
			else if (player.CouldGetAnotherHit() && player.GetWeapon().GetWeaponType() != WeaponType.Saw)
			{
				player.Animate("Damage01", WrapMode.Once);
				player.StopFire();
				player.SetState(player.GOTHIT_STATE);
			}
		}
	}
}
