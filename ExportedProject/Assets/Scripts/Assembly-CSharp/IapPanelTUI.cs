using UnityEngine;
using Zombie3D;

public class IapPanelTUI : MonoBehaviour
{
	public GameObject m_mask;

	public OnCashLabelFrash on_CashLabelFrash;

	protected IAPName iapProcessing = IAPName.None;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void IapBuy(IAPItem item)
	{
		IAP.NowPurchaseProduct(item.ID, "1");
		iapProcessing = item.Name;
		Debug.Log("IAP ID:" + item.ID);
		ShowMask();
	}

	public void GetPurchaseStatus()
	{
		if (iapProcessing == IAPName.None)
		{
			return;
		}
		int num = IAP.purchaseStatus(null);
		switch (num)
		{
		case 0:
			break;
		case 1:
			Debug.Log("statusCode:" + num);
			GameApp.GetInstance().GetGameState().DeliverIAPItem(iapProcessing);
			HideMask();
			iapProcessing = IAPName.None;
			if (on_CashLabelFrash != null)
			{
				on_CashLabelFrash();
			}
			GameApp.GetInstance().Save();
			break;
		default:
			Debug.Log("statusCode:" + num);
			HideMask();
			iapProcessing = IAPName.None;
			break;
		}
	}

	public void Show()
	{
		base.transform.localPosition = new Vector3(0f, 0f, base.transform.localPosition.z);
	}

	public void Hide()
	{
		base.transform.localPosition = new Vector3(0f, -2000f, base.transform.localPosition.z);
	}

	public void ShowMask()
	{
		m_mask.transform.localPosition = new Vector3(0f, 0f, base.transform.localPosition.z);
		bool flag = false;
		int num = 0;
		if (AutoRect.GetPlatform() == Platform.IPad)
		{
			flag = true;
			num = 0;
		}
		else
		{
			flag = false;
			num = 1;
		}
		Utils.ShowIndicatorSystem(num, flag, 1f, 1f, 1f, 1f);
	}

	public void HideMask()
	{
		m_mask.transform.localPosition = new Vector3(0f, -1000f, base.transform.localPosition.z);
		Utils.HideIndicatorSystem();
	}

	public void PurchaseFinished(string productId)
	{
		Debug.Log("Panel PurchaseFinished:" + productId);
		GameApp.GetInstance().GetGameState().DeliverIAPItem(iapProcessing);
		HideMask();
		iapProcessing = IAPName.None;
		if (on_CashLabelFrash != null)
		{
			on_CashLabelFrash();
		}
		GameApp.GetInstance().Save();
	}

	public void PurchaseCanceled(string productId)
	{
		Debug.Log("Panel PurchaseCanceled:" + productId);
		HideMask();
		iapProcessing = IAPName.None;
	}
}
