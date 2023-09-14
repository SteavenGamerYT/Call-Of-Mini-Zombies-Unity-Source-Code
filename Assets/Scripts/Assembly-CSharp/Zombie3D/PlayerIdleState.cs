using UnityEngine;

namespace Zombie3D
{
	public class PlayerIdleState : PlayerState
	{
		public PlayerIdleState()
		{
			state_type = PlayerStateType.Idle;
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
			player.Animate("Idle01" + player.WeaponNameEnd, WrapMode.Loop);
			player.Move(inputInfo.moveDirection * (deltaTime * player.WalkSpeed));
			if (inputInfo.fire && !inputInfo.IsMoving())
			{
				player.SetState(player.SHOOT_STATE);
			}
			else if (inputInfo.fire && inputInfo.IsMoving())
			{
				player.SetState(player.RUNSHOOT_STATE);
			}
			else if (!inputInfo.fire && inputInfo.IsMoving())
			{
				player.SetState(player.RUN_STATE);
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
