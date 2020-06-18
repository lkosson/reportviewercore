using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal sealed class LinkedLRUCache<T> where T : ItemHolder
	{
		private int m_count;

		private ItemHolder m_sentinal;

		public int Count => m_count;

		public LinkedLRUCache()
		{
			m_sentinal = new ItemHolder();
			Clear();
		}

		public void Add(ItemHolder item)
		{
			m_count++;
			item.Next = m_sentinal;
			item.Previous = m_sentinal.Previous;
			m_sentinal.Previous.Next = item;
			m_sentinal.Previous = item;
		}

		public void Bump(ItemHolder item)
		{
			item.Previous.Next = item.Next;
			item.Next.Previous = item.Previous;
			item.Next = m_sentinal;
			item.Previous = m_sentinal.Previous;
			m_sentinal.Previous.Next = item;
			m_sentinal.Previous = item;
		}

		public T ExtractLRU()
		{
			if (m_count == 0)
			{
				Global.Tracer.Assert(condition: false, "Cannot ExtractLRU from empty cache");
			}
			ItemHolder next = m_sentinal.Next;
			Remove(next);
			return (T)next;
		}

		public void Remove(ItemHolder item)
		{
			if (m_count == 0)
			{
				Global.Tracer.Assert(condition: false, "Cannot ExtractLRU from empty cache");
			}
			m_count--;
			item.Previous.Next = item.Next;
			item.Next.Previous = item.Previous;
			item.Next = null;
			item.Previous = null;
		}

		public T Peek()
		{
			if (m_count == 0)
			{
				return null;
			}
			return (T)m_sentinal.Next;
		}

		public void Clear()
		{
			m_count = 0;
			m_sentinal.Previous = m_sentinal;
			m_sentinal.Next = m_sentinal;
		}
	}
}
