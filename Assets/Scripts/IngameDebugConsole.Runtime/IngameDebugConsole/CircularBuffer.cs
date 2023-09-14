namespace IngameDebugConsole
{
	public class CircularBuffer<T>
	{
		private T[] arr;

		private int startIndex;

		public int Count { get; private set; }

		public T this[int index]
		{
			get
			{
				return arr[(startIndex + index) % arr.Length];
			}
		}

		public CircularBuffer(int capacity)
		{
			arr = new T[capacity];
		}

		public void Add(T value)
		{
			if (Count < arr.Length)
			{
				arr[Count++] = value;
				return;
			}
			arr[startIndex] = value;
			if (++startIndex >= arr.Length)
			{
				startIndex = 0;
			}
		}
	}
}
