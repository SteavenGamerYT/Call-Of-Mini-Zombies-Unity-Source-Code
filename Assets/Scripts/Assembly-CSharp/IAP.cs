using System.Runtime.InteropServices;
using UnityEngine;

public class IAP
{
	[DllImport("__Internal")]
	protected static extern void PurchaseProduct(string productId, string productCount);

	public static void ToPurchaseProduct(string productId, string productCount)
	{
		Debug.Log("ToPurchaseProduct");
		PurchaseProduct(productId, productCount);
	}

	public static void NowPurchaseProduct(string productId, string productCount)
	{
		if (Utils.IsIAPCrack())
		{
			Debug.Log("IsIAPCrack!!!!!!");
			return;
		}
		Debug.Log("NowPurchaseProduct");
		Debug.Log("NowPurchaseProduct android");
		if (AmazonIAPManager.is_itemDataRequestFinished)
		{
			AmazonIAP.initiatePurchaseRequest(productId);
		}
		else
		{
			Debug.LogError("itemDataRequest not Finished!");
		}
	}

	public static int purchaseStatus(object stateInfo)
	{
		return GetPurchaseStatus();
	}

	[DllImport("__Internal")]
	protected static extern int PurchaseStatus();

	public static int OnPurchaseStatus()
	{
		return PurchaseStatus();
	}

	public static int GetPurchaseStatus()
	{
		return 1;
	}
}
