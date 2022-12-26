using UnityEngine;

internal class GameUITexturePosition
{
	public static Rect LifePacket = AutoRect.AutoTex(new Rect(0f, 0f, 38f, 32f));

	public static Rect HelpAni1 = AutoRect.AutoTex(new Rect(38f, 0f, 88f, 74f));

	public static Rect HelpAni2 = AutoRect.AutoTex(new Rect(126f, 0f, 88f, 74f));

	public static Rect PlayerLogoBackground = AutoRect.AutoTex(new Rect(290f, 0f, 134f, 88f));

	public static Rect HPBackground = AutoRect.AutoTex(new Rect(572f, 50f, 288f, 50f));

	public static Rect HPImage = new Rect(572f, 0f, 288f, 50f);

	public static Rect WeaponLogoBackground = AutoRect.AutoTex(new Rect(424f, 0f, 148f, 88f));

	public static Rect WeaponLogoAssault = AutoRect.AutoTex(new Rect(206f, 347f, 206f, 112f));

	public static Rect WeaponLogoShotgun = AutoRect.AutoTex(new Rect(206f, 347f, 206f, 112f));

	public static Rect WeaponLogoRPG = AutoRect.AutoTex(new Rect(206f, 347f, 206f, 112f));

	public static Rect WeaponSwitchButtonLeft = AutoRect.AutoTex(new Rect(346f, 65f, 70f, 63f));

	public static Rect WeaponSwitchButtonRight = AutoRect.AutoTex(new Rect(346f, 128f, 64f, 63f));

	public static Rect WeaponSwitchButtonLeftPressed = AutoRect.AutoTex(new Rect(416f, 65f, 70f, 63f));

	public static Rect WeaponSwitchButtonRightPressed = AutoRect.AutoTex(new Rect(410f, 128f, 64f, 63f));

	public static Rect PauseButtonNormal = AutoRect.AutoTex(new Rect(808f, 264f, 78f, 50f));

	public static Rect PauseButtonPressed = AutoRect.AutoTex(new Rect(886f, 264f, 78f, 50f));

	public static Rect MoveJoystick = AutoRect.AutoTex(new Rect(0f, 0f, 198f, 198f));

	public static Rect MoveJoystickThumb = AutoRect.AutoTex(new Rect(198f, 0f, 92f, 92f));

	public static Rect ShootJoystick = AutoRect.AutoTex(new Rect(808f, 314f, 198f, 198f));

	public static Rect ShootJoystickThumb = AutoRect.AutoTex(new Rect(860f, 0f, 132f, 124f));

	public static Rect Reticle = AutoRect.AutoTex(new Rect(178f, 300f, 64f, 40f));

	public static Rect Dialog = AutoRect.AutoTex(new Rect(210f, 100f, 530f, 400f));

	public static Rect DialogSize = AutoRect.AutoTex(new Rect(210f, 100f, 588f, 444f));

	public static Rect DayClear = AutoRect.AutoTex(new Rect(464f, 736f, 502f, 120f));

	public static Rect GameOver = AutoRect.AutoTex(new Rect(464f, 856f, 502f, 120f));

	public static Rect Mask = AutoRect.AutoTex(new Rect(964f, 292f, 27f, 22f));

	public static Rect SemiMaskSize = AutoRect.AutoTex(new Rect(480f, 0f, 480f, 640f));

	public static Rect Switch = AutoRect.AutoTex(new Rect(59f, 426f, 34f, 50f));

	public static TexturePosInfo GetAvatarLogoRect(int avatarLogoIndex)
	{
		TexturePosInfo texturePosInfo = new TexturePosInfo();
		switch (avatarLogoIndex)
		{
		case 8:
			texturePosInfo.m_Material = UIResourceMgr.GetInstance().GetMaterial("buttons");
			texturePosInfo.m_TexRect = AutoRect.AutoTex(new Rect(388f, 340f, 116f, 81f));
			break;
		case 9:
			texturePosInfo.m_Material = UIResourceMgr.GetInstance().GetMaterial("GameUI01");
			texturePosInfo.m_TexRect = AutoRect.AutoTex(new Rect(0f, 0f, 116f, 81f));
			break;
		case 10:
			texturePosInfo.m_Material = UIResourceMgr.GetInstance().GetMaterial("GameUI02");
			texturePosInfo.m_TexRect = AutoRect.AutoTex(new Rect(0f, 0f, 116f, 81f));
			break;
		case 11:
			texturePosInfo.m_Material = UIResourceMgr.GetInstance().GetMaterial("GameUI04");
			texturePosInfo.m_TexRect = AutoRect.AutoTex(new Rect(0f, 0f, 116f, 81f));
			break;
		default:
			texturePosInfo.m_Material = UIResourceMgr.GetInstance().GetMaterial("GameUI");
			texturePosInfo.m_TexRect = AutoRect.AutoTex(new Rect(avatarLogoIndex % 4 * 116, 336 + avatarLogoIndex / 4 * 81 + 512, 116f, 81f));
			break;
		}
		return texturePosInfo;
	}

	public static TexturePosInfo GetWeaponLogoRect(int weaponLogoIndex)
	{
		TexturePosInfo texturePosInfo = new TexturePosInfo();
		switch (weaponLogoIndex)
		{
		case 13:
			texturePosInfo.m_Material = UIResourceMgr.GetInstance().GetMaterial("buttons");
			texturePosInfo.m_TexRect = AutoRect.AutoTex(new Rect(194f, 360f, 194f, 112f));
			break;
		case 14:
			texturePosInfo.m_Material = UIResourceMgr.GetInstance().GetMaterial("buttons");
			texturePosInfo.m_TexRect = AutoRect.AutoTex(new Rect(0f, 360f, 194f, 112f));
			break;
		case 15:
			texturePosInfo.m_Material = UIResourceMgr.GetInstance().GetMaterial("GameUI03");
			texturePosInfo.m_TexRect = AutoRect.AutoTex(new Rect(0f, 0f, 194f, 112f));
			break;
		default:
			texturePosInfo.m_Material = UIResourceMgr.GetInstance().GetMaterial("GameUI");
			if (weaponLogoIndex == 12)
			{
				texturePosInfo.m_TexRect = AutoRect.AutoTex(new Rect(808f, 124f, 194f, 112f));
			}
			else
			{
				texturePosInfo.m_TexRect = AutoRect.AutoTex(new Rect(weaponLogoIndex % 5 * 194, weaponLogoIndex / 5 * 112 + 512, 194f, 112f));
			}
			break;
		}
		return texturePosInfo;
	}

	public static Rect GetHPTextureRect(int width)
	{
		return AutoRect.AutoTex(new Rect(HPImage.xMin, HPImage.yMin, width, HPImage.height));
	}

	public static Rect GetNumberRect(int n)
	{
		if (n >= 0)
		{
			int num = n / 3;
			int num2 = n % 3;
			return AutoRect.AutoTex(new Rect(59 * num2, 198 + 76 * num, 59f, 76f));
		}
		return AutoRect.AutoTex(new Rect(0f, 0f, 0f, 0f));
	}
}
