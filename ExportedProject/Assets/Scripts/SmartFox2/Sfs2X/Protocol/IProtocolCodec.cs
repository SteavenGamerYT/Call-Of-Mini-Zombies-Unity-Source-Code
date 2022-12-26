using Sfs2X.Bitswarm;
using Sfs2X.Entities.Data;
using Sfs2X.Util;

namespace Sfs2X.Protocol
{
	public interface IProtocolCodec
	{
		void OnPacketRead(ISFSObject packet);

		void OnPacketRead(ByteArray packet);

		void OnPacketWrite(IMessage message);
	}
}
