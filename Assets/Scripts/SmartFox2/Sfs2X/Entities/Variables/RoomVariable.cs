namespace Sfs2X.Entities.Variables
{
	public interface RoomVariable : UserVariable
	{
		bool IsPrivate { set; }

		bool IsPersistent { set; }
	}
}
