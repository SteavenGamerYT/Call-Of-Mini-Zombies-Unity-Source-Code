using System;
using System.Collections;
using System.Collections.Generic;
using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using UnityEngine;

namespace Zombie3D
{
	public class GameVSScene : GameScene
	{
		public VSGameSceneTUI game_tui;

		public Dictionary<int, VSPlayerReport> SFS_Player_Report = new Dictionary<int, VSPlayerReport>();

		public Dictionary<User, Player> SFS_Player_Arr = new Dictionary<User, Player>();

		private SmartFox smartFox;

		public override void Init(int index)
		{
			if (SmartFoxConnection.IsInitialized)
			{
				smartFox = SmartFoxConnection.Connection;
				smartFox.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
				smartFox.AddEventListener(SFSEvent.USER_ENTER_ROOM, OnUserEnterRoom);
				smartFox.AddEventListener(SFSEvent.USER_EXIT_ROOM, OnUserExitRoom);
				smartFox.AddEventListener(SFSEvent.OBJECT_MESSAGE, OnObjectMessage);
				smartFox.AddEventListener(SFSEvent.ROOM_VARIABLES_UPDATE, OnRoomVarsUpdate);
				smartFox.AddEventListener(SFSEvent.USER_VARIABLES_UPDATE, OnUserVarsUpdate);
				smartFox.AddEventListener(SFSEvent.EXTENSION_RESPONSE, OnExtensionResponse);
				smartFox.AddEventListener(SFSEvent.PUBLIC_MESSAGE, OnPublicMessage);
			}
			else
			{
				Debug.LogError("smartFox init error!");
			}
			GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load("Prefabs/TUI/VSGameTUI")) as GameObject;
			game_tui = gameObject.transform.Find("SceneTUI").GetComponent<VSGameSceneTUI>();
			game_tui.SetMultiScene(this);
			game_tui.mission_total_time = 600f;
			game_tui.mission_cur_time = 600f;
			game_tui.mission_start_time = smartFox.LastJoinedRoom.GetVariable("GameStartTime").GetDoubleValue();
			GameApp.GetInstance().GetGameState().loot_cash = 0;
			is_game_excute = true;
			GameApp.GetInstance().DebugInfo = string.Empty;
			sceneIndex = index;
			sceneName = Application.loadedLevelName.Substring(9);
			CreateSceneData();
			hitBloodObjectPool.Init("HitBlood", GameApp.GetInstance().GetGameResourceConfig().hitBlood, 3, 0.4f);
			camera = GameObject.Find("Main Camera").GetComponent<TPSSimpleCameraScript>();
			if (camera == null)
			{
				camera = GameObject.Find("Main Camera").GetComponent<TopWatchingCameraScript>();
			}
			else if (!camera.enabled)
			{
				camera = GameObject.Find("Main Camera").GetComponent<TopWatchingCameraScript>();
			}
			player = new Player();
			player.Init();
			camera.Init();
			enemyList = new Hashtable();
			playingState = PlayingState.GamePlaying;
			enemyNum = 0;
			killed = 0;
			triggerCount = 0;
			enemyID = 0;
			Color[] array = new Color[8]
			{
				Color.white,
				Color.red,
				Color.blue,
				Color.yellow,
				Color.magenta,
				Color.gray,
				Color.grey,
				Color.cyan
			};
			int num = UnityEngine.Random.Range(0, array.Length);
			RenderSettings.ambientLight = array[num];
			m_woodboxs = GameObject.Find("WoodBoxes");
			scene_points = GameObject.FindGameObjectsWithTag("WayPoint");
			m_player_set = new List<Player>();
			m_multi_player_arr = new List<Player>();
			m_player_set.Add(player);
			SFS_Player_Arr[smartFox.MySelf] = player;
			SFS_Player_Report[smartFox.MySelf.Id] = VSPlayerReport.CreateReport(GameApp.GetInstance().GetGameState().nick_name, true);
			foreach (User user in smartFox.LastJoinedRoom.UserList)
			{
				if (user.Id != smartFox.MySelf.Id)
				{
					OnSFSPlayerBirth(user);
				}
			}
			game_tui.vs_seat_state.RefrashSeatList(SFS_Player_Arr.Count);
			GC.Collect();
		}

		public override void DoLogic(float deltaTime)
		{
			if (!is_game_excute)
			{
				return;
			}
			if (smartFox != null)
			{
				smartFox.ProcessEvents();
			}
			foreach (Player value in SFS_Player_Arr.Values)
			{
				value.DoLogic(deltaTime);
			}
		}

		public void MissionOver()
		{
			is_game_excute = false;
			SaveVsReport();
		}

		public void SaveVsReport()
		{
			GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
			gameObject.AddComponent<VSReprotData>();
			gameObject.name = "VSReprotObj";
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
			gameObject.GetComponent<VSReprotData>().player_reports = SFS_Player_Report;
		}

		public void QuitGameForDisconnect()
		{
			MissionOver();
			LeaveRoom();
			OnDestroy();
			SceneName.LoadLevel("VSReportTUI");
		}

		public User GetSFSUserFromArray(int id)
		{
			foreach (User key in SFS_Player_Arr.Keys)
			{
				if (key.Id == id)
				{
					return key;
				}
			}
			return null;
		}

		private void OnDestroy()
		{
			SmartFoxConnection.UnregisterSFSSceneCallbacks();
			SmartFoxConnection.Disconnect();
			smartFox = null;
		}

		private void OnDebugMessage(BaseEvent evt)
		{
			string text = (string)evt.Params["message"];
			Debug.Log("[SFS DEBUG] " + text);
		}

		private void OnConnectionLost(BaseEvent evt)
		{
			Debug.Log("Connection was lost, Reason: " + (string)evt.Params["reason"]);
			SmartFoxConnection.UnregisterSFSSceneCallbacks();
			game_tui.OnContectingLost();
		}

		private void LeaveRoom()
		{
			smartFox.Send(new LeaveRoomRequest());
			smartFox.Send(new LogoutRequest());
			SmartFoxConnection.UnregisterSFSSceneCallbacks();
		}

		private void OnUserEnterRoom(BaseEvent evt)
		{
			Room room = (Room)evt.Params["room"];
			User user = (User)evt.Params["user"];
			Debug.Log("User: " + user.Name + " has just joined Room: " + room.Name);
			string[] array = user.Name.Split('|');
			game_tui.message_panel.AddSFSRoom(array[0] + " JOINED THE GAME");
		}

		private void OnUserExitRoom(BaseEvent evt)
		{
			Room room = (Room)evt.Params["room"];
			User user = (User)evt.Params["user"];
			Debug.Log("User: " + user.Name + " has just left Room: " + room.Name);
			if (user == smartFox.MySelf)
			{
				OnDestroy();
				SceneName.LoadLevel("VSReportTUI");
				return;
			}
			Player player = null;
			if (SFS_Player_Arr.ContainsKey(user))
			{
				player = SFS_Player_Arr[user];
				SFS_Player_Arr.Remove(user);
				SFS_Player_Report.Remove(user.Id);
			}
			if (player != null)
			{
				GameApp.GetInstance().GetGameScene().GetCamera()
					.player = base.player;
				UnityEngine.Object.Destroy(player.PlayerObject);
			}
			string[] array = user.Name.Split('|');
			game_tui.message_panel.AddSFSRoom(array[0] + " LEFT THE GAME");
			game_tui.vs_seat_state.RefrashSeatList(SFS_Player_Arr.Count);
		}

		private void OnObjectMessage(BaseEvent evt)
		{
			ISFSObject iSFSObject = (SFSObject)evt.Params["message"];
			User user = (User)evt.Params["sender"];
			if (user != smartFox.MySelf && SFS_Player_Arr.ContainsKey(user))
			{
				if (iSFSObject.ContainsKey("damage"))
				{
					ISFSObject sFSObject = iSFSObject.GetSFSObject("damage");
					float @float = sFSObject.GetFloat("damageVal");
					int @int = sFSObject.GetInt("weaponType");
					SFS_Player_Arr[player.sfs_user].OnInjuredWithUser(user, @float, @int);
				}
				else if (iSFSObject.ContainsKey("killed"))
				{
					SFS_Player_Arr[player.sfs_user].PlusVsKillCount();
				}
				else if (iSFSObject.ContainsKey("deaded"))
				{
					SFS_Player_Arr[user].OnDead();
					SFS_Player_Arr[user].SetState(SFS_Player_Arr[user].DEAD_STATE);
					Debug.Log("user dead");
				}
				else if (iSFSObject.ContainsKey("rebirth"))
				{
					SFS_Player_Arr[user].OnVSRebirth();
					NetworkTransform ntransform = NetworkTransform.FromSFSObject(iSFSObject.GetSFSObject("rebirth"));
					SFS_Player_Arr[user].networkTransform.Load(ntransform);
					SFS_Player_Arr[user].UpdateNetworkTrans();
				}
				else if (iSFSObject.ContainsKey("pgmFire"))
				{
					ISFSObject sFSObject2 = iSFSObject.GetSFSObject("pgmFire");
					float float2 = sFSObject2.GetFloat("pgm_x");
					float float3 = sFSObject2.GetFloat("pgm_y");
					float float4 = sFSObject2.GetFloat("pgm_z");
					Multiplayer multiplayer = SFS_Player_Arr[user] as Multiplayer;
					multiplayer.MultiplayerSniperFire(new Vector3(float2, float3, float4));
				}
			}
		}

		private void OnRoomVarsUpdate(BaseEvent evt)
		{
			ArrayList arrayList = (ArrayList)evt.Params["changedVars"];
			Room room = (Room)evt.Params["room"];
			if (arrayList.Contains("GameStarted") && room.GetVariable("GameStarted").GetBoolValue())
			{
			}
		}

		private void OnUserVarsUpdate(BaseEvent evt)
		{
			ArrayList arrayList = (ArrayList)evt.Params["changedVars"];
			User user = (User)evt.Params["user"];
			if (user == smartFox.MySelf && !arrayList.Contains("userStatistic"))
			{
				return;
			}
			if (!SFS_Player_Arr.ContainsKey(user) && user.Id != smartFox.MySelf.Id)
			{
				OnSFSPlayerBirth(user);
				return;
			}
			if (arrayList.Contains("CurWeapon"))
			{
				((Multiplayer)SFS_Player_Arr[user]).ChangeWeaponWithindex(user.GetVariable("CurWeapon").GetIntValue());
			}
			if (arrayList.Contains("PlayerState"))
			{
				SFS_Player_Arr[user].SetStateWithType((PlayerStateType)user.GetVariable("PlayerState").GetIntValue());
			}
			if (arrayList.Contains("userStatistic"))
			{
				OnSFSPlayerStatisticUpdate(user);
			}
		}

		private void OnSFSPlayerBirth(User user)
		{
			if (!SFS_Player_Arr.ContainsKey(user) && user.ContainsVariable("avatarData"))
			{
				Debug.Log("OnSFSPlayerBirth name:" + user.ToString());
				SFSObject sFSObject = user.GetVariable("avatarData").GetSFSObjectValue() as SFSObject;
				Multiplayer multiplayer = new Multiplayer();
				multiplayer.nick_name = sFSObject.GetUtfString("NickName");
				multiplayer.InitAvatar((AvatarType)sFSObject.GetInt("avatarType"), 0u);
				multiplayer.InitWeaponList(sFSObject.GetInt("weapon1"), sFSObject.GetFloat("weaponPara1"), sFSObject.GetInt("weapon2"), sFSObject.GetFloat("weaponPara2"), sFSObject.GetInt("weapon3"), sFSObject.GetFloat("weaponPara3"));
				multiplayer.birth_point_index = (uint)sFSObject.GetInt("birthPoint");
				multiplayer.Init();
				multiplayer.sfs_user = user;
				SFS_Player_Arr[user] = multiplayer;
				((Multiplayer)SFS_Player_Arr[user]).ChangeWeaponWithindex(user.GetVariable("CurWeapon").GetIntValue());
				if (!SFS_Player_Report.ContainsKey(user.Id))
				{
					Debug.Log("CreateReport...");
					SFS_Player_Report[user.Id] = VSPlayerReport.CreateReport(multiplayer.nick_name, false);
				}
				game_tui.vs_seat_state.RefrashSeatList(SFS_Player_Arr.Count);
			}
		}

		private void OnSFSPlayerStatisticUpdate(User user)
		{
			if (!SFS_Player_Report.ContainsKey(user.Id))
			{
				Debug.LogError("OnSFSPlayerStatisticUpdate not contain key:" + user.Id);
				return;
			}
			SFSObject sFSObject = user.GetVariable("userStatistic").GetSFSObjectValue() as SFSObject;
			SFS_Player_Report[user.Id].kill_cout = sFSObject.GetInt("killCount");
			SFS_Player_Report[user.Id].death_count = sFSObject.GetInt("deathCount");
			SFS_Player_Report[user.Id].loot_cash = sFSObject.GetInt("cashLoot");
			SFS_Player_Report[user.Id].combo_kill = sFSObject.GetInt("vsCombo");
			Debug.Log("userStatistic id:" + user.Id + " kill:" + SFS_Player_Report[user.Id].kill_cout + " death:" + SFS_Player_Report[user.Id].death_count + " cash:" + SFS_Player_Report[user.Id].loot_cash + " vsCombo:" + SFS_Player_Report[user.Id].death_count);
			RefrashMasterKiller();
		}

		private void RefrashMasterKiller()
		{
			List<int> list = new List<int>();
			int num = -1;
			foreach (int key in SFS_Player_Report.Keys)
			{
				if (SFS_Player_Report[key].kill_cout > num)
				{
					num = SFS_Player_Report[key].kill_cout;
				}
			}
			foreach (int key2 in SFS_Player_Report.Keys)
			{
				if (SFS_Player_Report[key2].kill_cout >= num)
				{
					list.Add(key2);
				}
			}
			foreach (User key3 in SFS_Player_Arr.Keys)
			{
				SFS_Player_Arr[key3].RefrashMasterKiller(false);
			}
			foreach (int item in list)
			{
				SFS_Player_Arr[GetSFSUserFromArray(item)].RefrashMasterKiller(true);
			}
		}

		private void OnExtensionResponse(BaseEvent evt)
		{
			try
			{
				string text = (string)evt.Params["cmd"];
				ISFSObject iSFSObject = (SFSObject)evt.Params["params"];
				if (text == "Zone.Common.Keepalive")
				{
					HandleServerTime(iSFSObject);
				}
				else if (text == "Room.Common.UdpBroadcast")
				{
					User sFSUserFromArray = GetSFSUserFromArray(iSFSObject.GetInt("fromId"));
					if (sFSUserFromArray != null && sFSUserFromArray.Id != smartFox.MySelf.Id)
					{
						NetworkTransform ntransform = NetworkTransform.FromSFSObject(iSFSObject.GetSFSObject("data"));
						SFS_Player_Arr[sFSUserFromArray].networkTransform.Load(ntransform);
						SFS_Player_Arr[sFSUserFromArray].UpdateNetworkTrans();
					}
				}
			}
			catch (Exception ex)
			{
				Debug.Log("Exception handling response: " + ex.Message + " >>> " + ex.StackTrace);
			}
		}

		private void HandleServerTime(ISFSObject dt)
		{
			long @long = dt.GetLong("serverTime");
			TimeManager.Instance.Synchronize(Convert.ToDouble(@long));
		}

		private void OnPublicMessage(BaseEvent evt)
		{
			Debug.Log("Message: " + (string)evt.Params["message"] + ", Sender: " + (User)evt.Params["sender"]);
			game_tui.message_panel.AddSFSRoom((string)evt.Params["message"]);
		}

		public void SetGameExcute(bool status)
		{
			is_game_excute = status;
		}
	}
}
