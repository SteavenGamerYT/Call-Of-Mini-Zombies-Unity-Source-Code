using Zombie3D;

public class WeaponConfig
{
	public string name;

	public WeaponType wType;

	public float moveSpeedDrag;

	public float range;

	public int price;

	public int bulletPrice;

	public int initBullet;

	public int bullet;

	public float flySpeed;

	public WeaponExistState startEquip;

	public UpgradeConfig damageConf;

	public UpgradeConfig attackRateConf;

	public UpgradeConfig accuracyConf;

	public LootConfig lootConf;

	public VSParamConfig damage_param;

	public VSParamConfig frequency_param;
}
