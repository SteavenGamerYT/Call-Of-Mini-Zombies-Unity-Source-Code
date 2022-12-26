using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
using Zombie3D;

public class GameConfig
{
	public GlobalConfig globalConf;

	public PlayerConfig playerConf;

	public WeaponAutoRect weaponAutoRectConf;

	public ArrayList avatarConfTable = new ArrayList();

	public Hashtable monsterConfTable = new Hashtable();

	public ArrayList weaponConfTable = new ArrayList();

	public Hashtable equipConfTable = new Hashtable();

	public ArrayList Multi_AchievementConfTable = new ArrayList();

	public float endless_a1;

	public float endless_b1;

	public float endless_a2;

	public float endless_b2;

	public float endless_cash;

	public float hunting_hp;

	public float hunting_damage;

	public float hunting_run_speed;

	public float hunting_time;

	public int hunting_level;

	public MonsterConfig GetMonsterConfig(string name)
	{
		return monsterConfTable[name] as MonsterConfig;
	}

	public WeaponConfig GetWeaponConfig(string name)
	{
		IEnumerator enumerator = weaponConfTable.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				WeaponConfig weaponConfig = (WeaponConfig)enumerator.Current;
				if (weaponConfig.name == name)
				{
					return weaponConfig;
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
		return null;
	}

	public AvatarConfig GetAvatarConfig(int index)
	{
		return avatarConfTable[index - 1] as AvatarConfig;
	}

	public List<WeaponConfig> GetPossibleLootWeapons(int wave)
	{
		List<WeaponConfig> list = new List<WeaponConfig>();
		IEnumerator enumerator = weaponConfTable.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				WeaponConfig weaponConfig = (WeaponConfig)enumerator.Current;
				LootConfig lootConf = weaponConfig.lootConf;
				if (wave >= lootConf.fromWave && wave <= lootConf.toWave)
				{
					list.Add(weaponConfig);
				}
			}
			return list;
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = enumerator as IDisposable) != null)
			{
				disposable.Dispose();
			}
		}
	}

	public WeaponConfig GetUnLockWeapon(int wave)
	{
		IEnumerator enumerator = weaponConfTable.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				WeaponConfig weaponConfig = (WeaponConfig)enumerator.Current;
				LootConfig lootConf = weaponConfig.lootConf;
				if (wave == lootConf.giveAtWave)
				{
					return weaponConfig;
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
		return null;
	}

	public List<WeaponConfig> GetWeapons()
	{
		List<WeaponConfig> list = new List<WeaponConfig>();
		IEnumerator enumerator = weaponConfTable.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				WeaponConfig item = (WeaponConfig)enumerator.Current;
				list.Add(item);
			}
			return list;
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = enumerator as IDisposable) != null)
			{
				disposable.Dispose();
			}
		}
	}

	public void LoadMultiAchievementFromXML(string path)
	{
		XmlReader xmlReader = null;
		StringReader stringReader = null;
		Stream stream = null;
		if (path != null)
		{
			path = Application.dataPath + path;
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			stream = File.Open(path + "config.xml", FileMode.Open);
			xmlReader = XmlReader.Create(stream);
		}
		else
		{
			TextAsset configXml = GameApp.GetInstance().GetGloabResourceConfig().configXml;
			stringReader = new StringReader(configXml.text);
			xmlReader = XmlReader.Create(stringReader);
		}
		MultiAchievementCfg multiAchievementCfg = null;
		int num = 0;
		while (xmlReader.Read())
		{
			XmlNodeType nodeType = xmlReader.NodeType;
			if (nodeType == XmlNodeType.Element)
			{
				if (xmlReader.Name == "Achievement")
				{
					multiAchievementCfg = new MultiAchievementCfg();
					LoadMultiAchieveMentConf(xmlReader, multiAchievementCfg);
					multiAchievementCfg.m_index = num++;
					multiAchievementCfg.finish = GameApp.GetInstance().GetGameState().MultiAchievementData[multiAchievementCfg.m_index];
					Multi_AchievementConfTable.Add(multiAchievementCfg);
				}
				else if (xmlReader.Name == "A_Damage")
				{
					LoadMultiAchieveMentConfDamage(xmlReader, multiAchievementCfg);
				}
				else if (xmlReader.Name == "A_Reward")
				{
					LoadMultiAchieveMentConfReward(xmlReader, multiAchievementCfg);
				}
				else if (xmlReader.Name == "A_Weapon")
				{
					LoadMultiAchieveMentConfWeapon(xmlReader, multiAchievementCfg);
				}
				else if (xmlReader.Name == "A_Time")
				{
					LoadMultiAchieveMentConfTime(xmlReader, multiAchievementCfg);
				}
				else if (xmlReader.Name == "A_Monster")
				{
					LoadMultiAchieveMentConfMonster(xmlReader, multiAchievementCfg);
				}
			}
		}
		if (xmlReader != null)
		{
			xmlReader.Close();
		}
		if (stringReader != null)
		{
			stringReader.Close();
		}
		if (stream != null)
		{
			stream.Close();
		}
	}

	public void LoadFromXML(string path)
	{
		globalConf = new GlobalConfig();
		playerConf = new PlayerConfig();
		weaponAutoRectConf = new WeaponAutoRect();
		XmlReader xmlReader = null;
		StringReader stringReader = null;
		Stream stream = null;
		if (path != null)
		{
			path = Application.dataPath + path;
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			stream = File.Open(path + "config.xml", FileMode.Open);
			xmlReader = XmlReader.Create(stream);
		}
		else
		{
			TextAsset configXml = GameApp.GetInstance().GetGloabResourceConfig().configXml;
			stringReader = new StringReader(configXml.text);
			xmlReader = XmlReader.Create(stringReader);
		}
		WeaponConfig weaponConfig = null;
		AvatarConfig avatarConfig = null;
		while (xmlReader.Read())
		{
			XmlNodeType nodeType = xmlReader.NodeType;
			if (nodeType == XmlNodeType.Element)
			{
				if (xmlReader.Name == "Global")
				{
					LoadGlobalConf(xmlReader);
				}
				else if (xmlReader.Name == "Player")
				{
					LoadPlayerConf(xmlReader);
				}
				else if (xmlReader.Name == "WeaponAutoRect")
				{
					LoadAutoRectConf(xmlReader);
				}
				else if (xmlReader.Name == "EndlessPara")
				{
					LoadEndlessPara(xmlReader);
				}
				else if (xmlReader.Name == "Hunting")
				{
					LoadHuntingPara(xmlReader);
				}
				else if (xmlReader.Name == "Avatar")
				{
					avatarConfig = new AvatarConfig();
					LoadAvatarConf(xmlReader, avatarConfig);
					avatarConfTable.Add(avatarConfig);
				}
				else if (xmlReader.Name == "Monster")
				{
					LoadMonstersConf(xmlReader);
				}
				else if (xmlReader.Name == "Weapon")
				{
					weaponConfig = new WeaponConfig();
					LoadWeaponConf(xmlReader, weaponConfig);
					weaponConfTable.Add(weaponConfig);
				}
				else if (xmlReader.Name == "Damage")
				{
					LoadUpgradeConf(xmlReader, weaponConfig, "Damage");
				}
				else if (xmlReader.Name == "Frequency")
				{
					LoadUpgradeConf(xmlReader, weaponConfig, "Frequency");
				}
				else if (xmlReader.Name == "Accuracy")
				{
					LoadUpgradeConf(xmlReader, weaponConfig, "Accuracy");
				}
				else if (xmlReader.Name == "Loot")
				{
					LoadLootWeapon(xmlReader, weaponConfig);
				}
				else if (xmlReader.Name == "VSDamagePara")
				{
					LoadVSWeaponDamagePara(xmlReader, weaponConfig);
				}
				else if (xmlReader.Name == "VSFrequencyPara")
				{
					LoadVSWeaponFrequencyPara(xmlReader, weaponConfig);
				}
			}
		}
		if (xmlReader != null)
		{
			xmlReader.Close();
		}
		if (stringReader != null)
		{
			stringReader.Close();
		}
		if (stream != null)
		{
			stream.Close();
		}
	}

	private void LoadLootWeapon(XmlReader reader, WeaponConfig weaponConf)
	{
		LootConfig lootConfig = new LootConfig();
		if (reader.HasAttributes)
		{
			while (reader.MoveToNextAttribute())
			{
				if (reader.Name == "giveAtWave")
				{
					lootConfig.giveAtWave = int.Parse(reader.Value);
				}
				else if (reader.Name == "fromWave")
				{
					lootConfig.fromWave = int.Parse(reader.Value);
				}
				else if (reader.Name == "toWave")
				{
					lootConfig.toWave = int.Parse(reader.Value);
				}
				else if (reader.Name == "lootRate")
				{
					lootConfig.rate = float.Parse(reader.Value);
				}
				else if (reader.Name == "increaseRate")
				{
					lootConfig.increaseRate = float.Parse(reader.Value);
				}
			}
		}
		weaponConf.lootConf = lootConfig;
	}

	private void LoadVSWeaponDamagePara(XmlReader reader, WeaponConfig weaponConf)
	{
		VSParamConfig vSParamConfig = new VSParamConfig();
		if (reader.HasAttributes)
		{
			while (reader.MoveToNextAttribute())
			{
				if (reader.Name == "base")
				{
					vSParamConfig.baseVal = float.Parse(reader.Value);
				}
				else if (reader.Name == "delta")
				{
					vSParamConfig.delta = float.Parse(reader.Value);
				}
			}
		}
		weaponConf.damage_param = vSParamConfig;
	}

	private void LoadVSWeaponFrequencyPara(XmlReader reader, WeaponConfig weaponConf)
	{
		VSParamConfig vSParamConfig = new VSParamConfig();
		if (reader.HasAttributes)
		{
			while (reader.MoveToNextAttribute())
			{
				if (reader.Name == "base")
				{
					vSParamConfig.baseVal = float.Parse(reader.Value);
				}
				else if (reader.Name == "delta")
				{
					vSParamConfig.delta = float.Parse(reader.Value);
				}
			}
		}
		weaponConf.frequency_param = vSParamConfig;
	}

	private void LoadAvatarConf(XmlReader reader, AvatarConfig avatarConf)
	{
		if (!reader.HasAttributes)
		{
			return;
		}
		while (reader.MoveToNextAttribute())
		{
			if (reader.Name == "price")
			{
				avatarConf.price = int.Parse(reader.Value);
			}
		}
	}

	private void LoadGlobalConf(XmlReader reader)
	{
		if (!reader.HasAttributes)
		{
			return;
		}
		while (reader.MoveToNextAttribute())
		{
			if (reader.Name == "startMoney")
			{
				globalConf.startMoney = int.Parse(reader.Value);
			}
			else if (reader.Name == "enemyLimit")
			{
				globalConf.enemyLimit = int.Parse(reader.Value);
			}
			else if (reader.Name == "resolution")
			{
				globalConf.resolution = float.Parse(reader.Value);
			}
		}
	}

	private void LoadAutoRectConf(XmlReader reader)
	{
		if (!reader.HasAttributes)
		{
			return;
		}
		while (reader.MoveToNextAttribute())
		{
			if (reader.Name == "width")
			{
				weaponAutoRectConf.width = int.Parse(reader.Value);
			}
			else if (reader.Name == "height")
			{
				weaponAutoRectConf.height = int.Parse(reader.Value);
			}
		}
		AssaultRifle.lockAreaRect = AutoRect.AutoPos(new Rect(480 - GameApp.GetInstance().GetGameConfig().weaponAutoRectConf.width / 2, 320 - GameApp.GetInstance().GetGameConfig().weaponAutoRectConf.height / 2, GameApp.GetInstance().GetGameConfig().weaponAutoRectConf.width, GameApp.GetInstance().GetGameConfig().weaponAutoRectConf.height));
		MachineGun.lockAreaRect = AutoRect.AutoPos(new Rect(480 - GameApp.GetInstance().GetGameConfig().weaponAutoRectConf.width / 2, 320 - GameApp.GetInstance().GetGameConfig().weaponAutoRectConf.height / 2, GameApp.GetInstance().GetGameConfig().weaponAutoRectConf.width, GameApp.GetInstance().GetGameConfig().weaponAutoRectConf.height));
	}

	private void LoadEndlessPara(XmlReader reader)
	{
		if (!reader.HasAttributes)
		{
			return;
		}
		while (reader.MoveToNextAttribute())
		{
			if (reader.Name == "a1")
			{
				endless_a1 = float.Parse(reader.Value);
			}
			else if (reader.Name == "b1")
			{
				endless_b1 = float.Parse(reader.Value);
			}
			if (reader.Name == "a2")
			{
				endless_a2 = float.Parse(reader.Value);
			}
			else if (reader.Name == "b2")
			{
				endless_b2 = float.Parse(reader.Value);
			}
			else if (reader.Name == "cash")
			{
				endless_cash = float.Parse(reader.Value);
			}
		}
	}

	private void LoadHuntingPara(XmlReader reader)
	{
		if (!reader.HasAttributes)
		{
			return;
		}
		while (reader.MoveToNextAttribute())
		{
			if (reader.Name == "hp")
			{
				hunting_hp = float.Parse(reader.Value);
			}
			else if (reader.Name == "damage")
			{
				hunting_damage = float.Parse(reader.Value);
			}
			else if (reader.Name == "runspeed")
			{
				hunting_run_speed = float.Parse(reader.Value);
			}
			else if (reader.Name == "time")
			{
				hunting_time = float.Parse(reader.Value);
			}
			else if (reader.Name == "level")
			{
				hunting_level = int.Parse(reader.Value);
			}
		}
	}

	private void LoadPlayerConf(XmlReader reader)
	{
		if (!reader.HasAttributes)
		{
			return;
		}
		while (reader.MoveToNextAttribute())
		{
			if (reader.Name == "hp")
			{
				playerConf.hp = float.Parse(reader.Value);
			}
			else if (reader.Name == "walkSpeed")
			{
				playerConf.walkSpeed = float.Parse(reader.Value);
			}
			else if (reader.Name == "armorPrice")
			{
				playerConf.upgradeArmorPrice = int.Parse(reader.Value);
			}
			else if (reader.Name == "upPriceFactor")
			{
				playerConf.upPriceFactor = float.Parse(reader.Value);
			}
			else if (reader.Name == "maxArmorLevel")
			{
				playerConf.maxArmorLevel = int.Parse(reader.Value);
			}
		}
	}

	private void LoadMonstersConf(XmlReader reader)
	{
		MonsterConfig monsterConfig = new MonsterConfig();
		string key = string.Empty;
		if (reader.HasAttributes)
		{
			while (reader.MoveToNextAttribute())
			{
				if (reader.Name == "name")
				{
					key = reader.Value;
				}
				else if (reader.Name == "damage")
				{
					monsterConfig.damage = float.Parse(reader.Value);
				}
				else if (reader.Name == "attackRate")
				{
					monsterConfig.attackRate = float.Parse(reader.Value);
				}
				else if (reader.Name == "walkSpeed")
				{
					monsterConfig.walkSpeed = float.Parse(reader.Value);
				}
				else if (reader.Name == "hp")
				{
					monsterConfig.hp = float.Parse(reader.Value);
				}
				else if (reader.Name == "rushSpeed")
				{
					monsterConfig.rushSpeed = float.Parse(reader.Value);
				}
				else if (reader.Name == "rushDamage")
				{
					monsterConfig.rushDamage = float.Parse(reader.Value);
				}
				else if (reader.Name == "rushAttack")
				{
					monsterConfig.rushAttackDamage = float.Parse(reader.Value);
				}
				else if (reader.Name == "rushRate")
				{
					monsterConfig.rushInterval = float.Parse(reader.Value);
				}
				else if (reader.Name == "score")
				{
					monsterConfig.score = int.Parse(reader.Value);
				}
				else if (reader.Name == "lootCash")
				{
					monsterConfig.lootCash = int.Parse(reader.Value);
				}
			}
		}
		monsterConfTable.Add(key, monsterConfig);
	}

	private void LoadWeaponConf(XmlReader reader, WeaponConfig weaponConf)
	{
		if (!reader.HasAttributes)
		{
			return;
		}
		while (reader.MoveToNextAttribute())
		{
			if (reader.Name == "name")
			{
				weaponConf.name = reader.Value;
			}
			else if (reader.Name == "type")
			{
				switch (reader.Value)
				{
				case "Rifle":
					weaponConf.wType = WeaponType.AssaultRifle;
					break;
				case "ShotGun":
					weaponConf.wType = WeaponType.ShotGun;
					break;
				case "RPG":
					weaponConf.wType = WeaponType.RocketLauncher;
					break;
				case "MachineGun":
					weaponConf.wType = WeaponType.MachineGun;
					break;
				case "Laser":
					weaponConf.wType = WeaponType.LaserGun;
					break;
				case "Sniper":
					weaponConf.wType = WeaponType.Sniper;
					break;
				case "Saw":
					weaponConf.wType = WeaponType.Saw;
					break;
				case "Sword":
					weaponConf.wType = WeaponType.Sword;
					break;
				case "M32":
					weaponConf.wType = WeaponType.M32;
					break;
				case "HellFire":
					weaponConf.wType = WeaponType.FireGun;
					break;
				}
			}
			else if (reader.Name == "moveSpeedDrag")
			{
				weaponConf.moveSpeedDrag = float.Parse(reader.Value);
			}
			else if (reader.Name == "range")
			{
				weaponConf.range = float.Parse(reader.Value);
			}
			else if (reader.Name == "price")
			{
				weaponConf.price = int.Parse(reader.Value);
			}
			else if (reader.Name == "bulletPrice")
			{
				weaponConf.bulletPrice = int.Parse(reader.Value);
			}
			else if (reader.Name == "initBullet")
			{
				weaponConf.initBullet = int.Parse(reader.Value);
			}
			else if (reader.Name == "bullet")
			{
				weaponConf.bullet = int.Parse(reader.Value);
			}
			else if (reader.Name == "flySpeed")
			{
				weaponConf.flySpeed = float.Parse(reader.Value);
			}
			else if (reader.Name == "startEquip")
			{
				weaponConf.startEquip = (WeaponExistState)int.Parse(reader.Value);
			}
		}
	}

	private void LoadUpgradeConf(XmlReader reader, WeaponConfig weaponConf, string uType)
	{
		UpgradeConfig upgradeConfig = new UpgradeConfig();
		if (reader.HasAttributes)
		{
			while (reader.MoveToNextAttribute())
			{
				if (reader.Name == "base")
				{
					upgradeConfig.baseData = float.Parse(reader.Value);
				}
				else if (reader.Name == "upFactor")
				{
					upgradeConfig.upFactor = float.Parse(reader.Value);
				}
				else if (reader.Name == "basePrice")
				{
					upgradeConfig.basePrice = float.Parse(reader.Value);
				}
				else if (reader.Name == "upPriceFactor")
				{
					upgradeConfig.upPriceFactor = float.Parse(reader.Value);
				}
				else if (reader.Name == "maxLevel")
				{
					upgradeConfig.maxLevel = int.Parse(reader.Value);
				}
			}
		}
		switch (uType)
		{
		case "Damage":
			weaponConf.damageConf = upgradeConfig;
			break;
		case "Frequency":
			weaponConf.attackRateConf = upgradeConfig;
			break;
		case "Accuracy":
			weaponConf.accuracyConf = upgradeConfig;
			break;
		}
	}

	private void LoadMultiAchieveMentConf(XmlReader reader, MultiAchievementCfg AchievementConf)
	{
		if (!reader.HasAttributes)
		{
			return;
		}
		while (reader.MoveToNextAttribute())
		{
			if (reader.Name == "name")
			{
				AchievementConf.name = reader.Value;
			}
			else if (reader.Name == "type")
			{
				string value = reader.Value;
				AchievementConf.SetTypeWith(value);
			}
			else if (reader.Name == "content")
			{
				AchievementConf.content = reader.Value;
			}
			else if (reader.Name == "class")
			{
				AchievementConf.m_class = reader.Value;
			}
			else if (reader.Name == "level")
			{
				AchievementConf.level = int.Parse(reader.Value);
			}
			else if (reader.Name == "iconbk")
			{
				AchievementConf.icon_bk = reader.Value;
			}
			else if (reader.Name == "icon")
			{
				AchievementConf.icon = reader.Value;
			}
		}
	}

	private void LoadMultiAchieveMentConfDamage(XmlReader reader, MultiAchievementCfg AchievementConf)
	{
		if (!reader.HasAttributes)
		{
			return;
		}
		while (reader.MoveToNextAttribute())
		{
			if (reader.Name == "val")
			{
				AchievementConf.total_damage = float.Parse(reader.Value);
			}
		}
	}

	private void LoadMultiAchieveMentConfReward(XmlReader reader, MultiAchievementCfg AchievementConf)
	{
		if (!reader.HasAttributes)
		{
			return;
		}
		while (reader.MoveToNextAttribute())
		{
			if (reader.Name == "cash")
			{
				AchievementConf.reward_cash = int.Parse(reader.Value);
			}
			else if (reader.Name == "avata")
			{
				AchievementConf.reward_avata = (AvatarType)int.Parse(reader.Value);
			}
			else if (reader.Name == "weapon")
			{
				AchievementConf.reward_weapon = reader.Value;
			}
		}
	}

	private void LoadMultiAchieveMentConfWeapon(XmlReader reader, MultiAchievementCfg AchievementConf)
	{
		AchievementConf.weapon_type_kill = new MultiAchievementCfg.WeaponTypeKill();
		if (!reader.HasAttributes)
		{
			return;
		}
		while (reader.MoveToNextAttribute())
		{
			if (reader.Name == "type")
			{
				AchievementConf.weapon_type_kill.type = (WeaponType)int.Parse(reader.Value);
			}
			else if (reader.Name == "count")
			{
				AchievementConf.weapon_type_kill.count = int.Parse(reader.Value);
			}
		}
	}

	private void LoadMultiAchieveMentConfTime(XmlReader reader, MultiAchievementCfg AchievementConf)
	{
		if (!reader.HasAttributes)
		{
			return;
		}
		while (reader.MoveToNextAttribute())
		{
			if (reader.Name == "val")
			{
				AchievementConf.battle_time = float.Parse(reader.Value);
			}
		}
	}

	private void LoadMultiAchieveMentConfMonster(XmlReader reader, MultiAchievementCfg AchievementConf)
	{
		MultiAchievementCfg.MonsterTypeKill monsterTypeKill = new MultiAchievementCfg.MonsterTypeKill();
		if (!reader.HasAttributes)
		{
			return;
		}
		while (reader.MoveToNextAttribute())
		{
			if (reader.Name == "type")
			{
				monsterTypeKill.type = (EnemyType)int.Parse(reader.Value);
			}
			else if (reader.Name == "count")
			{
				monsterTypeKill.count = int.Parse(reader.Value);
				if (monsterTypeKill.type == EnemyType.E_NONE)
				{
					AchievementConf.total_kill = monsterTypeKill.count;
				}
			}
		}
		AchievementConf.monster_type_kill.Add(monsterTypeKill);
	}
}
