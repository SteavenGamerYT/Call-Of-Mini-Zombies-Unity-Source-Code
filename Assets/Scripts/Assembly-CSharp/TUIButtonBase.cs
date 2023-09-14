using UnityEngine;

public class TUIButtonBase : TUIControlImpl
{
	public GameObject frameNormal;

	public GameObject framePressed;

	public GameObject frameDisabled;

	public bool disabled;

	public bool pressed;

	protected int fingerId = -1;

	public new void Start()
	{
		base.Start();
		UpdateFrame();
	}

	public void OnEnable()
	{
		UpdateFrame();
	}

	public void OnDisable()
	{
		HideFrame();
	}

	public void SetDisabled(bool disabled)
	{
		this.disabled = disabled;
		UpdateFrame();
	}

	protected virtual void UpdateFrame()
	{
		HideFrame();
		ShowFrame();
	}

	protected virtual void HideFrame()
	{
		if ((bool)frameNormal)
		{
			frameNormal.active = false;
		}
		if ((bool)framePressed)
		{
			framePressed.active = false;
		}
		if ((bool)frameDisabled)
		{
			frameDisabled.active = false;
		}
	}

	protected virtual void ShowFrame()
	{
		if (disabled)
		{
			if ((bool)frameDisabled)
			{
				frameDisabled.active = true;
			}
		}
		else if (pressed)
		{
			if ((bool)framePressed)
			{
				framePressed.active = true;
			}
		}
		else if ((bool)frameNormal)
		{
			frameNormal.active = true;
		}
	}
}
