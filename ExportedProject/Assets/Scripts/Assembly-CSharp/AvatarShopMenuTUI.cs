using System.Collections.Generic;
using UnityEngine;
using Zombie3D;

public class AvatarShopMenuTUI : MonoBehaviour, TUIHandler
{
	private TUI m_tui;

	protected TUIInput[] input;

	public GameObject cash_label;

	public GameObject avatar;

	public GameObject avatar_mover;

	public GameObject scroll_obj;

	public TUIMeshText Avatar_Info;

	public TUIMeshText Avatar_price;

	public GameObject Avatar_price_bk;

	public TUIButtonClickText buy_button;

	public TUIButtonClickText select_button;

	public GetMoneyPanel get_money_panel;

	public IapPanelTUI iap_panel;

	public GameObject iap_cash_label;

	public Camera FrameCamera;

	protected AudioPlayer audioPlayer = new AudioPlayer();

	protected float lastMotionTime;

	protected Quaternion cur_rotation = Quaternion.identity;

	protected Vector3 availably_postion = new Vector3(0f, -1000f, 0f);

	protected Vector3 invalid_postion = new Vector3(0f, -2000f, 0f);

	protected List<GameObject> avatar_set = new List<GameObject>();

	protected GameObject weapon;

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
		float num = (float)Screen.height / (float)Screen.width * 1.95f;
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
		for (int i = 0; i < 12; i++)
		{
			GameObject gameObject = AvatarFactory.GetInstance().CreateAvatar((AvatarType)i);
			gameObject.transform.position = invalid_postion;
			gameObject.transform.rotation = Quaternion.Euler(0f, 163f, 0f);
			gameObject.AddComponent<AvatarItemData>().avatar_type = (AvatarType)i;
			avatar_set.Add(gameObject);
		}
		Weapon weapon = GameApp.GetInstance().GetGameState().GetBattleWeapons()[0];
		string weaponName = weapon.Name;
		string weaponNameEnd = Weapon.GetWeaponNameEnd(weapon.GetWeaponType());
		this.weapon = WeaponFactory.GetInstance().CreateWeaponModel(weaponName, invalid_postion, Quaternion.identity);
		Debug.Log("cur avatar:" + GameApp.GetInstance().GetGameState().Avatar);
		ChangeAvatar(GameApp.GetInstance().GetGameState().Avatar);
		if (scroll_obj != null)
		{
			foreach (GameObject item_button in scroll_obj.GetComponent<ScaleScroller>().item_buttons)
			{
				item_button.GetComponent<AvatarItemData>().SetAvatarItem();
				if (item_button.GetComponent<AvatarItemData>().avatar_type == GameApp.GetInstance().GetGameState().Avatar)
				{
					scroll_obj.GetComponent<ScaleScroller>().SetPositionWithItem(item_button);
				}
			}
		}
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

	public void ChangeAvatar(AvatarType type)
	{
		bool flag = false;
		if (avatar != null)
		{
			cur_rotation = avatar.transform.rotation;
			flag = true;
		}
		foreach (GameObject item in avatar_set)
		{
			if (item.GetComponent<AvatarItemData>().avatar_type == type)
			{
				item.transform.position = availably_postion;
				if (flag)
				{
					item.transform.rotation = cur_rotation;
				}
				avatar = item;
				if (avatar_mover != null)
				{
					avatar_mover.GetComponent<ShopUIAvatarMover>().slider_obj = avatar;
				}
				if (this.weapon != null)
				{
					Weapon weapon = GameApp.GetInstance().GetGameState().GetBattleWeapons()[0];
					string weaponNameEnd = Weapon.GetWeaponNameEnd(weapon.GetWeaponType());
					Transform parent = avatar.transform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 R Clavicle/Bip01 R UpperArm/Bip01 R Forearm/Bip01 R Hand/Weapon_Dummy");
					this.weapon.transform.parent = parent;
					this.weapon.transform.localPosition = new Vector3(0f, 0f, 0f);
					this.weapon.transform.localRotation = Quaternion.Euler(new Vector3(90f, 0f, 0f));
					this.weapon.transform.rotation = item.transform.rotation;
					avatar.GetComponent<Animation>()["Idle01" + weaponNameEnd].wrapMode = WrapMode.Loop;
					avatar.GetComponent<Animation>().Play("Idle01" + weaponNameEnd);
				}
			}
			else
			{
				item.transform.position = invalid_postion;
			}
		}
		if (buy_button != null && select_button != null)
		{
			if (GameApp.GetInstance().GetGameState().GetAvatarData(type) == AvatarState.Avaliable)
			{
				buy_button.gameObject.active = false;
				select_button.gameObject.active = true;
				Avatar_price.gameObject.active = false;
				Avatar_price_bk.active = false;
			}
			else if (GameApp.GetInstance().GetGameState().GetAvatarData(type) == AvatarState.ToBuy)
			{
				buy_button.gameObject.active = true;
				select_button.gameObject.active = false;
				Avatar_price.gameObject.active = true;
				Avatar_price_bk.active = true;
			}
		}
		Avatar_Info.text = AvatarInfo.AVATAR_INFO_SET[(int)type].ToUpper();
		if (type != 0)
		{
			GameConfig gameConfig = GameApp.GetInstance().GetGameConfig();
			Avatar_price.text = "-$" + gameConfig.GetAvatarConfig((int)type).price.ToString("N0");
		}
	}

	public void Update()
	{
		input = TUIInputManager.GetInput();
		for (int i = 0; i < input.Length; i++)
		{
			m_tui.HandleInput(input[i]);
		}
		AvatarType avatar_type = scroll_obj.GetComponent<ScaleScroller>().GetSelectedButton().GetComponent<AvatarItemData>()
			.avatar_type;
		if (avatar != null)
		{
			if (scroll_obj != null && avatar_type != avatar.GetComponent<AvatarItemData>().avatar_type)
			{
				ChangeAvatar(avatar_type);
				Debug.Log("type:" + avatar_type);
			}
			UpdateAnimation();
		}
	}

	public void UpdateAnimation()
	{
		Weapon weapon = GameApp.GetInstance().GetGameState().GetBattleWeapons()[0];
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
		}
		else if (control.name == "Select_Button" && eventType == 3)
		{
			Debug.Log(control.name);
			audioPlayer.PlayAudio("Button");
			GameApp.GetInstance().GetGameState().Avatar = avatar.GetComponent<AvatarItemData>().avatar_type;
			GameApp.GetInstance().Save();
		}
		else if (control.name == "Buy_Button" && eventType == 3)
		{
			Debug.Log(control.name);
			audioPlayer.PlayAudio("Button");
			GameConfig gameConfig = GameApp.GetInstance().GetGameConfig();
			AvatarType avatar_type = scroll_obj.GetComponent<ScaleScroller>().GetSelectedButton().GetComponent<AvatarItemData>()
				.avatar_type;
			int price = gameConfig.GetAvatarConfig((int)avatar_type).price;
			if (GameApp.GetInstance().GetGameState().BuyAvatar(avatar_type, price))
			{
				Debug.Log("avatar buy success.");
				RefashCashLebel();
				scroll_obj.GetComponent<ScaleScroller>().GetSelectedButton().GetComponent<AvatarItemData>()
					.SetAvatarItem();
				ChangeAvatar(scroll_obj.GetComponent<ScaleScroller>().GetSelectedButton().GetComponent<AvatarItemData>()
					.avatar_type);
					GameApp.GetInstance().Save();
				}
				else
				{
					get_money_panel.SetContent("SHORT ON MONEY!");
					get_money_panel.Show();
					FrameCamera.depth = -5f;
				}
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
				FrameCamera.depth = 1f;
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
