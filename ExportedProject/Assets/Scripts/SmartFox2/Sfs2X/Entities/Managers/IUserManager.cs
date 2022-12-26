using System.Collections.Generic;

namespace Sfs2X.Entities.Managers
{
	public interface IUserManager
	{
		int UserCount { get; }

		bool ContainsUser(User user);

		User GetUserById(int userId);

		void AddUser(User user);

		void RemoveUser(User user);

		List<User> GetUserList();

		void ClearAll();
	}
}
