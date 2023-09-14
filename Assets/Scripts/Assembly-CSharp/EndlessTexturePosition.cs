using UnityEngine;

internal class EndlessTexturePosition
{
	public static Rect EndMission01 = AutoRect.AutoTex(new Rect(0f, 0f, 594f, 254f));

	public static Rect EndMission02 = AutoRect.AutoTex(new Rect(0f, 254f, 594f, 254f));

	public static Rect EndMission03 = AutoRect.AutoTex(new Rect(0f, 508f, 594f, 254f));

	public static Rect EndMission04 = AutoRect.AutoTex(new Rect(0f, 762f, 594f, 254f));

	public static Rect EndMission05 = AutoRect.AutoTex(new Rect(0f, 0f, 594f, 254f));

	public static Rect EndMission06 = AutoRect.AutoTex(new Rect(0f, 548f, 594f, 254f));

	public static Rect EndMissionBacK = AutoRect.AutoTex(new Rect(0f, 254f, 658f, 294f));

	public static Rect EndMissionMask = AutoRect.AutoTex(new Rect(594f, 0f, 32f, 32f));

	public static Rect GetEndMissionRect(int index)
	{
		Rect result = default(Rect);
		switch (index)
		{
		case 1:
			return EndMission01;
		case 2:
			return EndMission02;
		case 3:
			return EndMission03;
		case 4:
			return EndMission04;
		case 5:
			return EndMission05;
		case 6:
			return EndMission06;
		default:
			return result;
		}
	}
}
