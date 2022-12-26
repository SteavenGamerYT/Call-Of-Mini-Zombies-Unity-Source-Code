using UnityEngine;

namespace Zombie3D
{
	public class LaserGun : Weapon
	{
		protected float flySpeed;

		protected static float sbulletCount;

		private GameObject laserObj;

		protected Vector3 laserStartScale;

		protected float lastLaserHitInitiatTime;

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
		}

		public override void CreateGun()
		{
			GameObject gameObject = null;
			gameObject = Object.Instantiate(Resources.Load("Prefabs/Weapon/LaserGun")) as GameObject;
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

		public override void ConsumeBullet(int count)
		{
			sbulletCount -= count;
			sbulletCount = Mathf.Clamp(sbulletCount, 0f, maxCapacity);
			GameApp.GetInstance().GetGameState().bullet_comsume += count;
			base.ConsumeBullet(count);
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
			if (!(laserObj != null))
			{
				return;
			}
			ConsumeBulletTime(20f * deltaTime);
			Vector3 vector = cameraComponent.ScreenToWorldPoint(new Vector3(gameCamera.ReticlePosition.x, (float)Screen.height - gameCamera.ReticlePosition.y, 50f));
			Ray ray = new Ray(cameraTransform.position, vector - cameraTransform.position);
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo, 1000f, 34816))
			{
				aimTarget = hitInfo.point;
			}
			else
			{
				aimTarget = cameraTransform.TransformPoint(0f, 0f, 1000f);
			}
			Vector3 normalized = (aimTarget - gunfire.transform.position).normalized;
			float magnitude = (aimTarget - gunfire.transform.position).magnitude;
			laserObj.transform.position = gunfire.transform.position;
			laserObj.transform.LookAt(aimTarget);
			if (hitInfo.collider != null)
			{
				laserObj.transform.localScale = new Vector3(laserObj.transform.localScale.x, laserObj.transform.localScale.y, magnitude / 48.76f);
			}
			if (Time.time - lastLaserHitInitiatTime > 0.03f && (aimTarget - normalized - cameraTransform.position).sqrMagnitude > 9f)
			{
				Object.Instantiate(rConf.laserHit, aimTarget, Quaternion.identity);
				lastLaserHitInitiatTime = Time.time;
			}
			if (!CouldMakeNextShoot())
			{
				return;
			}
			GameObject[] woodBoxes = gameScene.GetWoodBoxes();
			GameObject[] array = woodBoxes;
			GameObject[] array2 = array;
			foreach (GameObject gameObject in array2)
			{
				if (gameObject != null && laserObj.GetComponent<Collider>().bounds.Intersects(gameObject.GetComponent<Collider>().bounds))
				{
					WoodBoxScript component = gameObject.GetComponent<WoodBoxScript>();
					component.OnHit(base.damage);
				}
			}
			if (shootAudio != null && !shootAudio.isPlaying)
			{
				AudioPlayer.PlayAudio(shootAudio, true);
			}
			lastShootTime = Time.time;
		}

		public override void Fire(float deltaTime)
		{
			gunfire.GetComponent<Renderer>().enabled = true;
			Vector3 vector = cameraComponent.ScreenToWorldPoint(new Vector3(gameCamera.ReticlePosition.x, (float)Screen.height - gameCamera.ReticlePosition.y, 50f));
			Ray ray = new Ray(cameraTransform.position, vector - cameraTransform.position);
			RaycastHit hitInfo;
			if (GameApp.GetInstance().GetGameState().VS_mode)
			{
				if (Physics.Raycast(ray, out hitInfo, 1000f, 35072))
				{
					aimTarget = hitInfo.point;
				}
				else
				{
					aimTarget = cameraTransform.TransformPoint(0f, 0f, 1000f);
				}
			}
			else if (Physics.Raycast(ray, out hitInfo, 1000f, 35328))
			{
				aimTarget = hitInfo.point;
			}
			else
			{
				aimTarget = cameraTransform.TransformPoint(0f, 0f, 1000f);
			}
			Vector3 normalized = (aimTarget - gunfire.transform.position).normalized;
			if (laserObj == null)
			{
				laserObj = Object.Instantiate(rConf.laser, gunfire.transform.position, Quaternion.LookRotation(normalized));
				laserStartScale = laserObj.transform.localScale;
				ProjectileScript component = laserObj.GetComponent<ProjectileScript>();
				component.dir = normalized;
				component.flySpeed = flySpeed;
				component.explodeRadius = 0f;
				component.hitForce = hitForce;
				component.life = 8f;
				component.damage = base.damage;
				component.GunType = WeaponType.LaserGun;
				component.player = base.WeaponPlayer;
			}
			lastShootTime = Time.time;
		}

		public override void StopFire()
		{
			if (laserObj != null)
			{
				Object.Destroy(laserObj);
				laserObj = null;
			}
			if (shootAudio != null)
			{
				shootAudio.Stop();
			}
			if (gunfire != null)
			{
				gunfire.GetComponent<Renderer>().enabled = false;
			}
		}

		public override WeaponType GetWeaponType()
		{
			return WeaponType.LaserGun;
		}

		public override void changeReticle()
		{
		}
	}
}
