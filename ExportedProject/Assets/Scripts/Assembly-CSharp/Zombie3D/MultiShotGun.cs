using System;
using System.Collections;
using UnityEngine;

namespace Zombie3D
{
	public class MultiShotGun : Weapon
	{
		protected static int sbulletCount;

		protected Timer shotgunFireTimer;

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

		public MultiShotGun()
		{
			maxCapacity = 9999;
			base.IsSelectedForBattle = true;
			shotgunFireTimer = new Timer();
		}

		public override WeaponType GetWeaponType()
		{
			return WeaponType.ShotGun;
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
		}

		public override void Init()
		{
			base.MultiInit();
			hitForce = 20f;
			gunfire = gun.transform.Find("gun_fire_new").gameObject;
		}

		public void PlayPumpAnimation()
		{
		}

		public override void changeReticle()
		{
		}

		public override void CreateGun()
		{
			GameObject gameObject = null;
			switch (base.Name)
			{
			case "Winchester-1200":
				gameObject = UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Weapon/Wechester1200")) as GameObject;
				break;
			case "Remington-870":
				gameObject = UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Weapon/Remington870")) as GameObject;
				break;
			case "XM-1014":
				gameObject = UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Weapon/XM1014")) as GameObject;
				break;
			}
			GunConfigScript component = gameObject.GetComponent<GunConfigScript>();
			gun = UnityEngine.Object.Instantiate(component.Gun_Instance, player.GetTransform().position, player.GetTransform().rotation);
			UnityEngine.Object.Destroy(gameObject);
		}

		public override void FireUpdate(float deltaTime)
		{
			base.FireUpdate(deltaTime);
			if (shotgunFireTimer.Ready())
			{
				if (gunfire != null)
				{
					gunfire.GetComponent<Renderer>().enabled = false;
				}
				shotgunFireTimer.Do();
			}
		}

		public override void Fire(float deltaTime)
		{
			AudioPlayer.PlayAudio(shootAudio, true);
			gun.GetComponent<Animation>()["Reload"].wrapMode = WrapMode.Once;
			gun.GetComponent<Animation>().Play("Reload");
			gunfire.GetComponent<Renderer>().enabled = true;
			shotgunFireTimer.SetTimer(0.2f, false);
			UnityEngine.Object.Instantiate(rConf.shotgunBullet, rightGun.position, player.GetTransform().rotation);
			player.LastHitPosition = player.GetTransform().position;
			GameObject gameObject = UnityEngine.Object.Instantiate(rConf.shotgunfire, gunfire.transform.position, player.GetTransform().rotation);
			gameObject.transform.parent = player.GetTransform();
			float num = Mathf.Tan((float)System.Math.PI / 3f);
			IEnumerator enumerator = gameScene.GetEnemies().Values.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Enemy enemy = (Enemy)enumerator.Current;
					if (enemy.GetState() == Enemy.DEAD_STATE)
					{
						continue;
					}
					Vector3 vector = player.GetTransform().InverseTransformPoint(enemy.GetPosition());
					float sqrMagnitude = (enemy.GetPosition() - player.GetTransform().position).sqrMagnitude;
					float num2 = range * range;
					if (!(vector.z > 0f) || !(Mathf.Abs(vector.z / vector.x) > num))
					{
						continue;
					}
					DamageProperty damageProperty = new DamageProperty();
					damageProperty.damage = 0f;
					if (sqrMagnitude < num2)
					{
						enemy.OnHit(damageProperty, GetWeaponType(), true, player);
					}
					else if (sqrMagnitude < num2 * 2f * 2f)
					{
						int num3 = UnityEngine.Random.Range(0, 100);
						if ((float)num3 < accuracy)
						{
							enemy.OnHit(damageProperty, GetWeaponType(), true, player);
						}
					}
					else if (sqrMagnitude < num2 * 3f * 3f)
					{
						int num4 = UnityEngine.Random.Range(0, 100);
						if ((float)num4 < accuracy / 2f)
						{
							enemy.OnHit(damageProperty, GetWeaponType(), true, player);
						}
					}
					else if (sqrMagnitude < num2 * 4f * 4f)
					{
						int num5 = UnityEngine.Random.Range(0, 100);
						if ((float)num5 < accuracy / 4f)
						{
							enemy.OnHit(damageProperty, GetWeaponType(), true, player);
						}
					}
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = enumerator as IDisposable) != null)
				{
					disposable.Dispose();
				}
			}
			GameObject[] woodBoxes = gameScene.GetWoodBoxes();
			GameObject[] array = woodBoxes;
			GameObject[] array2 = array;
			foreach (GameObject gameObject2 in array2)
			{
				if (gameObject2 != null)
				{
					Vector3 vector2 = player.GetTransform().InverseTransformPoint(gameObject2.transform.position);
					float sqrMagnitude2 = (gameObject2.transform.position - player.GetTransform().position).sqrMagnitude;
					float num6 = range * range;
					if (sqrMagnitude2 < num6 * 2f * 2f && vector2.z > 0f)
					{
						WoodBoxScript component = gameObject2.GetComponent<WoodBoxScript>();
						component.OnHit(0f);
					}
				}
			}
			lastShootTime = Time.time;
		}

		public override void StopFire()
		{
			if (gunfire != null)
			{
				gunfire.GetComponent<Renderer>().enabled = false;
			}
		}

		public override void GunOff()
		{
			base.GunOff();
		}
	}
}
