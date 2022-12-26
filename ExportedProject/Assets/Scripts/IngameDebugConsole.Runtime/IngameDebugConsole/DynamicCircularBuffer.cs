using System;

namespace IngameDebugConsole
{
	public class DynamicCircularBuffer<T>
	{
		private T[] arr;

		private int startIndex;

		public int Count { get; private set; }

		public int Capacity
		{
			get
			{
				return arr.Length;
			}
		}

		public T this[int index]
		{
			get
			{
				return arr[(startIndex + index) % arr.Length];
			}
			set
			{
				arr[(startIndex + index) % arr.Length] = value;
			}
		}

		public DynamicCircularBuffer(int initialCapacity = 2)
		{
			arr = new T[initialCapacity];
		}

		public void Add(T value)
		{
			if (Count >= arr.Length)
			{
				int num = arr.Length;
				int num2 = ((num <= 0) ? 2 : (num * 2));
				Array.Resize(ref arr, num2);
				if (startIndex > 0)
				{
					if (startIndex <= (num - 1) / 2)
					{
						for (int i = 0; i < startIndex; i++)
						{
							arr[i + num] = arr[i];
						}
					}
					else
					{
						int num3 = num2 - num;
						for (int num4 = num - 1; num4 >= startIndex; num4--)
						{
							arr[num4 + num3] = arr[num4];
						}
						startIndex += num3;
					}
				}
			}
			this[Count++] = value;
		}

		public T RemoveFirst()
		{
			T result = arr[startIndex];
			if (++startIndex >= arr.Length)
			{
				startIndex = 0;
			}
			Count--;
			return result;
		}

		public T RemoveLast()
		{
			T result = arr[Count - 1];
			Count--;
			return result;
		}
	}
}
