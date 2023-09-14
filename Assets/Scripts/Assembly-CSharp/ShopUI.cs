using System.Collections.Generic;
using UnityEngine;
using Zombie3D;

public class ShopUI : UIPanel, UIHandler
{
	protected const int BUTTON_NUM = 6;

	protected Rect[] buttonRect;

	public UIManager m_UIManager;

	public string m_ui_material_path;

	protected Material arenaMaterial;

	protected Material dialogMaterial;

	protected Material shop2Material;

	protected UIClickButton[] itemButton = new UIClickButton[6];

	protected UIText[] itemText = new UIText[6];

	protected UIImage background;

	protected UIImage dialogImage_up;

	protected UIImage dialogImage_down;

	protected UIImage titleImage;

	protected UIClickButton returnButton;

	protected IAPLockPanel iapLockPanel;

	protected Shop shop;

	protected List<IAPItem>[] itemList;

	protected int[] currentScroll = new int[6];

	protected UIPanel fromPanel;

	protected UIText categoryText;

	protected CashPanel cashPanel;

	private ShopUIPosition uiPos;

	protected float screenRatioX;

	protected float screenRatioY;

	protected IAPName iapProcessing = IAPName.None;

	protected int page;

	protected bool m_is_save_photo;

	public ShopUI()
	{
		shop = new Shop();
		shop.CreateIAPShopData();
		itemList = shop.GetIAPList();
		for (int i = 0; i < 3; i++)
		{
			currentScroll[i] = 0;
		}
		uiPos = new ShopUIPosition();
		arenaMaterial = UIResourceMgr.GetInstance().GetMaterial("ArenaMenu");
		dialogMaterial = UIResourceMgr.GetInstance().GetMaterial("ShopUI");
		shop2Material = UIResourceMgr.GetInstance().GetMaterial("ShopUI2");
		background = new UIImage();
		background.SetTexture(arenaMaterial, ArenaMenuTexturePosition.Background, AutoRect.AutoSize(ArenaMenuTexturePosition.Background));
		background.Rect = AutoRect.AutoPos(uiPos.Background);
		dialogImage_up = new UIImage();
		dialogImage_up.SetTexture(shop2Material, ShopTexturePosition.Dialog_Up, AutoRect.AutoSize(ShopTexturePosition.Dialog_Up));
		dialogImage_up.Rect = AutoRect.AutoPos(uiPos.Dialog_Up);
		dialogImage_down = new UIImage();
		dialogImage_down.SetTexture(shop2Material, ShopTexturePosition.Dialog_Down, AutoRect.AutoSize(ShopTexturePosition.Dialog_Down));
		dialogImage_down.Rect = AutoRect.AutoPos(uiPos.Dialog_Down);
		titleImage = new UIImage();
		titleImage.SetTexture(arenaMaterial, ArenaMenuTexturePosition.ShopTitleImage, AutoRect.AutoSize(ArenaMenuTexturePosition.ShopTitleImage));
		titleImage.Rect = AutoRect.AutoPos(uiPos.TitleImage);
		cashPanel = new CashPanel();
		for (int j = 0; j < 6; j++)
		{
			itemButton[j] = new UIClickButton();
			itemButton[j].Rect = AutoRect.AutoPos(new Rect(155 + 223 * (j % 3), 391 - j / 3 * 288, 202f, 170f));
			itemText[j] = new UIText();
			itemText[j].Set("font3", itemList[0][j].Desc, ColorName.fontColor_orange);
			itemText[j].AlignStyle = UIText.enAlignStyle.center;
			itemText[j].Rect = AutoRect.AutoPos(new Rect(155 + 223 * (j % 3), 327 - j / 3 * 288, 202f, 60f));
		}
		returnButton = new UIClickButton();
		returnButton.SetTexture(UIButtonBase.State.Normal, arenaMaterial, ArenaMenuTexturePosition.ReturnButtonNormal, AutoRect.AutoSize(ArenaMenuTexturePosition.ReturnButtonNormal));
		returnButton.SetTexture(UIButtonBase.State.Pressed, arenaMaterial, ArenaMenuTexturePosition.ReturnButtonPressed, AutoRect.AutoSize(ArenaMenuTexturePosition.ReturnButtonPressed));
		returnButton.Rect = AutoRect.AutoPos(uiPos.ReturnButton);
		categoryText = new UIText();
		categoryText.Set("font1", "CASH BAG    WEAPON     AVATAR", ColorName.fontColor_orange);
		categoryText.Rect = uiPos.CategoryText;
		iapLockPanel = new IAPLockPanel();
		Add(background);
		Add(dialogImage_up);
		Add(dialogImage_down);
		for (int k = 0; k < 6; k++)
		{
			Add(itemButton[k]);
			Add(itemText[k]);
		}
		Add(cashPanel);
		Add(returnButton);
		Add(iapLockPanel);
		SetUIHandler(this);
		Hide();
	}

	public void SetFromPanel(UIPanel panel)
	{
		fromPanel = panel;
	}

	public void GetPhotoSaveStatus()
	{
		if (m_is_save_photo)
		{
			int num = Utils.OnCheckPhotoSaveStatus();
			iapLockPanel.UpdateSpinner();
			if (num != 0 && num == 1)
			{
				iapLockPanel.Hide();
				Utils.OnResetPhotoSaveStatus();
				m_is_save_photo = false;
			}
		}
	}

	public void GetPurchaseStatus()
	{
		if (iapProcessing != IAPName.None)
		{
			int num = IAP.purchaseStatus(null);
			iapLockPanel.UpdateSpinner();
			switch (num)
			{
			case 0:
				break;
			case 1:
				Debug.Log("statusCode:" + num);
				GameApp.GetInstance().GetGameState().DeliverIAPItem(iapProcessing);
				cashPanel.SetCash(GameApp.GetInstance().GetGameState().GetCash());
				iapLockPanel.Hide();
				iapProcessing = IAPName.None;
				break;
			default:
				Debug.Log("statusCode:" + num);
				iapLockPanel.Hide();
				iapProcessing = IAPName.None;
				break;
			}
		}
	}

	public override void Show()
	{
		cashPanel.SetCash(GameApp.GetInstance().GetGameState().GetCash());
		cashPanel.Show();
		base.Show();
	}

	public void HandleEvent(UIControl control, int command, float wparam, float lparam)
	{
		for (int i = 0; i < 6; i++)
		{
			if (control == itemButton[i])
			{
				AudioPlayer.PlayAudio(ArenaMenuUI.GetInstance().GetComponent<AudioSource>(), true);
				IAP.NowPurchaseProduct(itemList[0][i].ID, "1");
				iapProcessing = (IAPName)i;
				Debug.Log("IAP ID:" + itemList[0][i].ID);
				iapLockPanel.Show();
			}
		}
		if (control == returnButton)
		{
			AudioPlayer.PlayAudio(ArenaMenuUI.GetInstance().GetComponent<AudioSource>(), true);
			Hide();
			if (fromPanel != null)
			{
				fromPanel.Show();
				return;
			}
			ArenaMenuUI component = GameObject.Find("ArenaMenuUI").GetComponent<ArenaMenuUI>();
			component.GetPanel(0).Show();
		}
	}

	protected void UpdateItemsUI()
	{
		if (page == 0)
		{
			for (int i = 0; i < 3; i++)
			{
				itemButton[i].SetTexture(UIButtonBase.State.Normal, shop2Material, ShopTexturePosition.GetIAPLogoRect(i), AutoRect.AutoSize(ShopTexturePosition.GetIAPLogoRect(i)));
				itemButton[i].SetTexture(UIButtonBase.State.Pressed, shop2Material, ShopTexturePosition.GetIAPLogoRect(i), AutoRect.AutoSize(ShopTexturePosition.GetIAPLogoRect(i)));
				itemText[i].SetText(itemList[0][i].Desc);
			}
			itemButton[2].Visible = true;
			itemButton[2].Enable = true;
			itemText[2].Visible = true;
			itemText[2].Enable = true;
		}
		else if (page == 1)
		{
			for (int j = 0; j < 2; j++)
			{
				itemButton[j].SetTexture(UIButtonBase.State.Normal, shop2Material, ShopTexturePosition.GetIAPLogoRect(j + 3), AutoRect.AutoSize(ShopTexturePosition.GetIAPLogoRect(j + 3)));
				itemButton[j].SetTexture(UIButtonBase.State.Pressed, shop2Material, ShopTexturePosition.GetIAPLogoRect(j + 3), AutoRect.AutoSize(ShopTexturePosition.GetIAPLogoRect(j + 3)));
				itemText[j].SetText(itemList[0][j + 3].Desc);
			}
			itemButton[2].Visible = false;
			itemButton[2].Enable = false;
			itemText[2].Visible = false;
			itemText[2].Enable = false;
		}
	}
}
