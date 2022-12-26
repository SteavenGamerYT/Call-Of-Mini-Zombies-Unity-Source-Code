using System.Collections.Generic;

public class Shop
{
	protected List<IAPItem>[] itemList = new List<IAPItem>[3];

	public void CreateIAPShopData()
	{
		for (int i = 0; i < 3; i++)
		{
			itemList[i] = new List<IAPItem>();
		}
		IAPItem iAPItem = new IAPItem();
		iAPItem.ID = "com.trinitigame.callofminizombies.099cents";
		iAPItem.iType = IAPType.Cash;
		iAPItem.Name = IAPName.Cash1D;
		iAPItem.Desc = "$62,500\nMINI PACK";
		iAPItem.textureRect = ShopTexturePosition.CashLogo;
		AddIAPItem(iAPItem);
		IAPItem iAPItem2 = new IAPItem();
		iAPItem2.ID = "com.trinitigame.callofminizombies.499cents";
		iAPItem2.iType = IAPType.Cash;
		iAPItem2.Name = IAPName.Cash5D;
		iAPItem2.Desc = "$350,000\nMEGA PACK";
		iAPItem2.textureRect = ShopTexturePosition.CashLogo;
		AddIAPItem(iAPItem2);
		IAPItem iAPItem3 = new IAPItem();
		iAPItem3.ID = "com.trinitigame.callofminizombies.999cents";
		iAPItem3.iType = IAPType.Cash;
		iAPItem3.Name = IAPName.Cash10D;
		iAPItem3.Desc = "$760,000\nXL MEGA PACK";
		iAPItem3.textureRect = ShopTexturePosition.CashLogo;
		AddIAPItem(iAPItem3);
		IAPItem iAPItem4 = new IAPItem();
		iAPItem4.ID = "com.trinitigame.callofminizombies.1999cents";
		iAPItem4.iType = IAPType.Cash;
		iAPItem4.Name = IAPName.Cash20D;
		iAPItem4.Desc = "$1,660,000\nXXL MEGA PACK";
		iAPItem4.textureRect = ShopTexturePosition.CashLogo;
		AddIAPItem(iAPItem4);
		IAPItem iAPItem5 = new IAPItem();
		iAPItem5.ID = "com.trinitigame.callofminizombies.4999cents";
		iAPItem5.iType = IAPType.Cash;
		iAPItem5.Name = IAPName.Cash50D;
		iAPItem5.Desc = "$4,160,000\nUBER PACK";
		iAPItem5.textureRect = ShopTexturePosition.CashLogo;
		AddIAPItem(iAPItem5);
		IAPItem iAPItem6 = new IAPItem();
		iAPItem6.ID = "com.trinitigame.callofminizombies.9999cents";
		iAPItem6.iType = IAPType.Cash;
		iAPItem6.Name = IAPName.Cash100D;
		iAPItem6.Desc = "$9,160,000\nMONSTER PACK";
		iAPItem6.textureRect = ShopTexturePosition.CashLogo;
		AddIAPItem(iAPItem6);
	}

	public void AddIAPItem(IAPItem item)
	{
		itemList[0].Add(item);
	}

	public List<IAPItem>[] GetIAPList()
	{
		return itemList;
	}
}
