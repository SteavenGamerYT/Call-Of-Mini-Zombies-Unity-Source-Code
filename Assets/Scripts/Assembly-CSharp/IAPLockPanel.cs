using UnityEngine;

public class IAPLockPanel : UIPanel
{
	protected UIImage maskImage;

	protected UIBlock block;

	public IAPLockPanel()
	{
		maskImage = new UIImage();
		Material material = UIResourceMgr.GetInstance().GetMaterial("Avatar");
		maskImage.SetTexture(material, AvatarTexturePosition.Mask, AutoRect.AutoSize(ArenaMenuTexturePosition.BackgroundMask));
		maskImage.Rect = AutoRect.AutoPos(new Rect(-20f, -20f, 1200f, 1200f));
		block = new UIBlock();
		block.Rect = AutoRect.AutoPos(new Rect(0f, 0f, 960f, 640f));
		Add(maskImage);
	}

	public void MaskShow(bool status)
	{
		maskImage.Visible = status;
	}

	public void UpdateSpinner()
	{
	}

	public override void Hide()
	{
		base.Hide();
		Utils.HideIndicatorSystem();
	}

	public override void Show()
	{
		base.Show();
		bool flag = false;
		int num = 0;
		if (AutoRect.GetPlatform() == Platform.IPad)
		{
			flag = true;
			num = 0;
		}
		else
		{
			flag = false;
			num = 1;
		}
		Utils.ShowIndicatorSystem(num, flag, 1f, 1f, 1f, 1f);
	}
}
