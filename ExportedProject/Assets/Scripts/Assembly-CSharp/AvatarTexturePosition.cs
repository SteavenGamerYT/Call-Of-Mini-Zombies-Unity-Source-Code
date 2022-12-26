using UnityEngine;

internal class AvatarTexturePosition
{
	public static Rect[] AvatarLogo = new Rect[12];

	public static Rect Frame = AutoRect.AutoTex(new Rect(0f, 800f, 490f, 222f));

	public static Vector2 AvatarLogoSize = AutoRect.AutoTex(new Vector2(448f, 200f));

	public static Vector2 AvatarLogoSpacing = AutoRect.AutoTex(new Vector2(0f, 82f));

	public static Rect Mask = AutoRect.AutoTex(new Rect(896f, 0f, 27f, 22f));

	public static Rect AvatarEskimo = AutoRect.AutoTex(new Rect(0f, 0f, 448f, 200f));

	public static Rect AvatarNinja = AutoRect.AutoTex(new Rect(0f, 0f, 448f, 200f));

	public static Rect AvatarPostor = AutoRect.AutoTex(new Rect(0f, 200f, 448f, 200f));

	public static void InitLogosTexturePos()
	{
		for (int i = 0; i < AvatarLogo.Length; i++)
		{
			if (i == 8)
			{
				AvatarLogo[i] = AutoRect.AutoTex(new Rect(490f, 800f, 448f, 200f));
				continue;
			}
			int num = i % 2;
			int num2 = i / 2;
			AvatarLogo[i] = AutoRect.AutoTex(new Rect(num * 448, num2 * 200, 448f, 200f));
		}
	}
}
