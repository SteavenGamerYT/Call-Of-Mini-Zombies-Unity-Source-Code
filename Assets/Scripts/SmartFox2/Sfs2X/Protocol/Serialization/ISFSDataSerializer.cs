using Sfs2X.Entities.Data;
using Sfs2X.Util;

namespace Sfs2X.Protocol.Serialization
{
	public interface ISFSDataSerializer
	{
		ByteArray Object2Binary(ISFSObject obj);

		ByteArray Array2Binary(ISFSArray array);

		ISFSObject Binary2Object(ByteArray data);
	}
}
