using UnityEngine;

internal class ButtonsTexturePosition
{
	public static Rect ButtonNormal = AutoRect.AutoTex(new Rect(0f, 0f, 356f, 116f));

	public static Rect ButtonPressed = AutoRect.AutoTex(new Rect(0f, 116f, 356f, 116f));

	public static Rect SoundButtonNormal = AutoRect.AutoTex(new Rect(356f, 0f, 148f, 80f));

	public static Rect SoundButtonPressed = AutoRect.AutoTex(new Rect(356f, 80f, 148f, 80f));

	public static Rect SoundButtonNormal_Small = AutoRect.AutoTex(new Rect(356f, 0f, 118f, 64f));

	public static Rect SoundButtonPressed_Small = AutoRect.AutoTex(new Rect(356f, 80f, 118f, 64f));

	public static Rect Label = AutoRect.AutoTex(new Rect(0f, 232f, 260f, 76f));

	public static Vector2 MiddleSizeButton = AutoRect.AutoTex(new Vector2(356f, 90f));

	public static Vector2 SmallSizeButton = AutoRect.AutoTex(new Vector2(250f, 80f));

	public static Vector2 TinySizeButton = AutoRect.AutoTex(new Vector2(226f, 80f));

	public static Vector2 LargeLabelSize = AutoRect.AutoTex(new Vector2(360f, 76f));

	public static Rect Day = AutoRect.AutoTex(new Rect(264f, 232f, 210f, 108f));

	public static Rect GetBulletsLogoRect(int index)
	{
		switch (index)
		{
		case 7:
		case 10:
			return new Rect(0f, 0f, 0f, 0f);
		case 11:
			return AutoRect.AutoTex(new Rect(400f, 160f, 44f, 52f));
		case 8:
			return AutoRect.AutoTex(new Rect(356f, 160f, 44f, 52f));
		default:
			return AutoRect.AutoTex(new Rect(44 * (index - 1), 308f, 44f, 52f));
		}
	}
}
