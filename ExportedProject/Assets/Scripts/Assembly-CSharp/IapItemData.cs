using UnityEngine;

public class IapItemData : MonoBehaviour
{
	public IAPItem iap_item = new IAPItem();

	private void Start()
	{
		SetIAPItem();
		base.transform.Find("Iap_content").GetComponent<TUIMeshText>().text = iap_item.Desc;
	}

	public void SetIAPItem()
	{
		switch (base.gameObject.name)
		{
		case "Iap_Buy_Button_1":
			iap_item.ID = "com.trinitigame.callofminizombies.099cents";
			iap_item.iType = IAPType.Cash;
			iap_item.Name = IAPName.Cash1D;
			iap_item.Desc = "$62,500\nMINI PACK";
			break;
		case "Iap_Buy_Button_2":
			iap_item.ID = "com.trinitigame.callofminizombies.499cents";
			iap_item.iType = IAPType.Cash;
			iap_item.Name = IAPName.Cash5D;
			iap_item.Desc = "$350,000\nMEGA PACK";
			break;
		case "Iap_Buy_Button_3":
			iap_item.ID = "com.trinitigame.callofminizombies.999cents";
			iap_item.iType = IAPType.Cash;
			iap_item.Name = IAPName.Cash10D;
			iap_item.Desc = "$760,000\nXL MEGA PACK";
			break;
		case "Iap_Buy_Button_4":
			iap_item.ID = "com.trinitigame.callofminizombies.1999cents";
			iap_item.iType = IAPType.Cash;
			iap_item.Name = IAPName.Cash20D;
			iap_item.Desc = "$1,660,000\nXXL MEGA PACK";
			break;
		case "Iap_Buy_Button_5":
			iap_item.ID = "com.trinitigame.callofminizombies.4999cents";
			iap_item.iType = IAPType.Cash;
			iap_item.Name = IAPName.Cash50D;
			iap_item.Desc = "$4,160,000\nUBER PACK";
			break;
		case "Iap_Buy_Button_6":
			iap_item.ID = "com.trinitigame.callofminizombies.9999cents";
			iap_item.iType = IAPType.Cash;
			iap_item.Name = IAPName.Cash100D;
			iap_item.Desc = "$9,160,000\nMONSTER PACK";
			break;
		}
	}
}
