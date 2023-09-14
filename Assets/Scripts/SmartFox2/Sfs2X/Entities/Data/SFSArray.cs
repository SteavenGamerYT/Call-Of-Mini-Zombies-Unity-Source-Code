using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Sfs2X.Protocol.Serialization;

namespace Sfs2X.Entities.Data
{
	public class SFSArray : ICollection, IEnumerable, ISFSArray
	{
		private ISFSDataSerializer serializer;

		private List<SFSDataWrapper> dataHolder;

		bool ICollection.IsSynchronized
		{
			get
			{
				return false;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				return this;
			}
		}

		int ICollection.Count
		{
			get
			{
				return dataHolder.Count;
			}
		}

		public SFSArray()
		{
			dataHolder = new List<SFSDataWrapper>();
			serializer = DefaultSFSDataSerializer.Instance;
		}

		void ICollection.CopyTo(Array toArray, int index)
		{
			foreach (SFSDataWrapper item in dataHolder)
			{
				toArray.SetValue(item, index);
				index++;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new SFSArrayEnumerator(dataHolder);
		}

		public static SFSArray NewInstance()
		{
			return new SFSArray();
		}

		public SFSDataWrapper GetWrappedElementAt(int index)
		{
			return dataHolder[index];
		}

		public object GetElementAt(int index)
		{
			object result = null;
			if (dataHolder[index] != null)
			{
				result = dataHolder[index].Data;
			}
			return result;
		}

		public int Size()
		{
			return dataHolder.Count;
		}

		public string GetDump(bool format)
		{
			if (!format)
			{
				return Dump();
			}
			return DefaultObjectDumpFormatter.PrettyPrintDump(Dump());
		}

		private string Dump()
		{
			StringBuilder stringBuilder = new StringBuilder(Convert.ToString(DefaultObjectDumpFormatter.TOKEN_INDENT_OPEN));
			for (int i = 0; i < dataHolder.Count; i++)
			{
				SFSDataWrapper sFSDataWrapper = dataHolder[i];
				int type = sFSDataWrapper.Type;
				string value;
				switch (type)
				{
				case 18:
					value = (sFSDataWrapper.Data as SFSObject).GetDump(false);
					break;
				case 17:
					value = (sFSDataWrapper.Data as SFSArray).GetDump(false);
					break;
				case 0:
					value = "NULL";
					break;
				case 9:
				case 10:
				case 11:
				case 12:
				case 13:
				case 14:
				case 15:
				case 16:
					value = string.Concat("[", sFSDataWrapper.Data, "]");
					break;
				default:
					value = sFSDataWrapper.Data.ToString();
					break;
				}
				stringBuilder.Append("(" + ((SFSDataType)type).ToString().ToLower() + ") ");
				stringBuilder.Append(value);
				stringBuilder.Append(Convert.ToString(DefaultObjectDumpFormatter.TOKEN_DIVIDER));
			}
			string text = stringBuilder.ToString();
			if (Size() > 0)
			{
				text = text.Substring(0, text.Length - 1);
			}
			return text + Convert.ToString(DefaultObjectDumpFormatter.TOKEN_INDENT_CLOSE);
		}

		public void AddNull()
		{
			AddObject(null, SFSDataType.NULL);
		}

		public void AddBool(bool val)
		{
			AddObject(val, SFSDataType.BOOL);
		}

		public void AddByte(byte val)
		{
			AddObject(val, SFSDataType.BYTE);
		}

		public void AddInt(int val)
		{
			AddObject(val, SFSDataType.INT);
		}

		public void AddDouble(double val)
		{
			AddObject(val, SFSDataType.DOUBLE);
		}

		public void AddUtfString(string val)
		{
			AddObject(val, SFSDataType.UTF_STRING);
		}

		public void AddSFSArray(ISFSArray val)
		{
			AddObject(val, SFSDataType.SFS_ARRAY);
		}

		public void AddSFSObject(ISFSObject val)
		{
			AddObject(val, SFSDataType.SFS_OBJECT);
		}

		public void Add(SFSDataWrapper wrappedObject)
		{
			dataHolder.Add(wrappedObject);
		}

		private void AddObject(object val, SFSDataType tp)
		{
			Add(new SFSDataWrapper((int)tp, val));
		}

		public T GetValue<T>(int index)
		{
			if (index >= dataHolder.Count)
			{
				return default(T);
			}
			SFSDataWrapper sFSDataWrapper = dataHolder[index];
			return (T)sFSDataWrapper.Data;
		}

		public bool GetBool(int index)
		{
			return GetValue<bool>(index);
		}

		public byte GetByte(int index)
		{
			return GetValue<byte>(index);
		}

		public short GetShort(int index)
		{
			return GetValue<short>(index);
		}

		public int GetInt(int index)
		{
			return GetValue<int>(index);
		}

		public string GetUtfString(int index)
		{
			return GetValue<string>(index);
		}

		public ISFSArray GetSFSArray(int index)
		{
			return GetValue<ISFSArray>(index);
		}

		public ISFSObject GetSFSObject(int index)
		{
			return GetValue<ISFSObject>(index);
		}
	}
}
