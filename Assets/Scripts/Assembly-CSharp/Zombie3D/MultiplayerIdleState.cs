using UnityEngine;

namespace Zombie3D
{
	public class MultiplayerIdleState : PlayerState
	{
		public MultiplayerIdleState()
		{
			state_type = PlayerStateType.Idle;
		}

		public override void DoStateLogic(Player player, float deltaTime)
		{
			player.StopFire();
			player.ResetSawAnimation();
			player.Animate("Idle01" + player.WeaponNameEnd, WrapMode.Loop);
			player.Move(player.m_direction * (deltaTime * player.WalkSpeed));
			Weapon weapon = player.GetWeapon();
			if (weapon.GetWeaponType() == WeaponType.Sword)
			{
				SwordEffectTrail swordEffectTrail = weapon.gun.GetComponent("SwordEffectTrail") as SwordEffectTrail;
				if (null != swordEffectTrail)
				{
					swordEffectTrail.ShowTrail(false);
				}
			}
		}
	}
}
