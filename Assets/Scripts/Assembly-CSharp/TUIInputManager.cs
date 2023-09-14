using UnityEngine;

public class TUIInputManager
{
	private static int m_lastFrameCount = -1;

	public static TUIInput[] GetInput()
	{
		if (Time.frameCount != m_lastFrameCount)
		{
			TUIInputManagerWindows.UpdateInput();
		}
		m_lastFrameCount = Time.frameCount;
		return TUIInputManagerWindows.GetInput();
	}
}
