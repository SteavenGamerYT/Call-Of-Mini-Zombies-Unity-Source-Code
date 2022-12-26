using Sfs2X.Entities.Data;

namespace Sfs2X.Entities.Variables
{
	public class SFSRoomVariable : SFSUserVariable, RoomVariable, UserVariable
	{
		private bool isPersistent;

		private bool isPrivate;

		public bool IsPrivate
		{
			set
			{
				isPrivate = value;
			}
		}

		public bool IsPersistent
		{
			set
			{
				isPersistent = value;
			}
		}

		public SFSRoomVariable(string name, object val)
			: base(name, val, -1)
		{
		}

		public SFSRoomVariable(string name, object val, int type)
			: base(name, val, type)
		{
		}

		public new static RoomVariable FromSFSArray(ISFSArray sfsa)
		{
			RoomVariable roomVariable = new SFSRoomVariable(sfsa.GetUtfString(0), sfsa.GetElementAt(2), sfsa.GetByte(1));
			roomVariable.IsPrivate = sfsa.GetBool(3);
			roomVariable.IsPersistent = sfsa.GetBool(4);
			return roomVariable;
		}

		public override string ToString()
		{
			return string.Concat("[RVar: ", name, ", type: ", type, ", value: ", val, ", isPriv: ", isPrivate, "]");
		}

		public override ISFSArray ToSFSArray()
		{
			ISFSArray iSFSArray = base.ToSFSArray();
			iSFSArray.AddBool(isPrivate);
			iSFSArray.AddBool(isPersistent);
			return iSFSArray;
		}
	}
}
