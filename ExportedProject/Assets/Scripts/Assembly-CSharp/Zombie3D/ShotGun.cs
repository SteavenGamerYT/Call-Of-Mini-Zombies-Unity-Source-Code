using System;
using System.Collections;
using UnityEngine;

namespace Zombie3D
{
	public class ShotGun : Weapon
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

		public ShotGun()
		{
			maxCapacity = 9999;
			base.IsSelectedForBattle = false;
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
			base.Init();
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
			int num2 = 0;
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
					float num3 = range * range;
					if (vector.z > 0f && Mathf.Abs(vector.z / vector.x) > num)
					{
						DamageProperty damageProperty = new DamageProperty();
						damageProperty.damage = base.damage * player.PowerBuff;
						if (sqrMagnitude < num3)
						{
							enemy.OnHit(damageProperty, GetWeaponType(), true, player);
						}
						else if (sqrMagnitude < num3 * 2f * 2f)
						{
							int num4 = UnityEngine.Random.Range(0, 100);
							if ((float)num4 < accuracy)
							{
								enemy.OnHit(damageProperty, GetWeaponType(), true, player);
							}
						}
						else if (sqrMagnitude < num3 * 3f * 3f)
						{
							int num5 = UnityEngine.Random.Range(0, 100);
							if ((float)num5 < accuracy / 2f)
							{
								enemy.OnHit(damageProperty, GetWeaponType(), true, player);
							}
						}
						else if (sqrMagnitude < num3 * 4f * 4f)
						{
							int num6 = UnityEngine.Random.Range(0, 100);
							if ((float)num6 < accuracy / 4f)
							{
								enemy.OnHit(damageProperty, GetWeaponType(), true, player);
							}
						}
					}
					if (enemy.HP <= 0f)
					{
						num2++;
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
			if (num2 >= 4)
			{
				GameApp.GetInstance().GetGameState().Achievement.CheckAchievemnet_WeaponMaster();
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
					float num7 = range * range;
					if (sqrMagnitude2 < num7 * 2f * 2f && vector2.z > 0f)
					{
						WoodBoxScript component = gameObject2.GetComponent<WoodBoxScript>();
						component.OnHit(base.damage * player.PowerBuff);
					}
				}
			}
			if (GameApp.GetInstance().GetGameState().VS_mode)
			{
				GameVSScene gameVSScene = GameApp.GetInstance().GetGameScene() as GameVSScene;
				foreach (Player value in gameVSScene.SFS_Player_Arr.Values)
				{
					if (value == null || value == player || value.GetPlayerState() == value.DEAD_STATE)
					{
						continue;
					}
					Vector3 vector3 = player.GetTransform().InverseTransformPoint(value.GetTransform().position);
					float sqrMagnitude3 = (value.GetTransform().position - player.GetTransform().position).sqrMagnitude;
					float num8 = range * range;
					if (!(vector3.z > 0f) || !(Mathf.Abs(vector3.z / vector3.x) > num))
					{
						continue;
					}
					if (sqrMagnitude3 < num8)
					{
						Debug.Log("hit player:" + base.damage * player.PowerBuff);
						value.OnVsInjured(player.sfs_user, base.damage * player.PowerBuff, (int)GetWeaponType());
					}
					else if (sqrMagnitude3 < num8 * 2f * 2f)
					{
						int num9 = UnityEngine.Random.Range(0, 100);
						if ((float)num9 < accuracy)
						{
							Debug.Log("hit player:" + base.damage * player.PowerBuff);
							value.OnVsInjured(player.sfs_user, base.damage * player.PowerBuff, (int)GetWeaponType());
						}
					}
					else if (sqrMagnitude3 < num8 * 3f * 3f)
					{
						int num10 = UnityEngine.Random.Range(0, 100);
						if ((float)num10 < accuracy / 2f)
						{
							Debug.Log("hit player:" + base.damage * player.PowerBuff);
							value.OnVsInjured(player.sfs_user, base.damage * player.PowerBuff, (int)GetWeaponType());
						}
					}
					else if (sqrMagnitude3 < num8 * 4f * 4f)
					{
						int num11 = UnityEngine.Random.Range(0, 100);
						if ((float)num11 < accuracy / 4f)
						{
							Debug.Log("hit player:" + base.damage * player.PowerBuff);
							value.OnVsInjured(player.sfs_user, base.damage * player.PowerBuff, (int)GetWeaponType());
						}
					}
				}
			}
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
