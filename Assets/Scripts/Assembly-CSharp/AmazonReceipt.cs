using System;
using System.Collections;
using System.Collections.Generic;

public class AmazonReceipt
{
	public string type;

	public string token;

	public string sku;

	public string subscriptionStartDate;

	public string subscriptionEndDate;

	public AmazonReceipt(Hashtable ht)
	{
		type = ht["type"].ToString();
		token = ht["token"].ToString();
		sku = ht["sku"].ToString();
		if (ht.ContainsKey("subscriptionStartDate"))
		{
			subscriptionStartDate = ht["subscriptionStartDate"].ToString();
		}
		if (ht.ContainsKey("subscriptionEndDate"))
		{
			subscriptionStartDate = ht["subscriptionEndDate"].ToString();
		}
	}

	public static List<AmazonReceipt> fromArrayList(ArrayList array)
	{
		List<AmazonReceipt> list = new List<AmazonReceipt>();
		IEnumerator enumerator = array.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Hashtable ht = (Hashtable)enumerator.Current;
				list.Add(new AmazonReceipt(ht));
			}
			return list;
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = enumerator as IDisposable) != null)
			{
				disposable.Dispose();
			}
		}
	}

	public override string ToString()
	{
		return string.Format("<AmazonReceipt> type: {0}, token: {1}, sku: {2}", type, token, sku);
	}
}
