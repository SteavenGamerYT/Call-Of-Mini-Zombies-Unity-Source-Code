namespace Zombie3D
{
	public class MultiplayerDeadState : PlayerState
	{
		public MultiplayerDeadState()
		{
			state_type = PlayerStateType.Dead;
		}

		public override void DoStateLogic(Player player, float deltaTime)
		{
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
