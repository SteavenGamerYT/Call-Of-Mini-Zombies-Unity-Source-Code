using UnityEngine;

public class UpgradeWeaponItem : MonoBehaviour
{
	public TUIMeshSprite weapon_tip;

	public void SetTipFrame(string frame)
	{
		if (weapon_tip != null)
		{
			weapon_tip.frameName = frame;
		}
	}
}
