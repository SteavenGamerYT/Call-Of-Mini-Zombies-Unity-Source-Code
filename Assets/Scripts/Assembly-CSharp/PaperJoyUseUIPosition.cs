using UnityEngine;

internal class PaperJoyUseUIPosition
{
	public Rect saveBK = new Rect(268f, 260f, 424f, 183f);

	public Rect Background = new Rect(0f, 0f, 960f, 640f);

	public Rect BackButton = new Rect(32f, 0f, 130f, 70f);

	public Rect[] ModelButton;

	public void SetModelButtonRect(int count)
	{
		ModelButton = new Rect[count];
		for (int i = 0; i < count; i++)
		{
			ModelButton[i] = new Rect(600f, 440 - 120 * i, 345f, 117f);
		}
	}
}
