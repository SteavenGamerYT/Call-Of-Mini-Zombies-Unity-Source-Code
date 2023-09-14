using UnityEngine;
using Zombie3D;

public class UpgradeScroll : MonoBehaviour
{
	private void Awake()
	{
		GameObject gameObject = null;
		gameObject = Object.Instantiate(Resources.Load("Prefabs/TUI/Upgrade_Weapon_Item")) as GameObject;
		gameObject.transform.parent = base.transform;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.name = "Item_Rescue";
		gameObject.GetComponent<TUIMeshSprite>().frameName = "Rescue_Packet";
		gameObject.GetComponent<WeaponItemData>().SetWeapon(null);
		gameObject.GetComponent<WeaponItemData>().is_rescue = true;
		gameObject.GetComponent<UpgradeWeaponItem>().SetTipFrame(string.Empty);
		base.gameObject.GetComponent<ScaleScroller>().item_buttons.Add(gameObject);
		gameObject = Object.Instantiate(Resources.Load("Prefabs/TUI/Upgrade_Weapon_Item")) as GameObject;
		gameObject.transform.parent = base.transform;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.name = "Item_Rebirth";
		gameObject.GetComponent<TUIMeshSprite>().frameName = "Rebirth_Packet";
		gameObject.GetComponent<WeaponItemData>().SetWeapon(null);
		gameObject.GetComponent<WeaponItemData>().is_rebirth = true;
		gameObject.GetComponent<UpgradeWeaponItem>().SetTipFrame(string.Empty);
		base.gameObject.GetComponent<ScaleScroller>().item_buttons.Add(gameObject);
		gameObject = Object.Instantiate(Resources.Load("Prefabs/TUI/Upgrade_Weapon_Item")) as GameObject;
		gameObject.transform.parent = base.transform;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.name = "Item_Armor";
		gameObject.GetComponent<TUIMeshSprite>().frameName = "ARMOR";
		gameObject.GetComponent<WeaponItemData>().SetWeapon(null);
		gameObject.GetComponent<WeaponItemData>().is_armor = true;
		gameObject.GetComponent<UpgradeWeaponItem>().SetTipFrame(string.Empty);
		base.gameObject.GetComponent<ScaleScroller>().item_buttons.Add(gameObject);
		foreach (Weapon weapon in GameApp.GetInstance().GetGameState().GetWeapons())
		{
			gameObject = Object.Instantiate(Resources.Load("Prefabs/TUI/Upgrade_Weapon_Item")) as GameObject;
			gameObject.transform.parent = base.transform;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.name = "Item_" + weapon.Name;
			gameObject.GetComponent<TUIMeshSprite>().frameName = weapon.Name;
			gameObject.GetComponent<WeaponItemData>().SetWeapon(weapon);
			base.gameObject.GetComponent<ScaleScroller>().item_buttons.Add(gameObject);
			if (weapon.Exist == WeaponExistState.Owned)
			{
				gameObject.GetComponent<UpgradeWeaponItem>().SetTipFrame(string.Empty);
			}
			else if (weapon.Exist == WeaponExistState.Locked)
			{
				gameObject.GetComponent<UpgradeWeaponItem>().SetTipFrame("lock_tip");
			}
			else if (weapon.Exist == WeaponExistState.Unlocked)
			{
				gameObject.GetComponent<UpgradeWeaponItem>().SetTipFrame("buy_tip");
			}
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}
