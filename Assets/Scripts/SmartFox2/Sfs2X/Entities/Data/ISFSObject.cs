using Sfs2X.Util;

namespace Sfs2X.Entities.Data
{
	public interface ISFSObject
	{
		bool IsNull(string key);

		bool ContainsKey(string key);

		string[] GetKeys();

		int Size();

		ByteArray ToBinary();

		string GetDump(bool format);

		string GetDump();

		string GetHexDump();

		SFSDataWrapper GetData(string key);

		bool GetBool(string key);

		byte GetByte(string key);

		short GetShort(string key);

		int GetInt(string key);

		long GetLong(string key);

		float GetFloat(string key);

		double GetDouble(string key);

		string GetUtfString(string key);

		string[] GetUtfStringArray(string key);

		ISFSArray GetSFSArray(string key);

		ISFSObject GetSFSObject(string key);

		void PutBool(string key, bool val);

		void PutByte(string key, byte val);

		void PutShort(string key, short val);

		void PutInt(string key, int val);

		void PutLong(string key, long val);

		void PutFloat(string key, float val);

		void PutDouble(string key, double val);

		void PutUtfString(string key, string val);

		void PutBoolArray(string key, bool[] val);

		void PutIntArray(string key, int[] val);

		void PutSFSArray(string key, ISFSArray val);

		void PutSFSObject(string key, ISFSObject val);

		void Put(string key, SFSDataWrapper val);
	}
}
