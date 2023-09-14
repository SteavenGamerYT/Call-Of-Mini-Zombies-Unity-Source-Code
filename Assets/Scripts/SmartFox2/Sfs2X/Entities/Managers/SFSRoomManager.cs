using System.Collections.Generic;

namespace Sfs2X.Entities.Managers
{
	public class SFSRoomManager : IRoomManager
	{
		private string ownerZone;

		private List<string> groups;

		private Dictionary<int, Room> roomsById;

		private Dictionary<string, Room> roomsByName;

		protected SmartFox smartFox;

		public SmartFox SmartFoxClient
		{
			get
			{
				return smartFox;
			}
		}

		public SFSRoomManager(SmartFox sfs)
		{
			smartFox = sfs;
			groups = new List<string>();
			roomsById = new Dictionary<int, Room>();
			roomsByName = new Dictionary<string, Room>();
		}

		public void AddRoom(Room room)
		{
			AddRoom(room, true);
		}

		public void AddRoom(Room room, bool addGroupIfMissing)
		{
			roomsById[room.Id] = room;
			roomsByName[room.Name] = room;
			if (addGroupIfMissing)
			{
				if (!ContainsGroup(room.GroupId))
				{
					AddGroup(room.GroupId);
				}
			}
			else
			{
				room.IsManaged = false;
			}
		}

		public Room ReplaceRoom(Room room)
		{
			return ReplaceRoom(room, true);
		}

		public Room ReplaceRoom(Room room, bool addToGroupIfMissing)
		{
			Room roomById = GetRoomById(room.Id);
			if (roomById != null)
			{
				roomById.Merge(room);
				return roomById;
			}
			AddRoom(room, addToGroupIfMissing);
			return room;
		}

		public void ChangeRoomName(Room room, string newName)
		{
			string name = room.Name;
			room.Name = newName;
			roomsByName[newName] = room;
			roomsByName.Remove(name);
		}

		public void ChangeRoomPasswordState(Room room, bool isPassProtected)
		{
			room.IsPasswordProtected = isPassProtected;
		}

		public void ChangeRoomCapacity(Room room, int maxUsers, int maxSpect)
		{
			room.MaxUsers = maxUsers;
			room.MaxSpectators = maxSpect;
		}

		public void AddGroup(string groupId)
		{
			groups.Add(groupId);
		}

		public void RemoveGroup(string groupId)
		{
			groups.Remove(groupId);
			List<Room> roomListFromGroup = GetRoomListFromGroup(groupId);
			foreach (Room item in roomListFromGroup)
			{
				if (!item.IsJoined)
				{
					RemoveRoom(item);
				}
				else
				{
					item.IsManaged = false;
				}
			}
		}

		public bool ContainsGroup(string groupId)
		{
			return groups.Contains(groupId);
		}

		public Room GetRoomById(int id)
		{
			if (!roomsById.ContainsKey(id))
			{
				return null;
			}
			return roomsById[id];
		}

		public List<Room> GetRoomListFromGroup(string groupId)
		{
			List<Room> list = new List<Room>();
			foreach (Room value in roomsById.Values)
			{
				if (value.GroupId == groupId)
				{
					list.Add(value);
				}
			}
			return list;
		}

		public void RemoveRoom(Room room)
		{
			RemoveRoom(room.Id, room.Name);
		}

		public List<Room> GetJoinedRooms()
		{
			List<Room> list = new List<Room>();
			foreach (Room value in roomsById.Values)
			{
				if (value.IsJoined)
				{
					list.Add(value);
				}
			}
			return list;
		}

		public List<Room> GetUserRooms(User user)
		{
			List<Room> list = new List<Room>();
			foreach (Room value in roomsById.Values)
			{
				if (value.ContainsUser(user))
				{
					list.Add(value);
				}
			}
			return list;
		}

		public void RemoveUser(User user)
		{
			foreach (Room value in roomsById.Values)
			{
				if (value.ContainsUser(user))
				{
					value.RemoveUser(user);
				}
			}
		}

		private void RemoveRoom(int id, string name)
		{
			roomsById.Remove(id);
			roomsByName.Remove(name);
		}
	}
}
