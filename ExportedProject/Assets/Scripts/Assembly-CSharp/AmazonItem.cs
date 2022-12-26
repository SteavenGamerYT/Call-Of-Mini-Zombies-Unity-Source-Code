using System;
using System.Collections;
using System.Collections.Generic;

public class AmazonItem
{
	public string description;

	public string type;

	public string price;

	public string sku;

	public string smallIconUrl;

	public string title;

	public AmazonItem(Hashtable ht)
	{
		description = ht["description"].ToString();
		type = ht["type"].ToString();
		price = ht["price"].ToString();
		sku = ht["sku"].ToString();
		smallIconUrl = ht["smallIconUrl"].ToString();
		title = ht["title"].ToString();
	}

	public static List<AmazonItem> fromArrayList(ArrayList array)
	{
		List<AmazonItem> list = new List<AmazonItem>();
		IEnumerator enumerator = array.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Hashtable ht = (Hashtable)enumerator.Current;
				list.Add(new AmazonItem(ht));
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
		return string.Format("<AmazonItem> type: {0}, sku: {1}, price: {2}, title: {3}, description: {4}", type, sku, price, title, description);
	}
}
