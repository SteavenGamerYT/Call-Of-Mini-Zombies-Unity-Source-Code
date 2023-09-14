using UnityEngine;

namespace Zombie3D
{
	public class MultiplayerRunAndShootState : PlayerState
	{
		public MultiplayerRunAndShootState()
		{
			state_type = PlayerStateType.RunShoot;
		}

		public override void DoStateLogic(Player player, float deltaTime)
		{
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
			if (weapon == null || weapon.GetWeaponType() == WeaponType.Sniper)
			{
				return;
			}
			if (weapon.GetWeaponType() == WeaponType.Sword)
			{
				if (weapon.CouldMakeNextShoot())
				{
					player.Fire(deltaTime);
				}
				SwordEffectTrail swordEffectTrail = weapon.gun.GetComponent("SwordEffectTrail") as SwordEffectTrail;
				if (player.IsPlayingAnimation("RunShoot01" + player.WeaponNameEnd))
				{
					if (player.IsAnimationPlayedPercentage("RunShoot01" + player.WeaponNameEnd, 1f))
					{
						if (null != swordEffectTrail)
						{
							swordEffectTrail.ShowTrail(true);
						}
						weapon.AutoAim(0f);
						player.RandomSwordAnimation();
						player.PlayAnimate("RunShoot01" + player.WeaponNameEnd, WrapMode.ClampForever);
					}
				}
				else
				{
					if (null != swordEffectTrail)
					{
						swordEffectTrail.ShowTrail(true);
					}
					weapon.AutoAim(0f);
					player.RandomSwordAnimation();
					player.PlayAnimate("RunShoot01" + player.WeaponNameEnd, WrapMode.ClampForever);
				}
				return;
			}
			weapon.FireUpdate(deltaTime);
			if (weapon.CouldMakeNextShoot())
			{
				if (weapon.GetWeaponType() == WeaponType.AssaultRifle)
				{
					weapon.AutoAim(deltaTime);
					player.Fire(deltaTime);
				}
				else
				{
					player.Fire(deltaTime);
				}
				switch (weapon.GetWeaponType())
				{
				case WeaponType.RocketLauncher:
					player.Animate("RunShoot01" + player.WeaponNameEnd, WrapMode.Once);
					break;
				case WeaponType.ShotGun:
				case WeaponType.M32:
					player.Animate("RunShoot01" + player.WeaponNameEnd, WrapMode.ClampForever);
					break;
				case WeaponType.Saw:
					player.RandomSawAnimation();
					player.Animate("RunShoot01" + player.WeaponNameEnd, WrapMode.Loop);
					break;
				default:
					player.Animate("RunShoot01" + player.WeaponNameEnd, WrapMode.Loop);
					break;
				}
			}
		}
	}
}
