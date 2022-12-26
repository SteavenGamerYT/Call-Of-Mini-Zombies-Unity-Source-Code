using System.Net;

namespace Sfs2X.Core.Sockets
{
	public interface ISocketLayer
	{
		bool IsConnected { get; }

		ConnectionDelegate OnConnect { get; set; }

		ConnectionDelegate OnDisconnect { get; set; }

		OnDataDelegate OnData { get; set; }

		OnErrorDelegate OnError { get; set; }

		void Connect(IPAddress adr, int port);

		void Disconnect();

		void Write(byte[] data);
	}
}
