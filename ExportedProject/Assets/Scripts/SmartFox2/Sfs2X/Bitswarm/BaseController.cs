using Sfs2X.Logging;

namespace Sfs2X.Bitswarm
{
	public abstract class BaseController : IController
	{
		protected int id = -1;

		protected SmartFox sfs;

		protected BitSwarmClient bitSwarm;

		protected Logger log;

		public BaseController(BitSwarmClient bitSwarm)
		{
			this.bitSwarm = bitSwarm;
			if (bitSwarm != null)
			{
				log = bitSwarm.Log;
				sfs = bitSwarm.Sfs;
			}
		}

		public abstract void HandleMessage(IMessage message);
	}
}
