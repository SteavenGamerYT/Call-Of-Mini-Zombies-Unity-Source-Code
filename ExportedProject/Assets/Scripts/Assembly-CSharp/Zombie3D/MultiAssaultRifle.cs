using UnityEngine;

namespace Zombie3D
{
	public class MultiAssaultRifle : Weapon
	{
		protected ObjectPool bulletsObjectPool;

		protected ObjectPool firelineObjectPool;

		protected ObjectPool sparksObjectPool;

		protected static int sbulletCount;

		protected float lastPlayAudioTime;

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

		public MultiAssaultRifle()
		{
			maxCapacity = 9999;
			base.IsSelectedForBattle = true;
		}

		public override WeaponType GetWeaponType()
		{
			return WeaponType.AssaultRifle;
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
			Debug.Log("Multi Name:" + base.Name + " Frenquency:" + weaponConfig.attackRateConf.baseData);
		}

		public override void Init()
		{
			base.MultiInit();
			hitForce = 20f;
			gunfire = gun.transform.Find("gun_fire_new").gameObject;
			fire_ori = gun.transform.Find("fire_ori").gameObject;
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
			GameObject gameObject = null;
			switch (base.Name)
			{
			case "M4":
				gameObject = Object.Instantiate(Resources.Load("Prefabs/Weapon/M4")) as GameObject;
				break;
			case "AK-47":
				gameObject = Object.Instantiate(Resources.Load("Prefabs/Weapon/AK47")) as GameObject;
				break;
			case "MP5":
				gameObject = Object.Instantiate(Resources.Load("Prefabs/Weapon/MP5")) as GameObject;
				break;
			case "AUG":
				gameObject = Object.Instantiate(Resources.Load("Prefabs/Weapon/AUG")) as GameObject;
				break;
			case "P90":
				gameObject = Object.Instantiate(Resources.Load("Prefabs/Weapon/P90")) as GameObject;
				break;
			}
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

		public override void Fire(float deltaTime)
		{
			gunfire.GetComponent<Renderer>().enabled = true;
			Ray ray = default(Ray);
			ray = new Ray(fire_ori.transform.position, fire_ori.transform.TransformDirection(Vector3.forward));
			RaycastHit raycastHit = default(RaycastHit);
			RaycastHit[] array = Physics.RaycastAll(ray, 1000f, 559616);
			float num = 1000000f;
			for (int i = 0; i < array.Length; i++)
			{
				Vector3 zero = Vector3.zero;
				if (((array[i].collider.gameObject.layer == 9) ? gunfire.transform.InverseTransformPoint(array[i].collider.transform.position) : gunfire.transform.InverseTransformPoint(array[i].point)).z < 1f)
				{
					float sqrMagnitude = (array[i].point - gunfire.transform.position).sqrMagnitude;
					if (sqrMagnitude < num)
					{
						raycastHit = array[i];
						num = sqrMagnitude;
					}
				}
			}
			if (raycastHit.collider != null)
			{
				aimTarget = raycastHit.point;
				Vector3 normalized = fire_ori.transform.TransformDirection(Vector3.forward).normalized;
				Vector3 vector = player.GetTransform().InverseTransformPoint(aimTarget);
				if (vector.z > 2f)
				{
					GameObject gameObject = firelineObjectPool.CreateObject(fire_ori.transform.position + normalized * 2f, normalized);
					gameObject.transform.Rotate(180f, 0f, 0f);
					if (gameObject == null)
					{
						Debug.Log("fire line obj null");
					}
					else
					{
						FireLineScript component = gameObject.GetComponent<FireLineScript>();
						component.transform.Rotate(90f, 0f, 0f);
						component.beginPos = rightGun.position;
						component.endPos = raycastHit.point;
					}
				}
				bulletsObjectPool.CreateObject(rightGun.position, normalized);
				GameObject gameObject2 = raycastHit.collider.gameObject;
				if (gameObject2.name.StartsWith("E_"))
				{
					Enemy enemyByID = gameScene.GetEnemyByID(gameObject2.name);
					if (enemyByID.GetState() != Enemy.DEAD_STATE)
					{
						if (vector.z > 2f)
						{
							sparksObjectPool.CreateObject(raycastHit.point, -ray.direction);
						}
						DamageProperty damageProperty = new DamageProperty();
						damageProperty.hitForce = ray.direction * hitForce;
						damageProperty.damage = 0f;
						bool criticalAttack = false;
						int num2 = Random.Range(0, 100);
						if (num2 < 70)
						{
							criticalAttack = true;
						}
						float sqrMagnitude2 = (enemyByID.GetPosition() - player.GetTransform().position).sqrMagnitude;
						float num3 = range * range;
						if (sqrMagnitude2 < num3)
						{
							enemyByID.OnHit(damageProperty, GetWeaponType(), criticalAttack, player);
						}
						else if ((float)num2 < accuracy)
						{
							enemyByID.OnHit(damageProperty, GetWeaponType(), criticalAttack, player);
						}
					}
				}
				else
				{
					if (vector.z > 2f)
					{
						sparksObjectPool.CreateObject(raycastHit.point, -ray.direction);
					}
					if (gameObject2.layer == 19)
					{
						Debug.Log(gameObject2.name);
						WoodBoxScript component2 = gameObject2.GetComponent<WoodBoxScript>();
						if (component2.OnHit(0f))
						{
						}
					}
				}
				if (Time.time - lastPlayAudioTime >= shootAudio.clip.length)
				{
					lastPlayAudioTime = Time.time;
					AudioPlayer.PlayAudio(shootAudio, true);
				}
				player.LastHitPosition = raycastHit.point;
				lastShootTime = Time.time;
			}
			else
			{
				Vector3 normalized2 = fire_ori.transform.TransformDirection(Vector3.forward).normalized;
				bulletsObjectPool.CreateObject(rightGun.position, normalized2);
				GameObject gameObject3 = firelineObjectPool.CreateObject(fire_ori.transform.position + normalized2 * 2f, normalized2);
				if (gameObject3 == null)
				{
					Debug.Log("fire line obj null");
				}
				else
				{
					FireLineScript component3 = gameObject3.GetComponent<FireLineScript>();
					component3.transform.Rotate(90f, 0f, 0f);
					component3.beginPos = rightGun.position;
					component3.endPos = raycastHit.point;
				}
				if (!shootAudio.isPlaying)
				{
					AudioPlayer.PlayAudio(shootAudio, true);
				}
				player.LastHitPosition = raycastHit.point;
				lastShootTime = Time.time;
			}
		}

		public override void StopFire()
		{
			if (shootAudio != null)
			{
				shootAudio.Stop();
			}
			if (gunfire != null)
			{
				gunfire.GetComponent<Renderer>().enabled = false;
			}
		}

		public override void GunOff()
		{
			base.GunOff();
		}

		public override void AutoAim(float deltaTime)
		{
		}
	}
}
