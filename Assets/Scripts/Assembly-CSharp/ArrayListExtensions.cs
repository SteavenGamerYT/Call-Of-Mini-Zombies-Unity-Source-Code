using System;
using System.Collections;
using System.Collections.Generic;

public static class ArrayListExtensions
{
	public static List<T> ToList<T>(this ArrayList arrayList)
	{
		List<T> list = new List<T>(arrayList.Count);
		IEnumerator enumerator = arrayList.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				T item = (T)enumerator.Current;
				list.Add(item);
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
}
