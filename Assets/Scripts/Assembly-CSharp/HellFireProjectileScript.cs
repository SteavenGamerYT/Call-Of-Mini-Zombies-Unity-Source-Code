using UnityEngine;
using Zombie3D;

public class HellFireProjectileScript : MonoBehaviour
{
	protected GameScene gameScene;

	protected Player player;

	protected Weapon weapon;

	protected float last_damage_time;

	public void Start()
	{
		gameScene = GameApp.GetInstance().GetGameScene();
	}

	public void SetPlayer(Player p)
	{
		player = p;
	}

	private bool CouldInjured()
	{
		FireGun fireGun = weapon as FireGun;
		if (fireGun == null)
		{
			return false;
		}
		if (Time.time - last_damage_time > weapon.AttackFrequency)
		{
			last_damage_time = Time.time;
			return true;
		}
		return false;
	}

	private void OnParticleCollision(GameObject other)
	{
		weapon = this.player.GetWeapon();
		if (other.gameObject.layer == 9)
		{
			Enemy enemyByID = gameScene.GetEnemyByID(other.name);
			if (enemyByID != null && enemyByID.GetState() != Enemy.DEAD_STATE && Time.time - enemyByID.lastFireDamagedTime > weapon.AttackFrequency)
			{
				DamageProperty damageProperty = new DamageProperty();
				damageProperty.damage = this.player.GetWeapon().Damage;
				enemyByID.OnHit(damageProperty, WeaponType.FireGun, false, this.player);
				enemyByID.lastFireDamagedTime = Time.time;
			}
		}
		else if (other.gameObject.layer == 19)
		{
			WoodBoxScript component = other.gameObject.GetComponent<WoodBoxScript>();
			component.OnHit(this.player.GetWeapon().Damage);
		}
		else
		{
			if (other.gameObject.layer != 8 || !GameApp.GetInstance().GetGameState().VS_mode || this.player.PlayerObject == other.gameObject || !CouldInjured())
			{
				return;
			}
			Player player = other.gameObject.GetComponent<PlayerShell>().m_player;
			if (player != null && player.GetPlayerState() != player.DEAD_STATE)
			{
				float damage = this.player.GetWeapon().Damage;
				if (damage > 0f)
				{
					Debug.Log("hit player:" + damage * this.player.PowerBuff);
					player.OnVsInjured(this.player.sfs_user, damage * this.player.PowerBuff, 11);
				}
			}
		}
	}
}
