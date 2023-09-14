using UnityEngine;
using Zombie3D;

public class NewItemUI : UIPanel, UIHandler
{
	protected Rect[] buttonRect;

	protected Material gameuiMaterial;

	protected UIImage dialogImage;

	protected UIImage newitemLabel;

	protected UIText newitemLabelText;

	protected UIImage unlockWeaponImage;

	protected UITextButton okButton;

	protected UIText newEquipmentText;

	protected UIText returnText;

	protected UIText surviveTimeText;

	protected UIText firstLineText;

	protected UIText scoreText;

	protected UIText cashText;

	private NewItemUIPosition uiPos;

	protected float screenRatioX;

	protected float screenRatioY;

	protected bool uiInited;

	protected GameState gameState;

	protected Weapon selectedWeapon;

	public NewItemUI()
	{
		uiPos = new NewItemUIPosition();
		gameState = GameApp.GetInstance().GetGameState();
		selectedWeapon = gameState.GetWeapons()[0];
		gameuiMaterial = UIResourceMgr.GetInstance().GetMaterial("GameUI");
		Material material = UIResourceMgr.GetInstance().GetMaterial("Buttons");
		Material material2 = UIResourceMgr.GetInstance().GetMaterial("Dialog");
		dialogImage = new UIImage();
		dialogImage.SetTexture(material2, DialogTexturePosition.Dialog, AutoRect.AutoSize(DialogTexturePosition.Dialog));
		dialogImage.Rect = AutoRect.AutoPos(uiPos.DialogImage);
		unlockWeaponImage = new UIImage();
		unlockWeaponImage.Rect = AutoRect.AutoPos(uiPos.WeaponLogo);
		newitemLabel = new UIImage();
		newitemLabel.SetTexture(material, ButtonsTexturePosition.Label, AutoRect.AutoSize(ButtonsTexturePosition.Label));
		newitemLabel.Rect = AutoRect.AutoPos(uiPos.NewItemLabel);
		newitemLabelText = new UIText();
		newitemLabelText.Set("font2", " NEW ITEM", ColorName.fontColor_orange);
		newitemLabelText.Rect = AutoRect.AutoPos(uiPos.NewItemLabelText);
		cashText = new UIText();
		cashText.Set("font1", "Cash", ColorName.fontColor_yellow);
		cashText.Rect = AutoRect.AutoPos(uiPos.CashText);
		okButton = new UITextButton();
		okButton.Rect = AutoRect.AutoPos(uiPos.OKButton);
		okButton.SetText("font0", " OK", ColorName.fontColor_map);
		firstLineText = new UIText();
		firstLineText.Set("font2", string.Empty, ColorName.fontColor_darkorange);
		firstLineText.AlignStyle = UIText.enAlignStyle.center;
		firstLineText.Rect = AutoRect.AutoPos(uiPos.FirstLineText);
		Add(dialogImage);
		Add(newitemLabel);
		Add(newitemLabelText);
		Add(unlockWeaponImage);
		Add(firstLineText);
		Add(okButton);
		SetUIHandler(this);
		uiInited = true;
	}

	public void SetUnlockWeapon(Weapon w)
	{
		if (w != null)
		{
			int weaponIndex = GameApp.GetInstance().GetGameState().GetWeaponIndex(w);
			TexturePosInfo weaponLogoRect = GameUITexturePosition.GetWeaponLogoRect(weaponIndex);
			unlockWeaponImage.SetTexture(weaponLogoRect.m_Material, weaponLogoRect.m_TexRect);
			if (w.Name == "HellFire")
			{
				okButton.Visible = true;
				okButton.Enable = true;
				Time.timeScale = 0f;
			}
			else
			{
				okButton.Visible = false;
				okButton.Enable = false;
			}
		}
	}

	public override void Show()
	{
		SetUnlockWeapon(GameApp.GetInstance().GetGameScene().BonusWeapon);
		firstLineText.Set("font2", GameApp.GetInstance().GetGameScene().BonusWeapon.Name + " IS AVAILABLE FOR PURCHASE!", ColorName.fontColor_darkorange);
		base.Show();
	}

	public void DisplayBattleEndUI()
	{
	}

	public void HandleEvent(UIControl control, int command, float wparam, float lparam)
	{
		if (control == okButton)
		{
			Debug.Log("Hell Fire UI OK********" + Time.time);
			GameApp.GetInstance().Save();
			Hide();
			Time.timeScale = 1f;
		}
	}
}
