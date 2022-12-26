using System;
using System.Collections.Generic;
using System.Timers;

namespace Sfs2X.Util
{
	public class LagMonitor
	{
		private int lastReqTime;

		private List<int> valueQueue;

		private int interval;

		private int queueSize;

		private Timer pollTimer;

		private SmartFox sfs;

		public bool IsRunning
		{
			get
			{
				return pollTimer.Enabled;
			}
		}

		public int AveragePingTime
		{
			get
			{
				if (valueQueue.Count == 0)
				{
					return 0;
				}
				int num = 0;
				foreach (int item in valueQueue)
				{
					num += item;
				}
				return num / valueQueue.Count;
			}
		}

		public void Stop()
		{
			if (IsRunning)
			{
				pollTimer.Stop();
			}
		}

		public void Destroy()
		{
			Stop();
			pollTimer.Dispose();
			sfs = null;
		}

		public int OnPingPong()
		{
			int item = DateTime.Now.Millisecond - lastReqTime;
			if (valueQueue.Count >= queueSize)
			{
				valueQueue.RemoveAt(0);
			}
			valueQueue.Add(item);
			return AveragePingTime;
		}
	}
}
