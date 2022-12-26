using System;
using System.Collections;
using UnityEngine;
using Zombie3D;

public class MapUIPanel : UIPanel, UIHandler
{
	protected const int MAP_COUNT = 8;

	protected const int OFFSET_LIMIT = 40;

	protected Rect[] buttonRect;

	protected UITextImage daysPanel;

	protected CashPanel cashPanel;

	protected UIClickButton returnButton;

	protected UIClickButton optionsButton;

	protected UIClickButton endlessButton;

	private MapUIPosition uiPos;

	protected bool buttonPressed;

	protected float screenRatioX;

	protected float screenRatioY;

	protected bool init;

	protected bool setAudioTime;

	protected float startTime;

	protected Timer fadeTimer = new Timer();

	protected UIImage background;

	protected UIColoredButton[] mapButtons = new UIColoredButton[8];

	protected UIAnimatedImage[] zombieAnimations = new UIAnimatedImage[8];

	protected UIColoredImage[] preyTipImg = new UIColoredImage[8];

	protected UIAnimatedImage EndlessAnimation;

	protected UIAnimatedImage CoopAnimation;

	protected UITextButton newShopButton;

	protected UIClickButton paper_model_button;

	protected int[] infection = new int[8];

	protected int[] prey_mission = new int[8];

	protected Vector2 _offsetRect;

	protected ArrayList _Controls_Enable_Move;

	protected ArrayList _Buttons_Enable_Move;

	protected Vector2 MAP_OFFSET_LIMIT = AutoRect.AutoValuePos(new Vector2(380f, 204f));

	protected Rect _tempRect;

	public void Start()
	{
		if (GameObject.Find("StatisticsTimer") == null)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load("Prefabs/StatisticsTimer")) as GameObject;
			gameObject.name = "StatisticsTimer";
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
		}
		OpenClickPlugin.Hide();
		startTime = Time.time;
		for (int i = 0; i < 8; i++)
		{
			infection[i] = -1;
			prey_mission[i] = -1;
		}
		if (GameApp.GetInstance().GetGameState().LevelNum == 1)
		{
			infection[0] = 1;
		}
		else
		{
			for (int j = 0; j < 3; j++)
			{
				int num = UnityEngine.Random.Range(0, 8);
				infection[num] = 1;
				if (j == 0 && GameApp.GetInstance().GetGameState().LevelNum >= GameApp.GetInstance().GetGameConfig().hunting_level)
				{
					prey_mission[num] = 1;
				}
			}
		}
		_Controls_Enable_Move = new ArrayList();
		_Buttons_Enable_Move = new ArrayList();
		Init();
	}

	private void Init()
	{
		GameApp.GetInstance().ClearScene();
		SetUIHandler(this);
		uiPos = new MapUIPosition();
		Material material = UIResourceMgr.GetInstance().GetMaterial("MainMap01");
		Material material2 = UIResourceMgr.GetInstance().GetMaterial("MainMap02");
		Material material3 = UIResourceMgr.GetInstance().GetMaterial("MainMap03");
		background = new UIImage();
		background.SetTexture(material, MapUITexturePosition.Background1, AutoRect.AutoSize(MapUITexturePosition.Background1));
		background.Rect = AutoRect.AutoPos(uiPos.Background1);
		background.OriRect = background.Rect;
		Add(background);
		_Controls_Enable_Move.Add(background);
		background = new UIImage();
		background.SetTexture(material2, MapUITexturePosition.Background2, AutoRect.AutoSize(MapUITexturePosition.Background2));
		background.Rect = AutoRect.AutoPos(uiPos.Background2);
		if (Screen.width == 480)
		{
			background.Rect = new Rect(background.Rect.x - 1f, background.Rect.y, background.Rect.width, background.Rect.height);
		}
		background.OriRect = background.Rect;
		Add(background);
		_Controls_Enable_Move.Add(background);
		SetUIHandler(this);
		for (int i = 0; i < 8; i++)
		{
			mapButtons[i] = new UIColoredButton();
			mapButtons[i].SetAnimatedColor(new Color(0.7019608f, 0f, 0f));
			Add(mapButtons[i]);
			_Buttons_Enable_Move.Add(mapButtons[i]);
			zombieAnimations[i] = new UIAnimatedImage();
			Add(zombieAnimations[i]);
			_Controls_Enable_Move.Add(zombieAnimations[i]);
			zombieAnimations[i].SetAnimationFrameRate(5);
			zombieAnimations[i].AddAnimation(material, MapUITexturePosition.ZombieAnimation1, AutoRect.AutoSize(MapUITexturePosition.ZombieAnimation1));
			zombieAnimations[i].AddAnimation(material, MapUITexturePosition.ZombieAnimation2, AutoRect.AutoSize(MapUITexturePosition.ZombieAnimation2));
			zombieAnimations[i].AddAnimation(material, MapUITexturePosition.ZombieAnimation3, AutoRect.AutoSize(MapUITexturePosition.ZombieAnimation3));
			preyTipImg[i] = new UIColoredImage();
			Add(preyTipImg[i]);
			_Controls_Enable_Move.Add(preyTipImg[i]);
			preyTipImg[i].SetTexture(material, MapUITexturePosition.PreyTip, AutoRect.AutoSize(MapUITexturePosition.PreyTip));
			mapButtons[i].Enable = false;
			zombieAnimations[i].Visible = false;
			preyTipImg[i].Visible = false;
			preyTipImg[i].Enable = false;
		}
		EndlessAnimation = new UIAnimatedImage();
		Add(EndlessAnimation);
		_Controls_Enable_Move.Add(EndlessAnimation);
		EndlessAnimation.SetAnimationFrameRate(10);
		EndlessAnimation.AddAnimation(material2, MapUITexturePosition.EndlessAnimation1, AutoRect.AutoSize(MapUITexturePosition.EndlessAnimation1));
		EndlessAnimation.AddAnimation(material2, MapUITexturePosition.EndlessAnimation2, AutoRect.AutoSize(MapUITexturePosition.EndlessAnimation2));
		EndlessAnimation.AddAnimation(material2, MapUITexturePosition.EndlessAnimation3, AutoRect.AutoSize(MapUITexturePosition.EndlessAnimation3));
		for (int j = 0; j < 10; j++)
		{
			EndlessAnimation.AddAnimation(material2, MapUITexturePosition.EndlessAnimation4, AutoRect.AutoSize(MapUITexturePosition.EndlessAnimation4));
		}
		EndlessAnimation.Visible = true;
		EndlessAnimation.Rect = AutoRect.AutoPos(uiPos.EndlessButton);
		EndlessAnimation.OriRect = EndlessAnimation.Rect;
		CoopAnimation = new UIAnimatedImage();
		Add(CoopAnimation);
		_Controls_Enable_Move.Add(CoopAnimation);
		CoopAnimation.SetAnimationFrameRate(10);
		CoopAnimation.AddAnimation(material, MapUITexturePosition.CoopAnimation2, AutoRect.AutoSize(MapUITexturePosition.CoopAnimation1));
		CoopAnimation.AddAnimation(material, MapUITexturePosition.CoopAnimation3, AutoRect.AutoSize(MapUITexturePosition.CoopAnimation2));
		CoopAnimation.AddAnimation(material, MapUITexturePosition.CoopAnimation4, AutoRect.AutoSize(MapUITexturePosition.CoopAnimation3));
		for (int k = 0; k < 10; k++)
		{
			CoopAnimation.AddAnimation(material, MapUITexturePosition.CoopAnimation1, AutoRect.AutoSize(MapUITexturePosition.CoopAnimation4));
		}
		CoopAnimation.Visible = true;
		CoopAnimation.Rect = AutoRect.AutoPos(uiPos.CoopAni);
		CoopAnimation.OriRect = CoopAnimation.Rect;
		for (int l = 0; l < 8; l++)
		{
			if (infection[l] != -1)
			{
				mapButtons[l].Enable = true;
				zombieAnimations[l].Visible = true;
				if (prey_mission[l] != -1)
				{
					preyTipImg[l].Visible = true;
				}
			}
		}
		mapButtons[0].SetTexture(UIButtonBase.State.Normal, material2, MapUITexturePosition.FactoryImg, AutoRect.AutoSize(MapUITexturePosition.FactoryImg));
		mapButtons[1].SetTexture(UIButtonBase.State.Normal, material2, MapUITexturePosition.HospitalImg, AutoRect.AutoSize(MapUITexturePosition.HospitalImg));
		mapButtons[2].SetTexture(UIButtonBase.State.Normal, material2, MapUITexturePosition.ParkingImg, AutoRect.AutoSize(MapUITexturePosition.ParkingImg));
		mapButtons[3].SetTexture(UIButtonBase.State.Normal, material2, MapUITexturePosition.Village, AutoRect.AutoSize(MapUITexturePosition.Village));
		mapButtons[4].SetTexture(UIButtonBase.State.Normal, material2, MapUITexturePosition.Church, AutoRect.AutoSize(MapUITexturePosition.Church));
		mapButtons[5].SetTexture(UIButtonBase.State.Normal, material2, MapUITexturePosition.Village2, AutoRect.AutoSize(MapUITexturePosition.Village2));
		mapButtons[6].SetTexture(UIButtonBase.State.Normal, material3, MapUITexturePosition.Recycle, AutoRect.AutoSize(MapUITexturePosition.Recycle));
		mapButtons[7].SetTexture(UIButtonBase.State.Normal, material3, MapUITexturePosition.PowerStation, AutoRect.AutoSize(MapUITexturePosition.PowerStation));
		mapButtons[0].SetTexture(UIButtonBase.State.Pressed, material2, MapUITexturePosition.FactoryImg, AutoRect.AutoSize(MapUITexturePosition.FactoryImg));
		mapButtons[1].SetTexture(UIButtonBase.State.Pressed, material2, MapUITexturePosition.HospitalImg, AutoRect.AutoSize(MapUITexturePosition.HospitalImg));
		mapButtons[2].SetTexture(UIButtonBase.State.Pressed, material2, MapUITexturePosition.ParkingImg, AutoRect.AutoSize(MapUITexturePosition.ParkingImg));
		mapButtons[3].SetTexture(UIButtonBase.State.Pressed, material2, MapUITexturePosition.Village, AutoRect.AutoSize(MapUITexturePosition.Village));
		mapButtons[4].SetTexture(UIButtonBase.State.Pressed, material2, MapUITexturePosition.Church, AutoRect.AutoSize(MapUITexturePosition.Church));
		mapButtons[5].SetTexture(UIButtonBase.State.Pressed, material2, MapUITexturePosition.Village2, AutoRect.AutoSize(MapUITexturePosition.Village2));
		mapButtons[6].SetTexture(UIButtonBase.State.Pressed, material3, MapUITexturePosition.Recycle, AutoRect.AutoSize(MapUITexturePosition.Recycle));
		mapButtons[7].SetTexture(UIButtonBase.State.Pressed, material3, MapUITexturePosition.PowerStation, AutoRect.AutoSize(MapUITexturePosition.PowerStation));
		mapButtons[0].Rect = AutoRect.AutoPos(uiPos.FactoryButton);
		mapButtons[1].Rect = AutoRect.AutoPos(uiPos.HospitalButton);
		mapButtons[2].Rect = AutoRect.AutoPos(uiPos.ParkingButton);
		mapButtons[3].Rect = AutoRect.AutoPos(uiPos.VillageButton);
		mapButtons[4].Rect = AutoRect.AutoPos(uiPos.ChurchButton);
		mapButtons[5].Rect = AutoRect.AutoPos(uiPos.VillageButton2);
		mapButtons[6].Rect = AutoRect.AutoPos(uiPos.RecycleButton);
		mapButtons[7].Rect = AutoRect.AutoPos(uiPos.PowerStationButton);
		mapButtons[0].OriRect = mapButtons[0].Rect;
		mapButtons[1].OriRect = mapButtons[1].Rect;
		mapButtons[2].OriRect = mapButtons[2].Rect;
		mapButtons[3].OriRect = mapButtons[3].Rect;
		mapButtons[4].OriRect = mapButtons[4].Rect;
		mapButtons[5].OriRect = mapButtons[5].Rect;
		mapButtons[6].OriRect = mapButtons[6].Rect;
		mapButtons[7].OriRect = mapButtons[7].Rect;
		Rect rect = new Rect(46f, -30f, 0f, 0f);
		Rect rect2 = new Rect(66f, -30f, 0f, 0f);
		Rect rect3 = new Rect(66f, -30f, 0f, 0f);
		Rect rect4 = new Rect(90f, 15f, 0f, 0f);
		Rect rect5 = new Rect(90f, 15f, 0f, 0f);
		Rect rect6 = new Rect(90f, -50f, 0f, 0f);
		Rect rect7 = new Rect(40f, -20f, 0f, 0f);
		Rect rect8 = new Rect(40f, -20f, 0f, 0f);
		zombieAnimations[0].Rect = AutoRect.AutoPos(Math.AddRect(uiPos.FactoryButton, rect));
		zombieAnimations[1].Rect = AutoRect.AutoPos(Math.AddRect(uiPos.HospitalButton, rect2));
		zombieAnimations[2].Rect = AutoRect.AutoPos(Math.AddRect(uiPos.ParkingButton, rect3));
		zombieAnimations[3].Rect = AutoRect.AutoPos(Math.AddRect(uiPos.VillageButton, rect4));
		zombieAnimations[4].Rect = AutoRect.AutoPos(Math.AddRect(uiPos.ChurchButton, rect5));
		zombieAnimations[5].Rect = AutoRect.AutoPos(Math.AddRect(uiPos.VillageButton2, rect6));
		zombieAnimations[6].Rect = AutoRect.AutoPos(Math.AddRect(uiPos.RecycleButton, rect7));
		zombieAnimations[7].Rect = AutoRect.AutoPos(Math.AddRect(uiPos.PowerStationButton, rect8));
		zombieAnimations[0].OriRect = zombieAnimations[0].Rect;
		zombieAnimations[1].OriRect = zombieAnimations[1].Rect;
		zombieAnimations[2].OriRect = zombieAnimations[2].Rect;
		zombieAnimations[3].OriRect = zombieAnimations[3].Rect;
		zombieAnimations[4].OriRect = zombieAnimations[4].Rect;
		zombieAnimations[5].OriRect = zombieAnimations[5].Rect;
		zombieAnimations[6].OriRect = zombieAnimations[6].Rect;
		zombieAnimations[7].OriRect = zombieAnimations[7].Rect;
		Rect rect9 = new Rect(42f, 40f, 0f, 0f);
		Rect rect10 = new Rect(62f, 40f, 0f, 0f);
		Rect rect11 = new Rect(62f, 40f, 0f, 0f);
		Rect rect12 = new Rect(86f, 85f, 0f, 0f);
		Rect rect13 = new Rect(86f, 85f, 0f, 0f);
		Rect rect14 = new Rect(86f, 20f, 0f, 0f);
		Rect rect15 = new Rect(36f, 20f, 0f, 0f);
		Rect rect16 = new Rect(36f, 20f, 0f, 0f);
		preyTipImg[0].Rect = AutoRect.AutoPos(Math.AddRect(uiPos.FactoryButton, rect9));
		preyTipImg[1].Rect = AutoRect.AutoPos(Math.AddRect(uiPos.HospitalButton, rect10));
		preyTipImg[2].Rect = AutoRect.AutoPos(Math.AddRect(uiPos.ParkingButton, rect11));
		preyTipImg[3].Rect = AutoRect.AutoPos(Math.AddRect(uiPos.VillageButton, rect12));
		preyTipImg[4].Rect = AutoRect.AutoPos(Math.AddRect(uiPos.ChurchButton, rect13));
		preyTipImg[5].Rect = AutoRect.AutoPos(Math.AddRect(uiPos.VillageButton2, rect14));
		preyTipImg[6].Rect = AutoRect.AutoPos(Math.AddRect(uiPos.RecycleButton, rect15));
		preyTipImg[7].Rect = AutoRect.AutoPos(Math.AddRect(uiPos.PowerStationButton, rect16));
		preyTipImg[0].OriRect = preyTipImg[0].Rect;
		preyTipImg[1].OriRect = preyTipImg[1].Rect;
		preyTipImg[2].OriRect = preyTipImg[2].Rect;
		preyTipImg[3].OriRect = preyTipImg[3].Rect;
		preyTipImg[4].OriRect = preyTipImg[4].Rect;
		preyTipImg[5].OriRect = preyTipImg[5].Rect;
		preyTipImg[6].OriRect = preyTipImg[6].Rect;
		preyTipImg[7].OriRect = preyTipImg[7].Rect;
		Material material4 = UIResourceMgr.GetInstance().GetMaterial("ShopUI");
		daysPanel = new UITextImage();
		daysPanel.SetTexture(material4, ShopTexturePosition.DayLargePanel, AutoRect.AutoSize(ShopTexturePosition.DayLargePanel));
		daysPanel.Rect = AutoRect.AutoPos(uiPos.DaysPanel);
		daysPanel.SetText("font0", "DAY " + GameApp.GetInstance().GetGameState().LevelNum, ColorName.fontColor_darkred);
		cashPanel = new CashPanel();
		cashPanel.SetCash(GameApp.GetInstance().GetGameState().GetCash());
		cashPanel.Show();
		Material material5 = UIResourceMgr.GetInstance().GetMaterial("ArenaMenu");
		returnButton = new UIClickButton();
		returnButton.SetTexture(UIButtonBase.State.Normal, material5, ArenaMenuTexturePosition.ReturnButtonNormal, AutoRect.AutoSize(ArenaMenuTexturePosition.ReturnButtonNormal));
		returnButton.SetTexture(UIButtonBase.State.Pressed, material5, ArenaMenuTexturePosition.ReturnButtonPressed, AutoRect.AutoSize(ArenaMenuTexturePosition.ReturnButtonPressed));
		returnButton.Rect = AutoRect.AutoPos(uiPos.ReturnButton);
		newShopButton = new UITextButton();
		newShopButton.SetTexture(UIButtonBase.State.Normal, material4, ShopTexturePosition.MapButtonNormal, AutoRect.AutoSize(ShopTexturePosition.MapButtonNormal));
		newShopButton.SetTexture(UIButtonBase.State.Pressed, material4, ShopTexturePosition.MapButtonPressed, AutoRect.AutoSize(ShopTexturePosition.MapButtonPressed));
		newShopButton.Rect = AutoRect.AutoPos(uiPos.NewShopButton);
		newShopButton.SetText("font0", " SHOP", ColorName.fontColor_map);
		optionsButton = new UIClickButton();
		optionsButton.SetTexture(UIButtonBase.State.Normal, material5, ArenaMenuTexturePosition.OptionsButton, AutoRect.AutoSize(ArenaMenuTexturePosition.OptionsButton));
		optionsButton.SetTexture(UIButtonBase.State.Pressed, material5, ArenaMenuTexturePosition.OptionsButtonPressed, AutoRect.AutoSize(ArenaMenuTexturePosition.OptionsButtonPressed));
		optionsButton.Rect = AutoRect.AutoPos(uiPos.OptionsButton);
		paper_model_button = new UIClickButton();
		paper_model_button.SetTexture(UIButtonBase.State.Normal, material3, MapUITexturePosition.PaperModelImg, AutoRect.AutoSize(MapUITexturePosition.PaperModelImg));
		paper_model_button.SetTexture(UIButtonBase.State.Pressed, material3, MapUITexturePosition.PaperModelImg, AutoRect.AutoSize(MapUITexturePosition.PaperModelImg));
		paper_model_button.Rect = AutoRect.AutoPos(uiPos.PaperModelButton);
		endlessButton = new UIClickButton();
		endlessButton.Rect = AutoRect.AutoPos(uiPos.EndlessButton);
		endlessButton.OriRect = endlessButton.Rect;
		background = new UIImage();
		background.SetTexture(material3, MapUITexturePosition.Background3, AutoRect.AutoSize(MapUITexturePosition.Background3));
		background.Rect = AutoRect.AutoPos(uiPos.Background3);
		if (Screen.width == 480)
		{
			background.Rect = new Rect(background.Rect.x - 1f, background.Rect.y, background.Rect.width, background.Rect.height);
		}
		background.OriRect = background.Rect;
		background.Enable = false;
		Add(background);
		Add(daysPanel);
		Add(cashPanel);
		Add(paper_model_button);
		Add(optionsButton);
		Add(returnButton);
		Add(newShopButton);
		Add(endlessButton);
		_Buttons_Enable_Move.Add(endlessButton);
		_offsetRect = new Vector2(0f, 0f);
		OnPanelMove(new Vector2(0f, -200f));
	}

	public void ResetData()
	{
		daysPanel.SetText("font0", "DAY " + GameApp.GetInstance().GetGameState().LevelNum, ColorName.fontColor_darkred);
		cashPanel.SetCash(GameApp.GetInstance().GetGameState().GetCash());
		for (int i = 0; i < 8; i++)
		{
			infection[i] = i;
		}
		if (GameApp.GetInstance().GetGameState().LevelNum == 1)
		{
			for (int j = 1; j < 8; j++)
			{
				infection[j] = -1;
			}
		}
		for (int k = 0; k < 8; k++)
		{
			int num = infection[k];
			if (num != -1)
			{
				mapButtons[k].Enable = true;
				zombieAnimations[k].Visible = true;
			}
			else
			{
				mapButtons[k].Enable = false;
				zombieAnimations[k].Visible = false;
			}
		}
	}

	public override void Hide()
	{
		base.Hide();
	}

	public override void Show()
	{
		base.Show();
		buttonPressed = false;
	}

	public override void Update()
	{
		if (!fadeTimer.Ready())
		{
			return;
		}
		if (fadeTimer.Name == "0")
		{
			GameApp.GetInstance().GetGameState().endless_level = false;
			GameApp.GetInstance().GetGameState().endless_multiplayer = false;
			GameApp.GetInstance().GetGameState().hunting_level = prey_mission[0] != -1;
			if (GameApp.GetInstance().GetGameState().FirstTimeGame)
			{
				SceneName.LoadLevel("Zombie3D_Tutorial");
			}
			else
			{
				SceneName.LoadLevel("Zombie3D_Arena");
			}
		}
		else if (fadeTimer.Name == "1")
		{
			GameApp.GetInstance().GetGameState().endless_level = false;
			GameApp.GetInstance().GetGameState().endless_multiplayer = false;
			GameApp.GetInstance().GetGameState().hunting_level = prey_mission[1] != -1;
			SceneName.LoadLevel("Zombie3D_Hospital");
		}
		else if (fadeTimer.Name == "2")
		{
			GameApp.GetInstance().GetGameState().endless_level = false;
			GameApp.GetInstance().GetGameState().endless_multiplayer = false;
			GameApp.GetInstance().GetGameState().hunting_level = prey_mission[2] != -1;
			SceneName.LoadLevel("Zombie3D_ParkingLot");
		}
		else if (fadeTimer.Name == "3")
		{
			GameApp.GetInstance().GetGameState().endless_level = false;
			GameApp.GetInstance().GetGameState().endless_multiplayer = false;
			GameApp.GetInstance().GetGameState().hunting_level = prey_mission[3] != -1;
			SceneName.LoadLevel("Zombie3D_Village");
		}
		else if (fadeTimer.Name == "4")
		{
			GameApp.GetInstance().GetGameState().endless_level = false;
			GameApp.GetInstance().GetGameState().endless_multiplayer = false;
			GameApp.GetInstance().GetGameState().hunting_level = prey_mission[4] != -1;
			SceneName.LoadLevel("Zombie3D_Church");
		}
		else if (fadeTimer.Name == "5")
		{
			GameApp.GetInstance().GetGameState().endless_level = false;
			GameApp.GetInstance().GetGameState().endless_multiplayer = false;
			GameApp.GetInstance().GetGameState().hunting_level = prey_mission[5] != -1;
			string scene = "Zombie3D_Village2";
			SceneName.LoadLevel(scene);
		}
		else if (fadeTimer.Name == "6")
		{
			GameApp.GetInstance().GetGameState().endless_level = false;
			GameApp.GetInstance().GetGameState().endless_multiplayer = false;
			GameApp.GetInstance().GetGameState().hunting_level = prey_mission[6] != -1;
			SceneName.LoadLevel("Zombie3D_Recycle");
		}
		else if (fadeTimer.Name == "7")
		{
			GameApp.GetInstance().GetGameState().endless_level = false;
			GameApp.GetInstance().GetGameState().hunting_level = prey_mission[6] != -1;
			SceneName.LoadLevel("Zombie3D_PowerStation");
		}
		else if (fadeTimer.Name == "shop")
		{
			SceneName.LoadLevel("ShopMenuTUI");
		}
		else if (fadeTimer.Name == "return")
		{
			SceneName.LoadLevel("StartMenuUI");
		}
		else if (fadeTimer.Name == "endless")
		{
			if (GameApp.GetInstance().GetGameState().multiplay_named == 0)
			{
				SceneName.LoadLevel("NickNameTUI");
			}
			else
			{
				SceneName.LoadLevel("MultiplayRoomTUI");
			}
		}
		else if (fadeTimer.Name == "PaperShowUI")
		{
			GameApp.GetInstance().GetGameState().PaperMenuStatus = PaperUIEnterStatus.MapMenu;
			SceneName.LoadLevel("PaperJoyShowUI");
		}
		fadeTimer.Do();
	}

	public override bool HandleInput(UITouchInner touch)
	{
		if (touch.phase == TouchPhase.Began)
		{
			_offsetRect.x = 0f;
			_offsetRect.y = 0f;
		}
		else if (touch.phase == TouchPhase.Moved)
		{
			_offsetRect.x += Mathf.Abs(touch.deltaPosition.x);
			_offsetRect.y += Mathf.Abs(touch.deltaPosition.y);
		}
		OnPanelMove(touch.deltaPosition);
		if (Mathf.Abs(_offsetRect.x) > AutoRect.AutoValue(40f) || Mathf.Abs(_offsetRect.y) > AutoRect.AutoValue(40f))
		{
			if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
			{
				IEnumerator enumerator = _Buttons_Enable_Move.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						UIClickButton uIClickButton = (UIClickButton)enumerator.Current;
						uIClickButton.Reset();
					}
				}
				finally
				{
					IDisposable disposable;
					if ((disposable = enumerator as IDisposable) != null)
					{
						disposable.Dispose();
					}
				}
			}
			return false;
		}
		if (base.HandleInput(touch))
		{
			return true;
		}
		return false;
	}

	public bool OnPanelMove(Vector2 delta)
	{
		if (delta.x == 0f && delta.y == 0f)
		{
			return false;
		}
		IEnumerator enumerator = _Controls_Enable_Move.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				UIControl uIControl = (UIControl)enumerator.Current;
				_tempRect = Math.AddRect(uIControl.Rect, delta);
				_tempRect.x = Mathf.Clamp(_tempRect.x, uIControl.OriRect.x - MAP_OFFSET_LIMIT.x, uIControl.OriRect.x);
				_tempRect.y = Mathf.Clamp(_tempRect.y, uIControl.OriRect.y - MAP_OFFSET_LIMIT.y, uIControl.OriRect.y);
				uIControl.Rect = _tempRect;
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = enumerator as IDisposable) != null)
			{
				disposable.Dispose();
			}
		}
		IEnumerator enumerator2 = _Buttons_Enable_Move.GetEnumerator();
		try
		{
			while (enumerator2.MoveNext())
			{
				UIControl uIControl2 = (UIControl)enumerator2.Current;
				_tempRect = Math.AddRect(uIControl2.Rect, delta);
				_tempRect.x = Mathf.Clamp(_tempRect.x, uIControl2.OriRect.x - MAP_OFFSET_LIMIT.x, uIControl2.OriRect.x);
				_tempRect.y = Mathf.Clamp(_tempRect.y, uIControl2.OriRect.y - MAP_OFFSET_LIMIT.y, uIControl2.OriRect.y);
				uIControl2.Rect = _tempRect;
			}
		}
		finally
		{
			IDisposable disposable2;
			if ((disposable2 = enumerator2 as IDisposable) != null)
			{
				disposable2.Dispose();
			}
		}
		return true;
	}

	public void HandleEvent(UIControl control, int command, float wparam, float lparam)
	{
		if (!buttonPressed)
		{
			if (control == mapButtons[0])
			{
				MapUI.GetInstance().GetAudioPlayer().PlayAudio("Battle");
				FadeAnimationScript.GetInstance().FadeInBlack();
				fadeTimer.Name = "0";
				fadeTimer.SetTimer(2f, false);
				GameApp.GetInstance().Save();
				buttonPressed = true;
			}
			else if (control == mapButtons[1])
			{
				UnityEngine.Object.Destroy(GameObject.Find("Music"));
				MapUI.GetInstance().GetAudioPlayer().PlayAudio("Battle");
				FadeAnimationScript.GetInstance().FadeInBlack();
				fadeTimer.Name = "1";
				fadeTimer.SetTimer(2f, false);
				GameApp.GetInstance().Save();
				buttonPressed = true;
			}
			else if (control == mapButtons[2])
			{
				UnityEngine.Object.Destroy(GameObject.Find("Music"));
				MapUI.GetInstance().GetAudioPlayer().PlayAudio("Battle");
				FadeAnimationScript.GetInstance().FadeInBlack();
				fadeTimer.Name = "2";
				fadeTimer.SetTimer(2f, false);
				GameApp.GetInstance().Save();
				buttonPressed = true;
			}
			else if (control == mapButtons[3])
			{
				UnityEngine.Object.Destroy(GameObject.Find("Music"));
				MapUI.GetInstance().GetAudioPlayer().PlayAudio("Battle");
				FadeAnimationScript.GetInstance().FadeInBlack();
				fadeTimer.Name = "3";
				fadeTimer.SetTimer(2f, false);
				GameApp.GetInstance().Save();
				buttonPressed = true;
			}
			else if (control == mapButtons[4])
			{
				UnityEngine.Object.Destroy(GameObject.Find("Music"));
				MapUI.GetInstance().GetAudioPlayer().PlayAudio("Battle");
				FadeAnimationScript.GetInstance().FadeInBlack();
				fadeTimer.Name = "4";
				fadeTimer.SetTimer(2f, false);
				GameApp.GetInstance().Save();
				buttonPressed = true;
			}
			else if (control == mapButtons[5])
			{
				UnityEngine.Object.Destroy(GameObject.Find("Music"));
				MapUI.GetInstance().GetAudioPlayer().PlayAudio("Battle");
				FadeAnimationScript.GetInstance().FadeInBlack();
				fadeTimer.Name = "5";
				fadeTimer.SetTimer(2f, false);
				GameApp.GetInstance().Save();
				buttonPressed = true;
			}
			else if (control == mapButtons[6])
			{
				UnityEngine.Object.Destroy(GameObject.Find("Music"));
				MapUI.GetInstance().GetAudioPlayer().PlayAudio("Battle");
				FadeAnimationScript.GetInstance().FadeInBlack();
				fadeTimer.Name = "6";
				fadeTimer.SetTimer(2f, false);
				GameApp.GetInstance().Save();
				buttonPressed = true;
			}
			else if (control == mapButtons[7])
			{
				UnityEngine.Object.Destroy(GameObject.Find("Music"));
				MapUI.GetInstance().GetAudioPlayer().PlayAudio("Battle");
				FadeAnimationScript.GetInstance().FadeInBlack();
				fadeTimer.Name = "7";
				fadeTimer.SetTimer(2f, false);
				GameApp.GetInstance().Save();
				buttonPressed = true;
			}
			else if (control == newShopButton)
			{
				FadeAnimationScript.GetInstance().FadeInBlack(0.5f);
				MapUI.GetInstance().GetAudioPlayer().PlayAudio("Button");
				fadeTimer.Name = "shop";
				fadeTimer.SetTimer(0.5f, false);
				buttonPressed = true;
			}
			else if (control == returnButton)
			{
				FadeAnimationScript.GetInstance().FadeInBlack(1f);
				MapUI.GetInstance().GetAudioPlayer().PlayAudio("Button");
				fadeTimer.Name = "return";
				fadeTimer.SetTimer(1f, false);
				buttonPressed = true;
			}
			else if (control == optionsButton)
			{
				Hide();
				MapUI.GetInstance().GetAudioPlayer().PlayAudio("Button");
				MapUI.GetInstance().GetOptionsMenuUI().Show();
			}
			else if (control == endlessButton)
			{
				FadeAnimationScript.GetInstance().FadeInBlack(0.5f);
				MapUI.GetInstance().GetAudioPlayer().PlayAudio("Button");
				fadeTimer.Name = "endless";
				fadeTimer.SetTimer(0.5f, false);
				buttonPressed = true;
			}
			else if (control == paper_model_button)
			{
				FadeAnimationScript.GetInstance().FadeInBlack(0.5f);
				MapUI.GetInstance().GetAudioPlayer().PlayAudio("Button");
				fadeTimer.Name = "PaperShowUI";
				fadeTimer.SetTimer(0.5f, false);
				buttonPressed = true;
			}
		}
	}
}
