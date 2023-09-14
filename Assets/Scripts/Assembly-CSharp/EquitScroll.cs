using UnityEngine;
using Zombie3D;

public class EquitScroll : MonoBehaviour
{
	private void Awake()
	{
		GameObject gameObject = null;
		foreach (Weapon weapon in GameApp.GetInstance().GetGameState().GetWeapons())
		{
			if (weapon.Exist == WeaponExistState.Owned)
			{
				Debug.Log(weapon.Name);
				gameObject = Object.Instantiate(Resources.Load("Prefabs/TUI/Weapon_Item")) as GameObject;
				gameObject.transform.parent = base.transform;
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.name = "Item_" + weapon.Name;
				gameObject.GetComponent<TUIMeshSprite>().frameName = weapon.Name;
				gameObject.GetComponent<WeaponItemData>().SetWeapon(weapon);
				base.gameObject.GetComponent<ScaleScroller>().item_buttons.Add(gameObject);
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
