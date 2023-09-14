using UnityEngine;

namespace Zombie3D
{
	public class MultiplayerShootState : PlayerState
	{
		public MultiplayerShootState()
		{
			state_type = PlayerStateType.Shoot;
		}

		public override void DoStateLogic(Player player, float deltaTime)
		{
			Weapon weapon = player.GetWeapon();
			if (weapon == null)
			{
				return;
			}
			if (weapon.GetWeaponType() == WeaponType.Sniper)
			{
				MultiSniper multiSniper = weapon as MultiSniper;
				if (multiSniper.AimedTarget())
				{
					player.Fire(deltaTime);
				}
			}
			else if (weapon.GetWeaponType() == WeaponType.Sword)
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
						player.PlayAnimate("RunShoot01" + player.WeaponNameEnd, WrapMode.Once);
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
					player.PlayAnimate("RunShoot01" + player.WeaponNameEnd, WrapMode.Once);
				}
			}
			else if (!weapon.CouldMakeNextShoot())
			{
				weapon.FireUpdate(deltaTime);
			}
			else
			{
				if (weapon.GetWeaponType() == WeaponType.AssaultRifle || weapon.GetWeaponType() == WeaponType.MachineGun)
				{
					weapon.AutoAim(deltaTime);
					weapon.FireUpdate(deltaTime);
					player.Fire(deltaTime);
				}
				else
				{
					weapon.FireUpdate(deltaTime);
					player.Fire(deltaTime);
				}
				switch (weapon.GetWeaponType())
				{
				case WeaponType.ShotGun:
				case WeaponType.RocketLauncher:
				case WeaponType.M32:
					player.Animate("Shoot01" + player.WeaponNameEnd, WrapMode.Once);
					break;
				case WeaponType.Saw:
					player.RandomSawAnimation();
					player.Animate("RunShoot01" + player.WeaponNameEnd, WrapMode.Loop);
					break;
				default:
					player.Animate("Shoot01" + player.WeaponNameEnd, WrapMode.Loop);
					break;
				}
			}
		}
	}
}
