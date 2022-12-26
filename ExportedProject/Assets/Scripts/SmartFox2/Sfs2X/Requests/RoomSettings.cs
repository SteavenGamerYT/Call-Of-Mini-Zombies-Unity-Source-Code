using System.Collections.Generic;
using Sfs2X.Entities;
using Sfs2X.Entities.Variables;

namespace Sfs2X.Requests
{
	public class RoomSettings
	{
		private string name;

		private string password;

		private string groupId;

		private bool isGame;

		private short maxUsers;

		private short maxSpectators;

		private short maxVariables;

		private List<RoomVariable> variables;

		private RoomPermissions permissions;

		private RoomEvents events;

		private RoomExtension extension;

		public string Name
		{
			get
			{
				return name;
			}
		}

		public string Password
		{
			get
			{
				return password;
			}
			set
			{
				password = value;
			}
		}

		public bool IsGame
		{
			get
			{
				return isGame;
			}
		}

		public short MaxUsers
		{
			get
			{
				return maxUsers;
			}
			set
			{
				maxUsers = value;
			}
		}

		public short MaxVariables
		{
			get
			{
				return maxVariables;
			}
		}

		public short MaxSpectators
		{
			get
			{
				return maxSpectators;
			}
		}

		public List<RoomVariable> Variables
		{
			get
			{
				return variables;
			}
			set
			{
				variables = value;
			}
		}

		public RoomPermissions Permissions
		{
			get
			{
				return permissions;
			}
			set
			{
				permissions = value;
			}
		}

		public RoomEvents Events
		{
			get
			{
				return events;
			}
		}

		public RoomExtension Extension
		{
			get
			{
				return extension;
			}
			set
			{
				extension = value;
			}
		}

		public string GroupId
		{
			get
			{
				return groupId;
			}
			set
			{
				groupId = value;
			}
		}

		public RoomSettings(string name)
		{
			this.name = name;
			password = "";
			isGame = false;
			maxUsers = 10;
			maxSpectators = 0;
			maxVariables = 5;
			groupId = SFSConstants.DEFAULT_GROUP_ID;
			variables = new List<RoomVariable>();
		}
	}
}
