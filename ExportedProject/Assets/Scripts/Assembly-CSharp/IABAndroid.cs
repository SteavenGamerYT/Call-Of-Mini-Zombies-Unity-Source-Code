using UnityEngine;

public class IABAndroid
{
	private static AndroidJavaObject _plugin;

	static IABAndroid()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return;
		}
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.prime31.IABPlugin"))
		{
			_plugin = androidJavaClass.CallStatic<AndroidJavaObject>("instance", new object[0]);
		}
	}

	public static void init(string publicKey)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_plugin.Call("init", publicKey);
		}
	}

	public static void startCheckBillingAvailableRequest()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_plugin.Call("startCheckBillingAvailableRequest");
		}
	}

	public static void restoreTransactions()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_plugin.Call("restoreTransactions");
		}
	}

	public static void purchaseProduct(string productId)
	{
		Debug.Log("purchaseProduct:" + productId);
		if (Application.platform == RuntimePlatform.Android)
		{
			_plugin.Call("purchaseProduct", productId);
		}
	}

	public static void testPurchaseProduct()
	{
		purchaseProduct("android.test.purchased");
	}

	public static void testRefundedProduct()
	{
		purchaseProduct("android.test.refunded");
	}

	public static void stopBillingService()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_plugin.Call("stopService");
		}
	}
}
