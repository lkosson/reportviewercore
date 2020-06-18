using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal sealed class ScalableHybridList<T> : IEnumerable<T>, IEnumerable, IDisposable
	{
		private sealed class HybridListEnumerator : IEnumerator<T>, IDisposable, IEnumerator
		{
			private ScalableHybridList<T> m_list;

			private int m_currentIndex = -1;

			private bool m_afterLast;

			private int m_version;

			public T Current => m_list[m_currentIndex];

			object IEnumerator.Current => Current;

			internal HybridListEnumerator(ScalableHybridList<T> list)
			{
				m_list = list;
				m_version = m_list.m_version;
			}

			public void Dispose()
			{
			}

			public bool MoveNext()
			{
				if (m_version != m_list.m_version)
				{
					Global.Tracer.Assert(condition: false, "Cannot continue enumeration, backing list was modified");
				}
				if (m_afterLast)
				{
					return false;
				}
				if (m_currentIndex == -1)
				{
					m_currentIndex = m_list.First;
				}
				else
				{
					m_currentIndex = m_list.Next(m_currentIndex);
				}
				if (m_currentIndex == -1)
				{
					m_afterLast = true;
				}
				return !m_afterLast;
			}

			public void Reset()
			{
				m_currentIndex = -1;
				m_afterLast = false;
			}
		}

		private int m_count;

		private ScalableList<ScalableHybridListEntry> m_entries;

		private int m_first = -1;

		private int m_last = -1;

		private int m_firstFree = -1;

		private int m_version;

		internal const int InvalidIndex = -1;

		internal int Count => m_count;

		internal T this[int index] => (T)GetAndCheckEntry(index).Item;

		internal int First => m_first;

		internal int Last => m_last;

		internal ScalableHybridList(int scalabilityPriority, IScalabilityCache cache, int segmentSize, int initialCapacity)
		{
			m_entries = new ScalableList<ScalableHybridListEntry>(scalabilityPriority, cache, segmentSize, initialCapacity);
		}

		internal int Add(T item)
		{
			int num = -1;
			if (m_firstFree != -1)
			{
				num = m_firstFree;
				ScalableHybridListEntry item2;
				using (m_entries.GetAndPin(m_firstFree, out item2))
				{
					m_firstFree = item2.Next;
					SetupLastNode(item2, item);
				}
			}
			else
			{
				num = m_entries.Count;
				ScalableHybridListEntry scalableHybridListEntry = new ScalableHybridListEntry();
				SetupLastNode(scalableHybridListEntry, item);
				m_entries.Add(scalableHybridListEntry);
			}
			if (m_count == 0)
			{
				m_first = num;
			}
			else
			{
				ScalableHybridListEntry item3;
				using (m_entries.GetAndPin(m_last, out item3))
				{
					item3.Next = num;
				}
			}
			m_last = num;
			m_count++;
			return num;
		}

		internal void Remove(int index)
		{
			ScalableHybridListEntry item;
			using (m_entries.GetAndPin(index, out item))
			{
				CheckNonFreeEntry(item, index);
				if (item.Previous == -1)
				{
					m_first = item.Next;
				}
				else
				{
					ScalableHybridListEntry item2;
					using (m_entries.GetAndPin(item.Previous, out item2))
					{
						item2.Next = item.Next;
					}
				}
				if (item.Next == -1)
				{
					m_last = item.Previous;
				}
				else
				{
					ScalableHybridListEntry item3;
					using (m_entries.GetAndPin(item.Next, out item3))
					{
						item3.Previous = item.Previous;
					}
				}
				item.Next = m_firstFree;
				m_firstFree = index;
				item.Item = null;
				item.Previous = -1;
				m_count--;
			}
		}

		internal int Next(int index)
		{
			return GetAndCheckEntry(index).Next;
		}

		internal int Previous(int index)
		{
			return GetAndCheckEntry(index).Previous;
		}

		public void Dispose()
		{
			Clear();
		}

		internal void Clear()
		{
			m_entries.Clear();
			m_count = 0;
			m_first = -1;
			m_last = -1;
			m_firstFree = -1;
		}

		public IEnumerator<T> GetEnumerator()
		{
			return new HybridListEnumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		private void SetupLastNode(ScalableHybridListEntry entry, T item)
		{
			entry.Item = item;
			entry.Next = -1;
			entry.Previous = m_last;
		}

		private ScalableHybridListEntry GetAndCheckEntry(int index)
		{
			ScalableHybridListEntry scalableHybridListEntry = m_entries[index];
			CheckNonFreeEntry(scalableHybridListEntry, index);
			return scalableHybridListEntry;
		}

		private void CheckNonFreeEntry(ScalableHybridListEntry entry, int index)
		{
			if (entry.Previous == -1 && index != m_first)
			{
				Global.Tracer.Assert(false, "Cannot use index: {0}. It points to a free item", index);
			}
		}
	}
}
