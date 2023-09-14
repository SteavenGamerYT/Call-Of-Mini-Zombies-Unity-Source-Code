using UnityEngine;

internal class ShopTexturePosition
{
	public static Rect button_shop = AutoRect.AutoTex(new Rect(0f, 0f, 202f, 170f));

	public static Rect Dialog_Up = AutoRect.AutoTex(new Rect(0f, 0f, 743f, 293f));

	public static Rect Dialog_Down = AutoRect.AutoTex(new Rect(0f, 293f, 743f, 293f));

	public static Rect SoldOutLogo = AutoRect.AutoTex(new Rect(720f, 535f, 160f, 82f));

	public static Rect LockedLogo = AutoRect.AutoTex(new Rect(438f, 0f, 438f, 192f));

	public static Rect BuyLogo = AutoRect.AutoTex(new Rect(0f, 0f, 438f, 192f));

	public static Rect SmallBuyLogo = AutoRect.AutoTex(new Rect(0f, 192f, 448f, 200f));

	public static Rect CashLogo = AutoRect.AutoTex(new Rect(0f, 535f, 240f, 240f));

	public static Rect DayLargePanel = AutoRect.AutoTex(new Rect(448f, 192f, 358f, 76f));

	public static Rect MapButtonNormal = AutoRect.AutoTex(new Rect(448f, 268f, 356f, 112f));

	public static Rect MapButtonPressed = AutoRect.AutoTex(new Rect(448f, 380f, 356f, 112f));

	public static Rect ArrowNormal = AutoRect.AutoTex(new Rect(876f, 0f, 76f, 106f));

	public static Rect ArrowPressed = AutoRect.AutoTex(new Rect(876f, 106f, 76f, 106f));

	public static Rect RightArrowNormal = AutoRect.AutoTex(new Rect(882f, 212f, 76f, 106f));

	public static Rect RightArrowPressed = AutoRect.AutoTex(new Rect(806f, 212f, 76f, 106f));

	public static Rect GetIAPLogoRect(int index)
	{
		int num = index % 4;
		int num2 = index / 4;
		return AutoRect.AutoTex(new Rect(num * 252, num2 * 354, 252f, 354f));
	}
}
