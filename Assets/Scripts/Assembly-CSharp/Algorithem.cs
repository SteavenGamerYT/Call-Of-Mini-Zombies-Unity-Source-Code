using UnityEngine;

public class Algorithem<T>
{
	public static void RandomSort(T[] array)
	{
		for (int i = 0; i < array.Length; i++)
		{
			int num = Random.Range(i, array.Length);
			T val = array[num];
			array[num] = array[i];
			array[i] = val;
		}
	}
}
