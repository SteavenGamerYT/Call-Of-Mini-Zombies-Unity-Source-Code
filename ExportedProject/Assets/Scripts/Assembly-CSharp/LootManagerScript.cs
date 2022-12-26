using System.Collections.Generic;
using UnityEngine;
using Zombie3D;

internal class LootManagerScript : MonoBehaviour
{
	public float dropRate = 1f;

	public ItemType[] itemTables = new ItemType[10];

	public float[] rateTables = new float[10];

	protected bool _isPrey;

	private void Start()
	{
	}

	private void Update()
	{
	}

	private WeaponConfig BonusEquipment(int wave)
	{
		WeaponConfig result = null;
		GameScene gameScene = GameApp.GetInstance().GetGameScene();
		if (gameScene.BonusWeapon == null)
		{
			GameConfig gameConfig = GameApp.GetInstance().GetGameConfig();
			List<WeaponConfig> possibleLootWeapons = gameConfig.GetPossibleLootWeapons(wave);
			int count = possibleLootWeapons.Count;
			float[] array = new float[count];
			for (int i = 0; i < count; i++)
			{
				float num = possibleLootWeapons[i].lootConf.increaseRate * (float)(wave - possibleLootWeapons[i].lootConf.fromWave);
				array[i] = possibleLootWeapons[i].lootConf.rate + num;
				if (GameApp.GetInstance().GetGameState().Avatar == AvatarType.Nerd)
				{
					array[i] *= 1.3f;
				}
			}
			float value = Random.value;
			float num2 = 0f;
			for (int j = 0; j < array.Length; j++)
			{
				if (array[j] > 0f && value <= num2 + array[j])
				{
					result = possibleLootWeapons[j];
					break;
				}
				num2 += array[j];
			}
		}
		return result;
	}

	public static void LootSpawnItem(ItemType itemType, Vector3 position)
	{
		ResourceConfigScript resourceConfig = GameApp.GetInstance().GetResourceConfig();
		GameObject gameObject = null;
		GameObject original = GameApp.GetInstance().GetEnemyResourceConfig().enemy[6];
		switch (itemType)
		{
		case ItemType.Hp:
			gameObject = Object.Instantiate(resourceConfig.itemHP, position, Quaternion.identity);
			gameObject.GetComponent<ItemScript>().itemType = itemType;
			break;
		case ItemType.Power:
			gameObject = Object.Instantiate(resourceConfig.itemPower, position, Quaternion.identity);
			gameObject.GetComponent<ItemScript>().itemType = itemType;
			break;
		case ItemType.Gold:
			gameObject = Object.Instantiate(resourceConfig.itemGold, position, Quaternion.identity);
			gameObject.GetComponent<ItemScript>().itemType = itemType;
			break;
		case ItemType.Gold_Big:
			gameObject = Object.Instantiate(resourceConfig.itemGold_Big, position, Quaternion.identity);
			gameObject.GetComponent<ItemScript>().itemType = itemType;
			break;
		case ItemType.Monster:
		{
			gameObject = Object.Instantiate(original, position, Quaternion.identity);
			gameObject.name = "E_" + GameApp.GetInstance().GetGameScene().GetNextEnemyID();
			Enemy enemy = new Dog();
			enemy.IsElite = false;
			enemy.Init(gameObject);
			enemy.EnemyType = EnemyType.E_DOG;
			enemy.Name = gameObject.name;
			GameApp.GetInstance().GetGameScene().GetEnemies()
				.Add(gameObject.name, enemy);
			break;
		}
		case ItemType.RandomBullets:
			switch (GameApp.GetInstance().GetGameState().RandomBattleWeapons())
			{
			case WeaponType.AssaultRifle:
				gameObject = Object.Instantiate(resourceConfig.itemAssaultGun, position, Quaternion.identity);
				gameObject.AddComponent<ItemScript>();
				gameObject.AddComponent<SphereCollider>().isTrigger = true;
				gameObject.layer = 20;
				gameObject.GetComponent<ItemScript>().itemType = ItemType.AssaultGun;
				break;
			case WeaponType.ShotGun:
				gameObject = Object.Instantiate(resourceConfig.itemShotGun, position, Quaternion.identity);
				gameObject.AddComponent<ItemScript>();
				gameObject.AddComponent<SphereCollider>().isTrigger = true;
				gameObject.layer = 20;
				gameObject.GetComponent<ItemScript>().itemType = ItemType.ShotGun;
				break;
			case WeaponType.RocketLauncher:
				gameObject = Object.Instantiate(resourceConfig.itemRocketLauncer, position, Quaternion.identity);
				gameObject.AddComponent<ItemScript>();
				gameObject.AddComponent<SphereCollider>().isTrigger = true;
				gameObject.layer = 20;
				gameObject.GetComponent<ItemScript>().itemType = ItemType.RocketLauncer;
				break;
			case WeaponType.LaserGun:
				gameObject = Object.Instantiate(resourceConfig.itemLaser, position, Quaternion.identity);
				gameObject.AddComponent<ItemScript>();
				gameObject.AddComponent<SphereCollider>().isTrigger = true;
				gameObject.layer = 20;
				gameObject.GetComponent<ItemScript>().itemType = ItemType.LaserGun;
				break;
			case WeaponType.Sniper:
				gameObject = Object.Instantiate(resourceConfig.itemMissle, position, Quaternion.identity);
				gameObject.AddComponent<ItemScript>();
				gameObject.AddComponent<SphereCollider>().isTrigger = true;
				gameObject.layer = 20;
				gameObject.GetComponent<ItemScript>().itemType = ItemType.Sniper;
				break;
			case WeaponType.M32:
				gameObject = Object.Instantiate(resourceConfig.itemM32, position, Quaternion.identity);
				gameObject.AddComponent<ItemScript>();
				gameObject.AddComponent<SphereCollider>().isTrigger = true;
				gameObject.layer = 20;
				gameObject.GetComponent<ItemScript>().itemType = ItemType.M32;
				break;
			case WeaponType.FireGun:
				gameObject = Object.Instantiate(resourceConfig.itemFire, position, Quaternion.identity);
				gameObject.AddComponent<ItemScript>();
				gameObject.AddComponent<SphereCollider>().isTrigger = true;
				gameObject.layer = 20;
				gameObject.GetComponent<ItemScript>().itemType = ItemType.Fire;
				break;
			case WeaponType.MachineGun:
				gameObject = Object.Instantiate(resourceConfig.itemGatlin, position, Quaternion.identity);
				gameObject.AddComponent<ItemScript>();
				gameObject.AddComponent<SphereCollider>().isTrigger = true;
				gameObject.layer = 20;
				gameObject.GetComponent<ItemScript>().itemType = ItemType.MachineGun;
				break;
			case WeaponType.Saw:
			case WeaponType.NurseSaliva:
			case WeaponType.Sword:
				break;
			}
			break;
		}
	}

	public void SpawnItem(ItemType itemType)
	{
		ResourceConfigScript resourceConfig = GameApp.GetInstance().GetResourceConfig();
		GameObject gameObject = null;
		Ray ray = new Ray(base.transform.position + Vector3.up * 1f, Vector3.down);
		float num = 10000.1f;
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, 100f, 32768))
		{
			num = hitInfo.point.y;
		}
		if (GameApp.GetInstance().GetGameScene().GetPlayer()
			.GetAvatarType() == AvatarType.Pastor)
		{
			itemType = ItemType.Hp;
		}
		if (GameApp.GetInstance().GetGameState().endless_multiplayer)
		{
			Packet packet = CGEnemyLootPacket.MakePacket((uint)itemType, new Vector3(base.transform.position.x, num + 1f, base.transform.position.z));
			GameApp.GetInstance().GetGameState().net_com.Send(packet);
		}
		GameObject original = GameApp.GetInstance().GetEnemyResourceConfig().enemy[6];
		switch (itemType)
		{
		case ItemType.Hp:
			gameObject = Object.Instantiate(resourceConfig.itemHP, new Vector3(base.transform.position.x, num + 1f, base.transform.position.z), Quaternion.identity);
			gameObject.GetComponent<ItemScript>().itemType = itemType;
			break;
		case ItemType.Power:
			gameObject = Object.Instantiate(resourceConfig.itemPower, new Vector3(base.transform.position.x, num + 1f, base.transform.position.z), Quaternion.identity);
			gameObject.GetComponent<ItemScript>().itemType = itemType;
			break;
		case ItemType.Gold:
			gameObject = Object.Instantiate(resourceConfig.itemGold, new Vector3(base.transform.position.x, num + 1f, base.transform.position.z), Quaternion.identity);
			gameObject.GetComponent<ItemScript>().itemType = itemType;
			break;
		case ItemType.Gold_Big:
			gameObject = Object.Instantiate(resourceConfig.itemGold_Big, new Vector3(base.transform.position.x, num + 1f, base.transform.position.z), Quaternion.identity);
			gameObject.GetComponent<ItemScript>().itemType = itemType;
			break;
		case ItemType.Monster:
		{
			if (GameApp.GetInstance().GetGameScene().GetPlayer()
				.GetAvatarType() == AvatarType.Pastor)
			{
				gameObject = Object.Instantiate(resourceConfig.itemHP, new Vector3(base.transform.position.x, num + 1f, base.transform.position.z), Quaternion.identity);
				gameObject.GetComponent<ItemScript>().itemType = ItemType.Hp;
				break;
			}
			gameObject = Object.Instantiate(original, new Vector3(base.transform.position.x, num + 1f, base.transform.position.z), Quaternion.identity);
			gameObject.name = "E_" + GameApp.GetInstance().GetGameScene().GetNextEnemyID();
			Enemy enemy = new Dog();
			enemy.IsElite = false;
			enemy.Init(gameObject);
			enemy.EnemyType = EnemyType.E_DOG;
			enemy.Name = gameObject.name;
			GameApp.GetInstance().GetGameScene().GetEnemies()
				.Add(gameObject.name, enemy);
			break;
		}
		case ItemType.RandomBullets:
			switch (GameApp.GetInstance().GetGameState().RandomBattleWeapons())
			{
			case WeaponType.AssaultRifle:
				gameObject = Object.Instantiate(resourceConfig.itemAssaultGun, new Vector3(base.transform.position.x, num + 1f, base.transform.position.z), Quaternion.identity);
				gameObject.AddComponent<ItemScript>();
				gameObject.AddComponent<SphereCollider>().isTrigger = true;
				gameObject.layer = 20;
				gameObject.GetComponent<ItemScript>().itemType = ItemType.AssaultGun;
				break;
			case WeaponType.ShotGun:
				gameObject = Object.Instantiate(resourceConfig.itemShotGun, new Vector3(base.transform.position.x, num + 1f, base.transform.position.z), Quaternion.identity);
				gameObject.AddComponent<ItemScript>();
				gameObject.AddComponent<SphereCollider>().isTrigger = true;
				gameObject.layer = 20;
				gameObject.GetComponent<ItemScript>().itemType = ItemType.ShotGun;
				break;
			case WeaponType.RocketLauncher:
				gameObject = Object.Instantiate(resourceConfig.itemRocketLauncer, new Vector3(base.transform.position.x, num + 1f, base.transform.position.z), Quaternion.identity);
				gameObject.AddComponent<ItemScript>();
				gameObject.AddComponent<SphereCollider>().isTrigger = true;
				gameObject.layer = 20;
				gameObject.GetComponent<ItemScript>().itemType = ItemType.RocketLauncer;
				break;
			case WeaponType.LaserGun:
				gameObject = Object.Instantiate(resourceConfig.itemLaser, new Vector3(base.transform.position.x, num + 1f, base.transform.position.z), Quaternion.identity);
				gameObject.AddComponent<ItemScript>();
				gameObject.AddComponent<SphereCollider>().isTrigger = true;
				gameObject.layer = 20;
				gameObject.GetComponent<ItemScript>().itemType = ItemType.LaserGun;
				break;
			case WeaponType.Sniper:
				gameObject = Object.Instantiate(resourceConfig.itemMissle, new Vector3(base.transform.position.x, num + 1f, base.transform.position.z), Quaternion.identity);
				gameObject.AddComponent<ItemScript>();
				gameObject.AddComponent<SphereCollider>().isTrigger = true;
				gameObject.layer = 20;
				gameObject.GetComponent<ItemScript>().itemType = ItemType.Sniper;
				break;
			case WeaponType.M32:
				gameObject = Object.Instantiate(resourceConfig.itemM32, new Vector3(base.transform.position.x, num + 1f, base.transform.position.z), Quaternion.identity);
				gameObject.AddComponent<ItemScript>();
				gameObject.AddComponent<SphereCollider>().isTrigger = true;
				gameObject.layer = 20;
				gameObject.GetComponent<ItemScript>().itemType = ItemType.M32;
				break;
			case WeaponType.FireGun:
				gameObject = Object.Instantiate(resourceConfig.itemFire, new Vector3(base.transform.position.x, num + 1f, base.transform.position.z), Quaternion.identity);
				gameObject.AddComponent<ItemScript>();
				gameObject.AddComponent<SphereCollider>().isTrigger = true;
				gameObject.layer = 20;
				gameObject.GetComponent<ItemScript>().itemType = ItemType.Fire;
				break;
			case WeaponType.MachineGun:
				gameObject = Object.Instantiate(resourceConfig.itemGatlin, new Vector3(base.transform.position.x, num + 1f, base.transform.position.z), Quaternion.identity);
				gameObject.AddComponent<ItemScript>();
				gameObject.AddComponent<SphereCollider>().isTrigger = true;
				gameObject.layer = 20;
				gameObject.GetComponent<ItemScript>().itemType = ItemType.MachineGun;
				break;
			case WeaponType.Saw:
			case WeaponType.NurseSaliva:
			case WeaponType.Sword:
				break;
			}
			break;
		}
	}

	public void OnLoot(bool isPrey)
	{
		if (isPrey)
		{
			_isPrey = isPrey;
			SpawnItem(ItemType.Gold_Big);
			return;
		}
		float value = Random.value;
		float num = dropRate;
		if (GameApp.GetInstance().GetGameState().Avatar == AvatarType.Nerd)
		{
			num *= 1.3f;
		}
		if (!(value < num))
		{
			return;
		}
		value = Random.value;
		float num2 = 0f;
		for (int i = 0; i < itemTables.Length; i++)
		{
			if (rateTables[i] > 0f && value <= num2 + rateTables[i])
			{
				SpawnItem(itemTables[i]);
				break;
			}
			num2 += rateTables[i];
		}
	}
}
