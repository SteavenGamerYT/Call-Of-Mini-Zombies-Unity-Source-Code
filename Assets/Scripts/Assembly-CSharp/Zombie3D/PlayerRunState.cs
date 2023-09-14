using UnityEngine;

namespace Zombie3D
{
	public class PlayerRunState : PlayerState
	{
		public PlayerRunState()
		{
			state_type = PlayerStateType.Run;
		}

		public override void DoStateLogic(Player player, float deltaTime)
		{
			InputController inputController = player.InputController;
			InputInfo inputInfo = new InputInfo();
			inputController.ProcessInput(deltaTime, inputInfo);
			if (!inputInfo.fire)
			{
				player.ZoomOut(deltaTime);
			}
			player.ResetSawAnimation();
			player.Animate("Run01" + player.WeaponNameEnd, WrapMode.Loop);
			player.Move(inputInfo.moveDirection * (deltaTime * player.WalkSpeed));
			Weapon weapon = player.GetWeapon();
			if (!inputInfo.fire && !inputInfo.IsMoving())
			{
				player.SetState(player.IDLE_STATE);
			}
			else if (inputInfo.fire && inputInfo.IsMoving())
			{
				if (weapon.HaveBullets())
				{
					player.SetState(player.RUNSHOOT_STATE);
				}
			}
			else if (inputInfo.fire && !inputInfo.IsMoving())
			{
				player.SetState(player.SHOOT_STATE);
			}
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
