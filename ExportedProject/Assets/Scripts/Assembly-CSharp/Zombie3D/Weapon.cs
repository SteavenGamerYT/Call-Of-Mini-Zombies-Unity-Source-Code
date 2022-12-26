using UnityEngine;

namespace Zombie3D
{
	public abstract class Weapon
	{
		public int weapon_index;

		protected GameObject hitParticles;

		protected GameObject projectile;

		protected Camera cameraComponent;

		protected Transform cameraTransform;

		protected Transform rightGun;

		protected BaseCameraScript gameCamera;

		protected GameObject gunfire;

		protected GameObject fire_ori;

		public GameObject gun;

		protected Transform weaponBoneTrans;

		protected ResourceConfigScript rConf;

		protected AudioSource shootAudio;

		protected GameConfig gConfig;

		protected GameScene gameScene;

		protected Player player;

		protected Vector3 aimTarget;

		protected bool isCDing;

		protected float hitForce;

		protected float range;

		protected float lastShootTime;

		protected int maxCapacity;

		protected int maxGunLoad;

		protected int capacity;

		protected int bulletCount;

		protected float maxDeflection;

		protected Vector2 deflection;

		protected float ori_damage;

		protected float ori_attackFrenquency;

		protected float accuracy;

		protected float vs_damage;

		protected float vs_attackFrenquency;

		protected float speedDrag;

		protected Vector3 lastHitPosition;

		protected int price;

		public float damage
		{
			get
			{
				if (GameApp.GetInstance().GetGameState().VS_mode)
				{
					return vs_damage;
				}
				return ori_damage;
			}
			set
			{
				ori_damage = value;
			}
		}

		public float attackFrenquency
		{
			get
			{
				if (GameApp.GetInstance().GetGameState().VS_mode)
				{
					return vs_attackFrenquency;
				}
				return ori_attackFrenquency;
			}
			set
			{
				ori_attackFrenquency = value;
			}
		}

		public float VsAttackFrenquency
		{
			get
			{
				return vs_attackFrenquency;
			}
			set
			{
				vs_attackFrenquency = value;
			}
		}

		public int DamageLevel { get; set; }

		public int FrequencyLevel { get; set; }

		public int AccuracyLevel { get; set; }

		public WeaponConfig WConf { get; set; }

		public bool IsSelectedForBattle { get; set; }

		public WeaponExistState Exist { get; set; }

		public string Info
		{
			get
			{
				return Name;
			}
		}

		public string Name { get; set; }

		public int Price
		{
			get
			{
				return price;
			}
		}

		public float Accuracy
		{
			get
			{
				return accuracy;
			}
			set
			{
				accuracy = value;
			}
		}

		public Player WeaponPlayer
		{
			get
			{
				return player;
			}
			set
			{
				player = value;
			}
		}

		public int MaxGunLoad
		{
			get
			{
				return maxGunLoad;
			}
		}

		public float Damage
		{
			get
			{
				return damage;
			}
			set
			{
				damage = value;
			}
		}

		public float AttackFrequency
		{
			get
			{
				return attackFrenquency;
			}
			set
			{
				attackFrenquency = value;
			}
		}

		public virtual int BulletCount
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}

		public int Capacity
		{
			get
			{
				return capacity;
			}
		}

		public Vector2 Deflection
		{
			get
			{
				return deflection;
			}
		}

		public Weapon()
		{
		}

		public abstract void Fire(float deltaTime);

		public abstract void StopFire();

		public abstract WeaponType GetWeaponType();

		public abstract void changeReticle();

		public virtual void DoLogic()
		{
		}

		public virtual void LoadConfig()
		{
			gConfig = GameApp.GetInstance().GetGameConfig();
		}

		public float GetSpeedDrag()
		{
			return speedDrag;
		}

		public static string GetWeaponNameEnd(WeaponType wType)
		{
			string empty = string.Empty;
			switch (wType)
			{
			case WeaponType.RocketLauncher:
				return "_RPG";
			case WeaponType.ShotGun:
			case WeaponType.M32:
				return "_Shotgun";
			case WeaponType.Sniper:
				return "_RPG";
			case WeaponType.LaserGun:
			case WeaponType.FireGun:
				return string.Empty;
			case WeaponType.MachineGun:
				return string.Empty;
			case WeaponType.Saw:
			case WeaponType.Sword:
				return "_Saw";
			default:
				return string.Empty;
			}
		}

		public void Upgrade(float power, float frequency, float accur)
		{
			if (power != 0f)
			{
				damage += power;
				int num = (int)(damage * 100f);
				damage = (float)((double)num * 1.0) / 100f;
				DamageLevel++;
				GameApp.GetInstance().GetGameState().user_statistics.weapon_up_list.Add(Name + "_Power");
			}
			if (frequency != 0f)
			{
				attackFrenquency -= frequency;
				int num2 = (int)(attackFrenquency * 100f);
				attackFrenquency = (float)((double)num2 * 1.0) / 100f;
				FrequencyLevel++;
				GameApp.GetInstance().GetGameState().user_statistics.weapon_up_list.Add(Name + "_Frequency");
			}
			if (accur != 0f)
			{
				accuracy += accur;
				int num3 = (int)(accuracy * 100f);
				accuracy = (float)((double)num3 * 1.0) / 100f;
				AccuracyLevel++;
				GameApp.GetInstance().GetGameState().user_statistics.weapon_up_list.Add(Name + "_Accuracy");
			}
			if (DamageLevel + FrequencyLevel + AccuracyLevel == 10)
			{
				GameApp.GetInstance().GetGameState().Achievement.UpgradeTenTimes();
			}
		}

		public float GetNextLevelDamage()
		{
			float f = damage + damage * WConf.damageConf.upFactor;
			return Math.SignificantFigures(f, 4);
		}

		public bool IsMaxLevelDamage()
		{
			return (float)DamageLevel >= WConf.damageConf.maxLevel;
		}

		public bool IsMaxLevelCD()
		{
			return (float)FrequencyLevel >= WConf.attackRateConf.maxLevel;
		}

		public bool IsMaxLevelAccuracy()
		{
			return (float)AccuracyLevel >= WConf.accuracyConf.maxLevel;
		}

		public int GetDamageUpgradePrice()
		{
			int damageLevel = DamageLevel;
			float basePrice = WConf.damageConf.basePrice;
			float upPriceFactor = WConf.damageConf.upPriceFactor;
			float num = basePrice;
			for (int i = 0; i < damageLevel; i++)
			{
				num *= 1f + upPriceFactor;
			}
			return (int)num / 100 * 100;
		}

		public int GetFrequencyUpgradePrice()
		{
			int frequencyLevel = FrequencyLevel;
			float basePrice = WConf.attackRateConf.basePrice;
			float upPriceFactor = WConf.attackRateConf.upPriceFactor;
			float num = basePrice;
			for (int i = 0; i < frequencyLevel; i++)
			{
				num *= 1f + upPriceFactor;
			}
			return (int)num / 100 * 100;
		}

		public int GetAccuracyUpgradePrice()
		{
			int accuracyLevel = AccuracyLevel;
			float basePrice = WConf.accuracyConf.basePrice;
			float upPriceFactor = WConf.accuracyConf.upPriceFactor;
			float num = basePrice;
			for (int i = 0; i < accuracyLevel; i++)
			{
				num *= 1f + upPriceFactor;
			}
			return (int)num / 100 * 100;
		}

		public float GetNextLevelFrequency()
		{
			float f = attackFrenquency - attackFrenquency * WConf.attackRateConf.upFactor;
			return Math.SignificantFigures(f, 4);
		}

		public float GetNextLevelAccuracy()
		{
			float f = accuracy + accuracy * WConf.accuracyConf.upFactor;
			return Math.SignificantFigures(f, 4);
		}

		public float GetLastShootTime()
		{
			return lastShootTime;
		}

		public virtual void Init()
		{
			gameScene = GameApp.GetInstance().GetGameScene();
			rConf = GameApp.GetInstance().GetResourceConfig();
			gameCamera = gameScene.GetCamera();
			cameraComponent = gameCamera.GetComponent<Camera>();
			cameraTransform = gameCamera.CameraTransform;
			player = gameScene.GetPlayer();
			aimTarget = default(Vector3);
			hitParticles = rConf.hitparticles;
			projectile = rConf.projectile;
			hitForce = 0f;
			weaponBoneTrans = player.GetTransform().Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 R Clavicle/Bip01 R UpperArm/Bip01 R Forearm/Bip01 R Hand/Weapon_Dummy");
			CreateGun();
			gun.transform.parent = weaponBoneTrans;
			BindGunAndFire();
			shootAudio = gun.GetComponent<AudioSource>();
			if (shootAudio == null)
			{
			}
			GunOff();
			if (GameApp.GetInstance().GetGameState().VS_mode)
			{
				VSReset();
				SetVsParameter();
			}
		}

		public virtual void MultiInit()
		{
			gameScene = GameApp.GetInstance().GetGameScene();
			rConf = GameApp.GetInstance().GetResourceConfig();
			aimTarget = default(Vector3);
			IsSelectedForBattle = true;
			hitParticles = rConf.hitparticles;
			projectile = rConf.projectile;
			hitForce = 0f;
			weaponBoneTrans = player.GetTransform().Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 R Clavicle/Bip01 R UpperArm/Bip01 R Forearm/Bip01 R Hand/Weapon_Dummy");
			CreateGun();
			gun.transform.parent = weaponBoneTrans;
			BindGunAndFire();
			shootAudio = gun.GetComponent<AudioSource>();
			if (shootAudio == null)
			{
			}
			GunOff();
		}

		public abstract void CreateGun();

		public virtual void FireUpdate(float deltaTime)
		{
		}

		public virtual void AutoAim(float deltaTime)
		{
		}

		public virtual void ConsumeBullet(int count)
		{
			if (count <= 0)
			{
				Debug.Log("no bullets!");
			}
		}

		public void BindGunAndFire()
		{
			rightGun = gun.transform;
		}

		public virtual void GetBullet()
		{
			capacity += maxGunLoad;
			capacity = Mathf.Clamp(capacity, 0, maxCapacity);
		}

		public void AddBullets(int num)
		{
			if (GameApp.GetInstance().GetGameState().Avatar == AvatarType.Marine)
			{
				num = (int)((float)num * 1.2f);
			}
			BulletCount += num;
			BulletCount = Mathf.Clamp(BulletCount, 0, 9999);
			GameApp.GetInstance().GetGameState().user_statistics.bullets_list.Add(Name + "_BuyBullet");
		}

		public virtual void MaxBullet()
		{
			BulletCount = maxGunLoad;
		}

		public virtual void GunOn()
		{
			GameObject gameObject = gun.transform.Find("Bone01").gameObject;
			if (gameObject.GetComponent<Renderer>() == null)
			{
				gameObject = gameObject.transform.Find("Bone02").gameObject;
			}
			gameObject.GetComponent<Renderer>().enabled = true;
		}

		public virtual bool HaveBullets()
		{
			if (BulletCount == 0)
			{
				StopFire();
				ChangeNextWeapon();
				return false;
			}
			return true;
		}

		public virtual bool IsAvailably()
		{
			if (BulletCount == 0)
			{
				return false;
			}
			return true;
		}

		public void ChangeNextWeapon()
		{
			if (player != null && player.CheckWeaponAvailably())
			{
				player.NextWeapon();
			}
		}

		public virtual bool CouldMakeNextShoot()
		{
			if (Time.time - lastShootTime > attackFrenquency)
			{
				return true;
			}
			return false;
		}

		public virtual void GunOff()
		{
			GameObject gameObject = gun.transform.Find("Bone01").gameObject;
			if (gameObject.GetComponent<Renderer>() == null)
			{
				gameObject = gameObject.transform.Find("Bone02").gameObject;
			}
			gameObject.GetComponent<Renderer>().enabled = false;
			StopFire();
		}

		public void DestroyGun()
		{
			Object.Destroy(gun);
		}

		public virtual void VSReset()
		{
			if (GameApp.GetInstance().GetGameState().VS_mode)
			{
				gun.transform.localPosition = Vector3.zero;
				gun.transform.localRotation = Quaternion.Euler(new Vector3(90f, 0f, 0f));
			}
		}

		public void SetVsParameter()
		{
			if (GameApp.GetInstance().GetGameState().VS_mode)
			{
				WeaponConfig weaponConfig = gConfig.GetWeaponConfig(Name);
				vs_damage = weaponConfig.damage_param.baseVal + (float)(DamageLevel - 1) * weaponConfig.damage_param.delta;
				vs_attackFrenquency = weaponConfig.frequency_param.baseVal + (float)(FrequencyLevel - 1) * weaponConfig.frequency_param.delta;
				Debug.Log("vs weapon:" + Name + " damage:" + vs_damage + " Frenquency" + vs_attackFrenquency);
			}
		}
	}
}
