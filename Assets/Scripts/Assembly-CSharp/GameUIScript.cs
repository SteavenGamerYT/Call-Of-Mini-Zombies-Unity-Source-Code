using System;
using System.Collections;
using UnityEngine;
using Zombie3D;

public class GameUIScript : MonoBehaviour, UIHandler, ITutorialGameUI, UIDialogEventHandler
{
	protected Rect[] buttonRect;

	public UIManager m_UIManager;

	public string m_ui_material_path;

	protected Material gameuiMaterial;

	protected UIImage lifePacketImg;

	protected UIText lifePacketText;

	protected UIImage playerLogoImage;

	protected UIImage hpBackground;

	protected UIImage hpImage;

	protected UIImage weaponBackground;

	protected UIClickButton weaponLogo;

	protected UIImage joystickImage;

	protected UIImage joystickThumb;

	protected UIImage shootjoystickImage;

	protected UIImage shootjoystickThumb;

	protected UIImage playerLogoBackgroundImage;

	protected UIImage bulletsLogo;

	protected UIText cashText;

	protected UIText weaponInfoText;

	protected UIText fpsText;

	protected UIImage dayclear;

	protected UIImage mask;

	protected UIImage semiMask;

	protected UIImage switchImg;

	protected UIClickButton pauseButton;

	protected DayInfoPanel dayInfoPanel;

	protected UIText multi_nick_name1;

	protected UIImageID multiplayerLogoBackgroundImage1;

	protected UIImageID multiplayerLogoImage1;

	protected UIImageID multihpBackground1;

	protected UIImageID multihpImage1;

	protected UIAnimatedImage multiplayer_help1;

	protected UIText multi_nick_name2;

	protected UIImageID multiplayerLogoBackgroundImage2;

	protected UIImageID multiplayerLogoImage2;

	protected UIImageID multihpBackground2;

	protected UIImageID multihpImage2;

	protected UIAnimatedImage multiplayer_help2;

	protected UIText multi_nick_name3;

	protected UIImageID multiplayerLogoBackgroundImage3;

	protected UIImageID multiplayerLogoImage3;

	protected UIImageID multihpBackground3;

	protected UIImageID multihpImage3;

	protected UIAnimatedImage multiplayer_help3;

	protected UIText player_nick_name;

	protected UIText m_hunting_time_text;

	protected Enemy m_hunting_enemy;

	protected float m_hunting_time = 30f;

	protected UIPanel[] panels = new UIPanel[3];

	protected GameDialog dialog;

	private UnlockPanel unlockPanel;

	protected ITutorialUIEvent tutorialUIEvent;

	protected float frames;

	protected float updateInterval = 2f;

	protected float timeLeft;

	protected string fpsStr;

	protected float accum;

	protected int count;

	protected GameScene gameScene;

	protected Player player;

	private UIPosition uiPos;

	protected float screenRatioX;

	protected float screenRatioY;

	protected float lastUpdateTime;

	protected bool uiInited;

	protected float startTime;

	public bool _finishInit;

	protected NetworkObj net_com;

	protected int muilti_player_hp_bar_cout;

	public void OnLevelWasLoaded()
	{
	}

	private void Awake()
	{
		GameScript.CheckEnemyResourceConfig();
		GameScript.CheckGameResourceConfig();
		GameScript.CheckGlobalResourceConfig();
		UIResourceMgr.GetInstance().LoadAllGameUIMaterials();
		gameuiMaterial = UIResourceMgr.GetInstance().GetMaterial("GameUI");
		if (Application.loadedLevelName == "Zombie3D_Tutorial")
		{
			dialog = new GameDialog(UIDialog.DialogMode.TAP_TO_DISMISS);
			dialog.SetText("font2", string.Empty, ColorName.fontColor_darkorange);
			dialog.SetDialogEventHandler(this);
		}
	}

	public void SetTutorialUIEvent(ITutorialUIEvent tEvent)
	{
		tutorialUIEvent = tEvent;
	}

	public void SetTutorialText(string text)
	{
	}

	public static GameUIScript GetGameUIScript()
	{
		GameObject gameObject = GameObject.Find("SceneGUI");
		if ((bool)gameObject)
		{
			return gameObject.GetComponent<GameUIScript>();
		}
		return null;
	}

	public Material FGetGameUIMaterial()
	{
		return gameuiMaterial;
	}

	private IEnumerator Start()
	{
		yield return 0;
		if (GameApp.GetInstance().GetGameState().endless_multiplayer)
		{
			net_com = GameApp.GetInstance().GetGameState().net_com;
		}
		uiPos = new UIPosition();
		buttonRect = new Rect[4];
		buttonRect[0] = new Rect(650f, 540f, 205f, 89f);
		buttonRect[1] = new Rect(0.4f * (float)Screen.width, 0.25f * (float)Screen.height, 0.24f * (float)Screen.width, 0.08f * (float)Screen.height);
		buttonRect[2] = new Rect(0.4f * (float)Screen.width, 0.25f * (float)Screen.height, 0.14f * (float)Screen.width, 0.14f * (float)Screen.height);
		buttonRect[3] = new Rect(0.4f * (float)Screen.width, 0.25f * (float)Screen.height, 0.14f * (float)Screen.width, 0.14f * (float)Screen.height);
		if (AutoRect.GetPlatform() == Platform.IPad)
		{
			uiPos.LifePacket = new Rect(94f, 672f, 38f, 32f);
			uiPos.LifePacketText = new Rect(138f, 676f, 30f, 30f);
			uiPos.player_nick_name = new Rect(188f, 680f, 300f, 30f);
			uiPos.PlayerLogo = new Rect(-48f, 630f, 116f, 81f);
			uiPos.PlayerLogoBackground = new Rect(-32f, 620f, 134f, 88f);
			uiPos.HPBackground = new Rect(56f, 626f, 288f, 50f);
			uiPos.HPImage = new Rect(56f, 626f, 288f, 50f);
			uiPos.WeaponLogoBackground = new Rect(844f, 620f, 148f, 88f);
			uiPos.WeaponLogo = new Rect(812f, 620f, 194f, 112f);
			uiPos.BulletsLogo = new Rect(572f, 610f, 194f, 112f);
			uiPos.WeaponInfo = new Rect(688f, 622f, 100f, 64f);
			uiPos.PauseButton = new Rect(408f, 588f, 160f, 166f);
			uiPos.CashText = new Rect(0f, 640f, 1024f, 64f);
			uiPos.Mask = new Rect(0f, 0f, 1024f, 768f);
			uiPos.Switch = new Rect(724f, 626f, 148f, 88f);
			uiPos.player1_nick_name = new Rect(38f, 594f, 300f, 30f);
			uiPos.MultiPlayerLogo1 = new Rect(-48f, 574f, 58f, 40.5f);
			uiPos.MultiPlayerLogoBackground1 = new Rect(-32f, 568f, 67f, 44f);
			uiPos.MultiHPBackground1 = new Rect(12f, 572f, 144f, 25f);
			uiPos.MultiHPImage1 = new Rect(12f, 572f, 144f, 25f);
			uiPos.MultiHelpAni1 = new Rect(158f, 558f, 88f, 74f);
			uiPos.player2_nick_name = new Rect(38f, 538f, 300f, 30f);
			uiPos.MultiPlayerLogo2 = new Rect(-48f, 518f, 58f, 40.5f);
			uiPos.MultiPlayerLogoBackground2 = new Rect(-32f, 512f, 67f, 44f);
			uiPos.MultiHPBackground2 = new Rect(12f, 516f, 144f, 25f);
			uiPos.MultiHPImage2 = new Rect(12f, 516f, 144f, 25f);
			uiPos.MultiHelpAni2 = new Rect(158f, 502f, 88f, 74f);
			uiPos.player3_nick_name = new Rect(38f, 482f, 300f, 30f);
			uiPos.MultiPlayerLogo3 = new Rect(-48f, 462f, 58f, 40.5f);
			uiPos.MultiPlayerLogoBackground3 = new Rect(-32f, 456f, 67f, 44f);
			uiPos.MultiHPBackground3 = new Rect(12f, 460f, 144f, 25f);
			uiPos.MultiHPImage3 = new Rect(12f, 460f, 144f, 25f);
			uiPos.MultiHelpAni3 = new Rect(158f, 446f, 88f, 74f);
		}
		for (gameScene = GameApp.GetInstance().GetGameScene(); gameScene == null; gameScene = GameApp.GetInstance().GetGameScene())
		{
			yield return 1;
		}
		player = gameScene.GetPlayer();
		while (player == null)
		{
			yield return 1;
			gameScene = GameApp.GetInstance().GetGameScene();
		}
		_finishInit = true;
		m_UIManager = base.gameObject.AddComponent<UIManager>();
		m_UIManager.SetParameter(8, 1, false);
		m_UIManager.SetUIHandler(this);
		m_hunting_time = GameApp.GetInstance().GetGameConfig().hunting_time;
		int avatarLogoIndex = (int)player.GetAvatarType();
		TexturePosInfo logoInfo = GameUITexturePosition.GetAvatarLogoRect(avatarLogoIndex);
		playerLogoImage = new UIImage();
		playerLogoImage.Rect = AutoRect.AutoPos(uiPos.PlayerLogo);
		playerLogoImage.SetTexture(logoInfo.m_Material, logoInfo.m_TexRect, AutoRect.AutoSize(logoInfo.m_TexRect));
		if (GameApp.GetInstance().GetGameState().endless_multiplayer)
		{
			player_nick_name = new UIText();
			player_nick_name.AlignStyle = UIText.enAlignStyle.left;
			player_nick_name.Rect = AutoRect.AutoPos(uiPos.player_nick_name);
			player_nick_name.Set("font1", GameApp.GetInstance().GetGameState().nick_name, ColorName.fontColor_orange);
			Material material = UIResourceMgr.GetInstance().GetMaterial("MultigameUI");
			lifePacketImg = new UIImage();
			lifePacketImg.SetTexture(material, GameUITexturePosition.LifePacket, AutoRect.AutoSize(GameUITexturePosition.LifePacket));
			lifePacketImg.Rect = AutoRect.AutoPos(uiPos.LifePacket);
			lifePacketText = new UIText();
			lifePacketText.AlignStyle = UIText.enAlignStyle.left;
			lifePacketText.Rect = AutoRect.AutoPos(uiPos.LifePacketText);
			lifePacketText.Set("font2", "x" + player.m_life_packet_count, ColorName.fontColor_orange);
		}
		hpBackground = new UIImage();
		hpBackground.SetTexture(gameuiMaterial, GameUITexturePosition.HPBackground, AutoRect.AutoSize(GameUITexturePosition.HPBackground));
		hpBackground.Rect = AutoRect.AutoPos(uiPos.HPBackground);
		dayclear = new UIImage();
		dayclear.SetTexture(gameuiMaterial, GameUITexturePosition.DayClear, AutoRect.AutoSize(GameUITexturePosition.DayClear));
		dayclear.Rect = AutoRect.AutoPos(uiPos.DayClear);
		dayclear.Visible = false;
		dayclear.Enable = false;
		hpImage = new UIImage();
		hpImage.SetTexture(gameuiMaterial, GameUITexturePosition.HPImage, AutoRect.AutoSize(GameUITexturePosition.HPImage));
		playerLogoBackgroundImage = new UIImage();
		playerLogoBackgroundImage.SetTexture(gameuiMaterial, GameUITexturePosition.PlayerLogoBackground, AutoRect.AutoSize(GameUITexturePosition.PlayerLogoBackground));
		playerLogoBackgroundImage.Rect = AutoRect.AutoPos(uiPos.PlayerLogoBackground);
		weaponBackground = new UIImage();
		weaponBackground.Rect = AutoRect.AutoPos(uiPos.WeaponLogoBackground);
		weaponBackground.SetTexture(gameuiMaterial, GameUITexturePosition.WeaponLogoBackground, AutoRect.AutoSize(GameUITexturePosition.WeaponLogoBackground));
		int weaponLogoIndex = GameApp.GetInstance().GetGameState().GetWeaponIndex(player.GetWeapon());
		TexturePosInfo info = GameUITexturePosition.GetWeaponLogoRect(weaponLogoIndex);
		weaponLogo = new UIClickButton();
		weaponLogo.Rect = AutoRect.AutoPos(uiPos.WeaponLogo);
		weaponLogo.SetTexture(UIButtonBase.State.Normal, info.m_Material, info.m_TexRect, AutoRect.AutoSize(info.m_TexRect));
		weaponLogo.SetTexture(UIButtonBase.State.Pressed, info.m_Material, info.m_TexRect, AutoRect.AutoSize(info.m_TexRect));
		switchImg = new UIImage();
		switchImg.Rect = AutoRect.AutoPos(uiPos.Switch);
		switchImg.SetTexture(gameuiMaterial, GameUITexturePosition.Switch, AutoRect.AutoSize(GameUITexturePosition.Switch));
		switchImg.Enable = true;
		Material buttonsMaterial = UIResourceMgr.GetInstance().GetMaterial("Buttons");
		bulletsLogo = new UIImage();
		bulletsLogo.Rect = AutoRect.AutoPos(uiPos.BulletsLogo);
		Rect bulletlogoRect = ButtonsTexturePosition.GetBulletsLogoRect((int)player.GetWeapon().GetWeaponType());
		bulletsLogo.SetTexture(buttonsMaterial, bulletlogoRect, AutoRect.AutoSize(bulletlogoRect));
		bulletsLogo.Enable = false;
		InputController inputController = player.InputController;
		Vector2 thumbCenter3 = inputController.ThumbCenter;
		joystickImage = new UIImage();
		joystickImage.Rect = new Rect(thumbCenter3.x - inputController.ThumbRadius, (float)Screen.height - thumbCenter3.y - inputController.ThumbRadius, AutoRect.AutoValue(169f), AutoRect.AutoValue(168f));
		joystickImage.SetTexture(gameuiMaterial, GameUITexturePosition.MoveJoystick, AutoRect.AutoSize(GameUITexturePosition.MoveJoystick));
		joystickThumb = new UIImage();
		joystickThumb.SetTexture(gameuiMaterial, GameUITexturePosition.MoveJoystickThumb, AutoRect.AutoSize(GameUITexturePosition.MoveJoystickThumb));
		thumbCenter3 = inputController.ShootThumbCenter;
		shootjoystickImage = new UIImage();
		shootjoystickImage.Rect = new Rect(thumbCenter3.x - inputController.ThumbRadius, (float)Screen.height - thumbCenter3.y - inputController.ThumbRadius, AutoRect.AutoValue(169f), AutoRect.AutoValue(168f));
		shootjoystickImage.SetTexture(gameuiMaterial, GameUITexturePosition.ShootJoystick, AutoRect.AutoSize(GameUITexturePosition.ShootJoystick));
		shootjoystickImage.SetRotation((float)System.Math.PI);
		shootjoystickThumb = new UIImage();
		shootjoystickThumb.SetTexture(gameuiMaterial, GameUITexturePosition.ShootJoystickThumb, AutoRect.AutoSize(GameUITexturePosition.ShootJoystickThumb));
		pauseButton = new UIClickButton();
		pauseButton.Rect = AutoRect.AutoPos(uiPos.PauseButton);
		pauseButton.SetTexture(UIButtonBase.State.Normal, gameuiMaterial, GameUITexturePosition.PauseButtonNormal, AutoRect.AutoSize(GameUITexturePosition.PauseButtonNormal));
		pauseButton.SetTexture(UIButtonBase.State.Pressed, gameuiMaterial, GameUITexturePosition.PauseButtonPressed, AutoRect.AutoSize(GameUITexturePosition.PauseButtonPressed));
		cashText = new UIText();
		cashText.AlignStyle = UIText.enAlignStyle.center;
		cashText.Rect = AutoRect.AutoPos(uiPos.CashText);
		cashText.Set("font1", "$" + GameApp.GetInstance().GetGameState().GetCash()
			.ToString("N0"), ColorName.fontColor_orange);
		weaponInfoText = new UIText();
		weaponInfoText.AlignStyle = UIText.enAlignStyle.left;
		weaponInfoText.Rect = AutoRect.AutoPos(uiPos.WeaponInfo);
		weaponInfoText.Set("font2", fpsStr, ColorName.fontColor_darkorange);
		fpsText = new UIText();
		fpsText.AlignStyle = UIText.enAlignStyle.left;
		fpsText.Rect = AutoRect.AutoPos(uiPos.LevelInfo);
		fpsText.Set("font3", string.Empty, Color.white);
		dayInfoPanel = new DayInfoPanel();
		dayInfoPanel.SetDay(GameApp.GetInstance().GetGameState().LevelNum);
		mask = new UIImage();
		mask.SetTexture(gameuiMaterial, GameUITexturePosition.Mask, AutoRect.AutoSize(uiPos.Mask));
		mask.Rect = AutoRect.AutoValuePos(uiPos.Mask);
		Vector2 size = AutoRect.AutoSize(GameUITexturePosition.SemiMaskSize);
		Rect pos = AutoRect.AutoPos(uiPos.RightSemiMask);
		if (AutoRect.GetPlatform() == Platform.IPad)
		{
			size = new Vector2(512f, 768f);
			pos = new Rect(512f, 0f, 512f, 768f);
		}
		semiMask = new UIImage();
		semiMask.SetTexture(gameuiMaterial, GameUITexturePosition.Mask, size);
		semiMask.Rect = pos;
		unlockPanel = new UnlockPanel();
		m_UIManager.Add(dayInfoPanel);
		m_UIManager.Add(hpBackground);
		m_UIManager.Add(hpImage);
		m_UIManager.Add(playerLogoBackgroundImage);
		m_UIManager.Add(playerLogoImage);
		if (GameApp.GetInstance().GetGameState().endless_multiplayer)
		{
			m_UIManager.Add(player_nick_name);
			m_UIManager.Add(lifePacketImg);
			m_UIManager.Add(lifePacketText);
		}
		m_UIManager.Add(joystickImage);
		m_UIManager.Add(joystickThumb);
		m_UIManager.Add(shootjoystickImage);
		m_UIManager.Add(shootjoystickThumb);
		m_UIManager.Add(weaponBackground);
		m_UIManager.Add(weaponLogo);
		m_UIManager.Add(switchImg);
		m_UIManager.Add(pauseButton);
		if (Application.loadedLevelName == "Zombie3D_Tutorial")
		{
			m_UIManager.Add(dialog);
		}
		m_UIManager.Add(bulletsLogo);
		m_UIManager.Add(weaponInfoText);
		m_UIManager.Add(mask);
		m_UIManager.Add(dayclear);
		m_UIManager.Add(unlockPanel);
		semiMask.Enable = false;
		semiMask.Visible = false;
		if (!GameApp.GetInstance().GetGameState().endless_level && !GameApp.GetInstance().GetGameState().VS_mode)
		{
			dayInfoPanel.Show();
		}
		uiInited = true;
		EnableTutorialOKButton(false);
		mask.Enable = false;
		mask.Visible = false;
		SetWeaponLogo(player.GetWeapon().GetWeaponType());
		panels[0] = new PauseMenuUI();
		((PauseMenuUI)panels[0]).SetGameUIScript(this);
		panels[1] = new GameOverUI();
		panels[2] = new NewItemUI();
		for (int i = 0; i < 3; i++)
		{
			m_UIManager.Add(panels[i]);
		}
		startTime = Time.time;
		OpenClickPlugin.Hide();
		UIResourceMgr.GetInstance().UnloadAllUIMaterials();
	}

	public void SetLifePacketCount()
	{
		lifePacketText.SetText("x" + player.m_life_packet_count);
	}

	public void InitHuntingTimeText(Enemy enemy)
	{
		m_hunting_enemy = enemy;
		TimeSpan timeSpan = new TimeSpan(0, 0, (int)m_hunting_time);
		m_hunting_time_text = new UIText();
		m_hunting_time_text.Rect = new Rect(0.05f * (float)Screen.width, 0.75f * (float)Screen.height, 400f, 50f);
		m_hunting_time_text.AlignStyle = UIText.enAlignStyle.left;
		m_hunting_time_text.Rect = AutoRect.AutoPos(uiPos.HuntingText);
		m_hunting_time_text.Set("font1", timeSpan.ToString(), ColorName.fontColor_orange);
		m_UIManager.Add(m_hunting_time_text);
	}

	public void DeleteHuntingTimeText()
	{
		m_UIManager.Remove(m_hunting_time_text);
		m_hunting_time_text = null;
	}

	public UIPanel GetPanel(int menuID)
	{
		return panels[menuID];
	}

	public void EnableTutorialOKButton(bool enable)
	{
	}

	public void ClearLevelInfo()
	{
	}

	public void RemoveMultiHpBar(int id)
	{
		if (multihpImage1 != null && multihpImage1.m_image_id == id)
		{
			m_UIManager.Remove(multihpBackground1);
			m_UIManager.Remove(multihpImage1);
			m_UIManager.Remove(multiplayerLogoBackgroundImage1);
			m_UIManager.Remove(multiplayerLogoImage1);
			m_UIManager.Remove(multi_nick_name1);
			m_UIManager.Remove(multiplayer_help1);
		}
		else if (multihpImage2 != null && multihpImage2.m_image_id == id)
		{
			m_UIManager.Remove(multihpBackground2);
			m_UIManager.Remove(multihpImage2);
			m_UIManager.Remove(multiplayerLogoBackgroundImage2);
			m_UIManager.Remove(multiplayerLogoImage2);
			m_UIManager.Remove(multi_nick_name2);
			m_UIManager.Remove(multiplayer_help2);
		}
		else if (multihpImage3 != null && multihpImage3.m_image_id == id)
		{
			m_UIManager.Remove(multihpBackground3);
			m_UIManager.Remove(multihpImage3);
			m_UIManager.Remove(multiplayerLogoBackgroundImage3);
			m_UIManager.Remove(multiplayerLogoImage3);
			m_UIManager.Remove(multi_nick_name3);
			m_UIManager.Remove(multiplayer_help3);
		}
	}

	public void AddMultiHpBar(int id, AvatarType type, string nick_name, int index)
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add("id", id);
		hashtable.Add("type", (int)type);
		hashtable.Add("name", nick_name);
		hashtable.Add("index", index);
		StartCoroutine("AddMultiHpBarTime", hashtable);
	}

	public IEnumerator AddMultiHpBarTime(Hashtable data)
	{
		while (m_UIManager == null)
		{
			yield return 1;
		}
		int id = (int)data["id"];
		int type = (int)data["type"];
		string nick_name = (string)data["name"];
		int index = (int)data["index"];
		if (muilti_player_hp_bar_cout == 0)
		{
			TexturePosInfo avatarLogoRect = GameUITexturePosition.GetAvatarLogoRect(type);
			multiplayerLogoBackgroundImage1 = new UIImageID();
			multiplayerLogoBackgroundImage1.SetTexture(gameuiMaterial, GameUITexturePosition.PlayerLogoBackground, AutoRect.AutoSize(GameUITexturePosition.PlayerLogoBackground, 0.5f));
			multiplayerLogoBackgroundImage1.Rect = AutoRect.AutoPos(uiPos.MultiPlayerLogoBackground1);
			multiplayerLogoBackgroundImage1.m_image_id = id;
			multiplayerLogoImage1 = new UIImageID();
			multiplayerLogoImage1.Rect = AutoRect.AutoPos(uiPos.MultiPlayerLogo1);
			multiplayerLogoImage1.SetTexture(avatarLogoRect.m_Material, avatarLogoRect.m_TexRect, AutoRect.AutoSize(avatarLogoRect.m_TexRect, 0.5f));
			multiplayerLogoImage1.m_image_id = id;
			multihpBackground1 = new UIImageID();
			multihpBackground1.SetTexture(gameuiMaterial, GameUITexturePosition.HPBackground, AutoRect.AutoSize(GameUITexturePosition.HPBackground, 0.5f));
			multihpBackground1.Rect = AutoRect.AutoPos(uiPos.MultiHPBackground1);
			multihpBackground1.m_image_id = id;
			multihpImage1 = new UIImageID();
			multihpImage1.SetTexture(gameuiMaterial, GameUITexturePosition.HPImage, AutoRect.AutoSize(GameUITexturePosition.HPImage, 0.5f));
			multihpImage1.Rect = AutoRect.AutoPos(uiPos.MultiHPImage1);
			multihpImage1.m_image_id = id;
			multi_nick_name1 = new UIText();
			multi_nick_name1.AlignStyle = UIText.enAlignStyle.left;
			multi_nick_name1.Rect = AutoRect.AutoPos(uiPos.player1_nick_name);
			multi_nick_name1.Set("font2", nick_name, ColorName.GetPlayerMarkColor(index));
			Material material = UIResourceMgr.GetInstance().GetMaterial("MultigameUI");
			multiplayer_help1 = new UIAnimatedImage();
			multiplayer_help1.SetAnimationFrameRate(5);
			multiplayer_help1.AddAnimation(material, GameUITexturePosition.HelpAni1, AutoRect.AutoSize(GameUITexturePosition.HelpAni1));
			multiplayer_help1.AddAnimation(material, GameUITexturePosition.HelpAni2, AutoRect.AutoSize(GameUITexturePosition.HelpAni2));
			multiplayer_help1.Rect = AutoRect.AutoPos(uiPos.MultiHelpAni1);
			multiplayer_help1.Visible = false;
			m_UIManager.Add(multihpBackground1);
			m_UIManager.Add(multihpImage1);
			m_UIManager.Add(multiplayerLogoBackgroundImage1);
			m_UIManager.Add(multiplayerLogoImage1);
			m_UIManager.Add(multi_nick_name1);
			m_UIManager.Add(multiplayer_help1);
		}
		else if (muilti_player_hp_bar_cout == 1)
		{
			TexturePosInfo avatarLogoRect2 = GameUITexturePosition.GetAvatarLogoRect(type);
			multiplayerLogoBackgroundImage2 = new UIImageID();
			multiplayerLogoBackgroundImage2.SetTexture(gameuiMaterial, GameUITexturePosition.PlayerLogoBackground, AutoRect.AutoSize(GameUITexturePosition.PlayerLogoBackground, 0.5f));
			multiplayerLogoBackgroundImage2.Rect = AutoRect.AutoPos(uiPos.MultiPlayerLogoBackground2);
			multiplayerLogoBackgroundImage2.m_image_id = id;
			multiplayerLogoImage2 = new UIImageID();
			multiplayerLogoImage2.Rect = AutoRect.AutoPos(uiPos.MultiPlayerLogo2);
			multiplayerLogoImage2.SetTexture(avatarLogoRect2.m_Material, avatarLogoRect2.m_TexRect, AutoRect.AutoSize(avatarLogoRect2.m_TexRect, 0.5f));
			multiplayerLogoImage2.m_image_id = id;
			multihpBackground2 = new UIImageID();
			multihpBackground2.SetTexture(gameuiMaterial, GameUITexturePosition.HPBackground, AutoRect.AutoSize(GameUITexturePosition.HPBackground, 0.5f));
			multihpBackground2.Rect = AutoRect.AutoPos(uiPos.MultiHPBackground2);
			multihpBackground2.m_image_id = id;
			multihpImage2 = new UIImageID();
			multihpImage2.SetTexture(gameuiMaterial, GameUITexturePosition.HPImage, AutoRect.AutoSize(GameUITexturePosition.HPImage, 0.5f));
			multihpImage2.Rect = AutoRect.AutoPos(uiPos.MultiHPImage2);
			multihpImage2.m_image_id = id;
			multi_nick_name2 = new UIText();
			multi_nick_name2.AlignStyle = UIText.enAlignStyle.left;
			multi_nick_name2.Rect = AutoRect.AutoPos(uiPos.player2_nick_name);
			multi_nick_name2.Set("font2", nick_name, ColorName.GetPlayerMarkColor(index));
			Material material2 = UIResourceMgr.GetInstance().GetMaterial("MultigameUI");
			multiplayer_help2 = new UIAnimatedImage();
			multiplayer_help2.SetAnimationFrameRate(5);
			multiplayer_help2.AddAnimation(material2, GameUITexturePosition.HelpAni1, AutoRect.AutoSize(GameUITexturePosition.HelpAni1));
			multiplayer_help2.AddAnimation(material2, GameUITexturePosition.HelpAni2, AutoRect.AutoSize(GameUITexturePosition.HelpAni2));
			multiplayer_help2.Rect = AutoRect.AutoPos(uiPos.MultiHelpAni2);
			multiplayer_help2.Visible = false;
			m_UIManager.Add(multihpBackground2);
			m_UIManager.Add(multihpImage2);
			m_UIManager.Add(multiplayerLogoBackgroundImage2);
			m_UIManager.Add(multiplayerLogoImage2);
			m_UIManager.Add(multi_nick_name2);
			m_UIManager.Add(multiplayer_help2);
		}
		else if (muilti_player_hp_bar_cout == 2)
		{
			TexturePosInfo avatarLogoRect3 = GameUITexturePosition.GetAvatarLogoRect(type);
			multiplayerLogoBackgroundImage3 = new UIImageID();
			multiplayerLogoBackgroundImage3.SetTexture(gameuiMaterial, GameUITexturePosition.PlayerLogoBackground, AutoRect.AutoSize(GameUITexturePosition.PlayerLogoBackground, 0.5f));
			multiplayerLogoBackgroundImage3.Rect = AutoRect.AutoPos(uiPos.MultiPlayerLogoBackground3);
			multiplayerLogoBackgroundImage3.m_image_id = id;
			multiplayerLogoImage3 = new UIImageID();
			multiplayerLogoImage3.Rect = AutoRect.AutoPos(uiPos.MultiPlayerLogo3);
			multiplayerLogoImage3.SetTexture(avatarLogoRect3.m_Material, avatarLogoRect3.m_TexRect, AutoRect.AutoSize(avatarLogoRect3.m_TexRect, 0.5f));
			multiplayerLogoImage3.m_image_id = id;
			multihpBackground3 = new UIImageID();
			multihpBackground3.SetTexture(gameuiMaterial, GameUITexturePosition.HPBackground, AutoRect.AutoSize(GameUITexturePosition.HPBackground, 0.5f));
			multihpBackground3.Rect = AutoRect.AutoPos(uiPos.MultiHPBackground3);
			multihpBackground3.m_image_id = id;
			multihpImage3 = new UIImageID();
			multihpImage3.SetTexture(gameuiMaterial, GameUITexturePosition.HPImage, AutoRect.AutoSize(GameUITexturePosition.HPImage, 0.5f));
			multihpImage3.Rect = AutoRect.AutoPos(uiPos.MultiHPImage3);
			multihpImage3.m_image_id = id;
			multi_nick_name3 = new UIText();
			multi_nick_name3.AlignStyle = UIText.enAlignStyle.left;
			multi_nick_name3.Rect = AutoRect.AutoPos(uiPos.player3_nick_name);
			multi_nick_name3.Set("font2", nick_name, ColorName.GetPlayerMarkColor(index));
			Material material3 = UIResourceMgr.GetInstance().GetMaterial("MultigameUI");
			multiplayer_help3 = new UIAnimatedImage();
			multiplayer_help3.SetAnimationFrameRate(5);
			multiplayer_help3.AddAnimation(material3, GameUITexturePosition.HelpAni1, AutoRect.AutoSize(GameUITexturePosition.HelpAni1));
			multiplayer_help3.AddAnimation(material3, GameUITexturePosition.HelpAni2, AutoRect.AutoSize(GameUITexturePosition.HelpAni2));
			multiplayer_help3.Rect = AutoRect.AutoPos(uiPos.MultiHelpAni3);
			multiplayer_help3.Visible = false;
			m_UIManager.Add(multihpBackground3);
			m_UIManager.Add(multihpImage3);
			m_UIManager.Add(multiplayerLogoBackgroundImage3);
			m_UIManager.Add(multiplayerLogoImage3);
			m_UIManager.Add(multi_nick_name3);
			m_UIManager.Add(multiplayer_help3);
		}
		muilti_player_hp_bar_cout++;
	}

	public void SetMultiHelpAniStatus(int id, bool status)
	{
		if (multihpImage1 != null && multihpImage1.m_image_id == id)
		{
			multiplayer_help1.Visible = status;
		}
		else if (multihpImage2 != null && multihpImage2.m_image_id == id)
		{
			multiplayer_help2.Visible = status;
		}
		else if (multihpImage3 != null && multihpImage3.m_image_id == id)
		{
			multiplayer_help3.Visible = status;
		}
	}

	public void updateMultiHpBar(int id, Player mPlayer)
	{
		if (multihpImage1 != null && multihpImage1.m_image_id == id)
		{
			float guiHp = mPlayer.GetGuiHp();
			float num = uiPos.HPImage.width * guiHp / mPlayer.GetMaxHp();
			int num2 = (int)num;
			if (num2 % 2 != 0)
			{
				num2++;
			}
			if (multihpImage1 != null)
			{
				multihpImage1.Rect = AutoRect.AutoPos(new Rect(uiPos.MultiHPImage1.xMin, uiPos.MultiHPImage1.yMin, num2 / 2, uiPos.MultiHPImage1.height));
				multihpImage1.SetTexture(gameuiMaterial, GameUITexturePosition.GetHPTextureRect(num2), AutoRect.AutoSize(GameUITexturePosition.GetHPTextureRect(num2), 0.5f));
			}
		}
		else if (multihpImage2 != null && multihpImage2.m_image_id == id)
		{
			float guiHp2 = mPlayer.GetGuiHp();
			float num3 = uiPos.HPImage.width * guiHp2 / mPlayer.GetMaxHp();
			int num4 = (int)num3;
			if (num4 % 2 != 0)
			{
				num4++;
			}
			if (multihpImage2 != null)
			{
				multihpImage2.Rect = AutoRect.AutoPos(new Rect(uiPos.MultiHPImage2.xMin, uiPos.MultiHPImage2.yMin, num4 / 2, uiPos.MultiHPImage2.height));
				multihpImage2.SetTexture(gameuiMaterial, GameUITexturePosition.GetHPTextureRect(num4), AutoRect.AutoSize(GameUITexturePosition.GetHPTextureRect(num4), 0.5f));
			}
		}
		else if (multihpImage3 != null && multihpImage3.m_image_id == id)
		{
			float guiHp3 = mPlayer.GetGuiHp();
			float num5 = uiPos.HPImage.width * guiHp3 / mPlayer.GetMaxHp();
			int num6 = (int)num5;
			if (num6 % 2 != 0)
			{
				num6++;
			}
			if (multihpImage3 != null)
			{
				multihpImage3.Rect = AutoRect.AutoPos(new Rect(uiPos.MultiHPImage3.xMin, uiPos.MultiHPImage3.yMin, num6 / 2, uiPos.MultiHPImage3.height));
				multihpImage3.SetTexture(gameuiMaterial, GameUITexturePosition.GetHPTextureRect(num6), AutoRect.AutoSize(GameUITexturePosition.GetHPTextureRect(num6), 0.5f));
			}
		}
	}

	private void Update()
	{
		if (uiInited)
		{
			dayInfoPanel.UpdateAnimation();
			panels[1].UpdateLogic();
			if (FadeAnimationScript.GetInstance().FadeOutComplete())
			{
				UITouchInner[] array = ((!Application.isMobilePlatform) ? WindowsInputMgr.MockTouches() : iPhoneInputMgr.MockTouches());
				UITouchInner[] array2 = array;
				foreach (UITouchInner touch in array2)
				{
					if (m_UIManager.HandleInput(touch))
					{
					}
				}
			}
			if (gameScene.GamePlayingState == PlayingState.GameWin)
			{
				dayclear.Visible = true;
			}
			if (!GameApp.GetInstance().GetGameScene().GetPlayer()
				.InputController.EnableShootingInput)
			{
				semiMask.Visible = true;
			}
			else
			{
				semiMask.Visible = false;
			}
		}
		if (player != null && m_hunting_time_text != null)
		{
			m_hunting_time -= Time.deltaTime;
			int seconds = (int)m_hunting_time;
			TimeSpan timeSpan = new TimeSpan(0, 0, seconds);
			m_hunting_time_text.Set("font1", timeSpan.ToString(), ColorName.fontColor_orange);
			if (m_hunting_time <= 0f)
			{
				m_hunting_enemy.RemoveEnemyNow();
				DeleteHuntingTimeText();
			}
		}
		if (Time.time - lastUpdateTime < 0.03f || !uiInited)
		{
			return;
		}
		lastUpdateTime = Time.time;
		if (player != null)
		{
			InputController inputController = player.InputController;
			float guiHp = player.GetGuiHp();
			float num = uiPos.HPImage.width * guiHp / player.GetMaxHp();
			int num2 = (int)num;
			if (num2 % 2 != 0)
			{
				num2++;
			}
			if (hpImage != null)
			{
				hpImage.Rect = AutoRect.AutoPos(new Rect(uiPos.HPImage.xMin, uiPos.HPImage.yMin, num2, uiPos.HPImage.height));
				hpImage.SetTexture(gameuiMaterial, GameUITexturePosition.GetHPTextureRect(num2), AutoRect.AutoSize(GameUITexturePosition.GetHPTextureRect(num2)));
			}
			Weapon weapon = player.GetWeapon();
			if (weapon.GetWeaponType() == WeaponType.Saw || weapon.GetWeaponType() == WeaponType.Sword)
			{
				weaponInfoText.SetText(string.Empty);
			}
			else
			{
				weaponInfoText.SetText(" x" + weapon.BulletCount);
			}
			Vector2 lastTouchPos = inputController.LastTouchPos;
			joystickThumb.Rect = new Rect(lastTouchPos.x - AutoRect.AutoValue(0.5f * GameUITexturePosition.MoveJoystickThumb.width), lastTouchPos.y - AutoRect.AutoValue(0.5f * GameUITexturePosition.MoveJoystickThumb.height), AutoRect.AutoValue(GameUITexturePosition.MoveJoystickThumb.width), AutoRect.AutoValue(GameUITexturePosition.MoveJoystickThumb.height));
			shootjoystickThumb.Rect = new Rect(inputController.LastShootTouch.x - AutoRect.AutoValue(0.5f * GameUITexturePosition.ShootJoystickThumb.width), inputController.LastShootTouch.y - AutoRect.AutoValue(0.5f * GameUITexturePosition.ShootJoystickThumb.height), AutoRect.AutoValue(GameUITexturePosition.ShootJoystickThumb.width), AutoRect.AutoValue(GameUITexturePosition.ShootJoystickThumb.height));
		}
		if (!GameApp.GetInstance().GetGameState().endless_multiplayer)
		{
			return;
		}
		int num3 = 0;
		for (int j = 0; j < 4; j++)
		{
			if (net_com.netUserInfo_array[j] != null && net_com.netUserInfo_array[j].multiplayer != null)
			{
				updateMultiHpBar(net_com.netUserInfo_array[j].user_id, net_com.netUserInfo_array[j].multiplayer);
				num3++;
			}
		}
	}

	public void SetWeaponLogo(WeaponType weaponType)
	{
		if (uiInited)
		{
			int weaponIndex = GameApp.GetInstance().GetGameState().GetWeaponIndex(player.GetWeapon());
			TexturePosInfo weaponLogoRect = GameUITexturePosition.GetWeaponLogoRect(weaponIndex);
			weaponLogo.SetTexture(UIButtonBase.State.Normal, weaponLogoRect.m_Material, weaponLogoRect.m_TexRect, AutoRect.AutoSize(weaponLogoRect.m_TexRect));
			weaponLogo.SetTexture(UIButtonBase.State.Pressed, weaponLogoRect.m_Material, weaponLogoRect.m_TexRect, AutoRect.AutoSize(weaponLogoRect.m_TexRect));
			Material material = UIResourceMgr.GetInstance().GetMaterial("Buttons");
			Rect bulletsLogoRect = ButtonsTexturePosition.GetBulletsLogoRect((int)player.GetWeapon().GetWeaponType());
			bulletsLogo.SetTexture(material, bulletsLogoRect, AutoRect.AutoSize(bulletsLogoRect));
		}
	}

	public void HandleEvent(UIControl control, int command, float wparam, float lparam)
	{
		if (control == weaponLogo || control == switchImg)
		{
			player.NextWeapon();
		}
		else
		{
			if (control != pauseButton)
			{
				return;
			}
			if (GameApp.GetInstance().GetGameScene().GamePlayingState == PlayingState.GamePlaying)
			{
				if (GameApp.GetInstance().GetGameState().endless_multiplayer || GameApp.GetInstance().GetGameState().VS_mode)
				{
					panels[0].Show();
					OpenClickPlugin.Show(false);
				}
				else
				{
					Time.timeScale = 0f;
					panels[0].Show();
					OpenClickPlugin.Show(false);
				}
			}
			else if (GameApp.GetInstance().GetGameScene().GamePlayingState == PlayingState.GameLose && GameApp.GetInstance().GetGameState().endless_multiplayer)
			{
				GameMultiplayerScene gameMultiplayerScene = GameApp.GetInstance().GetGameScene() as GameMultiplayerScene;
				if (gameMultiplayerScene.m_multi_player_arr.Count > 0)
				{
					panels[0].Show();
					OpenClickPlugin.Show(false);
				}
			}
		}
	}

	public GameDialog GetDialog()
	{
		return dialog;
	}

	public void Yes()
	{
		tutorialUIEvent.OK(player);
	}

	public void No()
	{
	}
}
