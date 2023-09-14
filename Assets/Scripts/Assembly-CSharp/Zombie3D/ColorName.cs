using UnityEngine;

namespace Zombie3D
{
	public class ColorName
	{
		public static Color fontColor_darkred = new Color(1f, 6f / 85f, 0.0627451f, 1f);

		public static Color fontColor_red = new Color(84f / 85f, 0.20392157f, 1f / 85f, 1f);

		public static Color fontColor_yellow = new Color(1f, 73f / 85f, 2f / 15f, 1f);

		public static Color fontColor_orange = new Color(0.7921569f, 0.5294118f, 7f / 85f, 1f);

		public static Color fontColor_darkorange = new Color(0.6039216f, 0.41960785f, 0.101960786f, 1f);

		public static Color fontColor_map = new Color(1f, 54f / 85f, 0f, 1f);

		public static Color32 GetPlayerMarkColor(int index)
		{
			Color32 result = Color.white;
			switch (index)
			{
			case 0:
				result = new Color32(246, 218, 22, byte.MaxValue);
				break;
			case 1:
				result = new Color32(22, 234, 11, byte.MaxValue);
				break;
			case 2:
				result = new Color32(220, 80, 245, byte.MaxValue);
				break;
			case 3:
				result = new Color32(39, 93, 204, byte.MaxValue);
				break;
			}
			if (GameApp.GetInstance().GetGameState().VS_mode)
			{
				result = new Color32(246, 218, 22, byte.MaxValue);
			}
			return result;
		}
	}
}
