using System.Collections.Generic;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using Sfs2X.Entities.Variables;
using Sfs2X.Exceptions;

namespace Sfs2X.Requests
{
	public class CreateRoomRequest : BaseRequest
	{
		public static readonly string KEY_ROOM = "r";

		public static readonly string KEY_NAME = "n";

		public static readonly string KEY_PASSWORD = "p";

		public static readonly string KEY_GROUP_ID = "g";

		public static readonly string KEY_ISGAME = "ig";

		public static readonly string KEY_MAXUSERS = "mu";

		public static readonly string KEY_MAXSPECTATORS = "ms";

		public static readonly string KEY_MAXVARS = "mv";

		public static readonly string KEY_ROOMVARS = "rv";

		public static readonly string KEY_PERMISSIONS = "pm";

		public static readonly string KEY_EVENTS = "ev";

		public static readonly string KEY_EXTID = "xn";

		public static readonly string KEY_EXTCLASS = "xc";

		public static readonly string KEY_EXTPROP = "xp";

		public static readonly string KEY_AUTOJOIN = "aj";

		public static readonly string KEY_ROOM_TO_LEAVE = "rl";

		private RoomSettings settings;

		private bool autoJoin;

		private Room roomToLeave;

		public CreateRoomRequest(RoomSettings settings, bool autoJoin, Room roomToLeave)
			: base(RequestType.CreateRoom)
		{
			Init(settings, autoJoin, roomToLeave);
		}

		private void Init(RoomSettings settings, bool autoJoin, Room roomToLeave)
		{
			this.settings = settings;
			this.autoJoin = autoJoin;
			this.roomToLeave = roomToLeave;
		}

		public override void Validate(SmartFox sfs)
		{
			List<string> list = new List<string>();
			if (settings.Name == null || settings.Name.Length == 0)
			{
				list.Add("Missing room name");
			}
			if (settings.MaxUsers <= 0)
			{
				list.Add("maxUsers must be > 0");
			}
			if (settings.Extension != null)
			{
				if (settings.Extension.ClassName == null || settings.Extension.ClassName.Length == 0)
				{
					list.Add("Missing Extension class name");
				}
				if (settings.Extension.Id == null || settings.Extension.Id.Length == 0)
				{
					list.Add("Missing Extension id");
				}
			}
			if (list.Count > 0)
			{
				throw new SFSValidationError("CreateRoom request error", list);
			}
		}

		public override void Execute(SmartFox sfs)
		{
			sfso.PutUtfString(KEY_NAME, settings.Name);
			sfso.PutUtfString(KEY_GROUP_ID, settings.GroupId);
			sfso.PutUtfString(KEY_PASSWORD, settings.Password);
			sfso.PutBool(KEY_ISGAME, settings.IsGame);
			sfso.PutShort(KEY_MAXUSERS, settings.MaxUsers);
			sfso.PutShort(KEY_MAXSPECTATORS, settings.MaxSpectators);
			sfso.PutShort(KEY_MAXVARS, settings.MaxVariables);
			if (settings.Variables != null && settings.Variables.Count > 0)
			{
				ISFSArray iSFSArray = SFSArray.NewInstance();
				foreach (RoomVariable variable in settings.Variables)
				{
					if (variable is RoomVariable)
					{
						RoomVariable roomVariable = variable as RoomVariable;
						iSFSArray.AddSFSArray(roomVariable.ToSFSArray());
					}
				}
				sfso.PutSFSArray(KEY_ROOMVARS, iSFSArray);
			}
			if (settings.Permissions != null)
			{
				List<bool> list = new List<bool>();
				list.Add(settings.Permissions.AllowNameChange);
				list.Add(settings.Permissions.AllowPasswordStateChange);
				list.Add(settings.Permissions.AllowPublicMessages);
				list.Add(settings.Permissions.AllowResizing);
				sfso.PutBoolArray(KEY_PERMISSIONS, list.ToArray());
			}
			if (settings.Events != null)
			{
				List<bool> list2 = new List<bool>();
				list2.Add(settings.Events.AllowUserEnter);
				list2.Add(settings.Events.AllowUserExit);
				list2.Add(settings.Events.AllowUserCountChange);
				list2.Add(settings.Events.AllowUserVariablesUpdate);
				sfso.PutBoolArray(KEY_EVENTS, list2.ToArray());
			}
			if (settings.Extension != null)
			{
				sfso.PutUtfString(KEY_EXTID, settings.Extension.Id);
				sfso.PutUtfString(KEY_EXTCLASS, settings.Extension.ClassName);
				if (settings.Extension.PropertiesFile != null && settings.Extension.PropertiesFile.Length > 0)
				{
					sfso.PutUtfString(KEY_EXTPROP, settings.Extension.PropertiesFile);
				}
			}
			sfso.PutBool(KEY_AUTOJOIN, autoJoin);
			if (roomToLeave != null)
			{
				sfso.PutInt(KEY_ROOM_TO_LEAVE, roomToLeave.Id);
			}
		}
	}
}
