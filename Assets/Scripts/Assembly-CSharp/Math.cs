using UnityEngine;

public class Math
{
	public static float SignificantFigures(float f, int digitalNum)
	{
		string text = f.ToString("0.0000000000");
		int startIndex = Mathf.Max(text.IndexOf("."), digitalNum);
		text = text.Remove(startIndex);
		f = float.Parse(text);
		return f;
	}

	public static bool RandomRate(float rate)
	{
		int num = Random.Range(0, 100);
		if ((float)num < rate)
		{
			return true;
		}
		return false;
	}

	public static Rect AddRect(Rect rect1, Rect rect2)
	{
		return new Rect(rect1.x + rect2.x, rect1.y + rect2.y, rect1.width + rect2.width, rect1.height + rect2.height);
	}

	public static Rect AddRect(Rect rect1, Vector2 offset)
	{
		return new Rect(rect1.x + offset.x, rect1.y + offset.y, rect1.width, rect1.height);
	}
}
