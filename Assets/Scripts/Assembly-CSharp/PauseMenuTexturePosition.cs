using UnityEngine;

internal class PauseMenuTexturePosition
{
	public Rect Background = AutoRect.AutoTex(new Rect(512f, 0f, 512f, 509f));

	public Rect ButtonNormal = AutoRect.AutoTex(new Rect(0f, 595f, 350f, 101f));

	public Rect ButtonPressed = AutoRect.AutoTex(new Rect(350f, 595f, 350f, 101f));

	public Rect MusicButtonNormal = AutoRect.AutoTex(new Rect(0f, 696f, 292f, 97f));

	public Rect MusicButtonPressed = AutoRect.AutoTex(new Rect(292f, 696f, 292f, 97f));

	public Rect MusicOnLogoButtonNormal = AutoRect.AutoTex(new Rect(0f, 793f, 189f, 97f));

	public Rect MusicOnLogoButtonPressed = AutoRect.AutoTex(new Rect(189f, 793f, 189f, 97f));

	public Rect MusicOffLogoButtonNormal = AutoRect.AutoTex(new Rect(378f, 793f, 189f, 97f));

	public Rect MusicOffLogoButtonPressed = AutoRect.AutoTex(new Rect(567f, 793f, 189f, 97f));
}
