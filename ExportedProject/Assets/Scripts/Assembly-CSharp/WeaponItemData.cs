using UnityEngine;
using Zombie3D;

public class WeaponItemData : MonoBehaviour
{
	public bool is_armor;

	public bool is_rescue;

	public bool is_rebirth;

	protected Weapon weapon;

	public string weapon_name;

	public Weapon GetWeapon()
	{
		return weapon;
	}

	public void SetWeapon(Weapon w)
	{
		if (w != null)
		{
			weapon_name = w.Name;
		}
		else
		{
			weapon_name = "none";
		}
		weapon = w;
	}
}
