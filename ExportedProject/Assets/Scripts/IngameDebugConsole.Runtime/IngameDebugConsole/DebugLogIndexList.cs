using System;

namespace IngameDebugConsole
{
	public class DebugLogIndexList<T>
	{
		private T[] indices;

		private int size;

		public int Count
		{
			get
			{
				return size;
			}
		}

		public T this[int index]
		{
			get
			{
				return indices[index];
			}
			set
			{
				indices[index] = value;
			}
		}

		public DebugLogIndexList()
		{
			indices = new T[64];
			size = 0;
		}

		public void Add(T value)
		{
			if (size == indices.Length)
			{
				Array.Resize(ref indices, size * 2);
			}
			indices[size++] = value;
		}

		public void Clear()
		{
			size = 0;
		}

		public int IndexOf(T value)
		{
			return Array.IndexOf(indices, value);
		}
	}
}
