using UnityEngine;
using Zombie3D;

internal class ReviewDialogUI : MonoBehaviour, UIHandler, UIDialogEventHandler
{
	public UIManager m_UIManager;

	protected ReviewDialog reviewDialog;

	private void Start()
	{
		m_UIManager = base.gameObject.AddComponent<UIManager>();
		m_UIManager.SetParameter(9, 3, false);
		m_UIManager.SetUIHandler(this);
		m_UIManager.CLEAR = false;
		reviewDialog = new ReviewDialog();
		reviewDialog.SetDialogEventHandler(this);
		m_UIManager.Add(reviewDialog);
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

	public static ReviewDialogUI GetInstance()
	{
		return GameObject.Find("ReviewDialogUI").GetComponent<ReviewDialogUI>();
	}

	public bool IsVisible()
	{
		return reviewDialog.Visible;
	}

	public void ShowDialog()
	{
		reviewDialog.Show();
	}

	public void HideDialog()
	{
		reviewDialog.Hide();
	}

	public void HandleEvent(UIControl control, int command, float wparam, float lparam)
	{
	}

	public void Yes()
	{
		reviewDialog.Hide();
		GameApp.GetInstance().GetGameState().AddScore(1000);
		GameApp.GetInstance().Save();
		Application.OpenURL("http://www.trinitigame.com/callofminizombies/review/");
	}

	public void No()
	{
		reviewDialog.Hide();
	}
}
