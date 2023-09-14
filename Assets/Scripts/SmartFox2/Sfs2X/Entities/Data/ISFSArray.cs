using System.Collections;

namespace Sfs2X.Entities.Data
{
	public interface ISFSArray : ICollection, IEnumerable
	{
		object GetElementAt(int index);

		SFSDataWrapper GetWrappedElementAt(int index);

		int Size();

		string GetDump(bool format);

		void AddNull();

		void AddBool(bool val);

		void AddByte(byte val);

		void AddInt(int val);

		void AddDouble(double val);

		void AddUtfString(string val);

		void AddSFSArray(ISFSArray val);

		void AddSFSObject(ISFSObject val);

		void Add(SFSDataWrapper val);

		bool GetBool(int index);

		byte GetByte(int index);

		short GetShort(int index);

		int GetInt(int index);

		string GetUtfString(int index);

		ISFSArray GetSFSArray(int index);

		ISFSObject GetSFSObject(int index);
	}
}
