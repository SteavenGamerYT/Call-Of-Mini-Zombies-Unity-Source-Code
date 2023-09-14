using UnityEngine;

namespace Zombie3D
{
	public class PlayerRunAndShootState : PlayerState
	{
		public PlayerRunAndShootState()
		{
			state_type = PlayerStateType.RunShoot;
		}

		public override void DoStateLogic(Player player, float deltaTime)
		{
			InputController inputController = player.InputController;
			InputInfo inputInfo = new InputInfo();
			inputController.ProcessInput(deltaTime, inputInfo);
			player.ZoomIn(deltaTime);
			player.Move(inputInfo.moveDirection * (deltaTime * player.WalkSpeed * 0.8f));
			Weapon weapon = player.GetWeapon();
			if (weapon != null)
			{
				if (weapon.GetWeaponType() == WeaponType.Sniper)
				{
					Sniper sniper = weapon as Sniper;
					if (!sniper.HaveBullets())
					{
						player.SetState(player.RUN_STATE);
					}
					else if (inputInfo.fire)
					{
						sniper.AutoAim(deltaTime);
						player.Animate("Run01" + player.WeaponNameEnd, WrapMode.Loop);
					}
					else if (sniper.AimedTarget())
					{
						player.Fire(deltaTime);
						player.Animate("RunShoot01" + player.WeaponNameEnd, WrapMode.Once);
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
					}
					else if (null != swordEffectTrail)
					{
						swordEffectTrail.ShowTrail(false);
					}
				}
				else
				{
					weapon.FireUpdate(deltaTime);
					if (inputInfo.fire)
					{
						if (!weapon.HaveBullets())
						{
							player.SetState(player.RUN_STATE);
						}
						else if (weapon.CouldMakeNextShoot())
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
			bool flag = player.IsPlayingAnimation("RunShoot01" + player.WeaponNameEnd);
			bool flag2 = player.AnimationEnds("RunShoot01" + player.WeaponNameEnd);
			if ((flag && flag2) || !flag)
			{
				if (!inputInfo.fire && !inputInfo.IsMoving())
				{
					player.StopFire();
					player.SetState(player.IDLE_STATE);
				}
				else if (!inputInfo.fire && inputInfo.IsMoving())
				{
					player.StopFire();
					player.SetState(player.RUN_STATE);
				}
				else if (inputInfo.fire && !inputInfo.IsMoving())
				{
					player.SetState(player.SHOOT_STATE);
				}
				else if (weapon.GetWeaponType() == WeaponType.RocketLauncher)
				{
					player.Animate("Run01" + player.WeaponNameEnd, WrapMode.Loop);
				}
				else if (weapon.GetWeaponType() == WeaponType.ShotGun || weapon.GetWeaponType() == WeaponType.M32)
				{
					player.Animate("Run02" + player.WeaponNameEnd, WrapMode.Loop);
				}
				else if (weapon.GetWeaponType() == WeaponType.AssaultRifle || weapon.GetWeaponType() == WeaponType.MachineGun || weapon.GetWeaponType() == WeaponType.LaserGun || weapon.GetWeaponType() == WeaponType.FireGun)
				{
					player.Animate("RunShoot01" + player.WeaponNameEnd, WrapMode.Loop);
				}
			}
		}
	}
}
