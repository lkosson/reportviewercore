using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal sealed class LinkedBucketedQueue<T> : IEnumerable<T>, IEnumerable where T : class
	{
		private class QueueBucket
		{
			internal T[] Array;

			internal QueueBucket PreviousBucket;

			internal QueueBucket NextBucket;

			internal int Count;

			internal QueueBucket(int size)
			{
				Array = new T[size];
				PreviousBucket = null;
				NextBucket = null;
				Count = 0;
			}
		}

		private class QueueEnumerator : IDecumulator<T>, IEnumerator<T>, IDisposable, IEnumerator
		{
			private LinkedBucketedQueue<T> m_queue;

			private QueueBucket m_currentBucket;

			private int m_currentIndex;

			public T Current => m_currentBucket.Array[m_currentIndex];

			object IEnumerator.Current => Current;

			internal QueueEnumerator(LinkedBucketedQueue<T> queue)
			{
				m_queue = queue;
				Reset();
			}

			public void RemoveCurrent()
			{
				m_currentBucket.Array[m_currentIndex] = null;
				m_currentBucket.Count--;
				m_queue.m_count--;
				if (m_currentBucket.Count != 0)
				{
					return;
				}
				if (m_currentBucket == m_queue.m_firstBucket)
				{
					m_queue.RemoveFirstBucket();
					m_currentBucket = m_queue.m_firstBucket;
					m_currentIndex = -1;
				}
				else if (m_currentBucket == m_queue.m_lastBucket)
				{
					if (m_currentBucket.PreviousBucket == null)
					{
						m_queue.m_firstBucket = null;
						m_queue.m_lastBucket = null;
						m_queue.m_count = 0;
					}
					else
					{
						m_queue.m_lastBucket = m_currentBucket.PreviousBucket;
						m_queue.m_lastBucket.NextBucket = null;
						m_queue.m_insertIndex = m_queue.m_bucketSize;
						m_currentBucket.PreviousBucket = null;
					}
				}
				else
				{
					m_currentBucket.NextBucket.PreviousBucket = m_currentBucket.PreviousBucket;
					m_currentBucket.PreviousBucket.NextBucket = m_currentBucket.NextBucket;
					QueueBucket currentBucket = m_currentBucket;
					m_currentBucket = m_currentBucket.NextBucket;
					m_currentIndex = -1;
					currentBucket.NextBucket = null;
					currentBucket.PreviousBucket = null;
				}
			}

			public void Dispose()
			{
			}

			public bool MoveNext()
			{
				if (m_currentBucket == null)
				{
					m_currentBucket = m_queue.m_firstBucket;
					m_currentIndex = -1;
				}
				do
				{
					m_currentIndex++;
					if (m_currentBucket != null && m_currentIndex == m_queue.m_bucketSize)
					{
						m_currentBucket = m_currentBucket.NextBucket;
						m_currentIndex = 0;
					}
				}
				while (m_currentBucket != null && m_currentBucket.Array[m_currentIndex] == null);
				return m_currentBucket != null;
			}

			public void Reset()
			{
				m_currentBucket = null;
				m_currentIndex = -1;
			}
		}

		private QueueBucket m_firstBucket;

		private QueueBucket m_lastBucket;

		private int m_count;

		private int m_bucketSize = 20;

		private int m_insertIndex;

		private int m_removeIndex;

		internal int Count => m_count;

		internal LinkedBucketedQueue(int bucketSize)
		{
			m_count = 0;
			m_bucketSize = bucketSize;
		}

		internal void Enqueue(T item)
		{
			if (m_firstBucket == null)
			{
				m_firstBucket = new QueueBucket(m_bucketSize);
				m_lastBucket = m_firstBucket;
				m_firstBucket.NextBucket = null;
				m_firstBucket.PreviousBucket = null;
				m_insertIndex = 0;
			}
			if (m_insertIndex == m_bucketSize)
			{
				QueueBucket lastBucket = m_lastBucket;
				m_lastBucket = new QueueBucket(m_bucketSize);
				m_lastBucket.NextBucket = null;
				m_lastBucket.PreviousBucket = lastBucket;
				lastBucket.NextBucket = m_lastBucket;
				m_insertIndex = 0;
			}
			m_lastBucket.Array[m_insertIndex] = item;
			m_lastBucket.Count++;
			m_insertIndex++;
			m_count++;
		}

		internal T Dequeue()
		{
			T val = null;
			while (val == null && m_count > 0)
			{
				val = m_firstBucket.Array[m_removeIndex];
				m_firstBucket.Array[m_removeIndex] = null;
				m_removeIndex++;
				if (val != null)
				{
					m_firstBucket.Count--;
					m_count--;
				}
				if (m_firstBucket.Count == 0)
				{
					RemoveFirstBucket();
				}
			}
			return val;
		}

		private void RemoveFirstBucket()
		{
			m_firstBucket = m_firstBucket.NextBucket;
			m_removeIndex = 0;
			if (m_firstBucket == null)
			{
				m_lastBucket = null;
			}
			else
			{
				m_firstBucket.PreviousBucket = null;
			}
		}

		internal void Clear()
		{
			m_count = 0;
			m_firstBucket = null;
			m_lastBucket = null;
		}

		public IDecumulator<T> GetDecumulator()
		{
			return new QueueEnumerator(this);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return GetDecumulator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetDecumulator();
		}
	}
}
