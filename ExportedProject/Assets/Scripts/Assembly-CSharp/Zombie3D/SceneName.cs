using UnityEngine;

namespace Zombie3D
{
	public class SceneName
	{
		public const string START_MENU = "StartMenuUI";

		public const string ARENA_MENU = "ArenaMenuUI";

		public const string ENDLESS_MENU = "EndlessMenuUI";

		public const string MAP = "MapUI";

		public const string SCENE_ARENA = "Zombie3D_Arena";

		public const string SCENE_VILLAGE = "Zombie3D_Village";

		public const string SCENE_PARKING = "Zombie3D_ParkingLot";

		public const string SCENE_HOSPITAL = "Zombie3D_Hospital";

		public const string SCENE_CHURCH = "Zombie3D_Church";

		public const string SCENE_VILLAGE2 = "Zombie3D_Village2";

		public const string SCENE_VILLAGE2_3g = "Zombie3D_Village2_3g";

		public const string SCENE_RECYCLE = "Zombie3D_Recycle";

		public const string SCENE_TUTORIAL = "Zombie3D_Tutorial";

		public const string SCENE_POWERSTATION = "Zombie3D_PowerStation";

		public const string SCENE_MULTIPLAYER = "Zombie3D_Arena_Multiplay";

		public const string ENDLESS_MODE_MENU = "EndlessModeTUI";

		public const string MULTIPLAY_ROOM = "MultiplayRoomTUI";

		public const string NICK_NAME_UI = "NickNameTUI";

		public const string MULTIPLAY_CREATE_UI = "MultiplayCreateMapUI";

		public const string ROOM_OWNER_UI = "RoomOwnerTUI";

		public const string JOY_SHOW_UI = "PaperJoyShowUI";

		public const string JOY_USE_UI = "PaperJoyUseUI";

		public const string JOY_AD_UI = "PaperJoyAdUI";

		public const string MULTI_REPORT_TUI = "MultiReportTUI";

		public const string MULTI_ACHIEVEMENT_SHOW_TUI = "MultiAchievementTUI";

		public const string MULTI_TUTORIAL = "Zombie3D_Multi_Toturial";

		public const string SHOP_MENU = "ShopMenuTUI";

		public const string AVATAR_SHOP = "AvatarShopMenuTUI";

		public const string EQUIP_SHOP = "EquipMenuTUI";

		public const string UPGRADE_MENU = "UpgradeTUI";

		public const string MAIN_MAP = "MainMapTUI";

		public const string OPTION_MENU = "OptionTUI";

		public const string NET_MAP = "NetMapTUI";

		public const string COOP_AD = "CoopAdTUI";

		public const string COOP_AD1 = "CoopAd1TUI";

		public const string NET_VS_MAP = "NetMapVSTUI";

		public const string VS_HALL = "VSRoomTUI";

		public const string VS_ROOM = "VSRoomOwnerTUI";

		public const string VS_REPORT_UI = "VSReportTUI";

		public static void LoadLevel(string scene)
		{
			if (GameApp.GetInstance().GetGameState().GetInited())
			{
				GameApp.GetInstance().GetGameState().last_scene = GameApp.GetInstance().GetGameState().user_statistics.last_scene;
				GameApp.GetInstance().GetGameState().user_statistics.scene_enter_data.Add(scene);
				GameApp.GetInstance().GetGameState().user_statistics.last_scene = scene;
				GameApp.GetInstance().GetGameState().SaveSceneStatistics();
			}
			Application.LoadLevel(scene);
		}

		public static void SaveSceneStatistics(string scene)
		{
			if (GameApp.GetInstance().GetGameState().GetInited())
			{
				GameApp.GetInstance().GetGameState().last_scene = GameApp.GetInstance().GetGameState().user_statistics.last_scene;
				GameApp.GetInstance().GetGameState().user_statistics.scene_enter_data.Add(scene);
				GameApp.GetInstance().GetGameState().user_statistics.last_scene = scene;
				GameApp.GetInstance().GetGameState().SaveSceneStatistics();
			}
		}

		public static string GetLastSceneStatistics()
		{
			return GameApp.GetInstance().GetGameState().user_statistics.last_scene;
		}

		public static int GetNetMapIndex(string scene)
		{
			int result = 0;
			switch (scene)
			{
			case "Zombie3D_Arena":
				result = 1;
				break;
			case "Zombie3D_PowerStation":
				result = 2;
				break;
			case "Zombie3D_Village":
				result = 3;
				break;
			case "Zombie3D_ParkingLot":
				result = 4;
				break;
			case "Zombie3D_Hospital":
				result = 5;
				break;
			case "Zombie3D_Church":
				result = 6;
				break;
			case "Zombie3D_Village2":
			case "Zombie3D_Village2_3g":
				result = 7;
				break;
			case "Zombie3D_Recycle":
				result = 8;
				break;
			}
			return result;
		}

		public static string GetNetMapName(int index)
		{
			string result = string.Empty;
			switch (index)
			{
			case 1:
				result = "Zombie3D_Arena";
				break;
			case 2:
				result = "Zombie3D_PowerStation";
				break;
			case 3:
				result = "Zombie3D_Village";
				break;
			case 4:
				result = "Zombie3D_ParkingLot";
				break;
			case 5:
				result = "Zombie3D_Hospital";
				break;
			case 6:
				result = "Zombie3D_Church";
				break;
			case 7:
				result = "Zombie3D_Village2";
				break;
			case 8:
				result = "Zombie3D_Recycle";
				break;
			}
			return result;
		}
	}
}
