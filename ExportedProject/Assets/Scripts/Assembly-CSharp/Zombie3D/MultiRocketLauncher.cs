using UnityEngine;

namespace Zombie3D
{
	public class MultiRocketLauncher : Weapon
	{
		protected const float shootLastingTime = 0.5f;

		protected float rocketFlySpeed;

		protected static int sbulletCount;

		public override int BulletCount
		{
			get
			{
				return sbulletCount;
			}
			set
			{
				sbulletCount = value;
			}
		}

		public MultiRocketLauncher()
		{
			maxCapacity = 9999;
			base.IsSelectedForBattle = true;
		}

		public override WeaponType GetWeaponType()
		{
			return WeaponType.RocketLauncher;
		}

		public override void LoadConfig()
		{
			base.LoadConfig();
			WeaponConfig weaponConfig = gConfig.GetWeaponConfig(base.Name);
			base.damage = weaponConfig.damageConf.baseData;
			base.attackFrenquency = weaponConfig.attackRateConf.baseData;
			speedDrag = weaponConfig.moveSpeedDrag;
			range = weaponConfig.range;
			accuracy = weaponConfig.accuracyConf.baseData;
			maxGunLoad = weaponConfig.initBullet;
			sbulletCount = maxGunLoad;
			rocketFlySpeed = weaponConfig.flySpeed;
		}

		public override void Init()
		{
			base.MultiInit();
			hitForce = 30f;
			fire_ori = gun.transform.Find("fire_ori").gameObject;
		}

		public override void changeReticle()
		{
		}

		public override void CreateGun()
		{
			GameObject gameObject = null;
			gameObject = Object.Instantiate(Resources.Load("Prefabs/Weapon/RPG")) as GameObject;
			GunConfigScript component = gameObject.GetComponent<GunConfigScript>();
			gun = Object.Instantiate(component.Gun_Instance, player.GetTransform().position, player.GetTransform().rotation);
			Object.Destroy(gameObject);
		}

		public override void Fire(float deltaTime)
		{
			Ray ray = default(Ray);
			ray = new Ray(fire_ori.transform.position, fire_ori.transform.TransformDirection(Vector3.forward));
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo, 1000f, 35328))
			{
				aimTarget = hitInfo.point;
			}
			else
			{
				aimTarget = fire_ori.transform.TransformPoint(0f, 0f, 1000f);
			}
			Vector3 normalized = (aimTarget - fire_ori.transform.position).normalized;
			GameObject gameObject = Object.Instantiate(projectile, fire_ori.transform.position, Quaternion.LookRotation(normalized));
			ProjectileScript component = gameObject.GetComponent<ProjectileScript>();
			component.dir = normalized;
			component.flySpeed = rocketFlySpeed;
			component.explodeRadius = range;
			component.hitForce = hitForce;
			component.life = 8f;
			component.damage = 0f;
			component.GunType = WeaponType.RocketLauncher;
			component.player = base.WeaponPlayer;
			lastShootTime = Time.time;
		}

		public override void StopFire()
		{
		}
	}
}
