using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportProcessing.ExprHostObjectModel
{
	[CLSCompliant(false)]
	public sealed class RemoteArrayWrapper<T> : MarshalByRefObject, IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
	{
		private readonly T[] m_array;

		public T this[int index]
		{
			get
			{
				return m_array[index];
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public int Count => m_array.Length;

		public bool IsReadOnly => true;

		public RemoteArrayWrapper(params T[] array)
		{
			m_array = array;
		}

		public int IndexOf(T item)
		{
			throw new NotSupportedException();
		}

		public void Insert(int index, T item)
		{
			throw new NotSupportedException();
		}

		public void RemoveAt(int index)
		{
			throw new NotSupportedException();
		}

		public void Add(T item)
		{
			throw new NotSupportedException();
		}

		public void Clear()
		{
			throw new NotSupportedException();
		}

		public bool Contains(T item)
		{
			throw new NotSupportedException();
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			m_array.CopyTo(array, arrayIndex);
		}

		public bool Remove(T item)
		{
			throw new NotSupportedException();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			int i = 0;
			while (i < m_array.Length)
			{
				yield return m_array[i];
				int num = i + 1;
				i = num;
			}
		}

		public IEnumerator<T> GetEnumerator()
		{
			int i = 0;
			while (i < m_array.Length)
			{
				yield return m_array[i];
				int num = i + 1;
				i = num;
			}
		}
	}
}
