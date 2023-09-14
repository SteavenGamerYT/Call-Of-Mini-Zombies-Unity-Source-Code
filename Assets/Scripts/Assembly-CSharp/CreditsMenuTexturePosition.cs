using UnityEngine;

internal class CreditsMenuTexturePosition
{
	public static Rect Background = AutoRect.AutoTex(new Rect(0f, 0f, 960f, 640f));

	public Rect TitleImage = AutoRect.AutoTex(new Rect(732f, 711f, 267f, 71f));

	public Rect Dialog = AutoRect.AutoTex(new Rect(0f, 0f, 736f, 376f));

	public Rect ReturnButtonNormal = AutoRect.AutoTex(new Rect(0f, 640f, 228f, 83f));

	public Rect ReturnButtonPressed = AutoRect.AutoTex(new Rect(228f, 640f, 228f, 83f));

	public Rect RightButtonNormal = AutoRect.AutoTex(new Rect(888f, 0f, 131f, 80f));

	public Rect RightButtonPressed = AutoRect.AutoTex(new Rect(888f, 80f, 131f, 80f));
}
