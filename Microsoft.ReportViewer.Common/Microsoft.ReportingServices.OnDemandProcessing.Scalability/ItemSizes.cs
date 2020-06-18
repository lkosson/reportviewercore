using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal static class ItemSizes
	{
		public const int BoolSize = 1;

		public const int ByteSize = 1;

		public const int SByteSize = 1;

		public const int Int16Size = 2;

		public const int UInt16Size = 2;

		public const int Int32Size = 4;

		public const int UInt32Size = 4;

		public const int Int64Size = 8;

		public const int UInt64Size = 8;

		public const int CharSize = 2;

		public const int DoubleSize = 8;

		public const int SingleSize = 4;

		public const int DecimalSize = 16;

		public const int DateTimeSize = 8;

		public const int TimeSpanSize = 8;

		public const int DateTimeOffsetSize = 16;

		public const int GuidSize = 16;

		public static readonly int ReferenceSize = IntPtr.Size;

		public static readonly int NullableBoolSize = PointerAlign(2);

		public static readonly int NullableByteSize = PointerAlign(2);

		public static readonly int NullableSByteSize = PointerAlign(2);

		public static readonly int NullableInt16Size = PointerAlign(3);

		public static readonly int NullableUInt16Size = PointerAlign(3);

		public static readonly int NullableInt32Size = PointerAlign(5);

		public static readonly int NullableUInt32Size = PointerAlign(5);

		public static readonly int NullableInt64Size = PointerAlign(9);

		public static readonly int NullableUInt64Size = PointerAlign(9);

		public static readonly int NullableCharSize = PointerAlign(3);

		public static readonly int NullableDoubleSize = PointerAlign(9);

		public static readonly int NullableSingleSize = PointerAlign(5);

		public static readonly int NullableDecimalSize = PointerAlign(17);

		public static readonly int NullableDateTimeSize = PointerAlign(9);

		public static readonly int NullableGuidSize = PointerAlign(17);

		public static readonly int NullableTimeSpanSize = PointerAlign(9);

		public static readonly int GdiColorSize = 12 + ReferenceSize;

		public const int Int32EnumSize = 4;

		public const int NullableOverhead = 1;

		public const int ListOverhead = 24;

		public const int ArrayOverhead = 8;

		public const int HashtableOverhead = 56;

		public const int HashtableEntryOverhead = 4;

		public static readonly int ObjectOverhead = ReferenceSize * 2;

		public static readonly int NonNullIStorableOverhead = ReferenceSize + ObjectOverhead;

		public static int PointerAlign(int size)
		{
			int num = size % ReferenceSize;
			if (num > 0)
			{
				return size - num + ReferenceSize;
			}
			return size;
		}

		public static int SizeOf(IStorable obj)
		{
			int num = ReferenceSize;
			if (obj != null)
			{
				num += ObjectOverhead + obj.Size;
			}
			return num;
		}

		public static int SizeOf<T>(ScalableList<T> obj)
		{
			return SizeOf((IStorable)obj);
		}

		public static int SizeOf<T>(List<T> list) where T : IStorable
		{
			int num = ReferenceSize;
			if (list != null)
			{
				num += 24;
				for (int i = 0; i < list.Count; i++)
				{
					num += SizeOf(list[i]);
				}
			}
			return num;
		}

		public static int SizeOfEmptyObjectArray(int length)
		{
			return ReferenceSize + 8 + ReferenceSize * length;
		}

		public static int SizeOf<T>(List<List<T>> listOfLists) where T : IStorable
		{
			int num = ReferenceSize;
			if (listOfLists != null)
			{
				num += 24;
				for (int i = 0; i < listOfLists.Count; i++)
				{
					num += SizeOf(listOfLists[i]);
				}
			}
			return num;
		}

		public static int SizeOf(List<object> list)
		{
			int num = ReferenceSize;
			if (list != null)
			{
				num += 24;
				for (int i = 0; i < list.Count; i++)
				{
					num += SizeOf(list[i]);
				}
			}
			return num;
		}

		public static int SizeOf<T>(T[] array) where T : IStorable
		{
			int num = ReferenceSize;
			if (array != null)
			{
				num += 8;
				for (int i = 0; i < array.Length; i++)
				{
					num += SizeOf(array[i]);
				}
			}
			return num;
		}

		public static int SizeOf(object[] array)
		{
			int num = ReferenceSize;
			if (array != null)
			{
				num += 8;
				for (int i = 0; i < array.Length; i++)
				{
					num += SizeOf(array[i]);
				}
			}
			return num;
		}

		public static int SizeOf(int[] obj)
		{
			int num = ReferenceSize;
			if (obj != null)
			{
				num += 8;
				num += obj.Length * 4;
			}
			return num;
		}

		public static int SizeOf(long[] obj)
		{
			int num = ReferenceSize;
			if (obj != null)
			{
				num += 8;
				num += obj.Length * 8;
			}
			return num;
		}

		public static int SizeOf(double[] obj)
		{
			int num = ReferenceSize;
			if (obj != null)
			{
				num += 8;
				num += obj.Length * 8;
			}
			return num;
		}

		public static int SizeOf(bool[] obj)
		{
			int num = ReferenceSize;
			if (obj != null)
			{
				num += 8;
				num += obj.Length;
			}
			return num;
		}

		public static int SizeOf(string[] obj)
		{
			int num = ReferenceSize;
			if (obj != null)
			{
				num += 8;
				for (int i = 0; i < obj.Length; i++)
				{
					num += SizeOf(obj[i]);
				}
			}
			return num;
		}

		public static int SizeOf(Array arr)
		{
			int num = ReferenceSize;
			if (arr != null)
			{
				num += 8;
				int[] indices = new int[arr.Rank];
				num += TraverseArrayDim(arr, 0, indices);
			}
			return num;
		}

		private static int TraverseArrayDim(Array arr, int dim, int[] indices)
		{
			int num = 0;
			bool flag = arr.Rank == dim + 1;
			int length = arr.GetLength(dim);
			for (int i = 0; i < length; i++)
			{
				indices[dim] = i;
				num = ((!flag) ? (num + TraverseArrayDim(arr, dim + 1, indices)) : (num + SizeOf(arr.GetValue(indices))));
			}
			return num;
		}

		public static int SizeOf(List<string> obj)
		{
			int num = ReferenceSize;
			if (obj != null)
			{
				num += 24;
				for (int i = 0; i < obj.Count; i++)
				{
					num += SizeOf(obj[i]);
				}
			}
			return num;
		}

		public static int SizeOf(List<int> obj)
		{
			int num = ReferenceSize;
			if (obj != null)
			{
				num += 24;
				num += obj.Count * 4;
			}
			return num;
		}

		public static int SizeOf(List<object>[] obj)
		{
			int num = ReferenceSize;
			if (obj != null)
			{
				num += 8;
				for (int i = 0; i < obj.Length; i++)
				{
					num += SizeOf(obj[i]);
				}
			}
			return num;
		}

		public static int SizeOf<T>(List<T>[] obj) where T : IStorable
		{
			int num = ReferenceSize;
			if (obj != null)
			{
				num += 8;
				for (int i = 0; i < obj.Length; i++)
				{
					num += SizeOf(obj[i]);
				}
			}
			return num;
		}

		public static int SizeOf<T>(List<T[]> obj) where T : IStorable
		{
			int num = ReferenceSize;
			if (obj != null)
			{
				num += 24;
				for (int i = 0; i < obj.Count; i++)
				{
					num += SizeOf(obj[i]);
				}
			}
			return num;
		}

		public static int SizeOf(Hashtable obj)
		{
			int referenceSize = ReferenceSize;
			if (obj != null)
			{
				referenceSize += 56;
				{
					foreach (DictionaryEntry item in obj)
					{
						referenceSize += 4;
						referenceSize += SizeOf(item.Key);
						referenceSize += SizeOf(item.Value);
					}
					return referenceSize;
				}
			}
			return referenceSize;
		}

		public static int SizeOf<K, V>(Dictionary<K, V> obj)
		{
			int referenceSize = ReferenceSize;
			if (obj != null)
			{
				referenceSize += 56;
				{
					foreach (KeyValuePair<K, V> item in obj)
					{
						referenceSize += 4;
						referenceSize += SizeOf(item.Key);
						referenceSize += SizeOf(item.Value);
					}
					return referenceSize;
				}
			}
			return referenceSize;
		}

		public static int SizeOf(IList obj)
		{
			int num = ReferenceSize;
			if (obj != null)
			{
				num += 24;
				for (int i = 0; i < obj.Count; i++)
				{
					num += SizeOf(obj[i]);
				}
			}
			return num;
		}

		public static int SizeOf(string str)
		{
			int num = ReferenceSize;
			if (str != null)
			{
				num += ObjectOverhead + 4 + 4 + str.Length * 2;
			}
			return num;
		}

		public static int SizeOfInObjectArray(object obj)
		{
			return SizeOf(obj) - ReferenceSize;
		}

		public static int SizeOf(object obj)
		{
			if (obj == null)
			{
				return ReferenceSize;
			}
			if (obj is IStorable)
			{
				return SizeOf((IStorable)obj);
			}
			if (obj is IConvertible)
			{
				switch (((IConvertible)obj).GetTypeCode())
				{
				case TypeCode.Boolean:
				case TypeCode.Char:
				case TypeCode.SByte:
				case TypeCode.Byte:
				case TypeCode.Int16:
				case TypeCode.UInt16:
				case TypeCode.Int32:
				case TypeCode.UInt32:
				case TypeCode.Single:
					return ReferenceSize + ObjectOverhead + ReferenceSize;
				case TypeCode.Int64:
				case TypeCode.UInt64:
				case TypeCode.Double:
					return 8 + ObjectOverhead + ReferenceSize;
				case TypeCode.String:
					return SizeOf((string)obj);
				case TypeCode.Decimal:
					return 16 + ObjectOverhead + ReferenceSize;
				case TypeCode.DateTime:
					return 8 + ObjectOverhead + ReferenceSize;
				case TypeCode.Object:
					if (obj is TimeSpan)
					{
						return 8 + ObjectOverhead + ReferenceSize;
					}
					if (obj is DateTimeOffset)
					{
						return 16 + ObjectOverhead + ReferenceSize;
					}
					if (obj is Guid)
					{
						return 16 + ObjectOverhead + ReferenceSize;
					}
					if (obj is Color)
					{
						return GdiColorSize + ObjectOverhead + ReferenceSize;
					}
					return ReferenceSize;
				}
			}
			else
			{
				if (obj is Array)
				{
					return SizeOf((Array)obj);
				}
				if (obj is IList)
				{
					return SizeOf((IList)obj);
				}
				if (Nullable.GetUnderlyingType(obj.GetType()) != null)
				{
					if (obj is bool?)
					{
						return NullableBoolSize + ObjectOverhead + ReferenceSize;
					}
					if (obj is byte?)
					{
						return NullableByteSize + ObjectOverhead + ReferenceSize;
					}
					if (obj is sbyte?)
					{
						return NullableByteSize + ObjectOverhead + ReferenceSize;
					}
					if (obj is short?)
					{
						return NullableInt16Size + ObjectOverhead + ReferenceSize;
					}
					if (obj is int?)
					{
						return NullableInt32Size + ObjectOverhead + ReferenceSize;
					}
					if (obj is long?)
					{
						return NullableInt64Size + ObjectOverhead + ReferenceSize;
					}
					if (obj is ushort?)
					{
						return NullableInt16Size + ObjectOverhead + ReferenceSize;
					}
					if (obj is uint?)
					{
						return NullableInt32Size + ObjectOverhead + ReferenceSize;
					}
					if (obj is ulong?)
					{
						return NullableInt64Size + ObjectOverhead + ReferenceSize;
					}
					if (obj is char?)
					{
						return NullableCharSize + ObjectOverhead + ReferenceSize;
					}
					if (obj is double?)
					{
						return NullableDoubleSize + ObjectOverhead + ReferenceSize;
					}
					if (obj is float?)
					{
						return NullableSingleSize + ObjectOverhead + ReferenceSize;
					}
					if (obj is DateTime?)
					{
						return NullableDateTimeSize + ObjectOverhead + ReferenceSize;
					}
					if (obj is Guid?)
					{
						return NullableGuidSize + ObjectOverhead + ReferenceSize;
					}
					if (obj is TimeSpan?)
					{
						return NullableTimeSpanSize + ObjectOverhead + ReferenceSize;
					}
				}
			}
			return ReferenceSize;
		}
	}
}
