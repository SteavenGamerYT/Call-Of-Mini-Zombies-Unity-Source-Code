using UnityEngine;
using Zombie3D;

public class AvatarInfoPanel : UIPanel
{
	protected UIImage background;

	protected UIText infoText;

	public AvatarInfoPanel()
	{
		AvatarUIPosition avatarUIPosition = new AvatarUIPosition();
		Material material = UIResourceMgr.GetInstance().GetMaterial("Dialog");
		background = new UIImage();
		background.SetTexture(material, DialogTexturePosition.TextBox, AutoRect.AutoSize(DialogTexturePosition.TextBox));
		background.Rect = AutoRect.AutoPos(avatarUIPosition.AvatarInfoPanel);
		background.Enable = false;
		Add(background);
		infoText = new UIText();
		infoText.Set("font3", string.Empty, ColorName.fontColor_darkorange);
		infoText.Rect = AutoRect.AutoPos(new Rect(avatarUIPosition.AvatarInfoPanel.x + 50f, avatarUIPosition.AvatarInfoPanel.y + 10f, avatarUIPosition.AvatarInfoPanel.width - 70f, avatarUIPosition.AvatarInfoPanel.height - 40f));
		Add(infoText);
	}

	public void SetText(string text)
	{
		infoText.SetText(text);
	}
}
