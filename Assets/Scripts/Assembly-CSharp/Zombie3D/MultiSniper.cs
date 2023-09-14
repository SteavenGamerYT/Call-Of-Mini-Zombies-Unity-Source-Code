using System.Collections.Generic;
using UnityEngine;

namespace Zombie3D
{
	public class MultiSniper : Weapon
	{
		protected float trimWidth = 25f;

		protected float trimHeight = 25f;

		protected List<NearestEnemyInfo> nearestEnemyInfoList;

		protected int maxLocks = 5;

		protected bool locked;

		protected float flySpeed;

		protected static int sbulletCount;

		public static Rect lockAreaRect = AutoRect.AutoPos(new Rect(230f, 200f, 500f, 250f));

		public int MaxLocks
		{
			get
			{
				return maxLocks;
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

		public List<NearestEnemyInfo> GetNearestEnemyInfoList()
		{
			return nearestEnemyInfoList;
		}

		public override WeaponType GetWeaponType()
		{
			return WeaponType.Sniper;
		}

		public override void Init()
		{
			base.MultiInit();
			hitForce = 60f;
			maxCapacity = 9999;
			nearestEnemyInfoList = new List<NearestEnemyInfo>();
		}

		public override void LoadConfig()
		{
			base.LoadConfig();
			WeaponConfig weaponConfig = gConfig.GetWeaponConfig(base.Name);
			base.damage = 0f;
			base.attackFrenquency = weaponConfig.attackRateConf.baseData;
			speedDrag = weaponConfig.moveSpeedDrag;
			range = weaponConfig.range;
			accuracy = weaponConfig.accuracyConf.baseData;
			maxGunLoad = weaponConfig.initBullet;
			sbulletCount = maxGunLoad;
			flySpeed = weaponConfig.flySpeed;
		}

		public override void changeReticle()
		{
		}

		public override void CreateGun()
		{
			GameObject gameObject = null;
			gameObject = Object.Instantiate(Resources.Load("Prefabs/Weapon/Missle")) as GameObject;
			GunConfigScript component = gameObject.GetComponent<GunConfigScript>();
			gun = Object.Instantiate(component.Gun_Instance, player.GetTransform().position, player.GetTransform().rotation);
			Object.Destroy(gameObject);
		}

		public bool AimedTarget()
		{
			if (nearestEnemyInfoList.Count == 0)
			{
				return false;
			}
			return true;
		}

		public void AddMultiTarget(Vector3 pos)
		{
			NearestEnemyInfo nearestEnemyInfo = new NearestEnemyInfo();
			nearestEnemyInfo.tar_pos = pos;
			nearestEnemyInfoList.Add(nearestEnemyInfo);
		}

		public override void Fire(float deltaTime)
		{
			foreach (NearestEnemyInfo nearestEnemyInfo in nearestEnemyInfoList)
			{
				Vector3 normalized = (nearestEnemyInfo.tar_pos - rightGun.position).normalized;
				GameObject gameObject = Object.Instantiate(rConf.isnipertile, rightGun.position + Vector3.up, Quaternion.LookRotation(-normalized));
				ProjectileScript component = gameObject.GetComponent<ProjectileScript>();
				component.dir = normalized;
				component.life = 10f;
				component.damage = base.damage;
				component.flySpeed = flySpeed;
				component.hitForce = hitForce;
				component.targetPos = nearestEnemyInfo.tar_pos;
				component.GunType = WeaponType.Sniper;
				component.explodeRadius = range;
				component.m_isMultiSniper = true;
				component.player = base.WeaponPlayer;
			}
			nearestEnemyInfoList.Clear();
			locked = false;
		}

		public override void StopFire()
		{
			if (nearestEnemyInfoList != null)
			{
				nearestEnemyInfoList.Clear();
			}
		}
	}
}
