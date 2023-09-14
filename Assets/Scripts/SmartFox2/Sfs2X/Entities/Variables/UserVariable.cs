using Sfs2X.Entities.Data;

namespace Sfs2X.Entities.Variables
{
	public interface UserVariable
	{
		string Name { get; }

		bool GetBoolValue();

		int GetIntValue();

		double GetDoubleValue();

		string GetStringValue();

		ISFSObject GetSFSObjectValue();

		ISFSArray GetSFSArrayValue();

		bool IsNull();

		ISFSArray ToSFSArray();
	}
}
