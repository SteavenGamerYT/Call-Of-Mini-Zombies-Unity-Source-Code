using UnityEngine;

namespace Zombie3D
{
	public class WeaponFactory
	{
		protected static WeaponFactory instance;

		public static WeaponFactory GetInstance()
		{
			if (instance == null)
			{
				instance = new WeaponFactory();
			}
			return instance;
		}

		public Weapon CreateWeapon(WeaponType wType, bool isMultiWeapon)
		{
			Weapon result = null;
			if (isMultiWeapon)
			{
				switch (wType)
				{
				case WeaponType.AssaultRifle:
					result = new MultiAssaultRifle();
					break;
				case WeaponType.ShotGun:
					result = new MultiShotGun();
					break;
				case WeaponType.RocketLauncher:
					result = new MultiRocketLauncher();
					break;
				case WeaponType.MachineGun:
					result = new MultiMachineGun();
					break;
				case WeaponType.LaserGun:
					result = new MultiLaserGun();
					break;
				case WeaponType.Sniper:
					result = new MultiSniper();
					break;
				case WeaponType.Saw:
					result = new MultiSaw();
					break;
				case WeaponType.Sword:
					result = new MultiSword();
					break;
				case WeaponType.M32:
					result = new MultiGrenadeRifle();
					break;
				case WeaponType.FireGun:
					result = new MultiFireGun();
					break;
				}
			}
			else
			{
				switch (wType)
				{
				case WeaponType.AssaultRifle:
					result = new AssaultRifle();
					break;
				case WeaponType.ShotGun:
					result = new ShotGun();
					break;
				case WeaponType.RocketLauncher:
					result = new RocketLauncher();
					break;
				case WeaponType.MachineGun:
					result = new MachineGun();
					break;
				case WeaponType.LaserGun:
					result = new LaserGun();
					break;
				case WeaponType.Sniper:
					result = new Sniper();
					break;
				case WeaponType.Saw:
					result = new Saw();
					break;
				case WeaponType.Sword:
					result = new Sword();
					break;
				case WeaponType.M32:
					result = new GrenadeRifle();
					break;
				case WeaponType.FireGun:
					result = new FireGun();
					break;
				}
			}
			return result;
		}

		public GameObject CreateWeaponModel(string weaponName, Vector3 pos, Quaternion rotation)
		{
			GameObject gameObject = null;
			GameObject gameObject2 = null;
			switch (weaponName)
			{
			case "M4":
				gameObject = Object.Instantiate(Resources.Load("Prefabs/Weapon/M4")) as GameObject;
				break;
			case "MP5":
				gameObject = Object.Instantiate(Resources.Load("Prefabs/Weapon/MP5")) as GameObject;
				break;
			case "AK-47":
				gameObject = Object.Instantiate(Resources.Load("Prefabs/Weapon/AK47")) as GameObject;
				break;
			case "P90":
				gameObject = Object.Instantiate(Resources.Load("Prefabs/Weapon/P90")) as GameObject;
				break;
			case "AUG":
				gameObject = Object.Instantiate(Resources.Load("Prefabs/Weapon/AUG")) as GameObject;
				break;
			case "Winchester-1200":
				gameObject = Object.Instantiate(Resources.Load("Prefabs/Weapon/Wechester1200")) as GameObject;
				break;
			case "Remington-870":
				gameObject = Object.Instantiate(Resources.Load("Prefabs/Weapon/Remington870")) as GameObject;
				break;
			case "XM-1014":
				gameObject = Object.Instantiate(Resources.Load("Prefabs/Weapon/XM1014")) as GameObject;
				break;
			case "RPG-7":
				gameObject = Object.Instantiate(Resources.Load("Prefabs/Weapon/RPG")) as GameObject;
				break;
			case "Laser":
				gameObject = Object.Instantiate(Resources.Load("Prefabs/Weapon/LaserGun")) as GameObject;
				break;
			case "Gatling":
				gameObject = Object.Instantiate(Resources.Load("Prefabs/Weapon/Gatlin")) as GameObject;
				break;
			case "PGM":
				gameObject = Object.Instantiate(Resources.Load("Prefabs/Weapon/Missle")) as GameObject;
				break;
			case "Chainsaw":
				gameObject = Object.Instantiate(Resources.Load("Prefabs/Weapon/Saw")) as GameObject;
				break;
			case "LightSword":
				gameObject = Object.Instantiate(Resources.Load("Prefabs/Weapon/Sword")) as GameObject;
				break;
			case "M32":
				gameObject = Object.Instantiate(Resources.Load("Prefabs/Weapon/M32")) as GameObject;
				break;
			case "HellFire":
				gameObject = Object.Instantiate(Resources.Load("Prefabs/Weapon/FireGun")) as GameObject;
				break;
			default:
				gameObject = Object.Instantiate(Resources.Load("Prefabs/Weapon/M4")) as GameObject;
				break;
			}
			GunConfigScript component = gameObject.GetComponent<GunConfigScript>();
			return Object.Instantiate(component.Gun_Instance, pos, rotation);
		}
	}
}
