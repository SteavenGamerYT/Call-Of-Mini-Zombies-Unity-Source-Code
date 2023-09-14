using UnityEngine;

namespace Zombie3D
{
	public class GrenadeRifle : Weapon
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

		public GrenadeRifle()
		{
			maxCapacity = 9999;
			base.IsSelectedForBattle = false;
		}

		public override WeaponType GetWeaponType()
		{
			return WeaponType.M32;
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
			gameObject = Object.Instantiate(Resources.Load("Prefabs/Weapon/M32")) as GameObject;
			GunConfigScript component = gameObject.GetComponent<GunConfigScript>();
			gun = Object.Instantiate(component.Gun_Instance, player.GetTransform().position, player.GetTransform().rotation);
			Object.Destroy(gameObject);
		}

		public override void Fire(float deltaTime)
		{
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
			Vector3 normalized = (aimTarget - rightGun.position).normalized;
			Object.Instantiate(rConf.m32spark, rightGun.position, Quaternion.LookRotation(normalized));
			GameObject gameObject = Object.Instantiate(rConf.m32tile, rightGun.position, Quaternion.LookRotation(normalized));
			ProjectileScript component = gameObject.GetComponent<ProjectileScript>();
			component.dir = normalized;
			component.flySpeed = rocketFlySpeed;
			component.explodeRadius = range;
			component.hitForce = hitForce;
			component.life = 8f;
			component.damage = base.damage;
			component.GunType = WeaponType.M32;
			component.player = base.WeaponPlayer;
			component.targetPos = new Ray(rightGun.position, normalized).GetPoint(100f);
			TPSSimpleCameraScript tPSSimpleCameraScript = (TPSSimpleCameraScript)gameCamera;
			float num = (tPSSimpleCameraScript.angelV - tPSSimpleCameraScript.minAngelV) / (tPSSimpleCameraScript.maxAngelV - tPSSimpleCameraScript.minAngelV);
			component.initAngel = 10f + num * 30f;
			component.targetPos.y = 10000f;
			lastShootTime = Time.time;
			ConsumeBullet(1);
		}

		public override void StopFire()
		{
		}

		public override void ConsumeBullet(int count)
		{
			sbulletCount -= count;
			sbulletCount = Mathf.Clamp(sbulletCount, 0, maxCapacity);
			GameApp.GetInstance().GetGameState().bullet_comsume += count;
			base.ConsumeBullet(count);
		}
	}
}
