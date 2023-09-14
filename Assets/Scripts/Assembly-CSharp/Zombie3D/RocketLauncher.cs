using UnityEngine;

namespace Zombie3D
{
	public class RocketLauncher : Weapon
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

		public RocketLauncher()
		{
			maxCapacity = 9999;
			base.IsSelectedForBattle = false;
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
			base.Init();
			hitForce = 30f;
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
			if (gameCamera.GetCameraType() == CameraType.TPSCamera)
			{
				Vector3 vector = cameraComponent.ScreenToWorldPoint(new Vector3(gameCamera.ReticlePosition.x, (float)Screen.height - gameCamera.ReticlePosition.y, 50f));
				ray = new Ray(cameraTransform.position, vector - cameraTransform.position);
			}
			else if (gameCamera.GetCameraType() == CameraType.TopWatchingCamera)
			{
				ray = new Ray(player.GetTransform().position + Vector3.up * 0.5f, player.GetTransform().TransformDirection(Vector3.forward));
			}
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
			Vector3 normalized = (aimTarget - rightGun.position).normalized;
			GameObject gameObject = Object.Instantiate(projectile, rightGun.position, Quaternion.LookRotation(normalized));
			ProjectileScript component = gameObject.GetComponent<ProjectileScript>();
			component.dir = normalized;
			component.flySpeed = rocketFlySpeed;
			component.explodeRadius = range;
			component.hitForce = hitForce;
			component.life = 8f;
			component.damage = base.damage;
			component.GunType = WeaponType.RocketLauncher;
			component.player = base.WeaponPlayer;
			lastShootTime = Time.time;
			ConsumeBullet(1);
		}

		public override void ConsumeBullet(int count)
		{
			sbulletCount -= count;
			sbulletCount = Mathf.Clamp(sbulletCount, 0, maxCapacity);
			GameApp.GetInstance().GetGameState().bullet_comsume += count;
			base.ConsumeBullet(count);
		}

		public override void StopFire()
		{
		}
	}
}
