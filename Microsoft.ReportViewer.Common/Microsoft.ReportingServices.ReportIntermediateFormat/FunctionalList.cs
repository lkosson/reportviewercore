using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	internal sealed class FunctionalList<T> : IEnumerable<T>, IEnumerable
	{
		private class FunctionalListEnumerator : IEnumerator<T>, IDisposable, IEnumerator
		{
			private FunctionalList<T> m_list;

			private FunctionalList<T> m_rest;

			private T m_item;

			public T Current
			{
				get
				{
					if (m_rest == null)
					{
						throw new InvalidOperationException("MoveNext must be called before calling Current");
					}
					return m_item;
				}
			}

			object IEnumerator.Current => Current;

			public FunctionalListEnumerator(FunctionalList<T> aList)
			{
				m_list = aList;
			}

			public void Dispose()
			{
			}

			public bool MoveNext()
			{
				if (m_rest == null)
				{
					m_rest = m_list;
				}
				if (m_rest.Count > 0)
				{
					m_item = m_rest.First;
					m_rest = m_rest.Rest;
					return true;
				}
				return false;
			}

			public void Reset()
			{
				m_rest = null;
			}
		}

		private T m_car;

		private FunctionalList<T> m_cdr;

		private int m_size;

		private static FunctionalList<T> m_emptyList = new FunctionalList<T>();

		internal T First => m_car;

		internal FunctionalList<T> Rest => m_cdr;

		internal int Count => m_size;

		internal static FunctionalList<T> Empty => m_emptyList;

		private FunctionalList()
		{
		}

		private FunctionalList(T aItem, FunctionalList<T> aCdr)
		{
			m_car = aItem;
			m_cdr = aCdr;
			m_size = aCdr.Count + 1;
		}

		internal FunctionalList<T> Add(T aItem)
		{
			return new FunctionalList<T>(aItem, this);
		}

		internal bool IsEmpty()
		{
			return m_size == 0;
		}

		internal int IndexOf(T aItem)
		{
			if (Count == 0)
			{
				return -1;
			}
			if (object.Equals(First, aItem))
			{
				return m_size - 1;
			}
			return Rest.IndexOf(aItem);
		}

		internal bool Contains(T aItem)
		{
			return IndexOf(aItem) != -1;
		}

		internal T Get(T aItem)
		{
			if (Count == 0)
			{
				return default(T);
			}
			if (object.Equals(First, aItem))
			{
				return First;
			}
			return Rest.Get(aItem);
		}

		internal FunctionalList<T> Reverse()
		{
			FunctionalList<T> functionalList = Empty;
			using (IEnumerator<T> enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					T current = enumerator.Current;
					functionalList = functionalList.Add(current);
				}
				return functionalList;
			}
		}

		public IEnumerator<T> GetEnumerator()
		{
			return new FunctionalListEnumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
