using System.Collections;
using System.Collections.Generic;
using Sfs2X.Bitswarm;
using Sfs2X.Core;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using Sfs2X.Entities.Invitation;
using Sfs2X.Entities.Managers;
using Sfs2X.Entities.Variables;
using Sfs2X.Requests;
using Sfs2X.Requests.Game;
using Sfs2X.Util;

namespace Sfs2X.Controllers
{
	public class SystemController : BaseController
	{
		private Dictionary<int, RequestDelegate> requestHandlers;

		public SystemController(BitSwarmClient bitSwarm)
			: base(bitSwarm)
		{
			requestHandlers = new Dictionary<int, RequestDelegate>();
			InitRequestHandlers();
		}

		private void InitRequestHandlers()
		{
			requestHandlers[0] = FnHandshake;
			requestHandlers[1] = FnLogin;
			requestHandlers[2] = FnLogout;
			requestHandlers[4] = FnJoinRoom;
			requestHandlers[6] = FnCreateRoom;
			requestHandlers[7] = FnGenericMessage;
			requestHandlers[8] = FnChangeRoomName;
			requestHandlers[9] = FnChangeRoomPassword;
			requestHandlers[19] = FnChangeRoomCapacity;
			requestHandlers[11] = FnSetRoomVariables;
			requestHandlers[12] = FnSetUserVariables;
			requestHandlers[15] = FnSubscribeRoomGroup;
			requestHandlers[16] = FnUnsubscribeRoomGroup;
			requestHandlers[17] = FnSpectatorToPlayer;
			requestHandlers[18] = FnPlayerToSpectator;
			requestHandlers[200] = FnInitBuddyList;
			requestHandlers[201] = FnAddBuddy;
			requestHandlers[203] = FnRemoveBuddy;
			requestHandlers[202] = FnBlockBuddy;
			requestHandlers[205] = FnGoOnline;
			requestHandlers[204] = FnSetBuddyVariables;
			requestHandlers[27] = FnFindRooms;
			requestHandlers[28] = FnFindUsers;
			requestHandlers[300] = FnInviteUsers;
			requestHandlers[301] = FnInvitationReply;
			requestHandlers[303] = FnQuickJoinGame;
			requestHandlers[29] = FnPingPong;
			requestHandlers[1000] = FnUserEnterRoom;
			requestHandlers[1001] = FnUserCountChange;
			requestHandlers[1002] = FnUserLost;
			requestHandlers[1003] = FnRoomLost;
			requestHandlers[1004] = FnUserExitRoom;
			requestHandlers[1005] = FnClientDisconnection;
		}

		public override void HandleMessage(IMessage message)
		{
			if (sfs.Debug)
			{
				log.Info(string.Concat("Message: ", (RequestType)message.Id, " ", message));
			}
			if (!requestHandlers.ContainsKey(message.Id))
			{
				log.Warn("Unknown message id: " + message.Id);
			}
			else
			{
				RequestDelegate requestDelegate = requestHandlers[message.Id];
				requestDelegate(message);
			}
		}

		private void FnHandshake(IMessage msg)
		{
			Hashtable hashtable = new Hashtable();
			hashtable["message"] = msg.Content;
			SFSEvent evt = new SFSEvent(SFSEvent.HANDSHAKE, hashtable);
			sfs.HandleHandShake(evt);
			sfs.DispatchEvent(evt);
		}

		private void FnLogin(IMessage msg)
		{
			ISFSObject content = msg.Content;
			Hashtable hashtable = new Hashtable();
			if (content.IsNull(BaseRequest.KEY_ERROR_CODE))
			{
				PopulateRoomList(content.GetSFSArray(LoginRequest.KEY_ROOMLIST));
				sfs.MySelf = new SFSUser(content.GetInt(LoginRequest.KEY_ID), content.GetUtfString(LoginRequest.KEY_USER_NAME), true);
				sfs.MySelf.UserManager = sfs.UserManager;
				sfs.MySelf.PrivilegeId = content.GetShort(LoginRequest.KEY_PRIVILEGE_ID);
				sfs.UserManager.AddUser(sfs.MySelf);
				sfs.SetReconnectionSeconds(content.GetShort(LoginRequest.KEY_RECONNECTION_SECONDS));
				sfs.MySelf.PrivilegeId = content.GetShort(LoginRequest.KEY_PRIVILEGE_ID);
				hashtable["zone"] = content.GetUtfString(LoginRequest.KEY_ZONE_NAME);
				hashtable["user"] = sfs.MySelf;
				hashtable["data"] = content.GetSFSObject(LoginRequest.KEY_PARAMS);
				SFSEvent evt = new SFSEvent(SFSEvent.LOGIN, hashtable);
				sfs.HandleLogin(evt);
				sfs.DispatchEvent(evt);
			}
			else
			{
				short @short = content.GetShort(BaseRequest.KEY_ERROR_CODE);
				string errorMessage = SFSErrorCodes.GetErrorMessage(@short, sfs.Log, content.GetUtfStringArray(BaseRequest.KEY_ERROR_PARAMS));
				hashtable["errorMessage"] = errorMessage;
				hashtable["errorCode"] = @short;
				sfs.DispatchEvent(new SFSEvent(SFSEvent.LOGIN_ERROR, hashtable));
			}
		}

		private void FnCreateRoom(IMessage msg)
		{
			ISFSObject content = msg.Content;
			Hashtable hashtable = new Hashtable();
			if (content.IsNull(BaseRequest.KEY_ERROR_CODE))
			{
				IRoomManager roomManager = sfs.RoomManager;
				Room room = SFSRoom.FromSFSArray(content.GetSFSArray(CreateRoomRequest.KEY_ROOM));
				room.RoomManager = sfs.RoomManager;
				roomManager.AddRoom(room);
				hashtable["room"] = room;
				sfs.DispatchEvent(new SFSEvent(SFSEvent.ROOM_ADD, hashtable));
			}
			else
			{
				short @short = content.GetShort(BaseRequest.KEY_ERROR_CODE);
				string errorMessage = SFSErrorCodes.GetErrorMessage(@short, sfs.Log, content.GetUtfStringArray(BaseRequest.KEY_ERROR_PARAMS));
				hashtable["errorMessage"] = errorMessage;
				hashtable["errorCode"] = @short;
				sfs.DispatchEvent(new SFSEvent(SFSEvent.ROOM_CREATION_ERROR, hashtable));
			}
		}

		private void FnJoinRoom(IMessage msg)
		{
			IRoomManager roomManager = sfs.RoomManager;
			ISFSObject content = msg.Content;
			Hashtable hashtable = new Hashtable();
			sfs.IsJoining = false;
			if (content.IsNull(BaseRequest.KEY_ERROR_CODE))
			{
				ISFSArray sFSArray = content.GetSFSArray(JoinRoomRequest.KEY_ROOM);
				ISFSArray sFSArray2 = content.GetSFSArray(JoinRoomRequest.KEY_USER_LIST);
				Room room = SFSRoom.FromSFSArray(sFSArray);
				room.RoomManager = sfs.RoomManager;
				room = roomManager.ReplaceRoom(room, roomManager.ContainsGroup(room.GroupId));
				for (int i = 0; i < sFSArray2.Size(); i++)
				{
					ISFSArray sFSArray3 = sFSArray2.GetSFSArray(i);
					User orCreateUser = GetOrCreateUser(sFSArray3, true, room);
					orCreateUser.SetPlayerId(sFSArray3.GetShort(3), room);
					room.AddUser(orCreateUser);
				}
				room.IsJoined = true;
				sfs.LastJoinedRoom = room;
				hashtable["room"] = room;
				sfs.DispatchEvent(new SFSEvent(SFSEvent.ROOM_JOIN, hashtable));
			}
			else
			{
				short @short = content.GetShort(BaseRequest.KEY_ERROR_CODE);
				string errorMessage = SFSErrorCodes.GetErrorMessage(@short, sfs.Log, content.GetUtfStringArray(BaseRequest.KEY_ERROR_PARAMS));
				hashtable["errorMessage"] = errorMessage;
				hashtable["errorCode"] = @short;
				sfs.DispatchEvent(new SFSEvent(SFSEvent.ROOM_JOIN_ERROR, hashtable));
			}
		}

		private void FnUserEnterRoom(IMessage msg)
		{
			ISFSObject content = msg.Content;
			Hashtable hashtable = new Hashtable();
			Room roomById = sfs.RoomManager.GetRoomById(content.GetInt("r"));
			if (roomById != null)
			{
				ISFSArray sFSArray = content.GetSFSArray("u");
				User orCreateUser = GetOrCreateUser(sFSArray, true, roomById);
				roomById.AddUser(orCreateUser);
				hashtable["user"] = orCreateUser;
				hashtable["room"] = roomById;
				sfs.DispatchEvent(new SFSEvent(SFSEvent.USER_ENTER_ROOM, hashtable));
			}
		}

		private void FnUserCountChange(IMessage msg)
		{
			ISFSObject content = msg.Content;
			Hashtable hashtable = new Hashtable();
			Room roomById = sfs.RoomManager.GetRoomById(content.GetInt("r"));
			if (roomById != null)
			{
				int @short = content.GetShort("uc");
				int num = 0;
				if (content.ContainsKey("sc"))
				{
					num = content.GetShort("sc");
				}
				roomById.UserCount = @short;
				roomById.SpectatorCount = num;
				hashtable["room"] = roomById;
				hashtable["uCount"] = @short;
				hashtable["sCount"] = num;
				sfs.DispatchEvent(new SFSEvent(SFSEvent.USER_COUNT_CHANGE, hashtable));
			}
		}

		private void FnUserLost(IMessage msg)
		{
			ISFSObject content = msg.Content;
			int @int = content.GetInt("u");
			User userById = sfs.UserManager.GetUserById(@int);
			if (userById == null)
			{
				return;
			}
			List<Room> userRooms = sfs.RoomManager.GetUserRooms(userById);
			sfs.RoomManager.RemoveUser(userById);
			sfs.UserManager.RemoveUser(userById);
			foreach (Room item in userRooms)
			{
				Hashtable hashtable = new Hashtable();
				hashtable["user"] = userById;
				hashtable["room"] = item;
				sfs.DispatchEvent(new SFSEvent(SFSEvent.USER_EXIT_ROOM, hashtable));
			}
		}

		private void FnRoomLost(IMessage msg)
		{
			ISFSObject content = msg.Content;
			Hashtable hashtable = new Hashtable();
			int @int = content.GetInt("r");
			Room roomById = sfs.RoomManager.GetRoomById(@int);
			IUserManager userManager = sfs.UserManager;
			if (roomById == null)
			{
				return;
			}
			sfs.RoomManager.RemoveRoom(roomById);
			foreach (User user in roomById.UserList)
			{
				userManager.RemoveUser(user);
			}
			hashtable["room"] = roomById;
			sfs.DispatchEvent(new SFSEvent(SFSEvent.ROOM_REMOVE, hashtable));
		}

		private void FnUserExitRoom(IMessage msg)
		{
			ISFSObject content = msg.Content;
			Hashtable hashtable = new Hashtable();
			int @int = content.GetInt("r");
			int int2 = content.GetInt("u");
			Room roomById = sfs.RoomManager.GetRoomById(@int);
			User userById = sfs.UserManager.GetUserById(int2);
			if (roomById != null && userById != null)
			{
				roomById.RemoveUser(userById);
				sfs.UserManager.RemoveUser(userById);
				if (userById.IsItMe && roomById.IsJoined)
				{
					roomById.IsJoined = false;
					if (sfs.JoinedRooms.Count == 0)
					{
						sfs.LastJoinedRoom = null;
					}
					if (!roomById.IsManaged)
					{
						sfs.RoomManager.RemoveRoom(roomById);
					}
				}
				hashtable["user"] = userById;
				hashtable["room"] = roomById;
				sfs.DispatchEvent(new SFSEvent(SFSEvent.USER_EXIT_ROOM, hashtable));
			}
			else
			{
				log.Debug(string.Concat("Failed to handle UserExit event. Room: ", roomById, ", User: ", userById));
			}
		}

		private void FnClientDisconnection(IMessage msg)
		{
			ISFSObject content = msg.Content;
			int @byte = content.GetByte("dr");
			sfs.HandleClientDisconnection(ClientDisconnectionReason.GetReason(@byte));
		}

		private void FnSetRoomVariables(IMessage msg)
		{
			ISFSObject content = msg.Content;
			Hashtable hashtable = new Hashtable();
			int @int = content.GetInt(SetRoomVariablesRequest.KEY_VAR_ROOM);
			ISFSArray sFSArray = content.GetSFSArray(SetRoomVariablesRequest.KEY_VAR_LIST);
			Room roomById = sfs.RoomManager.GetRoomById(@int);
			ArrayList arrayList = new ArrayList();
			if (roomById != null)
			{
				for (int i = 0; i < sFSArray.Size(); i++)
				{
					RoomVariable roomVariable = SFSRoomVariable.FromSFSArray(sFSArray.GetSFSArray(i));
					roomById.SetVariable(roomVariable);
					arrayList.Add(roomVariable.Name);
				}
				hashtable["changedVars"] = arrayList;
				hashtable["room"] = roomById;
				sfs.DispatchEvent(new SFSEvent(SFSEvent.ROOM_VARIABLES_UPDATE, hashtable));
			}
			else
			{
				log.Warn("RoomVariablesUpdate, unknown Room id = " + @int);
			}
		}

		private void FnSetUserVariables(IMessage msg)
		{
			ISFSObject content = msg.Content;
			Hashtable hashtable = new Hashtable();
			int @int = content.GetInt(SetUserVariablesRequest.KEY_USER);
			ISFSArray sFSArray = content.GetSFSArray(SetUserVariablesRequest.KEY_VAR_LIST);
			User userById = sfs.UserManager.GetUserById(@int);
			ArrayList arrayList = new ArrayList();
			if (userById != null)
			{
				for (int i = 0; i < sFSArray.Size(); i++)
				{
					UserVariable userVariable = SFSUserVariable.FromSFSArray(sFSArray.GetSFSArray(i));
					userById.SetVariable(userVariable);
					arrayList.Add(userVariable.Name);
				}
				hashtable["changedVars"] = arrayList;
				hashtable["user"] = userById;
				sfs.DispatchEvent(new SFSEvent(SFSEvent.USER_VARIABLES_UPDATE, hashtable));
			}
			else
			{
				log.Warn("UserVariablesUpdate: unknown user id = " + @int);
			}
		}

		private void FnSubscribeRoomGroup(IMessage msg)
		{
			ISFSObject content = msg.Content;
			Hashtable hashtable = new Hashtable();
			if (content.IsNull(BaseRequest.KEY_ERROR_CODE))
			{
				string utfString = content.GetUtfString(SubscribeRoomGroupRequest.KEY_GROUP_ID);
				ISFSArray sFSArray = content.GetSFSArray(SubscribeRoomGroupRequest.KEY_ROOM_LIST);
				if (sfs.RoomManager.ContainsGroup(utfString))
				{
					log.Warn("SubscribeGroup Error. Group:" + utfString + "already subscribed!");
				}
				PopulateRoomList(sFSArray);
				hashtable["groupId"] = utfString;
				hashtable["newRooms"] = sfs.RoomManager.GetRoomListFromGroup(utfString);
				sfs.DispatchEvent(new SFSEvent(SFSEvent.ROOM_GROUP_SUBSCRIBE, hashtable));
			}
			else
			{
				short @short = content.GetShort(BaseRequest.KEY_ERROR_CODE);
				string errorMessage = SFSErrorCodes.GetErrorMessage(@short, sfs.Log, content.GetUtfStringArray(BaseRequest.KEY_ERROR_PARAMS));
				hashtable["errorMessage"] = errorMessage;
				hashtable["errorCode"] = @short;
				sfs.DispatchEvent(new SFSEvent(SFSEvent.ROOM_GROUP_SUBSCRIBE_ERROR, hashtable));
			}
		}

		private void FnUnsubscribeRoomGroup(IMessage msg)
		{
			ISFSObject content = msg.Content;
			Hashtable hashtable = new Hashtable();
			if (content.IsNull(BaseRequest.KEY_ERROR_CODE))
			{
				string utfString = content.GetUtfString(UnsubscribeRoomGroupRequest.KEY_GROUP_ID);
				if (!sfs.RoomManager.ContainsGroup(utfString))
				{
					log.Warn("UnsubscribeGroup Error. Group:" + utfString + "is not subscribed!");
				}
				sfs.RoomManager.RemoveGroup(utfString);
				hashtable["groupId"] = utfString;
				sfs.DispatchEvent(new SFSEvent(SFSEvent.ROOM_GROUP_UNSUBSCRIBE, hashtable));
			}
			else
			{
				short @short = content.GetShort(BaseRequest.KEY_ERROR_CODE);
				string errorMessage = SFSErrorCodes.GetErrorMessage(@short, sfs.Log, content.GetUtfStringArray(BaseRequest.KEY_ERROR_PARAMS));
				hashtable["errorMessage"] = errorMessage;
				hashtable["errorCode"] = @short;
				sfs.DispatchEvent(new SFSEvent(SFSEvent.ROOM_GROUP_UNSUBSCRIBE_ERROR, hashtable));
			}
		}

		private void FnChangeRoomName(IMessage msg)
		{
			ISFSObject content = msg.Content;
			Hashtable hashtable = new Hashtable();
			if (content.IsNull(BaseRequest.KEY_ERROR_CODE))
			{
				int @int = content.GetInt(ChangeRoomNameRequest.KEY_ROOM);
				Room roomById = sfs.RoomManager.GetRoomById(@int);
				if (roomById != null)
				{
					hashtable["oldName"] = roomById.Name;
					sfs.RoomManager.ChangeRoomName(roomById, content.GetUtfString(ChangeRoomNameRequest.KEY_NAME));
					hashtable["room"] = roomById;
					sfs.DispatchEvent(new SFSEvent(SFSEvent.ROOM_NAME_CHANGE, hashtable));
				}
				else
				{
					log.Warn("Room not found, ID:" + @int + ", Room name change failed.");
				}
			}
			else
			{
				short @short = content.GetShort(BaseRequest.KEY_ERROR_CODE);
				string errorMessage = SFSErrorCodes.GetErrorMessage(@short, sfs.Log, content.GetUtfStringArray(BaseRequest.KEY_ERROR_PARAMS));
				hashtable["errorMessage"] = errorMessage;
				hashtable["errorCode"] = @short;
				sfs.DispatchEvent(new SFSEvent(SFSEvent.ROOM_NAME_CHANGE_ERROR, hashtable));
			}
		}

		private void FnChangeRoomPassword(IMessage msg)
		{
			ISFSObject content = msg.Content;
			Hashtable hashtable = new Hashtable();
			if (content.IsNull(BaseRequest.KEY_ERROR_CODE))
			{
				int @int = content.GetInt(ChangeRoomPasswordStateRequest.KEY_ROOM);
				Room roomById = sfs.RoomManager.GetRoomById(@int);
				if (roomById != null)
				{
					sfs.RoomManager.ChangeRoomPasswordState(roomById, content.GetBool(ChangeRoomPasswordStateRequest.KEY_PASS));
					hashtable["room"] = roomById;
					sfs.DispatchEvent(new SFSEvent(SFSEvent.ROOM_PASSWORD_STATE_CHANGE, hashtable));
				}
				else
				{
					log.Warn("Room not found, ID:" + @int + ", Room password change failed.");
				}
			}
			else
			{
				short @short = content.GetShort(BaseRequest.KEY_ERROR_CODE);
				string errorMessage = SFSErrorCodes.GetErrorMessage(@short, sfs.Log, content.GetUtfStringArray(BaseRequest.KEY_ERROR_PARAMS));
				hashtable["errorMessage"] = errorMessage;
				hashtable["errorCode"] = @short;
				sfs.DispatchEvent(new SFSEvent(SFSEvent.ROOM_PASSWORD_STATE_CHANGE_ERROR, hashtable));
			}
		}

		private void FnChangeRoomCapacity(IMessage msg)
		{
			ISFSObject content = msg.Content;
			Hashtable hashtable = new Hashtable();
			if (content.IsNull(BaseRequest.KEY_ERROR_CODE))
			{
				int @int = content.GetInt(ChangeRoomCapacityRequest.KEY_ROOM);
				Room roomById = sfs.RoomManager.GetRoomById(@int);
				if (roomById != null)
				{
					sfs.RoomManager.ChangeRoomCapacity(roomById, content.GetInt(ChangeRoomCapacityRequest.KEY_USER_SIZE), content.GetInt(ChangeRoomCapacityRequest.KEY_SPEC_SIZE));
					hashtable["room"] = roomById;
					sfs.DispatchEvent(new SFSEvent(SFSEvent.ROOM_CAPACITY_CHANGE, hashtable));
				}
				else
				{
					log.Warn("Room not found, ID:" + @int + ", Room capacity change failed.");
				}
			}
			else
			{
				short @short = content.GetShort(BaseRequest.KEY_ERROR_CODE);
				string errorMessage = SFSErrorCodes.GetErrorMessage(@short, sfs.Log, content.GetUtfStringArray(BaseRequest.KEY_ERROR_PARAMS));
				hashtable["errorMessage"] = errorMessage;
				hashtable["errorCode"] = @short;
				sfs.DispatchEvent(new SFSEvent(SFSEvent.ROOM_CAPACITY_CHANGE_ERROR, hashtable));
			}
		}

		private void FnLogout(IMessage msg)
		{
			sfs.HandleLogout();
			ISFSObject content = msg.Content;
			Hashtable hashtable = new Hashtable();
			hashtable["zoneName"] = content.GetUtfString(LogoutRequest.KEY_ZONE_NAME);
			sfs.DispatchEvent(new SFSEvent(SFSEvent.LOGOUT, hashtable));
		}

		private User GetOrCreateUser(ISFSArray userObj, bool addToGlobalManager, Room room)
		{
			int @int = userObj.GetInt(0);
			User user = sfs.UserManager.GetUserById(@int);
			if (user == null)
			{
				user = SFSUser.FromSFSArray(userObj, room);
				user.UserManager = sfs.UserManager;
			}
			else if (room != null)
			{
				user.SetPlayerId(userObj.GetShort(3), room);
				ISFSArray sFSArray = userObj.GetSFSArray(4);
				for (int i = 0; i < sFSArray.Size(); i++)
				{
					user.SetVariable(SFSUserVariable.FromSFSArray(sFSArray.GetSFSArray(i)));
				}
			}
			if (addToGlobalManager)
			{
				sfs.UserManager.AddUser(user);
			}
			return user;
		}

		private void FnSpectatorToPlayer(IMessage msg)
		{
			ISFSObject content = msg.Content;
			Hashtable hashtable = new Hashtable();
			if (content.IsNull(BaseRequest.KEY_ERROR_CODE))
			{
				int @int = content.GetInt(SpectatorToPlayerRequest.KEY_ROOM_ID);
				int int2 = content.GetInt(SpectatorToPlayerRequest.KEY_USER_ID);
				int @short = content.GetShort(SpectatorToPlayerRequest.KEY_PLAYER_ID);
				User userById = sfs.UserManager.GetUserById(int2);
				Room roomById = sfs.RoomManager.GetRoomById(@int);
				if (roomById != null)
				{
					if (userById != null)
					{
						if (userById.IsJoinedInRoom(roomById))
						{
							userById.SetPlayerId(@short, roomById);
							hashtable["room"] = roomById;
							hashtable["user"] = userById;
							hashtable["playerId"] = @short;
							sfs.DispatchEvent(new SFSEvent(SFSEvent.SPECTATOR_TO_PLAYER, hashtable));
						}
						else
						{
							log.Warn(string.Concat("User: ", userById, " not joined in Room: ", roomById, ", SpectatorToPlayer failed."));
						}
					}
					else
					{
						log.Warn("User not found, ID:" + int2 + ", SpectatorToPlayer failed.");
					}
				}
				else
				{
					log.Warn("Room not found, ID:" + @int + ", SpectatorToPlayer failed.");
				}
			}
			else
			{
				short short2 = content.GetShort(BaseRequest.KEY_ERROR_CODE);
				string errorMessage = SFSErrorCodes.GetErrorMessage(short2, sfs.Log, content.GetUtfStringArray(BaseRequest.KEY_ERROR_PARAMS));
				hashtable["errorMessage"] = errorMessage;
				hashtable["errorCode"] = short2;
				sfs.DispatchEvent(new SFSEvent(SFSEvent.SPECTATOR_TO_PLAYER_ERROR, hashtable));
			}
		}

		private void FnPlayerToSpectator(IMessage msg)
		{
			ISFSObject content = msg.Content;
			Hashtable hashtable = new Hashtable();
			if (content.IsNull(BaseRequest.KEY_ERROR_CODE))
			{
				int @int = content.GetInt(PlayerToSpectatorRequest.KEY_ROOM_ID);
				int int2 = content.GetInt(PlayerToSpectatorRequest.KEY_USER_ID);
				User userById = sfs.UserManager.GetUserById(int2);
				Room roomById = sfs.RoomManager.GetRoomById(@int);
				if (roomById != null)
				{
					if (userById != null)
					{
						if (userById.IsJoinedInRoom(roomById))
						{
							userById.SetPlayerId(-1, roomById);
							hashtable["room"] = roomById;
							hashtable["user"] = userById;
							sfs.DispatchEvent(new SFSEvent(SFSEvent.PLAYER_TO_SPECTATOR, hashtable));
						}
						else
						{
							log.Warn(string.Concat("User: ", userById, " not joined in Room: ", roomById, ", PlayerToSpectator failed."));
						}
					}
					else
					{
						log.Warn("User not found, ID:" + int2 + ", PlayerToSpectator failed.");
					}
				}
				else
				{
					log.Warn("Room not found, ID:" + @int + ", PlayerToSpectator failed.");
				}
			}
			else
			{
				short @short = content.GetShort(BaseRequest.KEY_ERROR_CODE);
				string errorMessage = SFSErrorCodes.GetErrorMessage(@short, sfs.Log, content.GetUtfStringArray(BaseRequest.KEY_ERROR_PARAMS));
				hashtable["errorMessage"] = errorMessage;
				hashtable["errorCode"] = @short;
				sfs.DispatchEvent(new SFSEvent(SFSEvent.PLAYER_TO_SPECTATOR_ERROR, hashtable));
			}
		}

		private void FnInitBuddyList(IMessage msg)
		{
			ISFSObject content = msg.Content;
			Hashtable hashtable = new Hashtable();
			if (content.IsNull(BaseRequest.KEY_ERROR_CODE))
			{
				ISFSArray sFSArray = content.GetSFSArray(InitBuddyListRequest.KEY_BLIST);
				ISFSArray sFSArray2 = content.GetSFSArray(InitBuddyListRequest.KEY_MY_VARS);
				string[] utfStringArray = content.GetUtfStringArray(InitBuddyListRequest.KEY_BUDDY_STATES);
				sfs.BuddyManager.ClearAll();
				for (int i = 0; i < sFSArray.Size(); i++)
				{
					Buddy buddy = SFSBuddy.FromSFSArray(sFSArray.GetSFSArray(i));
					sfs.BuddyManager.AddBuddy(buddy);
				}
				if (utfStringArray != null)
				{
					sfs.BuddyManager.BuddyStates = new List<string>(utfStringArray);
				}
				List<BuddyVariable> list = new List<BuddyVariable>();
				for (int j = 0; j < sFSArray2.Size(); j++)
				{
					list.Add(SFSBuddyVariable.FromSFSArray(sFSArray2.GetSFSArray(j)));
				}
				sfs.BuddyManager.MyVariables = list;
				sfs.BuddyManager.Inited = true;
				hashtable["buddyList"] = sfs.BuddyManager.BuddyList;
				hashtable["myVariables"] = sfs.BuddyManager.MyVariables;
				sfs.DispatchEvent(new SFSBuddyEvent(SFSBuddyEvent.BUDDY_LIST_INIT, hashtable));
			}
			else
			{
				short @short = content.GetShort(BaseRequest.KEY_ERROR_CODE);
				string errorMessage = SFSErrorCodes.GetErrorMessage(@short, sfs.Log, content.GetUtfStringArray(BaseRequest.KEY_ERROR_PARAMS));
				hashtable["errorMessage"] = errorMessage;
				hashtable["errorCode"] = @short;
				sfs.DispatchEvent(new SFSBuddyEvent(SFSBuddyEvent.BUDDY_ERROR, hashtable));
			}
		}

		private void FnAddBuddy(IMessage msg)
		{
			ISFSObject content = msg.Content;
			Hashtable hashtable = new Hashtable();
			if (content.IsNull(BaseRequest.KEY_ERROR_CODE))
			{
				Buddy buddy = SFSBuddy.FromSFSArray(content.GetSFSArray(AddBuddyRequest.KEY_BUDDY_NAME));
				sfs.BuddyManager.AddBuddy(buddy);
				hashtable["buddy"] = buddy;
				sfs.DispatchEvent(new SFSBuddyEvent(SFSBuddyEvent.BUDDY_ADD, hashtable));
			}
			else
			{
				short @short = content.GetShort(BaseRequest.KEY_ERROR_CODE);
				string errorMessage = SFSErrorCodes.GetErrorMessage(@short, sfs.Log, content.GetUtfStringArray(BaseRequest.KEY_ERROR_PARAMS));
				hashtable["errorMessage"] = errorMessage;
				hashtable["errorCode"] = @short;
				sfs.DispatchEvent(new SFSBuddyEvent(SFSBuddyEvent.BUDDY_ERROR, hashtable));
			}
		}

		private void FnRemoveBuddy(IMessage msg)
		{
			ISFSObject content = msg.Content;
			Hashtable hashtable = new Hashtable();
			if (content.IsNull(BaseRequest.KEY_ERROR_CODE))
			{
				string utfString = content.GetUtfString(RemoveBuddyRequest.KEY_BUDDY_NAME);
				Buddy buddy = sfs.BuddyManager.RemoveBuddyByName(utfString);
				if (buddy != null)
				{
					hashtable["buddy"] = buddy;
					sfs.DispatchEvent(new SFSBuddyEvent(SFSBuddyEvent.BUDDY_REMOVE, hashtable));
				}
				else
				{
					log.Warn("RemoveBuddy failed, buddy not found: " + utfString);
				}
			}
			else
			{
				short @short = content.GetShort(BaseRequest.KEY_ERROR_CODE);
				string errorMessage = SFSErrorCodes.GetErrorMessage(@short, sfs.Log, content.GetUtfStringArray(BaseRequest.KEY_ERROR_PARAMS));
				hashtable["errorMessage"] = errorMessage;
				hashtable["errorCode"] = @short;
				sfs.DispatchEvent(new SFSBuddyEvent(SFSBuddyEvent.BUDDY_ERROR, hashtable));
			}
		}

		private void FnBlockBuddy(IMessage msg)
		{
			ISFSObject content = msg.Content;
			Hashtable hashtable = new Hashtable();
			if (content.IsNull(BaseRequest.KEY_ERROR_CODE))
			{
				string utfString = content.GetUtfString(BlockBuddyRequest.KEY_BUDDY_NAME);
				Buddy buddyByName = sfs.BuddyManager.GetBuddyByName(utfString);
				if (buddyByName != null)
				{
					buddyByName.IsBlocked = content.GetBool(BlockBuddyRequest.KEY_BUDDY_BLOCK_STATE);
					hashtable["buddy"] = buddyByName;
					sfs.DispatchEvent(new SFSBuddyEvent(SFSBuddyEvent.BUDDY_BLOCK, hashtable));
				}
				else
				{
					log.Warn("BlockBuddy failed, buddy not found: " + utfString + ", in local BuddyList");
				}
			}
			else
			{
				short @short = content.GetShort(BaseRequest.KEY_ERROR_CODE);
				string errorMessage = SFSErrorCodes.GetErrorMessage(@short, sfs.Log, content.GetUtfStringArray(BaseRequest.KEY_ERROR_PARAMS));
				hashtable["errorMessage"] = errorMessage;
				hashtable["errorCode"] = @short;
				sfs.DispatchEvent(new SFSBuddyEvent(SFSBuddyEvent.BUDDY_ERROR, hashtable));
			}
		}

		private void FnGoOnline(IMessage msg)
		{
			ISFSObject content = msg.Content;
			Hashtable hashtable = new Hashtable();
			if (content.IsNull(BaseRequest.KEY_ERROR_CODE))
			{
				string utfString = content.GetUtfString(GoOnlineRequest.KEY_BUDDY_NAME);
				Buddy buddyByName = sfs.BuddyManager.GetBuddyByName(utfString);
				bool flag = utfString == sfs.MySelf.Name;
				int @byte = content.GetByte(GoOnlineRequest.KEY_ONLINE);
				bool flag2 = @byte == 0;
				bool flag3 = true;
				if (flag)
				{
					if (sfs.BuddyManager.MyOnlineState != flag2)
					{
						log.Warn("Unexpected: MyOnlineState is not in synch with the server. Resynching: " + flag2);
						sfs.BuddyManager.MyOnlineState = flag2;
					}
				}
				else
				{
					if (buddyByName == null)
					{
						log.Warn("GoOnline error, buddy not found: " + utfString + ", in local BuddyList.");
						return;
					}
					buddyByName.Id = content.GetInt(GoOnlineRequest.KEY_BUDDY_ID);
					BuddyVariable variable = new SFSBuddyVariable(ReservedBuddyVariables.BV_ONLINE, flag2);
					buddyByName.SetVariable(variable);
					if (@byte == 2)
					{
						buddyByName.ClearVolatileVariables();
					}
					flag3 = sfs.BuddyManager.MyOnlineState;
				}
				if (flag3)
				{
					hashtable["buddy"] = buddyByName;
					hashtable["isItMe"] = flag;
					sfs.DispatchEvent(new SFSBuddyEvent(SFSBuddyEvent.BUDDY_ONLINE_STATE_UPDATE, hashtable));
				}
			}
			else
			{
				short @short = content.GetShort(BaseRequest.KEY_ERROR_CODE);
				string errorMessage = SFSErrorCodes.GetErrorMessage(@short, sfs.Log, content.GetUtfStringArray(BaseRequest.KEY_ERROR_PARAMS));
				hashtable["errorMessage"] = errorMessage;
				hashtable["errorCode"] = @short;
				sfs.DispatchEvent(new SFSBuddyEvent(SFSBuddyEvent.BUDDY_ERROR, hashtable));
			}
		}

		private void FnSetBuddyVariables(IMessage msg)
		{
			ISFSObject content = msg.Content;
			Hashtable hashtable = new Hashtable();
			if (content.IsNull(BaseRequest.KEY_ERROR_CODE))
			{
				string utfString = content.GetUtfString(SetBuddyVariablesRequest.KEY_BUDDY_NAME);
				ISFSArray sFSArray = content.GetSFSArray(SetBuddyVariablesRequest.KEY_BUDDY_VARS);
				Buddy buddyByName = sfs.BuddyManager.GetBuddyByName(utfString);
				bool flag = utfString == sfs.MySelf.Name;
				List<string> list = new List<string>();
				List<BuddyVariable> list2 = new List<BuddyVariable>();
				bool flag2 = true;
				for (int i = 0; i < sFSArray.Size(); i++)
				{
					BuddyVariable buddyVariable = SFSBuddyVariable.FromSFSArray(sFSArray.GetSFSArray(i));
					list2.Add(buddyVariable);
					list.Add(buddyVariable.Name);
				}
				if (flag)
				{
					sfs.BuddyManager.MyVariables = list2;
				}
				else
				{
					if (buddyByName == null)
					{
						log.Warn("Unexpected. Target of BuddyVariables update not found: " + utfString);
						return;
					}
					buddyByName.SetVariables(list2);
					flag2 = sfs.BuddyManager.MyOnlineState;
				}
				if (flag2)
				{
					hashtable["isItMe"] = flag;
					hashtable["changedVars"] = list;
					hashtable["buddy"] = buddyByName;
					sfs.DispatchEvent(new SFSBuddyEvent(SFSBuddyEvent.BUDDY_VARIABLES_UPDATE, hashtable));
				}
			}
			else
			{
				short @short = content.GetShort(BaseRequest.KEY_ERROR_CODE);
				string errorMessage = SFSErrorCodes.GetErrorMessage(@short, sfs.Log, content.GetUtfStringArray(BaseRequest.KEY_ERROR_PARAMS));
				hashtable["errorMessage"] = errorMessage;
				hashtable["errorCode"] = @short;
				sfs.DispatchEvent(new SFSBuddyEvent(SFSBuddyEvent.BUDDY_ERROR, hashtable));
			}
		}

		private void FnFindRooms(IMessage msg)
		{
			ISFSObject content = msg.Content;
			Hashtable hashtable = new Hashtable();
			ISFSArray sFSArray = content.GetSFSArray(FindRoomsRequest.KEY_FILTERED_ROOMS);
			List<Room> list = new List<Room>();
			for (int i = 0; i < sFSArray.Size(); i++)
			{
				list.Add(SFSRoom.FromSFSArray(sFSArray.GetSFSArray(i)));
			}
			hashtable["rooms"] = list;
			sfs.DispatchEvent(new SFSEvent(SFSEvent.ROOM_FIND_RESULT, hashtable));
		}

		private void FnFindUsers(IMessage msg)
		{
			ISFSObject content = msg.Content;
			Hashtable hashtable = new Hashtable();
			ISFSArray sFSArray = content.GetSFSArray(FindUsersRequest.KEY_FILTERED_USERS);
			List<User> list = new List<User>();
			User mySelf = sfs.MySelf;
			for (int i = 0; i < sFSArray.Size(); i++)
			{
				User user = SFSUser.FromSFSArray(sFSArray.GetSFSArray(i));
				if (user.Id == mySelf.Id)
				{
					user = mySelf;
				}
				list.Add(user);
			}
			hashtable["users"] = list;
			sfs.DispatchEvent(new SFSEvent(SFSEvent.USER_FIND_RESULT, hashtable));
		}

		private void FnInviteUsers(IMessage msg)
		{
			ISFSObject content = msg.Content;
			Hashtable hashtable = new Hashtable();
			User user = null;
			user = ((!content.ContainsKey(InviteUsersRequest.KEY_USER_ID)) ? SFSUser.FromSFSArray(content.GetSFSArray(InviteUsersRequest.KEY_USER)) : sfs.UserManager.GetUserById(content.GetInt(InviteUsersRequest.KEY_USER_ID)));
			int @short = content.GetShort(InviteUsersRequest.KEY_TIME);
			int @int = content.GetInt(InviteUsersRequest.KEY_INVITATION_ID);
			ISFSObject sFSObject = content.GetSFSObject(InviteUsersRequest.KEY_PARAMS);
			Invitation invitation = new SFSInvitation(user, sfs.MySelf, @short, sFSObject);
			invitation.Id = @int;
			hashtable["invitation"] = invitation;
			sfs.DispatchEvent(new SFSEvent(SFSEvent.INVITATION, hashtable));
		}

		private void FnInvitationReply(IMessage msg)
		{
			ISFSObject content = msg.Content;
			Hashtable hashtable = new Hashtable();
			if (content.IsNull(BaseRequest.KEY_ERROR_CODE))
			{
				User user = null;
				user = ((!content.ContainsKey(InviteUsersRequest.KEY_USER_ID)) ? SFSUser.FromSFSArray(content.GetSFSArray(InviteUsersRequest.KEY_USER)) : sfs.UserManager.GetUserById(content.GetInt(InviteUsersRequest.KEY_USER_ID)));
				int @byte = content.GetByte(InviteUsersRequest.KEY_REPLY_ID);
				ISFSObject sFSObject = content.GetSFSObject(InviteUsersRequest.KEY_PARAMS);
				hashtable["invitee"] = user;
				hashtable["reply"] = @byte;
				hashtable["data"] = sFSObject;
				sfs.DispatchEvent(new SFSEvent(SFSEvent.INVITATION_REPLY, hashtable));
			}
			else
			{
				short @short = content.GetShort(BaseRequest.KEY_ERROR_CODE);
				string errorMessage = SFSErrorCodes.GetErrorMessage(@short, sfs.Log, content.GetUtfStringArray(BaseRequest.KEY_ERROR_PARAMS));
				hashtable["errorMessage"] = errorMessage;
				hashtable["errorCode"] = @short;
				sfs.DispatchEvent(new SFSEvent(SFSEvent.INVITATION_REPLY_ERROR, hashtable));
			}
		}

		private void FnQuickJoinGame(IMessage msg)
		{
			ISFSObject content = msg.Content;
			Hashtable hashtable = new Hashtable();
			if (content.ContainsKey(BaseRequest.KEY_ERROR_CODE))
			{
				short @short = content.GetShort(BaseRequest.KEY_ERROR_CODE);
				string errorMessage = SFSErrorCodes.GetErrorMessage(@short, sfs.Log, content.GetUtfStringArray(BaseRequest.KEY_ERROR_PARAMS));
				hashtable["errorMessage"] = errorMessage;
				hashtable["errorCode"] = @short;
				sfs.DispatchEvent(new SFSEvent(SFSEvent.ROOM_JOIN_ERROR, hashtable));
			}
		}

		private void FnPingPong(IMessage msg)
		{
			int num = sfs.LagMonitor.OnPingPong();
			Hashtable hashtable = new Hashtable();
			hashtable["lagValue"] = num;
			sfs.DispatchEvent(new SFSEvent(SFSEvent.PING_PONG, hashtable));
		}

		private void PopulateRoomList(ISFSArray roomList)
		{
			IRoomManager roomManager = sfs.RoomManager;
			for (int i = 0; i < roomList.Size(); i++)
			{
				ISFSArray sFSArray = roomList.GetSFSArray(i);
				Room room = SFSRoom.FromSFSArray(sFSArray);
				roomManager.ReplaceRoom(room);
			}
		}

		private void FnGenericMessage(IMessage msg)
		{
			ISFSObject content = msg.Content;
			switch (content.GetByte(GenericMessageRequest.KEY_MESSAGE_TYPE))
			{
			case 0:
				HandlePublicMessage(content);
				break;
			case 1:
				HandlePrivateMessage(content);
				break;
			case 5:
				HandleBuddyMessage(content);
				break;
			case 2:
				HandleModMessage(content);
				break;
			case 3:
				HandleAdminMessage(content);
				break;
			case 4:
				HandleObjectMessage(content);
				break;
			}
		}

		private void HandlePublicMessage(ISFSObject sfso)
		{
			Hashtable hashtable = new Hashtable();
			int @int = sfso.GetInt(GenericMessageRequest.KEY_ROOM_ID);
			Room roomById = sfs.RoomManager.GetRoomById(@int);
			if (roomById != null)
			{
				hashtable["room"] = roomById;
				hashtable["sender"] = sfs.UserManager.GetUserById(sfso.GetInt(GenericMessageRequest.KEY_USER_ID));
				hashtable["message"] = sfso.GetUtfString(GenericMessageRequest.KEY_MESSAGE);
				hashtable["data"] = sfso.GetSFSObject(GenericMessageRequest.KEY_XTRA_PARAMS);
				sfs.DispatchEvent(new SFSEvent(SFSEvent.PUBLIC_MESSAGE, hashtable));
			}
			else
			{
				log.Warn("Unexpected, PublicMessage target room doesn't exist. RoomId: " + @int);
			}
		}

		public void HandlePrivateMessage(ISFSObject sfso)
		{
			Hashtable hashtable = new Hashtable();
			int @int = sfso.GetInt(GenericMessageRequest.KEY_USER_ID);
			User user = sfs.UserManager.GetUserById(@int);
			if (user == null)
			{
				if (!sfso.ContainsKey(GenericMessageRequest.KEY_SENDER_DATA))
				{
					log.Warn("Unexpected. Private message has no Sender details!");
					return;
				}
				user = SFSUser.FromSFSArray(sfso.GetSFSArray(GenericMessageRequest.KEY_SENDER_DATA));
			}
			hashtable["sender"] = user;
			hashtable["message"] = sfso.GetUtfString(GenericMessageRequest.KEY_MESSAGE);
			hashtable["data"] = sfso.GetSFSObject(GenericMessageRequest.KEY_XTRA_PARAMS);
			sfs.DispatchEvent(new SFSEvent(SFSEvent.PRIVATE_MESSAGE, hashtable));
		}

		public void HandleBuddyMessage(ISFSObject sfso)
		{
			Hashtable hashtable = new Hashtable();
			int @int = sfso.GetInt(GenericMessageRequest.KEY_USER_ID);
			Buddy buddyById = sfs.BuddyManager.GetBuddyById(@int);
			hashtable["isItMe"] = sfs.MySelf.Id == @int;
			hashtable["buddy"] = buddyById;
			hashtable["message"] = sfso.GetUtfString(GenericMessageRequest.KEY_MESSAGE);
			hashtable["data"] = sfso.GetSFSObject(GenericMessageRequest.KEY_XTRA_PARAMS);
			sfs.DispatchEvent(new SFSBuddyEvent(SFSBuddyEvent.BUDDY_MESSAGE, hashtable));
		}

		public void HandleModMessage(ISFSObject sfso)
		{
			HandleModMessage(sfso, SFSEvent.MODERATOR_MESSAGE);
		}

		public void HandleAdminMessage(ISFSObject sfso)
		{
			HandleModMessage(sfso, SFSEvent.ADMIN_MESSAGE);
		}

		private void HandleModMessage(ISFSObject sfso, string evt)
		{
			Hashtable hashtable = new Hashtable();
			hashtable["sender"] = SFSUser.FromSFSArray(sfso.GetSFSArray(GenericMessageRequest.KEY_SENDER_DATA));
			hashtable["message"] = sfso.GetUtfString(GenericMessageRequest.KEY_MESSAGE);
			hashtable["data"] = sfso.GetSFSObject(GenericMessageRequest.KEY_XTRA_PARAMS);
			sfs.DispatchEvent(new SFSEvent(evt, hashtable));
		}

		public void HandleObjectMessage(ISFSObject sfso)
		{
			Hashtable hashtable = new Hashtable();
			int @int = sfso.GetInt(GenericMessageRequest.KEY_USER_ID);
			hashtable["sender"] = sfs.UserManager.GetUserById(@int);
			hashtable["message"] = sfso.GetSFSObject(GenericMessageRequest.KEY_XTRA_PARAMS);
			sfs.DispatchEvent(new SFSEvent(SFSEvent.OBJECT_MESSAGE, hashtable));
		}
	}
}
