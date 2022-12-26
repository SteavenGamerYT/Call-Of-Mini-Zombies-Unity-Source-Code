using UnityEngine;

internal class FirstEndlessTexturePosition
{
	public static Rect Board = AutoRect.AutoTex(new Rect(0f, 0f, 790f, 360f));

	public static Rect Board_small = AutoRect.AutoTex(new Rect(0f, 0f, 632f, 288f));

	public static Rect Button_normal = AutoRect.AutoTex(new Rect(0f, 360f, 440f, 102f));

	public static Rect Button_high = AutoRect.AutoTex(new Rect(440f, 360f, 440f, 102f));
}
