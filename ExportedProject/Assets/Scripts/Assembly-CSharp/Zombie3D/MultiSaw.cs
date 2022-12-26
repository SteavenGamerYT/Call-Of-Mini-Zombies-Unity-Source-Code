using UnityEngine;

namespace Zombie3D
{
	public class MultiSaw : Weapon
	{
		protected GameObject fireTrail;

		protected ObjectPool bulletsObjectPool;

		protected ObjectPool firelineObjectPool;

		protected ObjectPool sparksObjectPool;

		protected static float sbulletCount;

		public override int BulletCount
		{
			get
			{
				return (int)sbulletCount;
			}
			set
			{
				sbulletCount = value;
			}
		}

		public MultiSaw()
		{
			maxCapacity = 9999;
			base.IsSelectedForBattle = true;
		}

		public override WeaponType GetWeaponType()
		{
			return WeaponType.Saw;
		}

		public override void LoadConfig()
		{
			base.LoadConfig();
			WeaponConfig weaponConfig = gConfig.GetWeaponConfig(base.Name);
			base.damage = weaponConfig.damageConf.baseData;
			base.attackFrenquency = weaponConfig.attackRateConf.baseData;
			accuracy = weaponConfig.accuracyConf.baseData;
			speedDrag = weaponConfig.moveSpeedDrag;
			range = weaponConfig.range;
			maxGunLoad = weaponConfig.initBullet;
			sbulletCount = maxGunLoad;
		}

		public override void Init()
		{
			base.MultiInit();
			hitForce = 20f;
			fireTrail = GameObject.Find("muzzleFlash");
			gunfire = gun.transform.Find("gun_fire_new").gameObject;
			bulletsObjectPool = new ObjectPool();
			firelineObjectPool = new ObjectPool();
			sparksObjectPool = new ObjectPool();
			bulletsObjectPool.Init("MultiBullets", rConf.bullets, 6, 1f);
			firelineObjectPool.Init("MultiFirelines", rConf.fireline, 4, 0.5f);
			sparksObjectPool.Init("MultiSparks", rConf.hitparticles, 3, 0.22f);
		}

		public override void changeReticle()
		{
		}

		public override void CreateGun()
		{
			GameObject gameObject = Object.Instantiate(Resources.Load("Prefabs/Weapon/Saw")) as GameObject;
			GunConfigScript component = gameObject.GetComponent<GunConfigScript>();
			gun = Object.Instantiate(component.Gun_Instance, player.GetTransform().position, player.GetTransform().rotation);
			Object.Destroy(gameObject);
		}

		public override void DoLogic()
		{
			bulletsObjectPool.AutoDestruct();
			firelineObjectPool.AutoDestruct();
			sparksObjectPool.AutoDestruct();
		}

		public override void FireUpdate(float deltaTime)
		{
		}

		public override bool HaveBullets()
		{
			return true;
		}

		public override bool IsAvailably()
		{
			return true;
		}

		public override void Fire(float deltaTime)
		{
			if (shootAudio != null && !shootAudio.isPlaying)
			{
				AudioPlayer.PlayAudio(shootAudio, true);
			}
			lastShootTime = Time.time;
		}

		public override void GunOn()
		{
			GameObject gameObject = gun.transform.Find("Saw01").gameObject;
			GameObject gameObject2 = gun.transform.Find("Saw02").gameObject;
			if (gameObject.GetComponent<Renderer>() != null)
			{
				gameObject.GetComponent<Renderer>().enabled = true;
			}
			if (gameObject2.GetComponent<Renderer>() != null)
			{
				gameObject2.GetComponent<Renderer>().enabled = true;
			}
		}

		public override void GunOff()
		{
			GameObject gameObject = gun.transform.Find("Saw01").gameObject;
			GameObject gameObject2 = gun.transform.Find("Saw02").gameObject;
			if (gameObject.GetComponent<Renderer>() != null)
			{
				gameObject.GetComponent<Renderer>().enabled = false;
			}
			if (gameObject2.GetComponent<Renderer>() != null)
			{
				gameObject2.GetComponent<Renderer>().enabled = false;
			}
			StopFire();
		}

		public override void StopFire()
		{
			if (shootAudio != null)
			{
			}
			if (gunfire != null)
			{
				gunfire.GetComponent<Renderer>().enabled = false;
			}
		}
	}
}
