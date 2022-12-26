using UnityEngine;
using Zombie3D;

public class CashPanel : UIPanel
{
	public UIImage backPanel;

	public UIImage costPanel;

	public UIText cashText;

	public UIText costText;

	public CashPanel()
	{
		Material material = UIResourceMgr.GetInstance().GetMaterial("ArenaMenu");
		WeaponUpgradeUIPosition weaponUpgradeUIPosition = new WeaponUpgradeUIPosition();
		backPanel = new UITextImage();
		backPanel.SetTexture(material, ArenaMenuTexturePosition.CashPanel, AutoRect.AutoSize(ArenaMenuTexturePosition.CashPanel));
		backPanel.Rect = AutoRect.AutoPos(weaponUpgradeUIPosition.CashPanel);
		costPanel = new UITextImage();
		costPanel.SetTexture(material, ArenaMenuTexturePosition.CashPanel, AutoRect.AutoSize(ArenaMenuTexturePosition.CashPanel));
		costPanel.Rect = AutoRect.AutoPos(weaponUpgradeUIPosition.CostPanel);
		cashText = new UIText();
		cashText.Set("font2", string.Empty, ColorName.fontColor_darkorange);
		cashText.AlignStyle = UIText.enAlignStyle.left;
		cashText.Rect = AutoRect.AutoPos(new Rect(weaponUpgradeUIPosition.CashPanel.x + 40f, weaponUpgradeUIPosition.CashPanel.y, weaponUpgradeUIPosition.CashPanel.width * 0.6f, weaponUpgradeUIPosition.CashPanel.height - 10f));
		costText = new UIText();
		costText.Set("font2", string.Empty, ColorName.fontColor_red);
		costText.AlignStyle = UIText.enAlignStyle.center;
		costText.Rect = AutoRect.AutoPos(new Rect(weaponUpgradeUIPosition.CostPanel.x + 40f, weaponUpgradeUIPosition.CostPanel.y, weaponUpgradeUIPosition.CostPanel.width * 0.6f, weaponUpgradeUIPosition.CostPanel.height - 10f));
		costPanel.Visible = false;
		costPanel.Enable = false;
		Add(backPanel);
		Add(costPanel);
		Add(cashText);
		Add(costText);
	}

	public void SetCostPanelPosition(Rect pos)
	{
		WeaponUpgradeUIPosition weaponUpgradeUIPosition = new WeaponUpgradeUIPosition();
		weaponUpgradeUIPosition.CostPanel = pos;
		costPanel.Rect = AutoRect.AutoPos(weaponUpgradeUIPosition.CostPanel);
		costText.Rect = AutoRect.AutoPos(new Rect(weaponUpgradeUIPosition.CostPanel.x + 40f, weaponUpgradeUIPosition.CostPanel.y, weaponUpgradeUIPosition.CostPanel.width * 0.6f, weaponUpgradeUIPosition.CostPanel.height - 10f));
	}

	public void SetCash(int cash)
	{
		cashText.SetText("$" + cash.ToString("N0"));
	}

	public void SetCost(int cost)
	{
		costPanel.Visible = true;
		costText.Visible = true;
		costText.SetText("-$" + cost.ToString("N0"));
	}

	public void DisableCost()
	{
		costPanel.Visible = false;
		costText.Visible = false;
	}
}
