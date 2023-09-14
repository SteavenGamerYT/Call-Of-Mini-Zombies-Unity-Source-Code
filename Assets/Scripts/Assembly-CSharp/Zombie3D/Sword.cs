using System;
using System.Collections;
using UnityEngine;

namespace Zombie3D
{
	public class Sword : Weapon
	{
		protected float lightFlySpeed;

		protected static float sbulletCount;

		protected GameObject _swordlight;

		protected float _createtime;

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

		public override WeaponType GetWeaponType()
		{
			return WeaponType.Sword;
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
			lightFlySpeed = weaponConfig.flySpeed;
		}

		public override void Init()
		{
			base.Init();
		}

		public override void changeReticle()
		{
		}

		public override void CreateGun()
		{
			GameObject gameObject = null;
			gameObject = UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Weapon/Sword")) as GameObject;
			GunConfigScript component = gameObject.GetComponent<GunConfigScript>();
			gun = UnityEngine.Object.Instantiate(component.Gun_Instance, player.GetTransform().position, player.GetTransform().rotation);
			UnityEngine.Object.Destroy(gameObject);
		}

		public override void DoLogic()
		{
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
			Hashtable enemies = gameScene.GetEnemies();
			IEnumerator enumerator = enemies.Values.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Enemy enemy = (Enemy)enumerator.Current;
					Collider collider = enemy.GetCollider();
					if (gun.GetComponent<Collider>().bounds.Intersects(collider.bounds))
					{
						DamageProperty damageProperty = new DamageProperty();
						damageProperty.damage = base.damage;
						enemy.OnHit(damageProperty, WeaponType.Sword, true, player);
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
			foreach (GameObject gameObject in array2)
			{
				if (gameObject != null)
				{
					Collider component = gameObject.GetComponent<Collider>();
					if (gun.GetComponent<Collider>().bounds.Intersects(component.bounds))
					{
						WoodBoxScript component2 = gameObject.GetComponent<WoodBoxScript>();
						component2.OnHit(base.damage * player.PowerBuff);
					}
				}
			}
			if (GameApp.GetInstance().GetGameState().VS_mode)
			{
				GameVSScene gameVSScene = GameApp.GetInstance().GetGameScene() as GameVSScene;
				foreach (Player value in gameVSScene.SFS_Player_Arr.Values)
				{
					if (value != null && value != player && value.GetPlayerState() != value.DEAD_STATE)
					{
						Collider collider2 = value.GetCollider();
						if (gun.GetComponent<Collider>().bounds.Intersects(collider2.bounds))
						{
							Debug.Log("hit player:" + base.damage * player.PowerBuff);
							value.OnVsInjured(player.sfs_user, base.damage * player.PowerBuff, 10);
						}
					}
				}
			}
			lastShootTime = Time.time;
		}

		public override void AutoAim(float deltaTime)
		{
		}

		public override void GunOn()
		{
			GameObject gameObject = gun.transform.Find("GuangJian_01").gameObject;
			GameObject gameObject2 = gun.transform.Find("GuangJian_02").gameObject;
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
			GameObject gameObject = gun.transform.Find("GuangJian_01").gameObject;
			GameObject gameObject2 = gun.transform.Find("GuangJian_02").gameObject;
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
