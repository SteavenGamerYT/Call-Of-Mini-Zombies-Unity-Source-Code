namespace Zombie3D
{
	public class PlayerDeadState : PlayerState
	{
		public PlayerDeadState()
		{
			state_type = PlayerStateType.Dead;
		}

		public override void DoStateLogic(Player player, float deltaTime)
		{
			player.ZoomOut(deltaTime);
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

		public override void OnHit(Player player, float damage)
		{
		}
	}
}
