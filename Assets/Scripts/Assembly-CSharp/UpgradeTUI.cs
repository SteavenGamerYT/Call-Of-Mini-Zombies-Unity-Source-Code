using System.Collections.Generic;
using UnityEngine;
using Zombie3D;

public class UpgradeTUI : MonoBehaviour, TUIHandler
{
	private TUI m_tui;

	protected TUIInput[] input;

	public GameObject iap_cash_label;

	public GameObject cash_label;

	public GameObject scroll_obj;

	public GameObject buy_button;

	public GameObject upgrade_button;

	public GameObject weapon_damage;

	public GameObject weapon_fire_rate;

	public GameObject weapon_accuracy;

	public GameObject weapon_bullet_count;

	public TUIMeshText item_price;

	public TUIMeshText Label_Count;

	public TUIMeshText Label_Info;

	public TUIMeshSprite Label_BK;

	public GetMoneyPanel get_money_panel;

	public IapPanelTUI iap_panel;

	protected AudioPlayer audioPlayer = new AudioPlayer();

	protected float lastMotionTime;

	protected GameObject cur_weapon;

	protected List<GameObject> weapon_owner_set = new List<GameObject>();

	public int select_item_price = -1;

	protected GameObject select_weapon;

	protected GameObject last_select_weapon;

	public UpgradeItemType select_weapon_item;

	protected GameConfig gConf;

	private void Awake()
	{
		GameScript.CheckMenuResourceConfig();
		GameScript.CheckGlobalResourceConfig();
		if (GameObject.Find("Music") == null)
		{
			GameApp.GetInstance().InitForMenu();
			GameApp.GetInstance().GetGameState().InitWeapons();
			gConf = GameApp.GetInstance().GetGameConfig();
			GameObject gameObject = new GameObject("Music");
			gameObject.AddComponent<MusicFixer>();
			Object.DontDestroyOnLoad(gameObject);
			gameObject.transform.position = new Vector3(0f, 1f, -10f);
			AudioSource audioSource = gameObject.AddComponent<AudioSource>();
			audioSource.clip = GameApp.GetInstance().GetMenuResourceConfig().menuAudio;
			audioSource.loop = true;
			audioSource.bypassEffects = true;
			audioSource.rolloffMode = AudioRolloffMode.Linear;
			audioSource.mute = !GameApp.GetInstance().GetGameState().MusicOn;
			audioSource.Play();
		}
	}

	public void Start()
	{
		m_tui = TUI.Instance("TUI");
		m_tui.SetHandler(this);
		m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
			.FadeIn();
		Transform folderTrans = base.transform.Find("Audio");
		audioPlayer.AddAudio(folderTrans, "Button", true);
		audioPlayer.AddAudio(folderTrans, "Upgrade", true);
		RefashCashLebel();
		iap_panel.on_CashLabelFrash = RefashCashLebel;
		lastMotionTime = Time.time;
		OpenClickPlugin.Hide();
		GameObject gameObject = Object.Instantiate(Resources.Load("Prefabs/AmazonIAPEventListener")) as GameObject;
		gameObject.GetComponent<AmazonIAPEventListener>().iap_panel = iap_panel;
		Resources.UnloadUnusedAssets();
	}

	public GameObject GetWeaponObjWith(Weapon weapon)
	{
		foreach (GameObject item in weapon_owner_set)
		{
			if (item.GetComponent<WeaponItemData>().GetWeapon().Name == weapon.Name)
			{
				return item;
			}
		}
		return null;
	}

	public void Update()
	{
		input = TUIInputManager.GetInput();
		for (int i = 0; i < input.Length; i++)
		{
			m_tui.HandleInput(input[i]);
		}
		if (select_weapon == null && last_select_weapon == null)
		{
			select_weapon = (last_select_weapon = scroll_obj.GetComponent<ScaleScroller>().GetSelectedButton());
			ChangeCurWeapon(select_weapon);
		}
		select_weapon = scroll_obj.GetComponent<ScaleScroller>().GetSelectedButton();
		if (last_select_weapon.name != select_weapon.name)
		{
			ChangeCurWeapon(select_weapon);
			last_select_weapon = select_weapon;
		}
	}

	public void ResetButtonSelectState(GameObject weapon)
	{
		if (weapon.GetComponent<WeaponItemData>().is_armor)
		{
			weapon_damage.GetComponent<TUIButtonSelect>().disabled = false;
			weapon_fire_rate.GetComponent<TUIButtonSelect>().disabled = false;
			weapon_accuracy.GetComponent<TUIButtonSelect>().disabled = false;
			weapon_bullet_count.GetComponent<TUIButtonSelect>().disabled = false;
			weapon_damage.GetComponent<TUIButtonSelect>().SetSelected(true);
			buy_button.active = false;
			upgrade_button.active = true;
			weapon_damage.GetComponent<UpgradeItemController>().SetItem(UpgradeItemType.Armor, null);
			Label_Count.gameObject.active = false;
			Label_BK.gameObject.active = false;
			Label_Info.gameObject.active = false;
			return;
		}
		if (weapon.GetComponent<WeaponItemData>().is_rescue || weapon.GetComponent<WeaponItemData>().is_rebirth)
		{
			weapon_damage.GetComponent<TUIButtonSelect>().disabled = false;
			weapon_fire_rate.GetComponent<TUIButtonSelect>().disabled = false;
			weapon_accuracy.GetComponent<TUIButtonSelect>().disabled = false;
			weapon_bullet_count.GetComponent<TUIButtonSelect>().disabled = false;
			buy_button.active = true;
			upgrade_button.active = false;
			Label_Count.gameObject.active = true;
			Label_BK.gameObject.active = true;
			Label_Info.gameObject.active = true;
			return;
		}
		Label_Count.gameObject.active = false;
		Label_BK.gameObject.active = false;
		Label_Info.gameObject.active = false;
		if (weapon.GetComponent<WeaponItemData>().GetWeapon().Exist == WeaponExistState.Owned)
		{
			weapon_damage.GetComponent<TUIButtonSelect>().disabled = false;
			weapon_fire_rate.GetComponent<TUIButtonSelect>().disabled = false;
			weapon_accuracy.GetComponent<TUIButtonSelect>().disabled = false;
			weapon_bullet_count.GetComponent<TUIButtonSelect>().disabled = false;
			weapon_damage.GetComponent<TUIButtonSelectText>().SetSelected(true);
			weapon_fire_rate.GetComponent<TUIButtonSelectText>().SetSelected(false);
			weapon_accuracy.GetComponent<TUIButtonSelectText>().SetSelected(false);
			if (weapon_bullet_count.active)
			{
				weapon_bullet_count.GetComponent<TUIButtonSelectText>().SetSelected(false);
			}
			buy_button.active = false;
			upgrade_button.active = true;
			weapon_damage.GetComponent<UpgradeItemController>().SetItem(UpgradeItemType.Damage, weapon.GetComponent<WeaponItemData>().GetWeapon());
			weapon_fire_rate.GetComponent<UpgradeItemController>().SetItem(UpgradeItemType.FireRate, weapon.GetComponent<WeaponItemData>().GetWeapon());
			weapon_accuracy.GetComponent<UpgradeItemController>().SetItem(UpgradeItemType.Accuracy, weapon.GetComponent<WeaponItemData>().GetWeapon());
			weapon_bullet_count.GetComponent<UpgradeItemController>().SetItem(UpgradeItemType.Ammo, weapon.GetComponent<WeaponItemData>().GetWeapon());
			return;
		}
		weapon_damage.GetComponent<TUIButtonSelect>().disabled = true;
		weapon_fire_rate.GetComponent<TUIButtonSelect>().disabled = true;
		weapon_accuracy.GetComponent<TUIButtonSelect>().disabled = true;
		weapon_bullet_count.GetComponent<TUIButtonSelect>().disabled = true;
		weapon_damage.GetComponent<TUIButtonSelect>().SetSelected(false);
		weapon_fire_rate.GetComponent<TUIButtonSelect>().SetSelected(false);
		weapon_accuracy.GetComponent<TUIButtonSelect>().SetSelected(false);
		if (weapon_bullet_count.active)
		{
			weapon_bullet_count.GetComponent<TUIButtonSelectText>().SetSelected(false);
		}
		weapon_damage.GetComponent<UpgradeItemController>().SetItem(UpgradeItemType.Damage, weapon.GetComponent<WeaponItemData>().GetWeapon());
		weapon_fire_rate.GetComponent<UpgradeItemController>().SetItem(UpgradeItemType.FireRate, weapon.GetComponent<WeaponItemData>().GetWeapon());
		weapon_accuracy.GetComponent<UpgradeItemController>().SetItem(UpgradeItemType.Accuracy, weapon.GetComponent<WeaponItemData>().GetWeapon());
		weapon_bullet_count.GetComponent<UpgradeItemController>().SetItem(UpgradeItemType.Ammo, weapon.GetComponent<WeaponItemData>().GetWeapon());
		select_weapon_item = UpgradeItemType.None;
		if (weapon.GetComponent<WeaponItemData>().GetWeapon().Exist == WeaponExistState.Locked)
		{
			buy_button.active = false;
			upgrade_button.active = false;
		}
		else if (weapon.GetComponent<WeaponItemData>().GetWeapon().Exist == WeaponExistState.Unlocked)
		{
			buy_button.active = true;
			upgrade_button.active = false;
		}
	}

	public void RefashCurItem(GameObject weapon)
	{
		if (weapon.GetComponent<WeaponItemData>().is_armor)
		{
			weapon_damage.GetComponent<UpgradeItemController>().SetItem(UpgradeItemType.Armor, null);
		}
		else if (weapon.GetComponent<WeaponItemData>().GetWeapon() != null && weapon.GetComponent<WeaponItemData>().GetWeapon().Exist == WeaponExistState.Owned)
		{
			weapon_damage.GetComponent<TUIButtonSelect>().disabled = false;
			weapon_fire_rate.GetComponent<TUIButtonSelect>().disabled = false;
			weapon_accuracy.GetComponent<TUIButtonSelect>().disabled = false;
			weapon_bullet_count.GetComponent<TUIButtonSelect>().disabled = false;
			if (select_weapon_item == UpgradeItemType.Ammo)
			{
				buy_button.active = true;
				upgrade_button.active = false;
			}
			else
			{
				buy_button.active = false;
				upgrade_button.active = true;
			}
			weapon_damage.GetComponent<UpgradeItemController>().SetItem(UpgradeItemType.Damage, weapon.GetComponent<WeaponItemData>().GetWeapon());
			weapon_fire_rate.GetComponent<UpgradeItemController>().SetItem(UpgradeItemType.FireRate, weapon.GetComponent<WeaponItemData>().GetWeapon());
			weapon_accuracy.GetComponent<UpgradeItemController>().SetItem(UpgradeItemType.Accuracy, weapon.GetComponent<WeaponItemData>().GetWeapon());
			weapon_bullet_count.GetComponent<UpgradeItemController>().SetItem(UpgradeItemType.Ammo, weapon.GetComponent<WeaponItemData>().GetWeapon());
		}
		RefashCashLebel();
	}

	public void ChangeCurWeapon(GameObject cur_obj)
	{
		if (cur_obj.GetComponent<WeaponItemData>().is_armor)
		{
			weapon_damage.GetComponent<TUIButtonSelectText>().TextNormal.GetComponent<TUIMeshText>().text = "ARMOR";
			weapon_damage.GetComponent<TUIButtonSelectText>().TextPressed.GetComponent<TUIMeshText>().text = "ARMOR";
			weapon_damage.GetComponent<UpgradeItemController>().SetObjActive(true);
			weapon_fire_rate.GetComponent<UpgradeItemController>().SetObjActive(false);
			weapon_accuracy.GetComponent<UpgradeItemController>().SetObjActive(false);
			weapon_bullet_count.GetComponent<UpgradeItemController>().SetObjActive(false);
		}
		else if (cur_obj.GetComponent<WeaponItemData>().is_rescue || cur_obj.GetComponent<WeaponItemData>().is_rebirth)
		{
			weapon_damage.GetComponent<UpgradeItemController>().SetObjActive(false);
			weapon_fire_rate.GetComponent<UpgradeItemController>().SetObjActive(false);
			weapon_accuracy.GetComponent<UpgradeItemController>().SetObjActive(false);
			weapon_bullet_count.GetComponent<UpgradeItemController>().SetObjActive(false);
			if (cur_obj.GetComponent<WeaponItemData>().is_rescue)
			{
				Label_Count.text = "You Own:" + GameApp.GetInstance().GetGameState().m_rescue_packet_count;
				Label_Info.text = "Revives a downed teammate.\nMAX 10";
			}
			else if (cur_obj.GetComponent<WeaponItemData>().is_rebirth)
			{
				Label_Count.text = "You Own:" + GameApp.GetInstance().GetGameState().m_rebirth_packet_count;
				Label_Info.text = "Revives you in co-op mode.\nMAX 5";
			}
		}
		else
		{
			weapon_damage.GetComponent<TUIButtonSelectText>().TextNormal.GetComponent<TUIMeshText>().text = "DAMAGE";
			weapon_damage.GetComponent<TUIButtonSelectText>().TextPressed.GetComponent<TUIMeshText>().text = "DAMAGE";
			weapon_damage.GetComponent<UpgradeItemController>().SetObjActive(true);
			weapon_fire_rate.GetComponent<UpgradeItemController>().SetObjActive(true);
			weapon_accuracy.GetComponent<UpgradeItemController>().SetObjActive(true);
			weapon_bullet_count.GetComponent<UpgradeItemController>().SetObjActive(true);
			if (cur_obj.GetComponent<WeaponItemData>().GetWeapon() != null)
			{
				weapon_bullet_count.GetComponent<UpgradeItemController>().SetAmmoTip(GetBulletFrameWithWeaponType(cur_obj.GetComponent<WeaponItemData>().GetWeapon().GetWeaponType()));
				if (cur_obj.GetComponent<WeaponItemData>().GetWeapon().GetWeaponType() == WeaponType.Saw || cur_obj.GetComponent<WeaponItemData>().GetWeapon().GetWeaponType() == WeaponType.Sword)
				{
					weapon_bullet_count.GetComponent<UpgradeItemController>().SetObjActive(false);
				}
			}
		}
		ResetButtonSelectState(cur_obj);
		SetCurPrice();
	}

	public void SetCurPrice()
	{
		GameObject gameObject = select_weapon;
		select_item_price = -1;
		GameConfig gameConfig = GameApp.GetInstance().GetGameConfig();
		if (gameObject.GetComponent<WeaponItemData>().is_armor)
		{
			if (GameApp.GetInstance().GetGameState().ArmorLevel != gameConfig.playerConf.maxArmorLevel)
			{
				select_item_price = GameApp.GetInstance().GetGameState().GetArmorPrice();
			}
		}
		else if (gameObject.GetComponent<WeaponItemData>().is_rebirth)
		{
			select_item_price = 200000;
		}
		else if (gameObject.GetComponent<WeaponItemData>().is_rescue)
		{
			select_item_price = 50000;
		}
		else
		{
			Weapon weapon = gameObject.GetComponent<WeaponItemData>().GetWeapon();
			if (weapon.Exist != WeaponExistState.Owned)
			{
				select_item_price = weapon.WConf.price;
			}
			else
			{
				switch (select_weapon_item)
				{
				case UpgradeItemType.Damage:
					if (!weapon.IsMaxLevelDamage())
					{
						select_item_price = weapon.GetDamageUpgradePrice();
					}
					break;
				case UpgradeItemType.FireRate:
					if (!weapon.IsMaxLevelCD())
					{
						select_item_price = weapon.GetFrequencyUpgradePrice();
					}
					break;
				case UpgradeItemType.Accuracy:
					if (!weapon.IsMaxLevelAccuracy())
					{
						select_item_price = weapon.GetAccuracyUpgradePrice();
					}
					break;
				case UpgradeItemType.Ammo:
					select_item_price = weapon.WConf.bulletPrice;
					break;
				}
			}
		}
		if (select_item_price != -1)
		{
			item_price.gameObject.active = true;
			item_price.text = "-$" + select_item_price.ToString("N0");
			item_price.transform.Find("Cash_BK").gameObject.active = true;
		}
		else
		{
			item_price.gameObject.active = false;
			item_price.transform.Find("Cash_BK").gameObject.active = false;
		}
	}

	public string GetBulletFrameWithWeaponType(WeaponType type)
	{
		string result = string.Empty;
		switch (type)
		{
		case WeaponType.AssaultRifle:
			result = "AssaultRifle_bullet";
			break;
		case WeaponType.ShotGun:
			result = "Shotgun_bullet";
			break;
		case WeaponType.RocketLauncher:
			result = "RPG-7_bullet";
			break;
		case WeaponType.MachineGun:
			result = "Gatling_bullet";
			break;
		case WeaponType.LaserGun:
			result = "Laser_bullet";
			break;
		case WeaponType.Sniper:
			result = "PGM_bullet";
			break;
		case WeaponType.M32:
			result = "M32_bullet";
			break;
		case WeaponType.FireGun:
			result = "HellFire_bullet";
			break;
		}
		return result;
	}

	public void HandleEvent(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (control.name == "Back_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			SceneName.SaveSceneStatistics("ShopMenuTUI");
			m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
				.FadeOut("ShopMenuTUI");
			GameApp.GetInstance().Save();
		}
		else if (control.name == "Get_Money_Button" && eventType == 3)
		{
			Debug.Log(control.name);
			audioPlayer.PlayAudio("Button");
			iap_panel.Show();
		}
		else if (control.name == "Buy_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			if (select_weapon.GetComponent<WeaponItemData>().is_rescue)
			{
				switch (GameApp.GetInstance().GetGameState().BuyRescue(50000))
				{
				case PacketBuyStatus.Succeed:
					audioPlayer.PlayAudio("Upgrade");
					Label_Count.text = "You Own:" + GameApp.GetInstance().GetGameState().m_rescue_packet_count;
					RefashCashLebel();
					break;
				case PacketBuyStatus.NotEnoughCash:
					get_money_panel.SetContent("SHORT ON MONEY!");
					get_money_panel.Show();
					break;
				case PacketBuyStatus.Maxed:
					get_money_panel.SetContent("YOU CAN'T CARRY ANY \nMORE OF THOSE.");
					get_money_panel.Show();
					break;
				}
				return;
			}
			if (select_weapon.GetComponent<WeaponItemData>().is_rebirth)
			{
				switch (GameApp.GetInstance().GetGameState().BuyRebirth(200000))
				{
				case PacketBuyStatus.Succeed:
					audioPlayer.PlayAudio("Upgrade");
					Label_Count.text = "You Own:" + GameApp.GetInstance().GetGameState().m_rebirth_packet_count;
					RefashCashLebel();
					break;
				case PacketBuyStatus.NotEnoughCash:
					get_money_panel.SetContent("SHORT ON MONEY!");
					get_money_panel.Show();
					break;
				case PacketBuyStatus.Maxed:
					get_money_panel.SetContent("YOU CAN'T CARRY ANY \nMORE OF THOSE.");
					get_money_panel.Show();
					break;
				}
				return;
			}
			Debug.Log("buy " + select_weapon.GetComponent<WeaponItemData>().GetWeapon().Name);
			Weapon weapon = select_weapon.GetComponent<WeaponItemData>().GetWeapon();
			if (weapon != null && weapon.Exist != WeaponExistState.Owned)
			{
				switch (GameApp.GetInstance().GetGameState().BuyWeapon(weapon, weapon.WConf.price))
				{
				case WeaponBuyStatus.Succeed:
					Debug.Log("Weapon buy success.");
					audioPlayer.PlayAudio("Upgrade");
					select_weapon.GetComponent<UpgradeWeaponItem>().SetTipFrame(string.Empty);
					ChangeCurWeapon(select_weapon);
					break;
				case WeaponBuyStatus.NotEnoughCash:
					get_money_panel.SetContent("SHORT ON MONEY!");
					get_money_panel.Show();
					break;
				case WeaponBuyStatus.Locked:
					get_money_panel.SetContent("UNAVAILABLE WEAPON!");
					get_money_panel.Show();
					break;
				}
			}
			else if (weapon != null && weapon.Exist == WeaponExistState.Owned && select_weapon_item == UpgradeItemType.Ammo && weapon.BulletCount < 9999)
			{
				if (GameApp.GetInstance().GetGameState().BuyBullets(weapon, weapon.WConf.bullet, weapon.WConf.bulletPrice))
				{
					audioPlayer.PlayAudio("Upgrade");
				}
				else
				{
					get_money_panel.SetContent("SHORT ON MONEY!");
					get_money_panel.Show();
				}
			}
			RefashCurItem(select_weapon);
			SetCurPrice();
		}
		else if (control.name == "Upgrade_Button" && eventType == 3)
		{
			Debug.Log("upgrade " + select_weapon_item);
			audioPlayer.PlayAudio("Button");
			if (select_weapon.GetComponent<WeaponItemData>().is_armor && select_weapon_item == UpgradeItemType.Armor)
			{
				GameConfig gameConfig = GameApp.GetInstance().GetGameConfig();
				if (GameApp.GetInstance().GetGameState().ArmorLevel != gameConfig.playerConf.maxArmorLevel)
				{
					if (GameApp.GetInstance().GetGameState().UpgradeArmor(GameApp.GetInstance().GetGameState().GetArmorPrice()))
					{
						audioPlayer.PlayAudio("Upgrade");
					}
					else
					{
						get_money_panel.SetContent("SHORT ON MONEY!");
						get_money_panel.Show();
					}
				}
			}
			else if (select_weapon.GetComponent<WeaponItemData>().GetWeapon() != null && select_weapon_item != 0)
			{
				Weapon weapon2 = select_weapon.GetComponent<WeaponItemData>().GetWeapon();
				switch (select_weapon_item)
				{
				case UpgradeItemType.Damage:
					if (!weapon2.IsMaxLevelDamage())
					{
						if (GameApp.GetInstance().GetGameState().UpgradeWeapon(weapon2, weapon2.Damage * weapon2.WConf.damageConf.upFactor, 0f, 0f, weapon2.GetDamageUpgradePrice()))
						{
							Debug.Log("upgrade");
							audioPlayer.PlayAudio("Upgrade");
						}
						else
						{
							get_money_panel.SetContent("SHORT ON MONEY!");
							get_money_panel.Show();
						}
					}
					break;
				case UpgradeItemType.FireRate:
					if (!weapon2.IsMaxLevelCD())
					{
						if (GameApp.GetInstance().GetGameState().UpgradeWeapon(weapon2, 0f, weapon2.AttackFrequency * weapon2.WConf.attackRateConf.upFactor, 0f, weapon2.GetFrequencyUpgradePrice()))
						{
							audioPlayer.PlayAudio("Upgrade");
							break;
						}
						get_money_panel.SetContent("SHORT ON MONEY!");
						get_money_panel.Show();
					}
					break;
				case UpgradeItemType.Accuracy:
					if (!weapon2.IsMaxLevelAccuracy())
					{
						if (GameApp.GetInstance().GetGameState().UpgradeWeapon(weapon2, 0f, 0f, weapon2.Accuracy * weapon2.WConf.accuracyConf.upFactor, weapon2.GetAccuracyUpgradePrice()))
						{
							audioPlayer.PlayAudio("Upgrade");
							break;
						}
						get_money_panel.SetContent("SHORT ON MONEY!");
						get_money_panel.Show();
					}
					break;
				}
			}
			RefashCurItem(select_weapon);
			SetCurPrice();
		}
		else if (control.name == "Get_Money_Button_Msg" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			get_money_panel.Hide();
			iap_panel.Show();
		}
		else if (control.name == "Close_Msg_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			get_money_panel.Hide();
		}
		else if (control.name == "Iap_Back_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			iap_panel.Hide();
		}
		else if (control.name.StartsWith("Iap_Buy_Button_") && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			iap_panel.IapBuy(control.GetComponent<IapItemData>().iap_item);
		}
	}

	public void RefashCashLebel()
	{
		cash_label.GetComponent<TUIMeshText>().text = "$" + GameApp.GetInstance().GetGameState().GetCash()
			.ToString("N0");
		iap_cash_label.GetComponent<TUIMeshText>().text = "$" + GameApp.GetInstance().GetGameState().GetCash()
			.ToString("N0");
	}
}
