namespace Sfs2X.Util
{
	public class ConfigData
	{
		private string host;

		private int port;

		public string udpHost;

		public int udpPort;

		private string zone;

		private bool debug;

		private int httpPort;

		private bool useBlueBox;

		private int blueBoxPollingRate;

		public string Host
		{
			get
			{
				return host;
			}
		}

		public int Port
		{
			get
			{
				return port;
			}
		}

		public string UdpHost
		{
			get
			{
				return udpHost;
			}
		}

		public int UdpPort
		{
			get
			{
				return udpPort;
			}
		}

		public string Zone
		{
			get
			{
				return zone;
			}
		}

		public int HttpPort
		{
			get
			{
				return httpPort;
			}
		}
	}
}
