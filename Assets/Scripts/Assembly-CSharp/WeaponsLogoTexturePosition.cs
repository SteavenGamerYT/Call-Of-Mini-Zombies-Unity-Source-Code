using UnityEngine;

internal class WeaponsLogoTexturePosition
{
	public static Vector2 WeaponLogoSize = AutoRect.AutoTex(new Vector2(438f, 192f));

	public static Vector2 WeaponLogoSpacing = AutoRect.AutoTex(new Vector2(0f, 86f));

	public static TexturePosInfo GetWeaponTextureRect(int index)
	{
		TexturePosInfo texturePosInfo = new TexturePosInfo();
		if (index < 10)
		{
			texturePosInfo.m_Material = UIResourceMgr.GetInstance().GetMaterial("Weapons");
			int num = 438 * (index % 2);
			int num2 = 192 * (index / 2);
			texturePosInfo.m_TexRect = AutoRect.AutoTex(new Rect(num, num2, 438f, 192f));
		}
		else
		{
			texturePosInfo.m_Material = UIResourceMgr.GetInstance().GetMaterial("Weapons2");
			index -= 10;
			int num3 = 438 * (index % 2);
			int num4 = 192 * (index / 2);
			texturePosInfo.m_TexRect = AutoRect.AutoTex(new Rect(num3, num4, 438f, 192f));
		}
		return texturePosInfo;
	}

	public static Rect GetWeaponIconTextureRect(int index)
	{
		int num;
		int num2;
		switch (index)
		{
		case 15:
			num = 876;
			num2 = 0;
			break;
		case -1:
			num = 336;
			num2 = 336;
			break;
		default:
			num = 112 * (index % 4);
			num2 = 112 * (index / 4);
			break;
		}
		return AutoRect.AutoTex(new Rect(num, num2, 112f, 112f));
	}
}
