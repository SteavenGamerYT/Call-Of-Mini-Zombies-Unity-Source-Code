using System.Collections.Generic;
using UnityEngine;
using Zombie3D;

public class EquipmentUI : UIPanel, UIHandler
{
	protected Rect[] buttonRect;

	public UIManager m_UIManager;

	public string m_ui_material_path;

	protected Material arenaMenuMaterial;

	protected UIImage background;

	protected UIImage titleImage;

	protected UIImage selectionImage;

	protected UIClickButton returnButton;

	protected UIClickButton getMoreMoneyButton;

	protected UIImage[] panels = new UIImage[6];

	protected UIText[] numbers = new UIText[6];

	protected CashPanel cashPanel;

	protected UIText weaponInfoText;

	protected WeaponInfoPanel weaponInfoPanel;

	private EquipmentUIPosition uiPos;

	protected bool uiInited;

	protected GameState gameState;

	protected Weapon selectedWeapon;

	protected int currentSelectionWeaponIndex;

	protected int SELECTION_NUM = 3;

	protected Rect[] selectionRect = new Rect[3];

	protected int[] rectToWeaponMap;

	protected UIDragGrid battleWeaponGrid;

	protected int currentSelect = -1;

	protected List<Weapon> weaponList;

	protected Touch lastTouch;

	protected UIImageScroller weaponScroller;

	protected Avatar3DFrame avatarFrame;

	protected Material weaponUI;

	protected GameObject DropWeapon_Audio_Obj;

	protected AudioSource DropWeapon_Audio;

	public EquipmentUI()
	{
		uiPos = new EquipmentUIPosition();
		arenaMenuMaterial = UIResourceMgr.GetInstance().GetMaterial("ArenaMenu");
		background = new UIImage();
		background.SetTexture(arenaMenuMaterial, ArenaMenuTexturePosition.Background);
		background.Rect = AutoRect.AutoPos(uiPos.Background);
		returnButton = new UIClickButton();
		returnButton.SetTexture(UIButtonBase.State.Normal, arenaMenuMaterial, ArenaMenuTexturePosition.ReturnButtonNormal, AutoRect.AutoSize(ArenaMenuTexturePosition.ReturnButtonNormal));
		returnButton.SetTexture(UIButtonBase.State.Pressed, arenaMenuMaterial, ArenaMenuTexturePosition.ReturnButtonPressed, AutoRect.AutoSize(ArenaMenuTexturePosition.ReturnButtonPressed));
		returnButton.Rect = AutoRect.AutoPos(uiPos.ReturnButton);
		selectionImage = new UIImage();
		Add(background);
		Add(returnButton);
		uiInited = true;
		Init();
		selectionImage.Enable = false;
		Add(selectionImage);
		UpdateWeaponInfo();
		SetUIHandler(this);
		Hide();
	}

	private void Init()
	{
		Cursor.visible = true;
		selectionRect[0] = AutoRect.AutoPos(new Rect(28f, 88f, 112f, 112f));
		selectionRect[1] = AutoRect.AutoPos(new Rect(178f, 88f, 112f, 112f));
		selectionRect[2] = AutoRect.AutoPos(new Rect(328f, 88f, 112f, 112f));
		gameState = GameApp.GetInstance().GetGameState();
		weaponUI = UIResourceMgr.GetInstance().GetMaterial("Weapons");
		Material material = UIResourceMgr.GetInstance().GetMaterial("Weapons3");
		Rect weaponIconTextureRect = WeaponsLogoTexturePosition.GetWeaponIconTextureRect(-1);
		battleWeaponGrid = new UIDragGrid(0);
		rectToWeaponMap = GameApp.GetInstance().GetGameState().GetRectToWeaponMap();
		for (int i = 0; i < SELECTION_NUM; i++)
		{
			battleWeaponGrid.AddGrid(selectionRect[i], material, weaponIconTextureRect);
		}
		GameApp.GetInstance().InitForMenu();
		weaponList = GameApp.GetInstance().GetGameState().GetWeapons();
		PutBattleWeapons();
		weaponScroller = new UIImageScroller(AutoRect.AutoPos(new Rect(400f, 0f, 550f, 640f)), AutoRect.AutoPos(new Rect(440f, 180f, 500f, 369f)), 1, AutoRect.AutoSize(WeaponsLogoTexturePosition.WeaponLogoSize), ScrollerDir.Vertical, true);
		weaponScroller.SetImageSpacing(AutoRect.AutoSize(WeaponsLogoTexturePosition.WeaponLogoSpacing));
		weaponScroller.EnableLongPress();
		for (int j = 0; j < weaponList.Count; j++)
		{
			if (weaponList[j].Exist == WeaponExistState.Owned)
			{
				UIImage uIImage = new UIImage();
				TexturePosInfo weaponTextureRect = WeaponsLogoTexturePosition.GetWeaponTextureRect(j);
				uIImage.SetTexture(weaponTextureRect.m_Material, weaponTextureRect.m_TexRect, AutoRect.AutoSize(weaponTextureRect.m_TexRect));
				weaponScroller.Add(uIImage);
			}
		}
		Add(weaponScroller);
		weaponScroller.EnableScroll();
		Material material2 = UIResourceMgr.GetInstance().GetMaterial("ShopUI");
		weaponScroller.AddOverlay(material2, ShopTexturePosition.LockedLogo);
		Material material3 = UIResourceMgr.GetInstance().GetMaterial("Avatar");
		weaponScroller.SetMaskImage(material3, AvatarTexturePosition.Mask);
		Material material4 = UIResourceMgr.GetInstance().GetMaterial("Avatar");
		weaponScroller.SetCenterFrameTexture(material4, AvatarTexturePosition.Frame);
		weaponScroller.Show();
		battleWeaponGrid.Show();
		Add(battleWeaponGrid);
		getMoreMoneyButton = new UITextButton();
		getMoreMoneyButton.SetTexture(UIButtonBase.State.Normal, arenaMenuMaterial, ArenaMenuTexturePosition.GetMoneyButtonNormal, AutoRect.AutoSize(ArenaMenuTexturePosition.GetMoneyButtonSmallSize));
		getMoreMoneyButton.SetTexture(UIButtonBase.State.Pressed, arenaMenuMaterial, ArenaMenuTexturePosition.GetMoneyButtonPressed, AutoRect.AutoSize(ArenaMenuTexturePosition.GetMoneyButtonSmallSize));
		getMoreMoneyButton.Rect = AutoRect.AutoPos(uiPos.GetMoreMoneyButton);
		Add(getMoreMoneyButton);
		cashPanel = new CashPanel();
		cashPanel.Show();
		Add(cashPanel);
		weaponInfoPanel = new WeaponInfoPanel();
		Add(weaponInfoPanel);
		if (AutoRect.GetPlatform() == Platform.IPad)
		{
			avatarFrame = new Avatar3DFrame(AutoRect.AutoPos(new Rect(0f, 200f, 500f, 600f)), new Vector3(-1.3498182f, -0.6005478f, 4.420711f), new Vector3(1.3f, 1.3f, 1.3f) * 0.85f);
		}
		else
		{
			avatarFrame = new Avatar3DFrame(AutoRect.AutoPos(new Rect(0f, 200f, 500f, 600f)), new Vector3(-1.499798f, -0.6672753f, 4.420711f), new Vector3(1.3f, 1.3f, 1.3f));
		}
		Add(avatarFrame);
		DropWeapon_Audio_Obj = new GameObject();
		DropWeapon_Audio_Obj.transform.position = new Vector3(0f, 1f, -10f);
		DropWeapon_Audio = DropWeapon_Audio_Obj.AddComponent<AudioSource>();
		DropWeapon_Audio.clip = GameApp.GetInstance().GetMenuResourceConfig().EquipmentAudio;
		DropWeapon_Audio.loop = false;
		DropWeapon_Audio.bypassEffects = true;
		DropWeapon_Audio.rolloffMode = AudioRolloffMode.Linear;
		DropWeapon_Audio.playOnAwake = false;
	}

	public override void Update()
	{
		if (!uiInited || m_UIManager == null)
		{
			return;
		}
		UITouchInner[] array = ((!Application.isMobilePlatform) ? WindowsInputMgr.MockTouches() : iPhoneInputMgr.MockTouches());
		UITouchInner[] array2 = array;
		foreach (UITouchInner touch in array2)
		{
			if (m_UIManager.HandleInput(touch))
			{
			}
		}
	}

	public override void UpdateLogic()
	{
		if (avatarFrame != null)
		{
			avatarFrame.UpdateAnimation();
		}
	}

	protected void UpdateWeaponInfo()
	{
		Weapon weapon = weaponList[currentSelectionWeaponIndex];
		string text = "DAMAGE       " + Math.SignificantFigures(weapon.Damage, 4);
		string text2 = "FIRE RATE      " + Math.SignificantFigures(weapon.AttackFrequency, 4) + "s";
		string text3 = "ACCURACY     " + Math.SignificantFigures(weapon.Accuracy, 4) + "%";
		string bulletText = "x" + weapon.BulletCount;
		if (weapon.GetWeaponType() == WeaponType.Saw || weapon.GetWeaponType() == WeaponType.Sword)
		{
			bulletText = string.Empty;
		}
		weaponInfoPanel.UpdateBulletLogo((int)weapon.GetWeaponType());
		weaponInfoPanel.SetText(string.Empty + weapon.Name + "\n" + text + "\n" + text2 + "\n" + text3);
		weaponInfoPanel.SetBulletText(bulletText);
	}

	public void HandleEvent(UIControl control, int command, float wparam, float lparam)
	{
		if (control == returnButton)
		{
			AudioPlayer.PlayAudio(ArenaMenuUI.GetInstance().GetComponent<AudioSource>(), true);
			GameApp.GetInstance().Save();
			Hide();
			GameObject.Find("ArenaMenuUI").GetComponent<ArenaMenuUI>().GetPanel(0)
				.Show();
		}
		else if (control == weaponScroller)
		{
			int index = (int)wparam;
			switch (command)
			{
			case 0:
				currentSelectionWeaponIndex = gameState.GetWeaponByOwnedIndex(index);
				UpdateWeaponInfo();
				break;
			case 1:
				if (gameState != null)
				{
					Material material = ((currentSelectionWeaponIndex == 15) ? UIResourceMgr.GetInstance().GetMaterial("Weapons2") : UIResourceMgr.GetInstance().GetMaterial("Weapons3"));
					Rect weaponIconTextureRect = WeaponsLogoTexturePosition.GetWeaponIconTextureRect(currentSelectionWeaponIndex);
					selectionImage.SetTexture(material, weaponIconTextureRect, AutoRect.AutoSize(weaponIconTextureRect));
					selectionImage.Rect = weaponScroller.GetCenterRect();
				}
				break;
			case 2:
			{
				selectionImage.Rect = new Rect(-1000f, -1000f, 200f, 200f);
				for (int i = 0; i < SELECTION_NUM; i++)
				{
					if (selectionRect[i].Contains(new Vector2(wparam, lparam)))
					{
						SelectWeapon(currentSelectionWeaponIndex, i);
						AudioPlayer.PlayAudio(DropWeapon_Audio, true);
						break;
					}
				}
				break;
			}
			case 3:
				selectionImage.Rect = new Rect(wparam - 262.80002f, lparam - 115.200005f, 525.60004f, 230.40001f);
				break;
			}
		}
		else if (control == battleWeaponGrid)
		{
			switch (command)
			{
			case 4:
			{
				int num4 = (int)wparam;
				if (InBattleWeaponCount() > 1)
				{
					int num5 = rectToWeaponMap[num4];
					if (num5 != -1)
					{
						weaponList[num5].IsSelectedForBattle = false;
						rectToWeaponMap[num4] = -1;
						battleWeaponGrid.HideGridTexture(num4);
						avatarFrame.ChangeAvatar(GameApp.GetInstance().GetGameState().Avatar);
					}
					AudioPlayer.PlayAudio(DropWeapon_Audio, true);
				}
				else
				{
					battleWeaponGrid.SetGridTexturePosition(num4, num4);
				}
				break;
			}
			case 3:
			{
				int num = (int)wparam;
				int num2 = (int)lparam;
				int num3 = rectToWeaponMap[num];
				rectToWeaponMap[num] = rectToWeaponMap[num2];
				rectToWeaponMap[num2] = num3;
				if (rectToWeaponMap[num] != -1)
				{
					Material material2 = ((rectToWeaponMap[num] == 15) ? UIResourceMgr.GetInstance().GetMaterial("Weapons2") : UIResourceMgr.GetInstance().GetMaterial("Weapons3"));
					Rect weaponIconTextureRect2 = WeaponsLogoTexturePosition.GetWeaponIconTextureRect(rectToWeaponMap[num]);
					battleWeaponGrid.SetGridTexture(num, material2, weaponIconTextureRect2);
					battleWeaponGrid.SetGridTexturePosition(num, num);
				}
				else
				{
					battleWeaponGrid.HideGridTexture(num);
				}
				if (rectToWeaponMap[num2] != -1)
				{
					Material material3 = ((rectToWeaponMap[num2] == 15) ? UIResourceMgr.GetInstance().GetMaterial("Weapons2") : UIResourceMgr.GetInstance().GetMaterial("Weapons3"));
					Rect weaponIconTextureRect3 = WeaponsLogoTexturePosition.GetWeaponIconTextureRect(rectToWeaponMap[num2]);
					battleWeaponGrid.SetGridTexture(num2, material3, weaponIconTextureRect3);
					battleWeaponGrid.SetGridTexturePosition(num2, num2);
				}
				else
				{
					battleWeaponGrid.HideGridTexture(num2);
				}
				avatarFrame.ChangeAvatar(GameApp.GetInstance().GetGameState().Avatar);
				AudioPlayer.PlayAudio(DropWeapon_Audio, true);
				break;
			}
			}
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

	private void PutBattleWeapons()
	{
		for (int i = 0; i < SELECTION_NUM; i++)
		{
			int num = rectToWeaponMap[i];
			if (num != -1)
			{
				Material material = ((num == 15) ? UIResourceMgr.GetInstance().GetMaterial("Weapons2") : UIResourceMgr.GetInstance().GetMaterial("Weapons3"));
				Rect weaponIconTextureRect = WeaponsLogoTexturePosition.GetWeaponIconTextureRect(num);
				battleWeaponGrid.SetGridTexture(i, material, weaponIconTextureRect);
			}
		}
	}

	private int InBattleWeaponCount()
	{
		int num = 0;
		for (int i = 0; i < weaponList.Count; i++)
		{
			if (weaponList[i].IsSelectedForBattle)
			{
				num++;
			}
		}
		return num;
	}

	private void SelectWeapon(int weaponID, int selectRectIndex)
	{
		bool flag = false;
		for (int i = 0; i < SELECTION_NUM; i++)
		{
			if (rectToWeaponMap[i] != -1 && rectToWeaponMap[i] == weaponID)
			{
				flag = true;
			}
		}
		if (!flag)
		{
			int num = rectToWeaponMap[selectRectIndex];
			if (num != -1)
			{
				weaponList[num].IsSelectedForBattle = false;
			}
			weaponList[weaponID].IsSelectedForBattle = true;
			Material material = ((weaponID == 15) ? UIResourceMgr.GetInstance().GetMaterial("Weapons2") : UIResourceMgr.GetInstance().GetMaterial("Weapons3"));
			Rect weaponIconTextureRect = WeaponsLogoTexturePosition.GetWeaponIconTextureRect(weaponID);
			battleWeaponGrid.SetGridTexture(selectRectIndex, material, weaponIconTextureRect);
			battleWeaponGrid.SetGridTexturePosition(selectRectIndex, selectRectIndex);
			rectToWeaponMap[selectRectIndex] = weaponID;
			avatarFrame.ChangeAvatar(GameApp.GetInstance().GetGameState().Avatar);
		}
	}

	public override void Show()
	{
		currentSelectionWeaponIndex = 0;
		weaponScroller.Clear();
		weaponScroller.SetImageSpacing(AutoRect.AutoSize(WeaponsLogoTexturePosition.WeaponLogoSpacing));
		for (int i = 0; i < weaponList.Count; i++)
		{
			if (weaponList[i].Exist == WeaponExistState.Owned)
			{
				UIImage uIImage = new UIImage();
				TexturePosInfo weaponTextureRect = WeaponsLogoTexturePosition.GetWeaponTextureRect(i);
				uIImage.SetTexture(weaponTextureRect.m_Material, weaponTextureRect.m_TexRect);
				weaponScroller.Add(uIImage);
			}
		}
		weaponScroller.EnableScroll();
		Material material = UIResourceMgr.GetInstance().GetMaterial("ShopUI");
		weaponScroller.AddOverlay(material, new Rect(720f, 610f, 160f, 75f));
		Material material2 = UIResourceMgr.GetInstance().GetMaterial("Avatar");
		weaponScroller.SetMaskImage(material2, AvatarTexturePosition.Mask);
		weaponScroller.Show();
		base.Show();
		avatarFrame.ChangeAvatar(GameApp.GetInstance().GetGameState().Avatar);
		avatarFrame.Show();
		cashPanel.SetCash(gameState.GetCash());
		UpdateWeaponInfo();
		weaponInfoPanel.Show();
	}

	public override void Hide()
	{
		avatarFrame.Hide();
		weaponInfoPanel.Hide();
		base.Hide();
	}
}
