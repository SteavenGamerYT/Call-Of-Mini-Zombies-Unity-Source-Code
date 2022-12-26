using UnityEngine;

public class AutoRect
{
	public static Rect AutoPos(Rect rect)
	{
		ResetResolution();
		if (Application.platform == RuntimePlatform.Android || (Application.platform == RuntimePlatform.WindowsEditor && Screen.width == 800 && Screen.height == 480))
		{
			return new Rect(rect.x * ResolutionConstant.R, rect.y * ResolutionConstant.H, rect.width * ResolutionConstant.R, rect.height * ResolutionConstant.H);
		}
		if (Screen.width != 960 && Screen.width != 640 && Screen.width != 480 && Screen.width != 320)
		{
			if (Screen.width == 2048 || Screen.width == 1536)
			{
				Vector2 vector = new Vector2(960f, 640f);
				Vector2 vector2 = new Vector2(1024f, 768f);
				float x = vector2.x - vector.x + rect.x * 2f;
				float y = vector2.y - vector.y + rect.y * 2f;
				return new Rect(x, y, rect.width * ResolutionConstant.R, rect.height * ResolutionConstant.R);
			}
			Vector2 vector3 = new Vector2(480f, 320f);
			Vector2 vector4 = new Vector2(512f, 384f);
			float x2 = vector4.x + (rect.x - vector3.x) * 1f;
			float y2 = vector4.y + (rect.y - vector3.y) * 1f;
			return new Rect(x2, y2, rect.width * ResolutionConstant.R, rect.height * ResolutionConstant.R);
		}
		return new Rect(rect.x * ResolutionConstant.R, rect.y * ResolutionConstant.R, rect.width * ResolutionConstant.R, rect.height * ResolutionConstant.R);
	}

	public static Vector2 AutoPos(Vector2 v)
	{
		if (Screen.width != 960 && Screen.width != 640 && Screen.width != 480 && Screen.width != 320)
		{
			if (Screen.width == 2048 || Screen.width == 1536)
			{
				Vector2 vector = new Vector2(960f, 640f);
				Vector2 vector2 = new Vector2(1024f, 768f);
				float x = vector2.x - vector.x + v.x * 2f;
				float y = vector2.y - vector.y + v.y * 2f;
				return new Vector2(x, y);
			}
			Vector2 vector3 = new Vector2(480f, 320f);
			Vector2 vector4 = new Vector2(512f, 384f);
			float x2 = vector4.x + (v.x - vector3.x) * 1f;
			float y2 = vector4.y + (v.y - vector3.y) * 1f;
			return new Vector2(x2, y2);
		}
		ResetResolution();
		return new Vector2(v.x * ResolutionConstant.R, v.y * ResolutionConstant.R);
	}

	public static Rect AutoValuePos(Rect rect)
	{
		ResetResolution();
		return new Rect(rect.x * ResolutionConstant.R, rect.y * ResolutionConstant.R, rect.width * ResolutionConstant.R, rect.height * ResolutionConstant.R);
	}

	public static Vector2 AutoValuePos(Vector2 v)
	{
		ResetResolution();
		return new Vector2(v.x * ResolutionConstant.R, v.y * ResolutionConstant.R);
	}

	public static Vector2 AutoSize(Rect rect)
	{
		ResetResolution();
		if (Application.platform == RuntimePlatform.Android || (Application.platform == RuntimePlatform.WindowsEditor && Screen.width == 800 && Screen.height == 480))
		{
			return new Vector2(rect.width * ResolutionConstant.R, rect.height * ResolutionConstant.H);
		}
		if (ResolutionConstant.R == 1f)
		{
			return new Vector2(rect.width * ResolutionConstant.R, rect.height * ResolutionConstant.R);
		}
		if (ResolutionConstant.R == 2f)
		{
			return new Vector2(rect.width * ResolutionConstant.R, rect.height * ResolutionConstant.R);
		}
		return new Vector2(rect.width, rect.height);
	}

	public static Vector2 AutoSize(Rect rect, float zoom)
	{
		ResetResolution();
		if (ResolutionConstant.R == 1f || ResolutionConstant.R == 2f)
		{
			return new Vector2(rect.width * ResolutionConstant.R * zoom, rect.height * ResolutionConstant.R * zoom);
		}
		return new Vector2(rect.width * zoom, rect.height * zoom);
	}

	public static Vector2 AutoSize(Vector2 v)
	{
		ResetResolution();
		if (ResolutionConstant.R == 1f || ResolutionConstant.R == 2f)
		{
			return new Vector2(v.x * ResolutionConstant.R, v.y * ResolutionConstant.R);
		}
		return v;
	}

	public static Rect AutoTex(Rect rect)
	{
		ResetResolution();
		if (Application.platform == RuntimePlatform.Android || (Application.platform == RuntimePlatform.WindowsEditor && Screen.width == 800 && Screen.height == 480))
		{
			return new Rect(rect.x * 1f, rect.y * 1f, rect.width * 1f, rect.height * 1f);
		}
		if (ResolutionConstant.R == 2f)
		{
			return new Rect(rect.x * 1f, rect.y * 1f, rect.width * 1f, rect.height * 1f);
		}
		return new Rect(rect.x * ResolutionConstant.R, rect.y * ResolutionConstant.R, rect.width * ResolutionConstant.R, rect.height * ResolutionConstant.R);
	}

	public static Vector2 AutoTex(Vector2 v)
	{
		ResetResolution();
		if (ResolutionConstant.R == 2f)
		{
			return new Vector2(v.x * 1f, v.y * 1f);
		}
		return new Vector2(v.x * ResolutionConstant.R, v.y * ResolutionConstant.R);
	}

	public static float AutoX(float x)
	{
		ResetResolution();
		if (Application.platform == RuntimePlatform.Android || (Application.platform == RuntimePlatform.WindowsEditor && Screen.width == 800 && Screen.height == 480))
		{
			return x * ResolutionConstant.R;
		}
		if (Screen.width != 960 && Screen.width != 640 && Screen.width != 480 && Screen.width != 320)
		{
			if (Screen.width == 2048 || Screen.width == 1536)
			{
				float num = 960f;
				float num2 = 1024f;
				return num2 - num + x * 2f;
			}
			float num3 = 480f;
			float num4 = 512f;
			return num4 + (x - num3) * 1f;
		}
		return x * ResolutionConstant.R;
	}

	public static float AutoY(float y)
	{
		ResetResolution();
		if (Application.platform == RuntimePlatform.Android || (Application.platform == RuntimePlatform.WindowsEditor && Screen.width == 800 && Screen.height == 480))
		{
			return y * ResolutionConstant.H;
		}
		if (Screen.width != 960 && Screen.width != 640 && Screen.width != 480 && Screen.width != 320)
		{
			if (Screen.width == 2048 || Screen.width == 1536)
			{
				float num = 640f;
				float num2 = 768f;
				return num2 - num + y * 2f;
			}
			float num3 = 320f;
			float num4 = 384f;
			return num4 + (y - num3) * 1f;
		}
		return y * ResolutionConstant.R;
	}

	public static float AutoValue(float x)
	{
		return x * ResolutionConstant.R;
	}

	public static void ResetResolution()
	{
		if (Application.platform == RuntimePlatform.Android || (Application.platform == RuntimePlatform.WindowsEditor && Screen.width == 800 && Screen.height == 480))
		{
			ResolutionConstant.R = (float)Screen.width / 960f;
			ResolutionConstant.H = (float)Screen.height / 640f;
		}
		else if (Screen.width == 960 || Screen.width == 640)
		{
			ResolutionConstant.R = 1f;
		}
		else if (Screen.width == 480 || Screen.width == 320)
		{
			ResolutionConstant.R = 0.5f;
		}
		else if (Screen.width == 2048 || Screen.width == 1536)
		{
			ResolutionConstant.R = 2f;
		}
		else
		{
			ResolutionConstant.R = 1f;
		}
	}

	public static Platform GetPlatform()
	{
		if (Application.platform == RuntimePlatform.Android || (Application.platform == RuntimePlatform.WindowsEditor && Screen.width == 800 && Screen.height == 480))
		{
			return Platform.Android;
		}
		if (Screen.width == 960 || Screen.width == 640)
		{
			return Platform.iPhone4;
		}
		if (Screen.width == 480 || Screen.width == 320)
		{
			return Platform.iPhone3GS;
		}
		return Platform.IPad;
	}
}
