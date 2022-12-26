using UnityEngine;

public class AmazonIAPUIManager : MonoBehaviour
{
	private void OnGUI()
	{
		float num = 5f;
		float x = 5f;
		float width = ((Screen.width >= 800 || Screen.height >= 800) ? 320 : 160);
		float num2 = ((Screen.width >= 800 || Screen.height >= 800) ? 80 : 40);
		float num3 = num2 + 10f;
		if (GUI.Button(new Rect(x, num, width, num2), "Initiate Item Data Request"))
		{
			AmazonIAP.initiateItemDataRequest(new string[3] { "coinpack.tier.2", "coinpack.tier.5", "coinpack.tier.10" });
		}
		if (GUI.Button(new Rect(x, num += num3, width, num2), "Initiate Purchase Request"))
		{
			AmazonIAP.initiatePurchaseRequest("coinpack.tier.2");
		}
		if (GUI.Button(new Rect(x, num += num3, width, num2), "Initiate Get User ID Request"))
		{
			AmazonIAP.initiateGetUserIdRequest();
			Application.LoadLevel(5);
		}
	}
}
