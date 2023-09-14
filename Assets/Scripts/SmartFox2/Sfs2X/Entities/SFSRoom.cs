using System.Collections;
using System.Collections.Generic;
using Sfs2X.Entities.Data;
using Sfs2X.Entities.Managers;
using Sfs2X.Entities.Variables;
using Sfs2X.Exceptions;

namespace Sfs2X.Entities
{
	public class SFSRoom : Room
	{
		protected int id;

		protected string name;

		protected string groupId;

		protected bool isGame;

		protected bool isHidden;

		protected bool isJoined;

		protected bool isPasswordProtected;

		protected bool isManaged;

		protected Dictionary<string, RoomVariable> variables;

		protected Hashtable properties;

		protected IUserManager userManager;

		protected int maxUsers;

		protected int maxSpectators;

		protected int userCount;

		protected int specCount;

		protected IRoomManager roomManager;

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
			set
			{
				name = value;
			}
		}

		public string GroupId
		{
			get
			{
				return groupId;
			}
		}

		public bool IsGame
		{
			get
			{
				return isGame;
			}
			set
			{
				isGame = value;
			}
		}

		public bool IsHidden
		{
			set
			{
				isHidden = value;
			}
		}

		public bool IsJoined
		{
			get
			{
				return isJoined;
			}
			set
			{
				isJoined = value;
			}
		}

		public bool IsPasswordProtected
		{
			get
			{
				return isPasswordProtected;
			}
			set
			{
				isPasswordProtected = value;
			}
		}

		public bool IsManaged
		{
			get
			{
				return isManaged;
			}
			set
			{
				isManaged = value;
			}
		}

		public int MaxSpectators
		{
			set
			{
				maxSpectators = value;
			}
		}

		public int UserCount
		{
			get
			{
				if (isJoined)
				{
					return userManager.UserCount;
				}
				return userCount;
			}
			set
			{
				userCount = value;
			}
		}

		public int MaxUsers
		{
			set
			{
				maxUsers = value;
			}
		}

		public int Capacity
		{
			get
			{
				return maxUsers + maxSpectators;
			}
		}

		public int SpectatorCount
		{
			set
			{
				specCount = value;
			}
		}

		public List<User> UserList
		{
			get
			{
				return userManager.GetUserList();
			}
		}

		public IRoomManager RoomManager
		{
			get
			{
				return roomManager;
			}
			set
			{
				if (roomManager != null)
				{
					throw new SFSError("Room manager already assigned. Room: " + this);
				}
				roomManager = value;
			}
		}

		public SFSRoom(int id, string name, string groupId)
		{
			Init(id, name, groupId);
		}

		public static Room FromSFSArray(ISFSArray sfsa)
		{
			Room room = new SFSRoom(sfsa.GetInt(0), sfsa.GetUtfString(1), sfsa.GetUtfString(2));
			room.IsGame = sfsa.GetBool(3);
			room.IsHidden = sfsa.GetBool(4);
			room.IsPasswordProtected = sfsa.GetBool(5);
			room.UserCount = sfsa.GetShort(6);
			room.MaxUsers = sfsa.GetShort(7);
			ISFSArray sFSArray = sfsa.GetSFSArray(8);
			if (sFSArray.Size() > 0)
			{
				List<RoomVariable> list = new List<RoomVariable>();
				for (int i = 0; i < sFSArray.Size(); i++)
				{
					list.Add(SFSRoomVariable.FromSFSArray(sFSArray.GetSFSArray(i)));
				}
				room.SetVariables(list);
			}
			if (room.IsGame)
			{
				room.SpectatorCount = sfsa.GetShort(9);
				room.MaxSpectators = sfsa.GetShort(10);
			}
			return room;
		}

		private void Init(int id, string name, string groupId)
		{
			this.id = id;
			this.name = name;
			this.groupId = groupId;
			isJoined = (isGame = (isHidden = false));
			isManaged = true;
			userCount = (specCount = 0);
			variables = new Dictionary<string, RoomVariable>();
			properties = new Hashtable();
			userManager = new SFSUserManager(this);
		}

		public List<RoomVariable> GetVariables()
		{
			return new List<RoomVariable>(variables.Values);
		}

		public RoomVariable GetVariable(string name)
		{
			if (!variables.ContainsKey(name))
			{
				return null;
			}
			return variables[name];
		}

		public void RemoveUser(User user)
		{
			userManager.RemoveUser(user);
		}

		public void SetVariable(RoomVariable roomVariable)
		{
			if (roomVariable.IsNull())
			{
				variables.Remove(roomVariable.Name);
			}
			else
			{
				variables[roomVariable.Name] = roomVariable;
			}
		}

		public void SetVariables(ICollection<RoomVariable> roomVariables)
		{
			foreach (RoomVariable roomVariable in roomVariables)
			{
				SetVariable(roomVariable);
			}
		}

		public bool ContainsVariable(string name)
		{
			return variables.ContainsKey(name);
		}

		public void AddUser(User user)
		{
			userManager.AddUser(user);
		}

		public bool ContainsUser(User user)
		{
			return userManager.ContainsUser(user);
		}

		public override string ToString()
		{
			return "[Room: " + name + ", Id: " + id + ", GroupId: " + groupId + "]";
		}

		public void Merge(Room anotherRoom)
		{
			foreach (RoomVariable variable in anotherRoom.GetVariables())
			{
				variables[variable.Name] = variable;
			}
			userManager.ClearAll();
			foreach (User user in anotherRoom.UserList)
			{
				userManager.AddUser(user);
			}
		}
	}
}
