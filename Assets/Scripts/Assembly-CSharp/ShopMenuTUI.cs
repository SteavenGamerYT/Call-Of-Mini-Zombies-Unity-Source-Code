using UnityEngine;
using Zombie3D;

public class ShopMenuTUI : MonoBehaviour, TUIHandler
{
	private TUI m_tui;

	protected TUIInput[] input;

	public GameObject day_label;

	public GameObject cash_label;

	public GameObject avatar;

	public GameObject avatar_mover;

	public GameObject Msg_Box;

	public Camera FrameCamera;

	protected AudioPlayer audioPlayer = new AudioPlayer();

	protected float lastMotionTime;

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
		cash_label.GetComponent<TUIMeshText>().text = "$" + GameApp.GetInstance().GetGameState().GetCash()
			.ToString("N0");
		day_label.GetComponent<TUIMeshText>().text = "DAY " + GameApp.GetInstance().GetGameState().LevelNum;
		GameApp.GetInstance().GetGameState().VS_mode = false;
		GameObject gameObject = AvatarFactory.GetInstance().CreateAvatar(GameApp.GetInstance().GetGameState().Avatar);
		gameObject.transform.position = new Vector3(0f, -1000f, 0f);
		gameObject.transform.rotation = Quaternion.Euler(0f, 163f, 0f);
		avatar = gameObject;
		if (avatar_mover != null)
		{
			avatar_mover.GetComponent<ShopUIAvatarMover>().slider_obj = gameObject;
		}
		Weapon weapon = GameApp.GetInstance().GetGameState().GetBattleWeapons()[0];
		string weaponName = weapon.Name;
		string weaponNameEnd = Weapon.GetWeaponNameEnd(weapon.GetWeaponType());
		GameObject gameObject2 = WeaponFactory.GetInstance().CreateWeaponModel(weaponName, gameObject.transform.position, gameObject.transform.rotation);
		Transform parent = gameObject.transform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 R Clavicle/Bip01 R UpperArm/Bip01 R Forearm/Bip01 R Hand/Weapon_Dummy");
		gameObject2.transform.parent = parent;
		gameObject.GetComponent<Animation>()["Idle01" + weaponNameEnd].wrapMode = WrapMode.Loop;
		gameObject.GetComponent<Animation>().Play("Idle01" + weaponNameEnd);
		lastMotionTime = Time.time;
		OpenClickPlugin.Hide();
		Resources.UnloadUnusedAssets();
		NetworkObj.DestroyNetCom();
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
	}

	private void OnSceneOut()
	{
	}

	private void OnSceneIn()
	{
		FrameCamera.depth = 1f;
		if (!GameApp.GetInstance().GetGameState().AlreadyCountered)
		{
			GameApp.GetInstance().GetGameState().AddScore(1);
			GameApp.GetInstance().GetGameState().review_count++;
			GameApp.GetInstance().GetGameState().AlreadyCountered = true;
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
		if (control.name == "Map_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			FrameCamera.depth = -5f;
			GameApp.GetInstance().Save();
			if (GameApp.GetInstance().GetGameState().last_map_to_shop == "main_map")
			{
				SceneName.SaveSceneStatistics("MainMapTUI");
				m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
					.FadeOut("MainMapTUI");
			}
			else if (GameApp.GetInstance().GetGameState().last_map_to_shop == "net_map")
			{
				SceneName.SaveSceneStatistics("NetMapTUI");
				m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
					.FadeOut("NetMapTUI");
			}
			else if (GameApp.GetInstance().GetGameState().last_map_to_shop == "vs_net_map")
			{
				SceneName.SaveSceneStatistics("NetMapVSTUI");
				m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
					.FadeOut("NetMapVSTUI");
			}
		}
		else if (control.name == "Armory_Button" && eventType == 3)
		{
			Debug.Log(control.name);
			audioPlayer.PlayAudio("Button");
			FrameCamera.depth = -5f;
			SceneName.SaveSceneStatistics("UpgradeTUI");
			m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
				.FadeOut("UpgradeTUI");
		}
		else if (control.name == "Equip_Button" && eventType == 3)
		{
			Debug.Log(control.name);
			audioPlayer.PlayAudio("Button");
			FrameCamera.depth = -5f;
			SceneName.SaveSceneStatistics("EquipMenuTUI");
			m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
				.FadeOut("EquipMenuTUI");
		}
		else if (control.name == "Char_Button" && eventType == 3)
		{
			Debug.Log(control.name);
			audioPlayer.PlayAudio("Button");
			FrameCamera.depth = -5f;
			SceneName.SaveSceneStatistics("AvatarShopMenuTUI");
			m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
				.FadeOut("AvatarShopMenuTUI");
		}
		else if (control.name == "LeaderBoard_Button" && eventType == 3)
		{
			Debug.Log(control.name);
			audioPlayer.PlayAudio("Button");
			GameCenterPlugin.OpenLeaderboard();
		}
		else if (control.name == "Achivment_Button" && eventType == 3)
		{
			Debug.Log(control.name);
			audioPlayer.PlayAudio("Button");
			GameCenterPlugin.OpenAchievement();
		}
		else if (control.name == "Msg_Yes_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			FrameCamera.depth = 1f;
			Msg_Box.GetComponent<MsgBoxDelegate>().Hide();
			GameApp.GetInstance().GetGameState().AddScore(1000);
			GameApp.GetInstance().Save();
			Application.OpenURL("http://www.trinitigame.com/callofminizombies/review/");
		}
		else if (control.name == "Msg_No_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			FrameCamera.depth = 1f;
			Msg_Box.GetComponent<MsgBoxDelegate>().Hide();
		}
	}
}
