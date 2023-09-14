namespace Zombie3D
{
	public class MultiplayerGotHitState : PlayerState
	{
		public MultiplayerGotHitState()
		{
			state_type = PlayerStateType.GotHit;
		}

		public override void DoStateLogic(Player player, float deltaTime)
		{
			if (!player.IsPlayingAnimation("Damage01"))
			{
				player.SetState(player.IDLE_STATE);
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
