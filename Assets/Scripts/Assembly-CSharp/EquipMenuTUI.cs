using System.Collections.Generic;
using UnityEngine;
using Zombie3D;

public class EquipMenuTUI : MonoBehaviour, TUIHandler
{
	private TUI m_tui;

	protected TUIInput[] input;

	public GameObject cash_label;

	public GetMoneyPanel get_money_panel;

	public IapPanelTUI iap_panel;

	public GameObject iap_cash_label;

	public GameObject avatar;

	public GameObject avatar_mover;

	public GameObject scroll_obj;

	public TUIMeshText Avatar_Info;

	public TUIRect slider_rect;

	public TUIMeshText weapon_name;

	public TUIMeshText weapon_damage;

	public TUIMeshText weapon_fire_rate;

	public TUIMeshText weapon_accuracy;

	public TUIMeshText weapon_bullet_count;

	public TUIMeshSprite weapon_bullet_icon;

	public TUIMeshText Rebirth_Count_Label;

	public TUIMeshText Rescue_Count_Label;

	public Camera FrameCamera;

	protected AudioPlayer audioPlayer = new AudioPlayer();

	protected float lastMotionTime;

	protected Vector3 availably_postion = new Vector3(0f, -1000f, 0f);

	protected Vector3 invalid_postion = new Vector3(0f, -2000f, 0f);

	public GameObject[] EquipSheets = new GameObject[3];

	protected GameObject cur_weapon;

	protected List<GameObject> weapon_owner_set = new List<GameObject>();

	protected Weapon select_weapon;

	protected Weapon last_select_weapon;

	private void Awake()
	{
		GameScript.CheckMenuResourceConfig();
		GameScript.CheckGlobalResourceConfig();
		if (GameObject.Find("Music") == null)
		{
			GameApp.GetInstance().InitForMenu();
			GameApp.GetInstance().GetGameState().InitWeapons();
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
		float num = (float)Screen.height / (float)Screen.width * 2.175f;
		Debug.Log(num);
		FrameCamera.orthographicSize = num;
		FrameCamera.depth = -5f;
	}

	public void Start()
	{
		m_tui = TUI.Instance("TUI");
		m_tui.SetHandler(this);
		m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
			.FadeIn();
		m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFadeEx>()
			.m_fadein = OnSceneIn;
		Transform folderTrans = base.transform.Find("Audio");
		audioPlayer.AddAudio(folderTrans, "Button", true);
		RefashCashLebel();
		iap_panel.on_CashLabelFrash = RefashCashLebel;
		avatar = AvatarFactory.GetInstance().CreateAvatar(GameApp.GetInstance().GetGameState().Avatar);
		avatar.transform.position = availably_postion;
		avatar.transform.rotation = Quaternion.Euler(0f, 163f, 0f);
		Object.Destroy(avatar.transform.Find("shadow").gameObject);
		Weapon weapon = GameApp.GetInstance().GetGameState().GetBattleWeapons()[0];
		string text = weapon.Name;
		string weaponNameEnd = Weapon.GetWeaponNameEnd(weapon.GetWeaponType());
		cur_weapon = WeaponFactory.GetInstance().CreateWeaponModel(text, availably_postion, Quaternion.identity);
		cur_weapon.name = text;
		cur_weapon.AddComponent<WeaponItemData>().SetWeapon(weapon);
		AvatarChangeWeapon(cur_weapon);
		SetWeaponInfo(weapon);
		weapon_owner_set.Add(cur_weapon);
		GameObject gameObject = null;
		foreach (Weapon weapon3 in GameApp.GetInstance().GetGameState().GetWeapons())
		{
			if (weapon3.Exist == WeaponExistState.Owned && weapon3.Name != text)
			{
				string weaponName = weapon3.Name;
				gameObject = WeaponFactory.GetInstance().CreateWeaponModel(weaponName, invalid_postion, Quaternion.identity);
				gameObject.name = weaponName;
				gameObject.AddComponent<WeaponItemData>().SetWeapon(weapon3);
				weapon_owner_set.Add(gameObject);
			}
		}
		if (avatar_mover != null)
		{
			avatar_mover.GetComponent<ShopUIAvatarMover>().slider_obj = avatar;
		}
		for (int i = 0; i < GameApp.GetInstance().GetGameState().GetRectToWeaponMap()
			.Length; i++)
		{
			int num = GameApp.GetInstance().GetGameState().GetRectToWeaponMap()[i];
			if (num != -1)
			{
				Weapon weapon2 = GameApp.GetInstance().GetGameState().GetWeapons()[num];
				Debug.Log("sheet " + i + " weapon:" + weapon2.Name);
				Weapon old_weapon;
				EquipWeaponToSheet(weapon2, EquipSheets[i], out old_weapon);
			}
		}
		Rebirth_Count_Label.text = GameApp.GetInstance().GetGameState().m_rebirth_packet_count.ToString();
		Rescue_Count_Label.text = GameApp.GetInstance().GetGameState().m_rescue_packet_count.ToString();
		lastMotionTime = Time.time;
		OpenClickPlugin.Hide();
		GameObject gameObject2 = Object.Instantiate(Resources.Load("Prefabs/AmazonIAPEventListener")) as GameObject;
		gameObject2.GetComponent<AmazonIAPEventListener>().iap_panel = iap_panel;
		Resources.UnloadUnusedAssets();
	}

	private void OnSceneOut()
	{
	}

	private void OnSceneIn()
	{
		FrameCamera.depth = 1f;
	}

	public bool EquipWeaponToSheet(Weapon equip_weapon, GameObject sheet, out Weapon old_weapon)
	{
		old_weapon = sheet.GetComponent<WeaponItemData>().GetWeapon();
		if (!sheet.name.StartsWith("EquipSheet"))
		{
			return false;
		}
		bool flag = false;
		GameObject[] equipSheets = EquipSheets;
		GameObject[] array = equipSheets;
		foreach (GameObject gameObject in array)
		{
			if (gameObject.GetComponent<EquipSheet>().weapon_item != null && gameObject.GetComponent<EquipSheet>().weapon_item.GetComponent<WeaponItemData>().GetWeapon().Name == equip_weapon.Name)
			{
				flag = true;
			}
		}
		if (flag)
		{
			Debug.Log(equip_weapon.Name + " has already equip.");
			return false;
		}
		if (old_weapon != null)
		{
			Object.Destroy(sheet.GetComponent<EquipSheet>().weapon_item.gameObject);
		}
		Debug.Log(equip_weapon.Name + " to " + sheet.name);
		GameObject gameObject2 = Object.Instantiate(Resources.Load("Prefabs/TUI/Slider_Weapon")) as GameObject;
		gameObject2.transform.parent = sheet.transform;
		gameObject2.transform.localPosition = new Vector3(0f, 0f, -1f);
		gameObject2.GetComponent<TUIMeshSprite>().frameName = equip_weapon.Name + "_small";
		gameObject2.GetComponent<TUIButtonSliderCallBack>().sliderRect = slider_rect;
		gameObject2.GetComponent<SliderWeaponController>().equit_sheet = sheet;
		gameObject2.GetComponent<SliderWeaponController>().equip_menu_tui = this;
		gameObject2.GetComponent<WeaponItemData>().SetWeapon(equip_weapon);
		sheet.GetComponent<WeaponItemData>().SetWeapon(equip_weapon);
		sheet.GetComponent<EquipSheet>().weapon_item = gameObject2.GetComponent<SliderWeaponController>();
		equip_weapon.IsSelectedForBattle = true;
		GameApp.GetInstance().GetGameState().GetRectToWeaponMap()[sheet.GetComponent<EquipSheet>().sheet_index] = GameApp.GetInstance().GetGameState().GetWeaponIndex(equip_weapon);
		return true;
	}

	public void ClearSheet(GameObject sheet)
	{
		if (sheet.name.StartsWith("EquipSheet"))
		{
			if (sheet.GetComponent<WeaponItemData>().GetWeapon() != null)
			{
				sheet.GetComponent<WeaponItemData>().GetWeapon().IsSelectedForBattle = false;
			}
			sheet.GetComponent<WeaponItemData>().SetWeapon(null);
			sheet.GetComponent<EquipSheet>().weapon_item = null;
			GameApp.GetInstance().GetGameState().GetRectToWeaponMap()[sheet.GetComponent<EquipSheet>().sheet_index] = -1;
		}
	}

	public int GetavailablySheetCount()
	{
		int num = 0;
		GameObject[] equipSheets = EquipSheets;
		GameObject[] array = equipSheets;
		foreach (GameObject gameObject in array)
		{
			if (gameObject.GetComponent<EquipSheet>().weapon_item != null)
			{
				num++;
			}
		}
		return num;
	}

	public void AvatarChangeWeapon(GameObject weapon)
	{
		if (avatar != null && weapon.GetComponent<WeaponItemData>() != null)
		{
			Weapon weapon2;
			string weaponNameEnd;
			if (cur_weapon != null)
			{
				cur_weapon.transform.position = invalid_postion;
				weapon2 = cur_weapon.GetComponent<WeaponItemData>().GetWeapon();
				weaponNameEnd = Weapon.GetWeaponNameEnd(weapon2.GetWeaponType());
				avatar.GetComponent<Animation>()["Idle01" + weaponNameEnd].wrapMode = WrapMode.Loop;
				avatar.GetComponent<Animation>().Play("Idle01" + weaponNameEnd);
			}
			cur_weapon = weapon;
			weapon2 = cur_weapon.GetComponent<WeaponItemData>().GetWeapon();
			weaponNameEnd = Weapon.GetWeaponNameEnd(weapon2.GetWeaponType());
			Transform parent = avatar.transform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 R Clavicle/Bip01 R UpperArm/Bip01 R Forearm/Bip01 R Hand/Weapon_Dummy");
			cur_weapon.transform.parent = parent;
			cur_weapon.transform.localPosition = new Vector3(0f, 0f, 0f);
			cur_weapon.transform.localRotation = Quaternion.Euler(new Vector3(90f, 0f, 0f));
			avatar.GetComponent<Animation>()["Idle01" + weaponNameEnd].wrapMode = WrapMode.Loop;
			avatar.GetComponent<Animation>().Play("Idle01" + weaponNameEnd);
		}
	}

	public void CheckAvatarFirstWeapon()
	{
		GameObject[] equipSheets = EquipSheets;
		GameObject[] array = equipSheets;
		foreach (GameObject gameObject in array)
		{
			if (gameObject.GetComponent<EquipSheet>().weapon_item != null)
			{
				AvatarChangeWeapon(GetWeaponObjWith(gameObject.GetComponent<EquipSheet>().weapon_item.GetComponent<WeaponItemData>().GetWeapon()));
				break;
			}
		}
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
		if (avatar != null)
		{
			UpdateAnimation();
		}
		if (select_weapon == null && last_select_weapon == null)
		{
			select_weapon = (last_select_weapon = scroll_obj.GetComponent<ScaleScroller>().GetSelectedButton().GetComponent<WeaponItemData>()
				.GetWeapon());
		}
		select_weapon = scroll_obj.GetComponent<ScaleScroller>().GetSelectedButton().GetComponent<WeaponItemData>()
			.GetWeapon();
		if (last_select_weapon.Name != select_weapon.Name)
		{
			SetWeaponInfo(select_weapon);
			last_select_weapon = select_weapon;
		}
	}

	public void SetWeaponInfo(Weapon weapon)
	{
		weapon_name.text = weapon.Name;
		weapon_damage.text = weapon.Damage.ToString();
		weapon_fire_rate.text = weapon.AttackFrequency + "s";
		weapon_accuracy.text = weapon.Accuracy + "%";
		string bulletFrameWithWeaponType = GetBulletFrameWithWeaponType(weapon.GetWeaponType());
		weapon_bullet_icon.frameName = bulletFrameWithWeaponType;
		if (bulletFrameWithWeaponType != string.Empty)
		{
			weapon_bullet_count.text = "x" + weapon.BulletCount;
		}
		else
		{
			weapon_bullet_count.text = string.Empty;
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
			result = "Hellfire_bullet";
			break;
		}
		return result;
	}

	public void UpdateAnimation()
	{
		Weapon weapon = cur_weapon.GetComponent<WeaponItemData>().GetWeapon();
		string weaponNameEnd = Weapon.GetWeaponNameEnd(weapon.GetWeaponType());
		if (!(avatar != null))
		{
			return;
		}
		if (weapon.GetWeaponType() == WeaponType.RocketLauncher || weapon.GetWeaponType() == WeaponType.Sniper)
		{
			if (Time.time - lastMotionTime > 7f)
			{
				string empty = string.Empty;
				empty = (avatar.GetComponent<Animation>().IsPlaying("Run01" + weaponNameEnd) ? ("Idle01" + weaponNameEnd) : ("Run01" + weaponNameEnd));
				avatar.GetComponent<Animation>()[empty].wrapMode = WrapMode.Loop;
				avatar.GetComponent<Animation>().CrossFade(empty);
				lastMotionTime = Time.time;
			}
		}
		else if (weapon.GetWeaponType() == WeaponType.Saw || weapon.GetWeaponType() == WeaponType.Sword)
		{
			if (Time.time - lastMotionTime > 7f)
			{
				avatar.GetComponent<Animation>()["Shoot01_Saw2"].wrapMode = WrapMode.ClampForever;
				avatar.GetComponent<Animation>().CrossFade("Shoot01_Saw");
				avatar.GetComponent<Animation>().CrossFadeQueued("Shoot01_Saw2");
				lastMotionTime = Time.time;
			}
			if (avatar.GetComponent<Animation>().IsPlaying("Shoot01_Saw2") && Time.time - lastMotionTime > avatar.GetComponent<Animation>()["Shoot01_Saw2"].clip.length * 2f)
			{
				avatar.GetComponent<Animation>().CrossFade("Idle01" + weaponNameEnd);
			}
		}
		else
		{
			if (Time.time - lastMotionTime > 7f)
			{
				avatar.GetComponent<Animation>()["Standby03"].wrapMode = WrapMode.ClampForever;
				avatar.GetComponent<Animation>().CrossFade("Standby03");
				lastMotionTime = Time.time;
			}
			if (avatar.GetComponent<Animation>()["Standby03"].time > avatar.GetComponent<Animation>()["Standby03"].clip.length)
			{
				avatar.GetComponent<Animation>().CrossFade("Idle01" + weaponNameEnd);
			}
		}
	}

	public void HandleEvent(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (control.name == "Back_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			FrameCamera.depth = -5f;
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
			FrameCamera.depth = -5f;
		}
		else if (control.name == "Get_Money_Button_Msg" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			get_money_panel.Hide();
			iap_panel.Show();
			FrameCamera.depth = -5f;
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
			FrameCamera.depth = 1f;
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
