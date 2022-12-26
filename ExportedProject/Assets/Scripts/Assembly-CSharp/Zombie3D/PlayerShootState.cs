using UnityEngine;

namespace Zombie3D
{
	public class PlayerShootState : PlayerState
	{
		public PlayerShootState()
		{
			state_type = PlayerStateType.Shoot;
		}

		public override void DoStateLogic(Player player, float deltaTime)
		{
			InputController inputController = player.InputController;
			InputInfo inputInfo = new InputInfo();
			inputController.ProcessInput(deltaTime, inputInfo);
			player.ZoomIn(deltaTime);
			Weapon weapon = player.GetWeapon();
			if (weapon != null)
			{
				if (weapon.GetWeaponType() == WeaponType.Sniper)
				{
					Sniper sniper = weapon as Sniper;
					if (!sniper.HaveBullets())
					{
						player.SetState(player.IDLE_STATE);
					}
					else if (inputInfo.fire)
					{
						player.AutoAim(deltaTime);
					}
					else if (sniper.AimedTarget())
					{
						player.Fire(deltaTime);
					}
				}
				else if (weapon.GetWeaponType() == WeaponType.Sword)
				{
					if (inputInfo.fire && weapon.CouldMakeNextShoot())
					{
						player.Fire(deltaTime);
					}
					SwordEffectTrail swordEffectTrail = weapon.gun.GetComponent("SwordEffectTrail") as SwordEffectTrail;
					if (inputInfo.fire)
					{
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
					else if (null != swordEffectTrail)
					{
						swordEffectTrail.ShowTrail(false);
					}
				}
				else if (!weapon.HaveBullets())
				{
					player.SetState(player.IDLE_STATE);
				}
				else if (!weapon.CouldMakeNextShoot())
				{
					weapon.FireUpdate(deltaTime);
				}
				else if (inputInfo.fire)
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
			bool flag = player.IsPlayingAnimation("Shoot01" + player.WeaponNameEnd);
			bool flag2 = player.AnimationEnds("Shoot01" + player.WeaponNameEnd);
			if ((!flag || !flag2) && flag)
			{
				return;
			}
			if (!inputInfo.fire && !inputInfo.IsMoving())
			{
				player.SetState(player.IDLE_STATE);
				player.StopFire();
				return;
			}
			if (inputInfo.fire && inputInfo.IsMoving())
			{
				player.SetState(player.RUNSHOOT_STATE);
				return;
			}
			if (!inputInfo.fire && inputInfo.IsMoving())
			{
				player.SetState(player.RUN_STATE);
				player.StopFire();
				return;
			}
			WeaponType weaponType = weapon.GetWeaponType();
			if (weaponType == WeaponType.AssaultRifle || weapon.GetWeaponType() == WeaponType.MachineGun || weapon.GetWeaponType() == WeaponType.LaserGun || weapon.GetWeaponType() == WeaponType.Saw || weapon.GetWeaponType() == WeaponType.FireGun)
			{
				player.Animate("Shoot01" + player.WeaponNameEnd, WrapMode.Loop);
			}
			else
			{
				player.Animate("Idle01" + player.WeaponNameEnd, WrapMode.Loop);
			}
		}
	}
}
