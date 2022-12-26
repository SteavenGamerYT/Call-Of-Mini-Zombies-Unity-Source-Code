using System.Collections.Generic;

namespace Sfs2X.Entities.Managers
{
	public class SFSUserManager : IUserManager
	{
		private Dictionary<string, User> usersByName;

		private Dictionary<int, User> usersById;

		protected Room room;

		protected SmartFox sfs;

		public int UserCount
		{
			get
			{
				return usersById.Count;
			}
		}

		public SFSUserManager(SmartFox sfs)
		{
			this.sfs = sfs;
			usersByName = new Dictionary<string, User>();
			usersById = new Dictionary<int, User>();
		}

		public SFSUserManager(Room room)
		{
			this.room = room;
			usersByName = new Dictionary<string, User>();
			usersById = new Dictionary<int, User>();
		}

		protected void LogWarn(string msg)
		{
			if (sfs != null)
			{
				sfs.Log.Warn(msg);
			}
			else if (room != null && room.RoomManager != null)
			{
				room.RoomManager.SmartFoxClient.Log.Warn(msg);
			}
		}

		public bool ContainsUser(User user)
		{
			return usersByName.ContainsValue(user);
		}

		public User GetUserById(int userId)
		{
			if (!usersById.ContainsKey(userId))
			{
				return null;
			}
			return usersById[userId];
		}

		public virtual void AddUser(User user)
		{
			if (usersById.ContainsKey(user.Id))
			{
				LogWarn("Unexpected: duplicate user in UserManager: " + user);
			}
			AddUserInternal(user);
		}

		protected void AddUserInternal(User user)
		{
			usersByName[user.Name] = user;
			usersById[user.Id] = user;
		}

		public virtual void RemoveUser(User user)
		{
			usersByName.Remove(user.Name);
			usersById.Remove(user.Id);
		}

		public List<User> GetUserList()
		{
			return new List<User>(usersById.Values);
		}

		public void ClearAll()
		{
			usersByName = new Dictionary<string, User>();
			usersById = new Dictionary<int, User>();
		}
	}
}
