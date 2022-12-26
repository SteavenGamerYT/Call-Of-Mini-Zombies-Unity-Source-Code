using System.Collections.Generic;
using Sfs2X.Entities.Data;
using Sfs2X.Entities.Managers;
using Sfs2X.Entities.Variables;
using Sfs2X.Exceptions;

namespace Sfs2X.Entities
{
	public class SFSUser : User
	{
		protected int id = -1;

		protected int privilegeId = 0;

		protected string name;

		protected bool isItMe;

		protected Dictionary<string, UserVariable> variables;

		protected Dictionary<string, object> properties;

		protected bool isModerator;

		protected Dictionary<int, int> playerIdByRoomId;

		protected IUserManager userManager;

		public int Id
		{
			get
			{
				return id;
			}
		}

		public string Name
		{
			get
			{
				return name;
			}
		}

		public int PrivilegeId
		{
			set
			{
				privilegeId = value;
			}
		}

		public bool IsItMe
		{
			get
			{
				return isItMe;
			}
		}

		public IUserManager UserManager
		{
			set
			{
				if (userManager != null)
				{
					throw new SFSError("Cannot re-assign the User manager. Already set. User: " + this);
				}
				userManager = value;
			}
		}

		public SFSUser(int id, string name)
		{
			Init(id, name, false);
		}

		public SFSUser(int id, string name, bool isItMe)
		{
			Init(id, name, isItMe);
		}

		public static User FromSFSArray(ISFSArray sfsa, Room room)
		{
			User user = new SFSUser(sfsa.GetInt(0), sfsa.GetUtfString(1));
			user.PrivilegeId = sfsa.GetShort(2);
			if (room != null)
			{
				user.SetPlayerId(sfsa.GetShort(3), room);
			}
			ISFSArray sFSArray = sfsa.GetSFSArray(4);
			for (int i = 0; i < sFSArray.Size(); i++)
			{
				user.SetVariable(SFSUserVariable.FromSFSArray(sFSArray.GetSFSArray(i)));
			}
			return user;
		}

		public static User FromSFSArray(ISFSArray sfsa)
		{
			return FromSFSArray(sfsa, null);
		}

		private void Init(int id, string name, bool isItMe)
		{
			this.id = id;
			this.name = name;
			this.isItMe = isItMe;
			variables = new Dictionary<string, UserVariable>();
			properties = new Dictionary<string, object>();
			isModerator = false;
			playerIdByRoomId = new Dictionary<int, int>();
		}

		public bool IsJoinedInRoom(Room room)
		{
			return room.ContainsUser(this);
		}

		public void SetPlayerId(int id, Room room)
		{
			playerIdByRoomId[room.Id] = id;
		}

		public UserVariable GetVariable(string name)
		{
			if (!variables.ContainsKey(name))
			{
				return null;
			}
			return variables[name];
		}

		public void SetVariable(UserVariable userVariable)
		{
			if (userVariable != null)
			{
				if (userVariable.IsNull())
				{
					variables.Remove(userVariable.Name);
				}
				else
				{
					variables[userVariable.Name] = userVariable;
				}
			}
		}

		public bool ContainsVariable(string name)
		{
			return variables.ContainsKey(name);
		}

		public override string ToString()
		{
			return "[User: " + name + ", Id: " + id + ", isMe: " + isItMe + "]";
		}
	}
}
