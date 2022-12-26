using System.Collections.Generic;

namespace Sfs2X.Entities.Managers
{
	public interface IRoomManager
	{
		SmartFox SmartFoxClient { get; }

		void AddRoom(Room room, bool addGroupIfMissing);

		void AddRoom(Room room);

		void AddGroup(string groupId);

		Room ReplaceRoom(Room room, bool addToGroupIfMissing);

		Room ReplaceRoom(Room room);

		void RemoveGroup(string groupId);

		bool ContainsGroup(string groupId);

		void ChangeRoomName(Room room, string newName);

		void ChangeRoomPasswordState(Room room, bool isPassProtected);

		void ChangeRoomCapacity(Room room, int maxUsers, int maxSpect);

		Room GetRoomById(int id);

		List<Room> GetRoomListFromGroup(string groupId);

		List<Room> GetJoinedRooms();

		List<Room> GetUserRooms(User user);

		void RemoveRoom(Room room);

		void RemoveUser(User user);
	}
}
