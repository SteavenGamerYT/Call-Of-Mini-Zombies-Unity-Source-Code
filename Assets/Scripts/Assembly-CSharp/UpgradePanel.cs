using UnityEngine;
using Zombie3D;

public class UpgradePanel : UIPanel
{
	protected UISelectButton selectPanelButton;

	protected UIText buttonText;

	protected UIText currentValueText;

	protected UIText nextValueText;

	protected UIImage[] starsBackground = new UIImage[10];

	protected UIImage[] stars = new UIImage[10];

	protected UIImage arrowImage;

	protected Material arenaMaterial;

	protected UIImage bulletLogo;

	public UpgradePanel(Rect rect, int index)
	{
		arenaMaterial = UIResourceMgr.GetInstance().GetMaterial("ArenaMenu");
		selectPanelButton = new UISelectButton();
		selectPanelButton.SetTexture(UIButtonBase.State.Normal, arenaMaterial, ArenaMenuTexturePosition.UpgradeButtonNormal, AutoRect.AutoSize(ArenaMenuTexturePosition.UpgradeButtonNormal));
		selectPanelButton.SetTexture(UIButtonBase.State.Pressed, arenaMaterial, ArenaMenuTexturePosition.UpgradeButtonPressed, AutoRect.AutoSize(ArenaMenuTexturePosition.UpgradeButtonPressed));
		selectPanelButton.SetTexture(UIButtonBase.State.Disabled, arenaMaterial, ArenaMenuTexturePosition.UpgradeButtonNormal, AutoRect.AutoSize(ArenaMenuTexturePosition.UpgradeButtonNormal));
		selectPanelButton.Rect = AutoRect.AutoPos(rect);
		arrowImage = new UIImage();
		arrowImage.SetTexture(arenaMaterial, ArenaMenuTexturePosition.Arrow, AutoRect.AutoSize(ArenaMenuTexturePosition.Arrow));
		arrowImage.Rect = AutoRect.AutoPos(new Rect(rect.x + 276f, rect.y + 62f, 36f, 26f));
		Material material = UIResourceMgr.GetInstance().GetMaterial("Buttons");
		bulletLogo = new UIImage();
		Rect bulletsLogoRect = ButtonsTexturePosition.GetBulletsLogoRect(1);
		bulletLogo.SetTexture(material, bulletsLogoRect, AutoRect.AutoSize(bulletsLogoRect));
		bulletLogo.Rect = AutoRect.AutoPos(new Rect(rect.x + 156f, rect.y + 46f, 44f, 52f));
		bulletLogo.Visible = false;
		bulletLogo.Enable = false;
		buttonText = new UIText();
		buttonText.Set("font2", string.Empty, ColorName.fontColor_orange);
		buttonText.Rect = AutoRect.AutoPos(new Rect(rect.x + 56f, rect.y + 25f, 175f, 68f));
		currentValueText = new UIText();
		currentValueText.Set("font2", string.Empty, ColorName.fontColor_orange);
		currentValueText.Rect = AutoRect.AutoPos(new Rect(688f, 526 - 100 * index, 92f, 32f));
		currentValueText.AlignStyle = UIText.enAlignStyle.right;
		nextValueText = new UIText();
		nextValueText.Set("font2", string.Empty, ColorName.fontColor_orange);
		nextValueText.Rect = AutoRect.AutoPos(new Rect(810f, 526 - 100 * index, 92f, 32f));
		nextValueText.AlignStyle = UIText.enAlignStyle.left;
		for (int i = 0; i < 10; i++)
		{
			starsBackground[i] = new UIImage();
			starsBackground[i].SetTexture(arenaMaterial, ArenaMenuTexturePosition.StarEmpty, AutoRect.AutoSize(ArenaMenuTexturePosition.StarEmpty));
			stars[i] = new UIImage();
			stars[i].SetTexture(arenaMaterial, ArenaMenuTexturePosition.StarFull, AutoRect.AutoSize(ArenaMenuTexturePosition.StarFull));
			int num = 572 + i * 24;
			int num2 = 490 - index * 100;
			starsBackground[i].Rect = AutoRect.AutoPos(new Rect(num, num2, 24f, 22f));
			starsBackground[i].Enable = false;
			stars[i].Rect = AutoRect.AutoPos(new Rect(num, num2, 24f, 22f));
			stars[i].Enable = false;
		}
		Add(selectPanelButton);
		Add(arrowImage);
		Add(bulletLogo);
		Add(buttonText);
		Add(currentValueText);
		Add(nextValueText);
		for (int j = 0; j < 10; j++)
		{
			Add(starsBackground[j]);
		}
		for (int k = 0; k < 10; k++)
		{
			Add(stars[k]);
		}
	}

	public void SetButtonText(string text)
	{
		buttonText.SetText(text);
	}

	public void SetCurrentValue(float number)
	{
	}

	public void SetNextValue(float number)
	{
	}

	public void VisibleAll()
	{
		selectPanelButton.Visible = true;
		buttonText.Visible = true;
		currentValueText.Visible = true;
		nextValueText.Visible = true;
		arrowImage.Visible = true;
	}

	public void HideArrow()
	{
		arrowImage.Visible = false;
	}

	public void EnableAll()
	{
		selectPanelButton.Enable = true;
	}

	public void HideAll()
	{
		selectPanelButton.Visible = false;
		buttonText.Visible = false;
		currentValueText.Visible = false;
		nextValueText.Visible = false;
		arrowImage.Visible = false;
		bulletLogo.Visible = false;
		UpdateStar(0);
		UpdateStarBackground(0);
	}

	public void DisableUpgrade()
	{
		selectPanelButton.Enable = false;
		nextValueText.Visible = false;
		arrowImage.Visible = false;
		UpdateStar(0);
		UpdateStarBackground(0);
	}

	public void DisableNumbers()
	{
		currentValueText.Visible = false;
		nextValueText.Visible = false;
	}

	public void Select(bool bSelect)
	{
		selectPanelButton.Set(bSelect);
		if (bSelect)
		{
			buttonText.SetColor(ColorName.fontColor_yellow);
			currentValueText.SetColor(ColorName.fontColor_yellow);
			nextValueText.SetColor(ColorName.fontColor_yellow);
			arrowImage.SetTexture(arenaMaterial, ArenaMenuTexturePosition.ArrowSelected, AutoRect.AutoSize(ArenaMenuTexturePosition.ArrowSelected));
			for (int i = 0; i < 10; i++)
			{
				starsBackground[i].SetTexture(arenaMaterial, ArenaMenuTexturePosition.StarEmptySelected, AutoRect.AutoSize(ArenaMenuTexturePosition.StarEmptySelected));
				stars[i].SetTexture(arenaMaterial, ArenaMenuTexturePosition.StarFullSelected, AutoRect.AutoSize(ArenaMenuTexturePosition.StarFullSelected));
			}
		}
		else
		{
			buttonText.SetColor(ColorName.fontColor_orange);
			currentValueText.SetColor(ColorName.fontColor_orange);
			nextValueText.SetColor(ColorName.fontColor_orange);
			arrowImage.SetTexture(arenaMaterial, ArenaMenuTexturePosition.Arrow, AutoRect.AutoSize(ArenaMenuTexturePosition.Arrow));
			for (int j = 0; j < 10; j++)
			{
				starsBackground[j].SetTexture(arenaMaterial, ArenaMenuTexturePosition.StarEmpty, AutoRect.AutoSize(ArenaMenuTexturePosition.StarEmpty));
				stars[j].SetTexture(arenaMaterial, ArenaMenuTexturePosition.StarFull, AutoRect.AutoSize(ArenaMenuTexturePosition.StarFull));
			}
		}
	}

	protected void UpdateStar(int level)
	{
		for (int i = 0; i < 10; i++)
		{
			if (i < level)
			{
				stars[i].Visible = true;
			}
			else
			{
				stars[i].Visible = false;
			}
		}
	}

	protected void UpdateStarBackground(int maxlevel)
	{
		for (int i = 0; i < 10; i++)
		{
			if (i < maxlevel)
			{
				starsBackground[i].Visible = true;
			}
			else
			{
				starsBackground[i].Visible = false;
			}
		}
	}

	public void UpdateInfo(string cStr, string nStr, int weaponTypeIndex)
	{
		Debug.Log("-------------weaponTypeIndex = " + weaponTypeIndex);
		Material material = UIResourceMgr.GetInstance().GetMaterial("Buttons");
		Rect bulletsLogoRect = ButtonsTexturePosition.GetBulletsLogoRect(weaponTypeIndex);
		bulletLogo.SetTexture(material, bulletsLogoRect, AutoRect.AutoSize(bulletsLogoRect));
		arrowImage.Visible = false;
		bulletLogo.Visible = true;
		currentValueText.SetText(cStr);
		nextValueText.SetText(nStr);
		UpdateStar(0);
		UpdateStarBackground(0);
	}

	public void UpdateInfo(float cValue, float nValue, int maxLevel, int level, int uType)
	{
		currentValueText.SetText(Math.SignificantFigures(cValue, 4).ToString());
		if (level == maxLevel)
		{
			nextValueText.Visible = false;
			arrowImage.Visible = false;
		}
		else
		{
			arrowImage.Visible = true;
			nextValueText.Visible = true;
			nextValueText.SetText(Math.SignificantFigures(nValue, 4).ToString());
		}
		switch (uType)
		{
		case 1:
			currentValueText.SetText(currentValueText.GetText() + "s");
			nextValueText.SetText(nextValueText.GetText() + "s");
			break;
		case 2:
			currentValueText.SetText(currentValueText.GetText() + "%");
			nextValueText.SetText(nextValueText.GetText() + "%");
			break;
		}
		UpdateStarBackground(maxLevel);
		UpdateStar(level);
	}

	public void UpdateInfo(string cStr, string nStr, int maxLevel, int level)
	{
		currentValueText.SetText(cStr);
		nextValueText.SetText(nStr);
		UpdateStarBackground(maxLevel);
		UpdateStar(level);
	}
}
