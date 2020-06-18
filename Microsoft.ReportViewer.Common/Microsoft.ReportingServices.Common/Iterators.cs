using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.Common
{
	internal static class Iterators
	{
		public struct ReverseEnumerator<T> : IEnumerator<T>, IDisposable, IEnumerator, IEnumerable<T>, IEnumerable
		{
			private readonly IList<T> m_list;

			private int m_index;

			private T m_current;

			public T Current => m_current;

			object IEnumerator.Current => m_current;

			internal ReverseEnumerator(IList<T> list)
			{
				m_list = list;
				m_index = m_list.Count;
				m_current = default(T);
			}

			public void Reset()
			{
				m_index = m_list.Count;
				m_current = default(T);
			}

			public bool MoveNext()
			{
				if (m_index == 0)
				{
					return false;
				}
				m_index--;
				m_current = m_list[m_index];
				return true;
			}

			public ReverseEnumerator<T> GetEnumerator()
			{
				return new ReverseEnumerator<T>(m_list);
			}

			IEnumerator<T> IEnumerable<T>.GetEnumerator()
			{
				return GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return new ReverseEnumerator<T>(m_list);
			}

			void IDisposable.Dispose()
			{
			}
		}

		public static ReverseEnumerator<T> Reverse<T>(IList<T> list)
		{
			if (list == null)
			{
				throw new ArgumentNullException("list");
			}
			return new ReverseEnumerator<T>(list);
		}

		public static IEnumerable<T> FilterByType<T>(IEnumerable<T> items, Type returnType)
		{
			IList<T> list = items as IList<T>;
			if (list != null)
			{
				for (int i = 0; i < list.Count; i++)
				{
					if (returnType.IsInstanceOfType(list[i]))
					{
						yield return list[i];
					}
				}
				yield break;
			}
			foreach (T item in items)
			{
				if (returnType.IsInstanceOfType(item))
				{
					yield return item;
				}
			}
		}

		public static IEnumerable<T> Filter<T>(IEnumerable<T> items, Predicate<T> match)
		{
			IList<T> list = items as IList<T>;
			if (list != null)
			{
				for (int i = 0; i < list.Count; i++)
				{
					if (match(list[i]))
					{
						yield return list[i];
					}
				}
				yield break;
			}
			foreach (T item in items)
			{
				if (match(item))
				{
					yield return item;
				}
			}
		}
	}
}
