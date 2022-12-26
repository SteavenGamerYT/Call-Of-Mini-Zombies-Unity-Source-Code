using System;
using System.Text;

namespace IngameDebugConsole
{
	public struct DebugLogEntryTimestamp
	{
		public readonly DateTime dateTime;

		public readonly float elapsedSeconds;

		public readonly int frameCount;

		public DebugLogEntryTimestamp(DateTime dateTime, float elapsedSeconds, int frameCount)
		{
			this.dateTime = dateTime;
			this.elapsedSeconds = elapsedSeconds;
			this.frameCount = frameCount;
		}

		public void AppendTime(StringBuilder sb)
		{
			sb.Append("[");
			int hour = dateTime.Hour;
			if (hour >= 10)
			{
				sb.Append(hour);
			}
			else
			{
				sb.Append("0").Append(hour);
			}
			sb.Append(":");
			int minute = dateTime.Minute;
			if (minute >= 10)
			{
				sb.Append(minute);
			}
			else
			{
				sb.Append("0").Append(minute);
			}
			sb.Append(":");
			int second = dateTime.Second;
			if (second >= 10)
			{
				sb.Append(second);
			}
			else
			{
				sb.Append("0").Append(second);
			}
			sb.Append("]");
		}

		public void AppendFullTimestamp(StringBuilder sb)
		{
			AppendTime(sb);
			sb.Append("[").Append(elapsedSeconds.ToString("F1")).Append("s at ")
				.Append("#")
				.Append(frameCount)
				.Append("]");
		}
	}
}
