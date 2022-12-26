using UnityEngine;

namespace Zombie3D
{
	public class FireGun : Weapon
	{
		protected float flySpeed;

		protected static float sbulletCount;

		protected Vector3 laserStartScale;

		protected float lastLaserHitInitiatTime;

		protected ParticleEmitter FireDream;

		protected ParticleEmitter FireHeart1;

		protected ParticleEmitter FireHeart2;

		protected float temp_consume;

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

		public override void Init()
		{
			base.Init();
			maxCapacity = 9999;
			gunfire = gun.transform.Find("gun_fire_new").gameObject;
			GameObject gameObject = gun.transform.Find("gun_fire_new/hellfire/hellfire_01").gameObject;
			gameObject.GetComponent<HellFireProjectileScript>().SetPlayer(player);
			FireDream = gameObject.GetComponent<ParticleEmitter>();
			gameObject = gun.transform.Find("gun_fire_new/hellfire/hellfire_02").gameObject;
			FireHeart1 = gameObject.GetComponent<ParticleEmitter>();
			gameObject = gun.transform.Find("gun_fire_new/hellfire/hellfire_03").gameObject;
			FireHeart2 = gameObject.GetComponent<ParticleEmitter>();
			EnableFire(false);
		}

		public void EnableFire(bool status)
		{
			if (FireDream != null)
			{
				FireDream.emit = status;
			}
			if (FireHeart1 != null)
			{
				FireHeart1.emit = status;
			}
			if (FireHeart2 != null)
			{
				FireHeart2.emit = status;
			}
		}

		public override void CreateGun()
		{
			GameObject gameObject = null;
			gameObject = Object.Instantiate(Resources.Load("Prefabs/Weapon/FireGun")) as GameObject;
			GunConfigScript component = gameObject.GetComponent<GunConfigScript>();
			gun = Object.Instantiate(component.Gun_Instance, player.GetTransform().position, player.GetTransform().rotation);
			Object.Destroy(gameObject);
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
			flySpeed = weaponConfig.flySpeed;
		}

		public void PlayShootAudio()
		{
			if (shootAudio != null)
			{
				AudioPlayer.PlayAudio(shootAudio, true);
			}
		}

		public void SetShootTimeNow()
		{
			lastShootTime = Time.time;
		}

		public void ConsumeBulletTime(float count)
		{
			sbulletCount -= count;
			sbulletCount = Mathf.Clamp(sbulletCount, 0f, maxCapacity);
			temp_consume += count;
			if (temp_consume >= 1f)
			{
				GameApp.GetInstance().GetGameState().bullet_comsume++;
				temp_consume = 0f;
			}
		}

		public override void FireUpdate(float deltaTime)
		{
			if (!FireDream.emit)
			{
				return;
			}
			ConsumeBulletTime(20f * deltaTime);
			Vector3 worldPosition = cameraComponent.ScreenToWorldPoint(new Vector3(gameCamera.ReticlePosition.x, (float)Screen.height - gameCamera.ReticlePosition.y, 10f));
			FireDream.gameObject.transform.LookAt(worldPosition);
			if (CouldMakeNextShoot())
			{
				if (shootAudio != null && !shootAudio.isPlaying)
				{
					AudioPlayer.PlayAudio(shootAudio, true);
				}
				lastShootTime = Time.time;
			}
		}

		public override void Fire(float deltaTime)
		{
			EnableFire(true);
		}

		public override void StopFire()
		{
			EnableFire(false);
			if (shootAudio != null)
			{
				shootAudio.Stop();
			}
		}

		public override WeaponType GetWeaponType()
		{
			return WeaponType.FireGun;
		}

		public override void changeReticle()
		{
		}

		public override void ConsumeBullet(int count)
		{
			sbulletCount -= count;
			sbulletCount = Mathf.Clamp(sbulletCount, 0f, maxCapacity);
			GameApp.GetInstance().GetGameState().bullet_comsume += count;
			base.ConsumeBullet(count);
		}
	}
}
