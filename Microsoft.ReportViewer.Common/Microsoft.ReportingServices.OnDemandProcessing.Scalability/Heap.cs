using Microsoft.ReportingServices.ReportProcessing;
using System;

namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal sealed class Heap<K, V> where K : IComparable<K>
	{
		private struct HeapEntry : IComparable<HeapEntry>
		{
			private K m_key;

			private V m_value;

			private int m_insertIndex;

			public K Key => m_key;

			public V Value => m_value;

			public HeapEntry(K key, V value, int insertIndex)
			{
				m_key = key;
				m_value = value;
				m_insertIndex = insertIndex;
			}

			public int CompareTo(HeapEntry other)
			{
				int num = m_key.CompareTo(other.m_key);
				if (num == 0)
				{
					num = m_insertIndex - other.m_insertIndex;
				}
				return num;
			}
		}

		private HeapEntry[] m_keys;

		private int m_count;

		private int m_insertIndex;

		private int m_maxCapacity;

		public int Count => m_count;

		public int Capacity => m_keys.Length;

		public Heap(int capacity)
			: this(capacity, -1)
		{
		}

		public Heap(int initialCapacity, int maxCapacity)
		{
			m_keys = new HeapEntry[initialCapacity];
			m_count = 0;
			m_insertIndex = 0;
			m_maxCapacity = maxCapacity;
		}

		public void Insert(K key, V value)
		{
			if (m_keys.Length == m_count)
			{
				if (m_count < m_maxCapacity || m_maxCapacity == -1)
				{
					int num = (int)((double)m_keys.Length * 1.5);
					if (m_maxCapacity > 0 && num > m_maxCapacity)
					{
						num = m_maxCapacity;
					}
					Array.Resize(ref m_keys, num);
				}
				else
				{
					Global.Tracer.Assert(condition: false, "Invalid Operation: Cannot add to heap at maximum capacity");
				}
			}
			int num2 = m_count;
			m_count++;
			m_keys[num2] = new HeapEntry(key, value, m_insertIndex);
			m_insertIndex++;
			int num3 = (num2 - 1) / 2;
			while (num2 > 0 && LessThan(num3, num2))
			{
				Swap(num3, num2);
				num2 = num3;
				num3 = (num2 - 1) / 2;
			}
		}

		public V ExtractMax()
		{
			V result = Peek();
			int num = m_count - 1;
			m_keys[0] = m_keys[num];
			m_count--;
			Heapify(0);
			if (m_maxCapacity > 0 && (double)m_count < 0.5 * (double)m_keys.Length && m_keys.Length > 10)
			{
				int num2 = (int)(0.6 * (double)m_keys.Length);
				if (num2 < m_count)
				{
					num2 = m_count;
				}
				if (num2 < 10)
				{
					num2 = 10;
				}
				Array.Resize(ref m_keys, num2);
			}
			return result;
		}

		public V Peek()
		{
			if (m_count == 0)
			{
				Global.Tracer.Assert(condition: false, "Cannot Peek from empty heap");
			}
			return m_keys[0].Value;
		}

		private void Heapify(int startIndex)
		{
			int num = 2 * startIndex + 1;
			int num2 = num + 1;
			int num3 = -1;
			num3 = ((num >= m_count || !GreaterThan(num, startIndex)) ? startIndex : num);
			if (num2 < m_count && GreaterThan(num2, num3))
			{
				num3 = num2;
			}
			if (num3 != startIndex)
			{
				Swap(num3, startIndex);
				Heapify(num3);
			}
		}

		private bool GreaterThan(int index1, int index2)
		{
			return m_keys[index1].CompareTo(m_keys[index2]) > 0;
		}

		private bool LessThan(int index1, int index2)
		{
			return m_keys[index1].CompareTo(m_keys[index2]) < 0;
		}

		private void Swap(int index1, int index2)
		{
			HeapEntry heapEntry = m_keys[index1];
			m_keys[index1] = m_keys[index2];
			m_keys[index2] = heapEntry;
		}
	}
}
