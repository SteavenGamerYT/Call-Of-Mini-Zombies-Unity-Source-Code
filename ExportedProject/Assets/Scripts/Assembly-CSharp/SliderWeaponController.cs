using UnityEngine;
using Zombie3D;

public class SliderWeaponController : MonoBehaviour
{
	public EquipMenuTUI equip_menu_tui;

	public GameObject equit_sheet;

	protected bool is_final_one;

	private void Start()
	{
		base.gameObject.GetComponent<TUIButtonSliderCallBack>().Begin_delegate = OnSliderBegin;
		base.gameObject.GetComponent<TUIButtonSliderCallBack>().move_delegate = OnSliderMove;
		base.gameObject.GetComponent<TUIButtonSliderCallBack>().end_delegate = OnSliderEnd;
	}

	private void Update()
	{
	}

	private void OnSliderBegin(TUIInput input)
	{
		if (equit_sheet != null)
		{
			if (equip_menu_tui.GetavailablySheetCount() <= 1)
			{
				is_final_one = true;
				Debug.Log("just one, can not remove.");
			}
			equip_menu_tui.ClearSheet(equit_sheet);
		}
	}

	private void OnSliderMove(TUIInput input)
	{
	}

	private void OnSliderEnd(TUIInput input)
	{
		GameObject gameObject = null;
		GameObject[] equipSheets = equip_menu_tui.EquipSheets;
		GameObject[] array = equipSheets;
		foreach (GameObject gameObject2 in array)
		{
			if (gameObject2.GetComponent<TUIButtonBase>().PtInControl(input.position))
			{
				gameObject = gameObject2;
				break;
			}
		}
		Weapon old_weapon = null;
		if (gameObject != null)
		{
			if (gameObject.GetComponent<EquipSheet>().weapon_item == null)
			{
				if (equip_menu_tui.EquipWeaponToSheet(base.gameObject.GetComponent<WeaponItemData>().GetWeapon(), gameObject, out old_weapon) && equit_sheet != null && equit_sheet != gameObject)
				{
					equip_menu_tui.ClearSheet(equit_sheet);
				}
			}
			else if (equit_sheet != null)
			{
				if (equip_menu_tui.EquipWeaponToSheet(base.gameObject.GetComponent<WeaponItemData>().GetWeapon(), gameObject, out old_weapon))
				{
					if (equit_sheet != null)
					{
						equip_menu_tui.ClearSheet(equit_sheet);
					}
					Weapon old_weapon2;
					if (old_weapon != null && equip_menu_tui.EquipWeaponToSheet(old_weapon, equit_sheet, out old_weapon2))
					{
					}
				}
			}
			else if (equip_menu_tui.EquipWeaponToSheet(base.gameObject.GetComponent<WeaponItemData>().GetWeapon(), gameObject, out old_weapon) && equit_sheet != null)
			{
				equip_menu_tui.ClearSheet(equit_sheet);
			}
		}
		else if (equit_sheet != null)
		{
			if (is_final_one)
			{
				equip_menu_tui.EquipWeaponToSheet(base.gameObject.GetComponent<WeaponItemData>().GetWeapon(), equit_sheet, out old_weapon);
			}
			else
			{
				equip_menu_tui.ClearSheet(equit_sheet);
			}
		}
		Object.Destroy(base.gameObject);
		equip_menu_tui.CheckAvatarFirstWeapon();
	}
}
