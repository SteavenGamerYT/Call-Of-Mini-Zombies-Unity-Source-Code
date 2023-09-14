using System;
using System.Collections;
using UnityEngine;

namespace Zombie3D
{
	public class MachineGun : Weapon
	{
		protected ObjectPool bulletsObjectPool;

		protected ObjectPool firelineObjectPool;

		protected ObjectPool sparksObjectPool;

		protected static int sbulletCount;

		public static Rect lockAreaRect = AutoRect.AutoPos(new Rect(230f, 200f, 500f, 250f));

		protected AssaultRifleNearestEnemyInfo _curEnemyInfo;

		public AssaultRifleNearestEnemyInfo curEnemyInfo
		{
			get
			{
				return _curEnemyInfo;
			}
			set
			{
				_curEnemyInfo = value;
			}
		}

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

		public override WeaponType GetWeaponType()
		{
			return WeaponType.MachineGun;
		}

		public override void Init()
		{
			base.Init();
			maxCapacity = 9999;
			base.attackFrenquency = 0.25f;
			hitForce = 20f;
			maxDeflection = 4f;
			gunfire = gun.transform.Find("gun_fire_new").gameObject;
			fire_ori = gun.transform.Find("fire_ori").gameObject;
			bulletsObjectPool = new ObjectPool();
			firelineObjectPool = new ObjectPool();
			sparksObjectPool = new ObjectPool();
			bulletsObjectPool.Init("Bullets", rConf.bullets, 6, 1f);
			firelineObjectPool.Init("Firelines", rConf.fireline, 20, 0.5f);
			sparksObjectPool.Init("Sparks", rConf.hitparticles, 3, 0.22f);
			TimerManager.GetInstance().SetTimer(5, base.attackFrenquency, false);
		}

		public override void changeReticle()
		{
		}

		public override void CreateGun()
		{
			GameObject gameObject = null;
			gameObject = UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Weapon/Gatlin")) as GameObject;
			GunConfigScript component = gameObject.GetComponent<GunConfigScript>();
			gun = UnityEngine.Object.Instantiate(component.Gun_Instance, player.GetTransform().position, player.GetTransform().rotation);
			UnityEngine.Object.Destroy(gameObject);
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

		public override void FireUpdate(float deltaTime)
		{
			deflection.x += UnityEngine.Random.Range(-0.5f, 0.5f);
			deflection.y += UnityEngine.Random.Range(-0.5f, 0.5f);
			deflection.x = Mathf.Clamp(deflection.x, 0f - maxDeflection, maxDeflection);
			deflection.y = Mathf.Clamp(deflection.y, 0f - maxDeflection, maxDeflection);
		}

		public override void Fire(float deltaTime)
		{
			gunfire.GetComponent<Renderer>().enabled = true;
			Ray ray = default(Ray);
			if (gameCamera.GetCameraType() == CameraType.TPSCamera)
			{
				Vector3 vector = cameraComponent.ScreenToWorldPoint(new Vector3(gameCamera.ReticlePosition.x, (float)Screen.height - gameCamera.ReticlePosition.y, 50f));
				ray = new Ray(cameraTransform.position, vector - cameraTransform.position);
			}
			else if (gameCamera.GetCameraType() == CameraType.TopWatchingCamera)
			{
				ray = new Ray(base.player.GetTransform().position + Vector3.up * 0.5f, base.player.GetTransform().TransformDirection(Vector3.forward));
			}
			bool flag = false;
			RaycastHit hitInfo;
			if (GameApp.GetInstance().GetGameState().VS_mode ? Physics.Raycast(ray, out hitInfo, 1000f, 559360) : Physics.Raycast(ray, out hitInfo, 1000f, 559616))
			{
				aimTarget = hitInfo.point;
				Vector3 vector2 = Vector3.zero;
				Vector3 vector3 = base.player.GetTransform().InverseTransformPoint(aimTarget);
				if (vector3.z > 2f)
				{
					for (int i = -2; i <= 2; i++)
					{
						Vector3 zero = Vector3.zero;
						zero = ((i % 2 == 0) ? fire_ori.transform.TransformPoint(Vector3.left * i * 1.5f) : fire_ori.transform.TransformPoint(Vector3.up * i * 1.5f));
						vector2 = (aimTarget - fire_ori.transform.position).normalized;
						GameObject gameObject = firelineObjectPool.CreateObject(zero + vector2 * (Mathf.Abs(i) + 2), vector2);
						gameObject.transform.Rotate(180f, 0f, 0f);
						if (gameObject == null)
						{
							Debug.Log("fire line obj null");
							continue;
						}
						FireLineScript component = gameObject.GetComponent<FireLineScript>();
						component.transform.Rotate(90f, 0f, 0f);
						component.beginPos = rightGun.position;
						component.endPos = hitInfo.point;
					}
				}
				bulletsObjectPool.CreateObject(rightGun.position, vector2);
				GameObject gameObject2 = hitInfo.collider.gameObject;
				if (gameObject2.name.StartsWith("E_"))
				{
					Enemy enemyByID = gameScene.GetEnemyByID(gameObject2.name);
					if (enemyByID.GetState() != Enemy.DEAD_STATE)
					{
						if (vector3.z > 2f)
						{
							sparksObjectPool.CreateObject(hitInfo.point, -ray.direction);
						}
						DamageProperty damageProperty = new DamageProperty();
						damageProperty.hitForce = ray.direction * hitForce;
						damageProperty.damage = base.damage * base.player.PowerBuff;
						bool criticalAttack = false;
						int num = UnityEngine.Random.Range(0, 100);
						if (num < 70)
						{
							criticalAttack = true;
						}
						float sqrMagnitude = (enemyByID.GetPosition() - base.player.GetTransform().position).sqrMagnitude;
						float num2 = range * range;
						if (sqrMagnitude < num2)
						{
							enemyByID.OnHit(damageProperty, GetWeaponType(), criticalAttack, base.player);
						}
						else if ((float)num < accuracy)
						{
							enemyByID.OnHit(damageProperty, GetWeaponType(), criticalAttack, base.player);
						}
					}
				}
				else if (gameObject2.layer == 8)
				{
					Player player = gameObject2.GetComponent<PlayerShell>().m_player;
					if (player.GetPlayerState() != player.DEAD_STATE)
					{
						Debug.Log("hit player:" + base.damage * base.player.PowerBuff);
						player.OnVsInjured(base.player.sfs_user, base.damage * base.player.PowerBuff, (int)GetWeaponType());
					}
				}
				else
				{
					if (vector3.z > 2f)
					{
						sparksObjectPool.CreateObject(hitInfo.point, -ray.direction);
					}
					if (gameObject2.layer == 19)
					{
						WoodBoxScript component2 = gameObject2.GetComponent<WoodBoxScript>();
						component2.OnHit(base.damage * base.player.PowerBuff);
					}
					else if (gameObject2.layer == 15 && Application.loadedLevelName == "Zombie3D_Village")
					{
						UnityEngine.Object.Instantiate(GameApp.GetInstance().GetGameResourceConfig().snow_explosion_eff, hitInfo.point + Vector3.up * 1f, Quaternion.identity);
					}
				}
				if (shootAudio != null && !shootAudio.isPlaying)
				{
					AudioPlayer.PlayAudio(shootAudio, true);
				}
				ConsumeBullet(1);
				base.player.LastHitPosition = hitInfo.point;
				lastShootTime = Time.time;
			}
			else
			{
				aimTarget = cameraTransform.TransformPoint(0f, 0f, 1000f);
			}
		}

		public override void ConsumeBullet(int count)
		{
			sbulletCount -= count;
			sbulletCount = Mathf.Clamp(sbulletCount, 0, maxCapacity);
			GameApp.GetInstance().GetGameState().bullet_comsume += count;
			base.ConsumeBullet(count);
		}

		public override void DoLogic()
		{
			bulletsObjectPool.AutoDestruct();
			firelineObjectPool.AutoDestruct();
			sparksObjectPool.AutoDestruct();
		}

		public override void StopFire()
		{
			deflection = Vector2.zero;
			if (shootAudio != null)
			{
			}
			if (gunfire != null)
			{
				gunfire.GetComponent<Renderer>().enabled = false;
			}
			if (curEnemyInfo != null)
			{
				curEnemyInfo = null;
			}
		}

		public override void GunOff()
		{
			base.GunOff();
		}

		public override void AutoAim(float deltaTime)
		{
			if (GameApp.GetInstance().GetGameState().is_auto_aim == 0)
			{
				return;
			}
			if (curEnemyInfo != null && curEnemyInfo.curEnemy != null && curEnemyInfo._type == NearestEnemyType.NearestEnemyType_Enemy && curEnemyInfo.curEnemy.GetState() == Enemy.DEAD_STATE)
			{
				curEnemyInfo = null;
			}
			if (curEnemyInfo != null && curEnemyInfo.transform != null)
			{
				Vector2 vector = cameraComponent.WorldToScreenPoint(curEnemyInfo.transform.position);
				curEnemyInfo.screenPos = new Vector2(vector.x, (float)Screen.height - vector.y);
				curEnemyInfo.currentScreenPos = curEnemyInfo.screenPos;
			}
			if (!TimerManager.GetInstance().Ready(5))
			{
				return;
			}
			if (curEnemyInfo != null && curEnemyInfo.transform != null)
			{
				Vector3 vector2 = cameraComponent.WorldToScreenPoint(curEnemyInfo.transform.position);
				if (vector2.z < 0f || vector2.x < lockAreaRect.xMin || vector2.x > lockAreaRect.xMax || vector2.y < lockAreaRect.yMin || vector2.y > lockAreaRect.yMax)
				{
					if (curEnemyInfo.transform.gameObject.layer == 23)
					{
						UnityEngine.Object.Destroy(curEnemyInfo.transform.gameObject);
					}
					curEnemyInfo = null;
				}
			}
			if (curEnemyInfo != null)
			{
				return;
			}
			float num = 99999f;
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
					Transform aimedTransform = enemy.GetAimedTransform();
					Vector3 vector3 = cameraComponent.WorldToScreenPoint(aimedTransform.position);
					if (vector3.z < 0f || !(vector3.x > lockAreaRect.xMin) || !(vector3.x < lockAreaRect.xMax) || !(vector3.y > lockAreaRect.yMin) || !(vector3.y < lockAreaRect.yMax))
					{
						continue;
					}
					Ray ray = new Ray(rightGun.position, aimedTransform.position - rightGun.position);
					RaycastHit hitInfo;
					if (!Physics.Raycast(ray, out hitInfo, 1000f, 35328) || hitInfo.collider.gameObject.name.StartsWith("E_"))
					{
						float sqrMagnitude = (rightGun.position - aimedTransform.position).sqrMagnitude;
						if (sqrMagnitude < num)
						{
							num = sqrMagnitude;
							curEnemyInfo = new AssaultRifleNearestEnemyInfo();
							curEnemyInfo.transform = aimedTransform;
							curEnemyInfo.screenPos = new Vector2(vector3.x, (float)Screen.height - vector3.y);
							curEnemyInfo.distance = sqrMagnitude;
							curEnemyInfo.currentScreenPos = gameScene.GetCamera().ReticlePosition;
							curEnemyInfo.curEnemy = enemy;
							curEnemyInfo._type = NearestEnemyType.NearestEnemyType_Enemy;
							num = sqrMagnitude;
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
			if (curEnemyInfo != null)
			{
				if (shootAudio != null)
				{
					AudioPlayer.PlayAudio(shootAudio, true);
				}
			}
			else
			{
				GameObject[] woodBoxes = gameScene.GetWoodBoxes();
				num = 99999f;
				GameObject[] array = woodBoxes;
				GameObject[] array2 = array;
				foreach (GameObject gameObject in array2)
				{
					if (!(gameObject != null))
					{
						continue;
					}
					Transform transform = gameObject.transform;
					Vector3 vector4 = cameraComponent.WorldToScreenPoint(transform.position + Vector3.up * 0.6f);
					if (vector4.z < 0f || !(vector4.x > lockAreaRect.xMin) || !(vector4.x < lockAreaRect.xMax) || !(vector4.y > lockAreaRect.yMin) || !(vector4.y < lockAreaRect.yMax))
					{
						continue;
					}
					Ray ray2 = new Ray(rightGun.position, transform.position + Vector3.up * 0.6f - rightGun.position);
					RaycastHit hitInfo2;
					if (!Physics.Raycast(ray2, out hitInfo2, 1000f, 559104) || hitInfo2.collider.gameObject.layer == 19)
					{
						float sqrMagnitude2 = (rightGun.position - transform.position).sqrMagnitude;
						if (sqrMagnitude2 < num)
						{
							num = sqrMagnitude2;
							curEnemyInfo = new AssaultRifleNearestEnemyInfo();
							GameObject gameObject2 = new GameObject();
							gameObject2.transform.position = transform.position + Vector3.up * 0.6f;
							gameObject2.transform.parent = transform;
							gameObject2.layer = 23;
							curEnemyInfo.transform = gameObject2.transform;
							curEnemyInfo.screenPos = new Vector2(vector4.x, (float)Screen.height - vector4.y);
							curEnemyInfo.distance = sqrMagnitude2;
							curEnemyInfo.currentScreenPos = gameScene.GetCamera().ReticlePosition;
							curEnemyInfo._type = NearestEnemyType.NearestEnemyType_Box;
							curEnemyInfo.curEnemy = null;
							Debug.Log("woodbox" + curEnemyInfo.transform.position);
						}
					}
				}
				if (curEnemyInfo != null && shootAudio != null)
				{
					AudioPlayer.PlayAudio(shootAudio, true);
				}
			}
			TimerManager.GetInstance().Do(5);
		}
	}
}
