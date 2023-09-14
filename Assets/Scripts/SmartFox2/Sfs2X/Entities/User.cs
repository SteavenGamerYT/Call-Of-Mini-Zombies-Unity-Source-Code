using Sfs2X.Entities.Managers;
using Sfs2X.Entities.Variables;

namespace Sfs2X.Entities
{
	public interface User
	{
		int Id { get; }

		string Name { get; }

		int PrivilegeId { set; }

		IUserManager UserManager { set; }

		bool IsItMe { get; }

		void SetPlayerId(int id, Room room);

		bool IsJoinedInRoom(Room room);

		UserVariable GetVariable(string varName);

		void SetVariable(UserVariable userVariable);

		bool ContainsVariable(string name);
	}
}
