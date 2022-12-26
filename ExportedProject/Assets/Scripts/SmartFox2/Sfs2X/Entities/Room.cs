using System.Collections.Generic;
using Sfs2X.Entities.Managers;
using Sfs2X.Entities.Variables;

namespace Sfs2X.Entities
{
	public interface Room
	{
		int Id { get; }

		string Name { get; set; }

		string GroupId { get; }

		bool IsJoined { get; set; }

		bool IsGame { get; set; }

		bool IsHidden { set; }

		bool IsPasswordProtected { get; set; }

		bool IsManaged { get; set; }

		int UserCount { get; set; }

		int MaxUsers { set; }

		int SpectatorCount { set; }

		int MaxSpectators { set; }

		int Capacity { get; }

		List<User> UserList { get; }

		IRoomManager RoomManager { get; set; }

		void AddUser(User user);

		void RemoveUser(User user);

		bool ContainsUser(User user);

		RoomVariable GetVariable(string name);

		List<RoomVariable> GetVariables();

		void SetVariable(RoomVariable roomVariable);

		void SetVariables(ICollection<RoomVariable> roomVariables);

		bool ContainsVariable(string name);

		void Merge(Room anotherRoom);
	}
}
