using UnityEngine;
using Zombie3D;

public class UnlockPanel : UIPanel
{
	protected UIImage unlockWeaponImage;

	protected UIText unlockWeaponText;

	protected UIImage unlockAvatarImage;

	protected UIText unlockAvatarText;

	public UnlockPanel()
	{
		unlockWeaponText = new UIText();
		unlockAvatarImage = new UIImage();
		unlockAvatarText = new UIText();
		unlockWeaponImage = new UIImage();
		unlockWeaponImage.Rect = new Rect(240f, 220f, 194f, 112f);
		unlockWeaponText.Set("font2", " IS AVAILABLE!", ColorName.fontColor_yellow);
		unlockWeaponText.AlignStyle = UIText.enAlignStyle.center;
		unlockWeaponText.Rect = new Rect(440f, 220f, 294f, 112f);
		unlockWeaponText.Visible = false;
		Add(unlockWeaponImage);
		Add(unlockWeaponText);
		Add(unlockAvatarImage);
		Add(unlockAvatarText);
	}

	public override void Show()
	{
		base.Show();
		unlockWeaponImage.Visible = true;
	}

	public static GameUIScript GetInstance()
	{
		return GameObject.Find("SceneGUI").GetComponent<GameUIScript>();
	}

	public void SetUnlockWeapon(Weapon w)
	{
		if (w != null)
		{
			Material material = UIResourceMgr.GetInstance().GetMaterial("GameUI");
			int weaponIndex = GameApp.GetInstance().GetGameState().GetWeaponIndex(w);
			Rect texture_rect = new Rect(weaponIndex % 5 * 194, weaponIndex / 5 * 112 + 512, 194f, 112f);
			unlockWeaponImage.SetTexture(material, texture_rect);
			unlockWeaponText.Visible = true;
		}
	}
}
