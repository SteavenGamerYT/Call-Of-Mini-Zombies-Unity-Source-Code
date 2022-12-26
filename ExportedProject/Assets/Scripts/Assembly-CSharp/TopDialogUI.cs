using UnityEngine;

internal class TopDialogUI : MonoBehaviour, UIHandler, UIDialogEventHandler
{
	public UIManager m_UIManager;

	protected IAPDialog iapDialog;

	private void Start()
	{
		m_UIManager = base.gameObject.AddComponent<UIManager>();
		m_UIManager.SetParameter(9, 3, false);
		m_UIManager.SetUIHandler(this);
		m_UIManager.CLEAR = false;
		iapDialog = new IAPDialog(UIDialog.DialogMode.YES_OR_NO);
		iapDialog.SetDialogEventHandler(this);
		iapDialog.SetText("\n\n\n  SHORT ON MONEY!");
		m_UIManager.Add(iapDialog);
		m_UIManager.SetUIHandler(this);
	}

	private void Update()
	{
		UITouchInner[] array = ((!Application.isMobilePlatform) ? WindowsInputMgr.MockTouches() : iPhoneInputMgr.MockTouches());
		UITouchInner[] array2 = array;
		foreach (UITouchInner touch in array2)
		{
			if (!(m_UIManager != null) || m_UIManager.HandleInput(touch))
			{
			}
		}
	}

	public static TopDialogUI GetInstance()
	{
		return GameObject.Find("TopDialogUI").GetComponent<TopDialogUI>();
	}

	public void ShowDialog()
	{
		iapDialog.Show();
	}

	public void HideDialog()
	{
		iapDialog.Hide();
	}

	public void HandleEvent(UIControl control, int command, float wparam, float lparam)
	{
	}

	public void Yes()
	{
		Debug.Log("yes");
		AvatarUI avatarUI = ArenaMenuUI.GetInstance().GetPanel(3) as AvatarUI;
		avatarUI.Hide();
		iapDialog.Hide();
		ShopUI shopUI = ArenaMenuUI.GetInstance().GetPanel(4) as ShopUI;
		shopUI.SetFromPanel(avatarUI);
		shopUI.Show();
	}

	public void No()
	{
		iapDialog.Hide();
	}
}
