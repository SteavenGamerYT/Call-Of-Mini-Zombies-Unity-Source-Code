using UnityEngine;

public class TUITextField : TUIControlImpl
{
	public TextFieldType text_type;

	public Vector2 positon = Vector2.zero;

	public int length = 6;

	public GUIStyle style;

	protected string textToEdit = string.Empty;

	public Vector2 tex_off = Vector2.zero;

	public Vector2 content_off = Vector2.zero;

	public OnTextFieldActive callback;

	public new void Start()
	{
		base.Start();
		size = AutoSize(size);
		positon = AutoSize(positon);
		if (ResetResolution() == 1)
		{
			style.font = Resources.Load("font/CAI_18") as UnityEngine.Font;
		}
		else if (ResetResolution() == 2)
		{
			style.font = Resources.Load("font/CAI_36") as UnityEngine.Font;
		}
		else if (ResetResolution() == 4)
		{
			style.font = Resources.Load("font/CAI_72") as UnityEngine.Font;
		}
	}

	public void OnEnable()
	{
	}

	public void OnDisable()
	{
	}

	private void OnGUI()
	{
		float x = (float)(Screen.width / 2) - size.x / 2f + base.transform.localPosition.x + positon.x;
		float y = (float)(Screen.height / 2) - size.y / 2f - base.transform.localPosition.y - positon.y;
		if (text_type == TextFieldType.text)
		{
			textToEdit = GUI.TextArea(new Rect(x, y, size.x, size.y), textToEdit, length, style);
		}
		else if (text_type == TextFieldType.password)
		{
			textToEdit = GUI.PasswordField(new Rect(x, y, size.x, size.y), textToEdit, "*"[0], length, style);
		}
		if (Event.current.type == EventType.KeyDown && Event.current.character == '\n')
		{
			Debug.Log("Sending login request");
		}
		if (textToEdit != null && callback != null)
		{
			callback();
		}
	}

	public Vector2 AutoSize(Vector2 size)
	{
		int num = ResetResolution();
		if (num == 2 || num == 4)
		{
			return new Vector2(size.x * (float)num, size.y * (float)num);
		}
		return new Vector2(size.x, size.y);
	}

	public int ResetResolution()
	{
		int num = 2;
		if (Screen.width == 960 || Screen.width == 640)
		{
			return 2;
		}
		if (Screen.width == 480 || Screen.width == 320)
		{
			return 1;
		}
		if (Screen.width == 2048 || Screen.width == 1536)
		{
			return 4;
		}
		return 2;
	}

	public string GetText()
	{
		return textToEdit;
	}

	public void ResetText()
	{
		textToEdit = string.Empty;
	}

	public void Show()
	{
		base.transform.localPosition = new Vector3(0f, 0f, base.transform.localPosition.z);
	}

	public void Hide()
	{
		base.transform.localPosition = new Vector3(0f, 5000f, base.transform.localPosition.z);
	}
}
