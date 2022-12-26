using UnityEngine;
using Zombie3D;

public class WeaponInfoPanel : UIPanel
{
	protected UIImage background;

	protected UIText infoText;

	protected UIText bulletText;

	protected UIImage bulletLogo;

	public WeaponInfoPanel()
	{
		EquipmentUIPosition equipmentUIPosition = new EquipmentUIPosition();
		Material material = UIResourceMgr.GetInstance().GetMaterial("Dialog");
		background = new UIImage();
		background.SetTexture(material, DialogTexturePosition.TextBox, AutoRect.AutoSize(DialogTexturePosition.TextBox));
		background.Rect = AutoRect.AutoPos(equipmentUIPosition.WeaponInfoPanel);
		background.Enable = false;
		Add(background);
		Material material2 = UIResourceMgr.GetInstance().GetMaterial("Buttons");
		bulletLogo = new UIImage();
		Rect bulletsLogoRect = ButtonsTexturePosition.GetBulletsLogoRect(1);
		bulletLogo.SetTexture(material2, bulletsLogoRect, AutoRect.AutoSize(bulletsLogoRect));
		bulletLogo.Rect = AutoRect.AutoPos(new Rect(equipmentUIPosition.WeaponInfoPanel.x + 250f, equipmentUIPosition.WeaponInfoPanel.y + 48f, 44f, 52f));
		bulletLogo.Visible = false;
		bulletLogo.Enable = false;
		Add(bulletLogo);
		infoText = new UIText();
		infoText.Set("font3", string.Empty, ColorName.fontColor_darkorange);
		infoText.Rect = AutoRect.AutoPos(new Rect(equipmentUIPosition.WeaponInfoPanel.x + 50f, equipmentUIPosition.WeaponInfoPanel.y, equipmentUIPosition.WeaponInfoPanel.width, equipmentUIPosition.WeaponInfoPanel.height - 40f));
		Add(infoText);
		bulletText = new UIText();
		bulletText.Set("font3", string.Empty, ColorName.fontColor_darkorange);
		bulletText.AlignStyle = UIText.enAlignStyle.left;
		bulletText.Rect = AutoRect.AutoPos(new Rect(equipmentUIPosition.WeaponInfoPanel.x + 300f, equipmentUIPosition.WeaponInfoPanel.y - 62f, 144f, 152f));
		Add(bulletText);
	}

	public void SetText(string text)
	{
		infoText.SetText(text);
	}

	public void SetBulletText(string text)
	{
		bulletText.SetText(text);
	}

	public void UpdateBulletLogo(int wTypeIndex)
	{
		if (wTypeIndex == 7 || wTypeIndex == 10)
		{
			bulletLogo.Visible = false;
			return;
		}
		bulletLogo.Visible = true;
		Material material = UIResourceMgr.GetInstance().GetMaterial("Buttons");
		Rect bulletsLogoRect = ButtonsTexturePosition.GetBulletsLogoRect(wTypeIndex);
		bulletLogo.SetTexture(material, bulletsLogoRect, AutoRect.AutoSize(bulletsLogoRect));
	}
}
