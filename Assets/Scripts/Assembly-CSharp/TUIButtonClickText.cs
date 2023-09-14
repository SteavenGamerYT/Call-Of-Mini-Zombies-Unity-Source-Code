using UnityEngine;

public class TUIButtonClickText : TUIButtonClick
{
	public GameObject TextNormal;

	public GameObject TextPressed;

	public GameObject TextDisabled;

	public void ResetFrame()
	{
		if (pressed)
		{
			pressed = false;
			fingerId = -1;
			UpdateFrame();
		}
	}

	protected override void HideFrame()
	{
		base.HideFrame();
		HideText();
	}

	protected void HideText()
	{
		if ((bool)TextNormal)
		{
			TextNormal.active = false;
		}
		if ((bool)TextPressed)
		{
			TextPressed.active = false;
		}
		if ((bool)TextDisabled)
		{
			TextDisabled.active = false;
		}
	}

	protected override void ShowFrame()
	{
		base.ShowFrame();
		ShowText();
	}

	protected void ShowText()
	{
		if (disabled)
		{
			if ((bool)TextDisabled)
			{
				TextDisabled.active = true;
			}
		}
		else if (pressed)
		{
			if ((bool)TextPressed)
			{
				TextPressed.active = true;
			}
		}
		else if ((bool)TextNormal)
		{
			TextNormal.active = true;
		}
	}
}
