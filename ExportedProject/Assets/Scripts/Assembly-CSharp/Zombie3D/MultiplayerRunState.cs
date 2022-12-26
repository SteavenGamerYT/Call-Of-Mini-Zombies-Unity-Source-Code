using UnityEngine;

namespace Zombie3D
{
	public class MultiplayerRunState : PlayerState
	{
		public MultiplayerRunState()
		{
			state_type = PlayerStateType.Run;
		}

		public override void DoStateLogic(Player player, float deltaTime)
		{
			player.StopFire();
			player.ResetSawAnimation();
			player.Animate("Run01" + player.WeaponNameEnd, WrapMode.Loop);
			Multiplayer multiplayer = player as Multiplayer;
			if (multiplayer.m_is_lerp_position)
			{
				player.Move(player.m_direction * (Mathf.Lerp(deltaTime + player.net_ping_sum, deltaTime, deltaTime) * player.WalkSpeed));
			}
			else
			{
				player.Move(player.m_direction * deltaTime * player.WalkSpeed);
			}
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
