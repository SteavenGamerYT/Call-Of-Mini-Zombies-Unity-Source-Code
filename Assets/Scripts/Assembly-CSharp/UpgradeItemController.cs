using UnityEngine;
using Zombie3D;

public class UpgradeItemController : MonoBehaviour
{
	public UpgradeItemType m_type;

	public bool enabled_upgrade = true;

	public int star_open_count;

	public int star_max_count = 10;

	public float cur_val;

	public float next_val;

	public int bullets_count;

	public int buy_bullets_count;

	public Weapon cur_weapon;

	protected TUIMeshSprite[] star_array;

	protected GameConfig gConf;

	public UpgradeTUI upgrade_tui;

	public TUIMeshSprite arrow_tip;

	public TUIMeshText Label_val;

	public TUIMeshText Label_val_next;

	public TUIMeshSprite ammo_tip;

	public TUIMeshText ammo_count;

	public TUIMeshText buy_count;

	protected Color32 label_color_n = new Color32(202, 135, 21, byte.MaxValue);

	protected Color32 label_color_p = new Color32(byte.MaxValue, 227, 41, byte.MaxValue);

	private void Awake()
	{
		if (base.transform.Find("ItemLevel") != null)
		{
			GameObject gameObject = base.transform.Find("ItemLevel").gameObject;
			star_array = gameObject.GetComponentsInChildren<TUIMeshSprite>(false);
		}
		GetComponent<TUIButtonSelectText>().select_true_delegate = OnSelectTrue;
		GetComponent<TUIButtonSelectText>().select_false_delegate = OnSelectFalse;
		if (base.transform.Find("arrow_tip") != null)
		{
			arrow_tip = base.transform.Find("arrow_tip").gameObject.GetComponent<TUIMeshSprite>();
		}
		if (base.transform.Find("Label_Val") != null)
		{
			Label_val = base.transform.Find("Label_Val").gameObject.GetComponent<TUIMeshText>();
		}
		if (base.transform.Find("Label_Val_Next") != null)
		{
			Label_val_next = base.transform.Find("Label_Val_Next").gameObject.GetComponent<TUIMeshText>();
		}
		if (base.transform.Find("ammo_tip") != null)
		{
			ammo_tip = base.transform.Find("ammo_tip").gameObject.GetComponent<TUIMeshSprite>();
		}
		if (base.transform.Find("ammo_count") != null)
		{
			ammo_count = base.transform.Find("ammo_count").gameObject.GetComponent<TUIMeshText>();
		}
		if (base.transform.Find("buy_count") != null)
		{
			buy_count = base.transform.Find("buy_count").gameObject.GetComponent<TUIMeshText>();
		}
	}

	private void Start()
	{
	}

	public void UpdateContent()
	{
	}

	public void SetAmmoTip(string frame)
	{
		if (ammo_tip != null)
		{
			ammo_tip.frameName = frame;
		}
	}

	public void SetObjActive(bool state)
	{
		base.gameObject.active = state;
		if (star_array != null)
		{
			TUIMeshSprite[] array = star_array;
			TUIMeshSprite[] array2 = array;
			foreach (TUIMeshSprite tUIMeshSprite in array2)
			{
				tUIMeshSprite.gameObject.active = state;
			}
		}
		for (int j = 0; j < base.transform.GetChildCount(); j++)
		{
			base.transform.GetChild(j).gameObject.active = state;
		}
	}

	public void OnSelectTrue()
	{
		SetItemLevel(true);
		SetItemLabel(true);
		if (upgrade_tui != null)
		{
			upgrade_tui.select_weapon_item = m_type;
			upgrade_tui.SetCurPrice();
			if (m_type == UpgradeItemType.Ammo)
			{
				upgrade_tui.buy_button.active = true;
				upgrade_tui.upgrade_button.active = false;
			}
			else
			{
				upgrade_tui.buy_button.active = false;
				upgrade_tui.upgrade_button.active = true;
			}
		}
	}

	public void OnSelectFalse()
	{
		SetItemLevel(false);
		SetItemLabel(false);
	}

	public void SetItemLevel(bool is_selected)
	{
		string empty = string.Empty;
		empty = (is_selected ? "_p" : "_n");
		if (star_array == null)
		{
			return;
		}
		int num = 0;
		TUIMeshSprite[] array = star_array;
		TUIMeshSprite[] array2 = array;
		foreach (TUIMeshSprite tUIMeshSprite in array2)
		{
			if (num < star_open_count)
			{
				tUIMeshSprite.frameName = "star_full" + empty;
			}
			else if (num < star_max_count)
			{
				tUIMeshSprite.frameName = "star_empty" + empty;
			}
			else
			{
				tUIMeshSprite.frameName = string.Empty;
			}
			num++;
		}
	}

	public void SetItemLabel(bool is_selected)
	{
		if (Label_val != null)
		{
			if (cur_val != -1f)
			{
				Label_val.text = cur_val.ToString();
				if (m_type == UpgradeItemType.FireRate)
				{
					Label_val.text += "s";
				}
				else if (m_type == UpgradeItemType.Accuracy)
				{
					Label_val.text += "%";
				}
			}
			else
			{
				Label_val.text = string.Empty;
			}
		}
		if (Label_val_next != null)
		{
			if (next_val != -1f)
			{
				Label_val_next.text = next_val.ToString();
				if (m_type == UpgradeItemType.FireRate)
				{
					Label_val_next.text += "s";
				}
				else if (m_type == UpgradeItemType.Accuracy)
				{
					Label_val_next.text += "%";
				}
			}
			else
			{
				Label_val_next.text = string.Empty;
			}
		}
		if (arrow_tip != null && next_val != -1f)
		{
			if (is_selected)
			{
				arrow_tip.frameName = "arrow_p";
			}
			else
			{
				arrow_tip.frameName = "arrow_n";
			}
		}
		else if (arrow_tip != null && next_val == -1f)
		{
			arrow_tip.frameName = string.Empty;
		}
		if (ammo_count != null && bullets_count != -1)
		{
			ammo_count.text = "x" + bullets_count;
		}
		if (buy_count != null && buy_bullets_count != -1)
		{
			buy_count.text = "+" + buy_bullets_count;
		}
		if (is_selected)
		{
			if (Label_val != null)
			{
				Label_val.color = label_color_p;
			}
			if (Label_val_next != null)
			{
				Label_val_next.color = label_color_p;
			}
			if (ammo_count != null)
			{
				ammo_count.color = label_color_p;
			}
			if (buy_count != null)
			{
				buy_count.color = label_color_p;
			}
		}
		else
		{
			if (Label_val != null)
			{
				Label_val.color = label_color_n;
			}
			if (Label_val_next != null)
			{
				Label_val_next.color = label_color_n;
			}
			if (ammo_count != null)
			{
				ammo_count.color = label_color_n;
			}
			if (buy_count != null)
			{
				buy_count.color = label_color_n;
			}
		}
		if (GetComponent<TUIButtonSelectText>().disabled)
		{
			if (arrow_tip != null)
			{
				arrow_tip.frameName = string.Empty;
			}
			if (Label_val_next != null)
			{
				Label_val_next.color = label_color_n;
				Label_val_next.text = string.Empty;
			}
			if (buy_count != null)
			{
				buy_count.color = label_color_n;
				buy_count.text = string.Empty;
			}
		}
	}

	public void SetUpgradeEnabled(bool status)
	{
		enabled_upgrade = status;
		if (enabled_upgrade)
		{
		}
	}

	public void SetItem(UpgradeItemType type, Weapon weapon)
	{
		bool flag = GetComponent<TUIButtonSelectText>().IsSelected();
		gConf = GameApp.GetInstance().GetGameConfig();
		int num = 0;
		int num2 = 0;
		cur_val = -1f;
		next_val = -1f;
		bullets_count = -1;
		buy_bullets_count = -1;
		m_type = type;
		cur_weapon = weapon;
		if (m_type == UpgradeItemType.Armor)
		{
			num = GameApp.GetInstance().GetGameState().ArmorLevel;
			num2 = gConf.playerConf.maxArmorLevel;
		}
		else if (weapon != null)
		{
			if (weapon.Exist == WeaponExistState.Owned)
			{
				switch (m_type)
				{
				case UpgradeItemType.Damage:
					num = weapon.DamageLevel;
					num2 = (int)weapon.WConf.damageConf.maxLevel;
					cur_val = Math.SignificantFigures(weapon.Damage, 4);
					if (num != num2)
					{
						next_val = Math.SignificantFigures(weapon.GetNextLevelDamage(), 4);
					}
					break;
				case UpgradeItemType.FireRate:
					num = weapon.FrequencyLevel;
					num2 = (int)weapon.WConf.attackRateConf.maxLevel;
					cur_val = Math.SignificantFigures(weapon.AttackFrequency, 4);
					if (num != num2)
					{
						next_val = Math.SignificantFigures(weapon.GetNextLevelFrequency(), 4);
					}
					break;
				case UpgradeItemType.Accuracy:
					num = weapon.AccuracyLevel;
					num2 = (int)weapon.WConf.accuracyConf.maxLevel;
					cur_val = Math.SignificantFigures(weapon.Accuracy, 4);
					if (num != num2)
					{
						next_val = Math.SignificantFigures(weapon.GetNextLevelAccuracy(), 4);
					}
					break;
				case UpgradeItemType.Ammo:
					num = 0;
					num2 = 0;
					bullets_count = weapon.BulletCount;
					buy_bullets_count = weapon.WConf.bullet;
					break;
				}
			}
			else if (weapon.Exist == WeaponExistState.Locked || weapon.Exist == WeaponExistState.Unlocked)
			{
				num = 0;
				num2 = 0;
				switch (m_type)
				{
				case UpgradeItemType.Damage:
					cur_val = Math.SignificantFigures(weapon.Damage, 4);
					break;
				case UpgradeItemType.FireRate:
					cur_val = Math.SignificantFigures(weapon.AttackFrequency, 4);
					break;
				case UpgradeItemType.Accuracy:
					cur_val = Math.SignificantFigures(weapon.Accuracy, 4);
					break;
				case UpgradeItemType.Ammo:
					bullets_count = weapon.BulletCount;
					break;
				}
			}
		}
		star_open_count = num;
		star_max_count = num2;
		SetItemLevel(flag);
		SetItemLabel(flag);
		if (upgrade_tui != null && flag)
		{
			upgrade_tui.select_weapon_item = m_type;
		}
	}
}
