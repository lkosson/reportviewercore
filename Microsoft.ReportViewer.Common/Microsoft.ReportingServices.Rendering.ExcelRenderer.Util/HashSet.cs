using System.Collections;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.Rendering.ExcelRenderer.Util
{
	internal sealed class HashSet<T> : ICollection<T>, IEnumerable<T>, IEnumerable
	{
		private Dictionary<T, T> mHashTable = new Dictionary<T, T>();

		public int Count => mHashTable.Count;

		public bool IsReadOnly => false;

		public void Add(T item)
		{
			if (!mHashTable.ContainsKey(item))
			{
				mHashTable.Add(item, item);
			}
		}

		public void Clear()
		{
			mHashTable.Clear();
		}

		public bool Contains(T item)
		{
			return mHashTable.ContainsKey(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			foreach (KeyValuePair<T, T> item in mHashTable)
			{
				if (arrayIndex >= mHashTable.Count || arrayIndex >= array.Length)
				{
					break;
				}
				array[arrayIndex] = item.Value;
				arrayIndex++;
			}
		}

		public bool Remove(T item)
		{
			return mHashTable.Remove(item);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return mHashTable.Values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)mHashTable.Values).GetEnumerator();
		}
	}
}
