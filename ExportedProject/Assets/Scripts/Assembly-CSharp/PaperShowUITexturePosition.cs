using UnityEngine;

internal class PaperShowUITexturePosition
{
	public static Rect MsgBoxT = AutoRect.AutoTex(new Rect(0f, 542f, 572f, 256f));

	public static Rect MsgBox = AutoRect.AutoTex(new Rect(0f, 838f, 424f, 183f));

	public static Rect Background = AutoRect.AutoTex(new Rect(0f, 0f, 960f, 640f));

	public static Rect ButtonNormal = AutoRect.AutoTex(new Rect(0f, 646f, 356f, 112f));

	public static Rect ButtonPressed = AutoRect.AutoTex(new Rect(368f, 646f, 356f, 112f));

	public static Rect PlayNormal = AutoRect.AutoTex(new Rect(0f, 828f, 438f, 196f));

	public static Rect PlayPressed = AutoRect.AutoTex(new Rect(0f, 828f, 438f, 196f));

	public static Rect JoyNormal = AutoRect.AutoTex(new Rect(442f, 818f, 456f, 206f));

	public static Rect JoyPressed = AutoRect.AutoTex(new Rect(442f, 818f, 456f, 206f));

	public static Rect BuyNormal = AutoRect.AutoTex(new Rect(0f, 642f, 352f, 145f));

	public static Rect BuyPressed = AutoRect.AutoTex(new Rect(352f, 642f, 352f, 145f));

	public static Rect UseNormal = AutoRect.AutoTex(new Rect(0f, 878f, 352f, 145f));

	public static Rect UsePressed = AutoRect.AutoTex(new Rect(352f, 878f, 352f, 145f));

	public static Rect BackButtonNormal = AutoRect.AutoTex(new Rect(750f, 660f, 130f, 70f));

	public static Rect BackButtonPressed = AutoRect.AutoTex(new Rect(880f, 660f, 130f, 70f));

	public static Rect AdTitleImg = AutoRect.AutoTex(new Rect(0f, 650f, 256f, 134f));

	public static Rect CloseNormal = AutoRect.AutoTex(new Rect(732f, 736f, 88f, 80f));

	public static Rect ClosePressed = AutoRect.AutoTex(new Rect(824f, 736f, 88f, 80f));

	public static Rect[] JoyButtonNormal = new Rect[4];

	public static Rect[] JoyButtonPressed = new Rect[4];

	public static void SetJoyButtonRect()
	{
		JoyButtonNormal[0] = AutoRect.AutoTex(new Rect(0f, 0f, 345f, 117f));
		JoyButtonPressed[0] = AutoRect.AutoTex(new Rect(360f, 0f, 345f, 117f));
		JoyButtonNormal[1] = AutoRect.AutoTex(new Rect(0f, 132f, 345f, 117f));
		JoyButtonPressed[1] = AutoRect.AutoTex(new Rect(356f, 130f, 345f, 117f));
		JoyButtonNormal[2] = AutoRect.AutoTex(new Rect(0f, 266f, 345f, 117f));
		JoyButtonPressed[2] = AutoRect.AutoTex(new Rect(352f, 262f, 345f, 117f));
		JoyButtonNormal[3] = AutoRect.AutoTex(new Rect(0f, 398f, 345f, 117f));
		JoyButtonPressed[3] = AutoRect.AutoTex(new Rect(360f, 400f, 345f, 117f));
	}
}
