using System.Collections.Generic;
using UnityEngine;
using Zombie3D;

public class WeaponUpgradeUI : UIPanel, UIHandler, UIDialogEventHandler
{
	protected Rect[] buttonRect;

	public AudioSource upgradeSucceed;

	protected Material arenaMaterial;

	protected UIImage background;

	protected CashPanel cashPanel;

	protected UIClickButton returnButton;

	protected UIClickButton getMoreMoneyButton;

	protected UITextButton upgradeButton;

	protected UpgradePanel[] upgradePanels = new UpgradePanel[4];

	private WeaponUpgradeUIPosition uiPos;

	protected float screenRatioX;

	protected float screenRatioY;

	protected bool uiInited;

	protected GameState gameState;

	protected Weapon selectedWeapon;

	protected List<Weapon> weaponList;

	protected List<GameObject> weaponModles;

	protected int currentWeaponIndex;

	protected int upgradeSelection = 1;

	protected UIImageScroller weaponScroller;

	protected IAPDialog iapDialog;

	protected Material weaponUI;

	protected Material weaponUI2;

	public WeaponUpgradeUI()
	{
		uiPos = new WeaponUpgradeUIPosition();
		GameApp.GetInstance().InitForMenu();
		gameState = GameApp.GetInstance().GetGameState();
		currentWeaponIndex = -1;
		selectedWeapon = null;
		weaponList = GameApp.GetInstance().GetGameState().GetWeapons();
		arenaMaterial = UIResourceMgr.GetInstance().GetMaterial("ArenaMenu");
		background = new UIImage();
		background.SetTexture(arenaMaterial, ArenaMenuTexturePosition.Background, AutoRect.AutoSize(ArenaMenuTexturePosition.Background));
		background.Rect = AutoRect.AutoPos(uiPos.Background);
		for (int i = 0; i < 4; i++)
		{
			upgradePanels[i] = new UpgradePanel(new Rect(500f, 465 - i * 100, 424f, 108f), i);
			upgradePanels[i].Show();
		}
		upgradePanels[0].SetButtonText("DAMAGE");
		upgradePanels[1].SetButtonText("FIRE RATE");
		upgradePanels[2].SetButtonText("ACCURACY");
		upgradePanels[3].SetButtonText("AMMO");
		returnButton = new UIClickButton();
		returnButton.SetTexture(UIButtonBase.State.Normal, arenaMaterial, ArenaMenuTexturePosition.ReturnButtonNormal, AutoRect.AutoSize(ArenaMenuTexturePosition.ReturnButtonNormal));
		returnButton.SetTexture(UIButtonBase.State.Pressed, arenaMaterial, ArenaMenuTexturePosition.ReturnButtonPressed, AutoRect.AutoSize(ArenaMenuTexturePosition.ReturnButtonPressed));
		returnButton.Rect = AutoRect.AutoPos(uiPos.ReturnButton);
		Material material = UIResourceMgr.GetInstance().GetMaterial("Buttons");
		upgradeButton = new UITextButton();
		upgradeButton.SetTexture(UIButtonBase.State.Normal, material, ButtonsTexturePosition.ButtonNormal, AutoRect.AutoSize(ButtonsTexturePosition.ButtonNormal));
		upgradeButton.SetTexture(UIButtonBase.State.Pressed, material, ButtonsTexturePosition.ButtonPressed, AutoRect.AutoSize(ButtonsTexturePosition.ButtonPressed));
		upgradeButton.Rect = AutoRect.AutoPos(uiPos.UpgradeButton);
		getMoreMoneyButton = new UITextButton();
		getMoreMoneyButton.SetTexture(UIButtonBase.State.Normal, arenaMaterial, ArenaMenuTexturePosition.GetMoneyButtonNormal, AutoRect.AutoSize(ArenaMenuTexturePosition.GetMoneyButtonSmallSize));
		getMoreMoneyButton.SetTexture(UIButtonBase.State.Pressed, arenaMaterial, ArenaMenuTexturePosition.GetMoneyButtonPressed, AutoRect.AutoSize(ArenaMenuTexturePosition.GetMoneyButtonSmallSize));
		getMoreMoneyButton.Rect = AutoRect.AutoPos(uiPos.GetMoreMoneyButton);
		cashPanel = new CashPanel();
		upgradeButton.SetText("font0", " UPGRADE", ColorName.fontColor_orange);
		UpdateWeaponInfo();
		InitSelection();
		Add(background);
		Add(returnButton);
		Add(getMoreMoneyButton);
		Add(upgradeButton);
		for (int j = 0; j < 4; j++)
		{
			Add(upgradePanels[j]);
		}
		weaponUI = UIResourceMgr.GetInstance().GetMaterial("Weapons");
		weaponUI2 = UIResourceMgr.GetInstance().GetMaterial("Weapons2");
		weaponScroller = new UIImageScroller(AutoRect.AutoPos(new Rect(0f, 0f, 500f, 640f)), AutoRect.AutoPos(new Rect(10f, 120f, 500f, 440f)), 1, AutoRect.AutoSize(WeaponsLogoTexturePosition.WeaponLogoSize), ScrollerDir.Vertical, true);
		weaponScroller.SetImageSpacing(AutoRect.AutoSize(WeaponsLogoTexturePosition.WeaponLogoSpacing));
		Material material2 = UIResourceMgr.GetInstance().GetMaterial("ShopUI");
		Material material3 = UIResourceMgr.GetInstance().GetMaterial("Avatar");
		weaponScroller.SetCenterFrameTexture(material3, AvatarTexturePosition.Frame);
		weaponScroller.AddOverlay(material2, ShopTexturePosition.LockedLogo);
		weaponScroller.AddOverlay(material2, ShopTexturePosition.BuyLogo);
		UIImage uIImage = new UIImage();
		TexturePosInfo weaponTextureRect = WeaponsLogoTexturePosition.GetWeaponTextureRect(weaponList.Count);
		uIImage.SetTexture(weaponTextureRect.m_Material, weaponTextureRect.m_TexRect, AutoRect.AutoSize(weaponTextureRect.m_TexRect));
		uIImage.Rect = weaponTextureRect.m_TexRect;
		weaponScroller.Add(uIImage);
		for (int k = 0; k < weaponList.Count; k++)
		{
			uIImage = new UIImage();
			weaponTextureRect = WeaponsLogoTexturePosition.GetWeaponTextureRect(k);
			uIImage.SetTexture(weaponTextureRect.m_Material, weaponTextureRect.m_TexRect, AutoRect.AutoSize(weaponTextureRect.m_TexRect));
			uIImage.Rect = weaponTextureRect.m_TexRect;
			weaponScroller.Add(uIImage);
		}
		Add(weaponScroller);
		weaponScroller.EnableScroll();
		Add(cashPanel);
		for (int l = 0; l < weaponList.Count; l++)
		{
			if (weaponList[l].Exist == WeaponExistState.Locked)
			{
				weaponScroller.SetOverlay(l + 1, 0);
			}
			else if (weaponList[l].Exist == WeaponExistState.Unlocked)
			{
				weaponScroller.SetOverlay(l + 1, 1);
			}
		}
		Material material4 = UIResourceMgr.GetInstance().GetMaterial("Avatar");
		weaponScroller.SetMaskImage(material4, AvatarTexturePosition.Mask);
		weaponScroller.Show();
		iapDialog = new IAPDialog(UIDialog.DialogMode.YES_OR_NO);
		iapDialog.SetDialogEventHandler(this);
		Add(iapDialog);
		cashPanel.Show();
		SetUIHandler(this);
		uiInited = true;
		Hide();
	}

	public override void Hide()
	{
		base.Hide();
		iapDialog.Hide();
	}

	public override void Show()
	{
		cashPanel.SetCostPanelPosition(new Rect(650f, 110f, 314f, 60f));
		base.Show();
		UpdateWeaponInfo();
	}

	public void SelectPanel(int index)
	{
		for (int i = 0; i < 4; i++)
		{
			upgradePanels[i].Select(false);
		}
		upgradePanels[index].Select(true);
	}

	public void UpdateSelectionButtonsState()
	{
		SelectPanel(upgradeSelection - 1);
	}

	public void InitSelection()
	{
		if (selectedWeapon != null && selectedWeapon.Exist == WeaponExistState.Owned)
		{
			upgradeSelection = 1;
			UpdateSelectionButtonsState();
		}
	}

	public void UpdateWeaponInfo()
	{
		if (selectedWeapon != null)
		{
			for (int i = 0; i < 4; i++)
			{
				upgradePanels[i].VisibleAll();
			}
			upgradePanels[0].SetButtonText("DAMAGE");
			int maxLevel = (int)selectedWeapon.WConf.damageConf.maxLevel;
			int damageLevel = selectedWeapon.DamageLevel;
			upgradePanels[0].UpdateInfo(selectedWeapon.Damage, selectedWeapon.GetNextLevelDamage(), maxLevel, damageLevel, 0);
			maxLevel = (int)selectedWeapon.WConf.attackRateConf.maxLevel;
			damageLevel = selectedWeapon.FrequencyLevel;
			upgradePanels[1].UpdateInfo(selectedWeapon.AttackFrequency, selectedWeapon.GetNextLevelFrequency(), maxLevel, damageLevel, 1);
			maxLevel = (int)selectedWeapon.WConf.accuracyConf.maxLevel;
			damageLevel = selectedWeapon.AccuracyLevel;
			upgradePanels[2].UpdateInfo(selectedWeapon.Accuracy, selectedWeapon.GetNextLevelAccuracy(), maxLevel, damageLevel, 2);
			upgradePanels[3].UpdateInfo("x" + selectedWeapon.BulletCount, "+" + selectedWeapon.WConf.bullet, (int)selectedWeapon.GetWeaponType());
			if (selectedWeapon.Exist == WeaponExistState.Owned)
			{
				for (int j = 0; j < 4; j++)
				{
					upgradePanels[j].EnableAll();
				}
				upgradeButton.SetText(" UPGRADE");
				switch (upgradeSelection)
				{
				case 1:
					maxLevel = (int)selectedWeapon.WConf.damageConf.maxLevel;
					damageLevel = selectedWeapon.DamageLevel;
					cashPanel.SetCost(selectedWeapon.GetDamageUpgradePrice());
					break;
				case 2:
					maxLevel = (int)selectedWeapon.WConf.attackRateConf.maxLevel;
					damageLevel = selectedWeapon.FrequencyLevel;
					cashPanel.SetCost(selectedWeapon.GetFrequencyUpgradePrice());
					break;
				case 3:
					maxLevel = (int)selectedWeapon.WConf.accuracyConf.maxLevel;
					damageLevel = selectedWeapon.AccuracyLevel;
					cashPanel.SetCost(selectedWeapon.GetAccuracyUpgradePrice());
					break;
				case 4:
					cashPanel.SetCost(selectedWeapon.WConf.bulletPrice);
					upgradeButton.SetText(" BUY");
					break;
				}
				if (maxLevel == damageLevel && upgradeSelection != 4)
				{
					cashPanel.DisableCost();
				}
			}
			else
			{
				for (int k = 0; k < 4; k++)
				{
					upgradePanels[k].DisableUpgrade();
					upgradePanels[k].Select(false);
				}
				upgradeButton.SetText(" BUY");
				cashPanel.SetCost(selectedWeapon.WConf.price);
			}
			if (selectedWeapon.GetWeaponType() == WeaponType.Saw || selectedWeapon.GetWeaponType() == WeaponType.Sword)
			{
				upgradePanels[3].HideAll();
				upgradePanels[3].DisableUpgrade();
			}
		}
		else
		{
			for (int l = 1; l < 4; l++)
			{
				upgradePanels[l].HideAll();
			}
			upgradePanels[0].VisibleAll();
			upgradePanels[0].EnableAll();
			upgradePanels[0].SetButtonText("ARMOR");
			upgradePanels[0].HideArrow();
			upgradePanels[0].Select(true);
			GameConfig gameConfig = GameApp.GetInstance().GetGameConfig();
			upgradePanels[0].UpdateInfo(string.Empty, string.Empty, gameConfig.playerConf.maxArmorLevel, gameState.ArmorLevel);
			cashPanel.SetCost(gameState.GetArmorPrice());
			if (gameConfig.playerConf.maxArmorLevel == gameState.ArmorLevel)
			{
				cashPanel.DisableCost();
			}
		}
		cashPanel.SetCash(gameState.GetCash());
	}

	public void HandleEvent(UIControl control, int command, float wparam, float lparam)
	{
		if (control == weaponScroller && command == 0)
		{
			currentWeaponIndex = (int)wparam - 1;
			upgradeSelection = 1;
			if (currentWeaponIndex >= 0)
			{
				selectedWeapon = weaponList[currentWeaponIndex];
				Debug.Log("*************currentWeaponIndex = " + selectedWeapon.ToString());
				UpdateWeaponInfo();
				InitSelection();
			}
			else
			{
				selectedWeapon = null;
				UpdateWeaponInfo();
				InitSelection();
			}
		}
		else if (control == upgradePanels[0])
		{
			AudioPlayer.PlayAudio(ArenaMenuUI.GetInstance().GetComponent<AudioSource>(), true);
			upgradeSelection = 1;
			UpdateSelectionButtonsState();
			UpdateWeaponInfo();
		}
		else if (control == upgradePanels[1])
		{
			AudioPlayer.PlayAudio(ArenaMenuUI.GetInstance().GetComponent<AudioSource>(), true);
			upgradeSelection = 2;
			UpdateSelectionButtonsState();
			UpdateWeaponInfo();
		}
		else if (control == upgradePanels[2])
		{
			AudioPlayer.PlayAudio(ArenaMenuUI.GetInstance().GetComponent<AudioSource>(), true);
			upgradeSelection = 3;
			UpdateSelectionButtonsState();
			UpdateWeaponInfo();
		}
		else if (control == upgradePanels[3])
		{
			AudioPlayer.PlayAudio(ArenaMenuUI.GetInstance().GetComponent<AudioSource>(), true);
			upgradeSelection = 4;
			UpdateSelectionButtonsState();
			UpdateWeaponInfo();
		}
		else if (control == upgradeButton)
		{
			if (selectedWeapon != null)
			{
				if (selectedWeapon.Exist == WeaponExistState.Owned)
				{
					switch (upgradeSelection)
					{
					case 1:
						if (!selectedWeapon.IsMaxLevelDamage())
						{
							if (gameState.UpgradeWeapon(selectedWeapon, selectedWeapon.Damage * selectedWeapon.WConf.damageConf.upFactor, 0f, 0f, selectedWeapon.GetDamageUpgradePrice()))
							{
								Debug.Log("upgrade");
								ArenaMenuUI.GetInstance().GetAudioPlayer().PlayAudio("Upgrade");
							}
							else
							{
								iapDialog.SetText("\n\n\n  SHORT ON MONEY!");
								iapDialog.Show();
							}
						}
						break;
					case 2:
						if (!selectedWeapon.IsMaxLevelCD())
						{
							if (gameState.UpgradeWeapon(selectedWeapon, 0f, selectedWeapon.AttackFrequency * selectedWeapon.WConf.attackRateConf.upFactor, 0f, selectedWeapon.GetFrequencyUpgradePrice()))
							{
								ArenaMenuUI.GetInstance().GetAudioPlayer().PlayAudio("Upgrade");
								break;
							}
							iapDialog.SetText("\n\n\n  SHORT ON MONEY!");
							iapDialog.Show();
						}
						break;
					case 3:
						if (!selectedWeapon.IsMaxLevelAccuracy())
						{
							if (gameState.UpgradeWeapon(selectedWeapon, 0f, 0f, selectedWeapon.Accuracy * selectedWeapon.WConf.accuracyConf.upFactor, selectedWeapon.GetAccuracyUpgradePrice()))
							{
								ArenaMenuUI.GetInstance().GetAudioPlayer().PlayAudio("Upgrade");
								break;
							}
							iapDialog.SetText("\n\n\n  SHORT ON MONEY!");
							iapDialog.Show();
						}
						break;
					case 4:
						if (selectedWeapon.BulletCount < 9999)
						{
							if (gameState.BuyBullets(selectedWeapon, selectedWeapon.WConf.bullet, selectedWeapon.WConf.bulletPrice))
							{
								ArenaMenuUI.GetInstance().GetAudioPlayer().PlayAudio("Upgrade");
								break;
							}
							iapDialog.SetText("\n\n\n  SHORT ON MONEY!");
							iapDialog.Show();
						}
						break;
					}
				}
				else
				{
					switch (gameState.BuyWeapon(selectedWeapon, selectedWeapon.WConf.price))
					{
					case WeaponBuyStatus.Succeed:
						ArenaMenuUI.GetInstance().GetAudioPlayer().PlayAudio("Upgrade");
						break;
					case WeaponBuyStatus.NotEnoughCash:
						iapDialog.SetText("\n\n\n  SHORT ON MONEY!");
						iapDialog.Show();
						break;
					case WeaponBuyStatus.Locked:
						iapDialog.SetText("\n\n\nUNAVAILABLE WEAPON!");
						iapDialog.Show();
						break;
					}
					if (selectedWeapon.Exist == WeaponExistState.Owned)
					{
						upgradePanels[0].Select(true);
						int weaponIndex = gameState.GetWeaponIndex(selectedWeapon);
						if (weaponScroller != null)
						{
							weaponScroller.SetOverlay(weaponIndex + 1, -1);
						}
					}
				}
			}
			else
			{
				GameConfig gameConfig = GameApp.GetInstance().GetGameConfig();
				if (gameState.ArmorLevel != gameConfig.playerConf.maxArmorLevel)
				{
					if (gameState.UpgradeArmor(gameState.GetArmorPrice()))
					{
						ArenaMenuUI.GetInstance().GetAudioPlayer().PlayAudio("Upgrade");
					}
					else
					{
						iapDialog.SetText("\n\n\n  SHORT ON MONEY!");
						iapDialog.Show();
					}
				}
			}
			UpdateWeaponInfo();
		}
		else if (control == returnButton)
		{
			AudioPlayer.PlayAudio(ArenaMenuUI.GetInstance().GetComponent<AudioSource>(), true);
			GameApp.GetInstance().Save();
			Hide();
			GameObject.Find("ArenaMenuUI").GetComponent<ArenaMenuUI>().GetPanel(0)
				.Show();
		}
		else if (control == getMoreMoneyButton)
		{
			AudioPlayer.PlayAudio(ArenaMenuUI.GetInstance().GetComponent<AudioSource>(), true);
			Hide();
			ShopUI shopUI = GameObject.Find("ArenaMenuUI").GetComponent<ArenaMenuUI>().GetPanel(4) as ShopUI;
			shopUI.SetFromPanel(this);
			shopUI.Show();
		}
	}

	public void Yes()
	{
		Hide();
		ShopUI shopUI = ArenaMenuUI.GetInstance().GetPanel(4) as ShopUI;
		shopUI.SetFromPanel(this);
		shopUI.Show();
	}

	public void No()
	{
		iapDialog.Hide();
	}
}
