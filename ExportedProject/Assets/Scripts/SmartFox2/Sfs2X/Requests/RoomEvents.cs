namespace Sfs2X.Requests
{
	public class RoomEvents
	{
		private bool allowUserEnter;

		private bool allowUserExit;

		private bool allowUserCountChange;

		private bool allowUserVariablesUpdate;

		public bool AllowUserEnter
		{
			get
			{
				return allowUserEnter;
			}
		}

		public bool AllowUserExit
		{
			get
			{
				return allowUserExit;
			}
		}

		public bool AllowUserCountChange
		{
			get
			{
				return allowUserCountChange;
			}
		}

		public bool AllowUserVariablesUpdate
		{
			get
			{
				return allowUserVariablesUpdate;
			}
		}
	}
}
