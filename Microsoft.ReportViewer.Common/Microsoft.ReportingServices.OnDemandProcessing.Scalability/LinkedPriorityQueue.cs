using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal sealed class LinkedPriorityQueue<T> where T : class
	{
		internal struct PriorityQueueDecumulator : IDecumulator<T>, IEnumerator<T>, IDisposable, IEnumerator
		{
			private IDecumulator<T> m_currentLevelDecumulator;

			private PriorityLevel m_currentLevel;

			private LinkedPriorityQueue<T> m_queue;

			private IEnumerator<PriorityLevel> m_enumerator;

			private List<int> m_pendingLevelRemovals;

			public T Current => m_currentLevelDecumulator.Current;

			object IEnumerator.Current => Current;

			internal PriorityQueueDecumulator(LinkedPriorityQueue<T> queue)
			{
				m_queue = queue;
				m_enumerator = m_queue.m_priorityLevels.Values.GetEnumerator();
				m_pendingLevelRemovals = new List<int>();
				m_currentLevelDecumulator = null;
				m_currentLevel = null;
			}

			public bool MoveNext()
			{
				if (m_currentLevel == null || (m_currentLevelDecumulator != null && !m_currentLevelDecumulator.MoveNext()))
				{
					if (!m_enumerator.MoveNext())
					{
						return false;
					}
					m_currentLevel = m_enumerator.Current;
					m_currentLevelDecumulator = m_currentLevel.Queue.GetDecumulator();
					return m_currentLevelDecumulator.MoveNext();
				}
				return true;
			}

			public void RemoveCurrent()
			{
				m_currentLevelDecumulator.RemoveCurrent();
				if (m_currentLevel.Queue.Count == 0)
				{
					m_pendingLevelRemovals.Add(m_currentLevel.Priority);
					m_currentLevel = null;
				}
			}

			public void Dispose()
			{
				m_enumerator.Dispose();
				m_enumerator = null;
				for (int i = 0; i < m_pendingLevelRemovals.Count; i++)
				{
					m_queue.m_priorityLevels.Remove(m_pendingLevelRemovals[i]);
				}
				m_queue.m_pendingDecumulatorCommit = false;
			}

			public void Reset()
			{
				Global.Tracer.Assert(condition: false, "Reset is not supported");
			}
		}

		internal class PriorityLevel
		{
			public LinkedBucketedQueue<T> Queue;

			public int Priority;
		}

		private SortedDictionary<int, PriorityLevel> m_priorityLevels;

		private bool m_pendingDecumulatorCommit;

		private const int QueueBucketSize = 100;

		internal int LevelCount
		{
			get
			{
				Global.Tracer.Assert(!m_pendingDecumulatorCommit, "Cannot perform operations on the queue until the open enumerator is Disposed");
				return m_priorityLevels.Count;
			}
		}

		internal LinkedPriorityQueue()
		{
			m_priorityLevels = new SortedDictionary<int, PriorityLevel>(EqualityComparers.ReversedInt32ComparerInstance);
		}

		internal void Enqueue(T item, int priority)
		{
			Global.Tracer.Assert(!m_pendingDecumulatorCommit, "Cannot perform operations on the queue until the open enumerator is Disposed");
			if (!m_priorityLevels.TryGetValue(priority, out PriorityLevel value))
			{
				value = new PriorityLevel();
				value.Priority = priority;
				value.Queue = new LinkedBucketedQueue<T>(100);
				m_priorityLevels[priority] = value;
			}
			value.Queue.Enqueue(item);
		}

		internal T Dequeue()
		{
			Global.Tracer.Assert(!m_pendingDecumulatorCommit, "Cannot perform operations on the queue until the open enumerator is Disposed");
			using (IDecumulator<T> decumulator = GetDecumulator())
			{
				decumulator.MoveNext();
				T current = decumulator.Current;
				decumulator.RemoveCurrent();
				return current;
			}
		}

		internal IDecumulator<T> GetDecumulator()
		{
			Global.Tracer.Assert(!m_pendingDecumulatorCommit, "Cannot perform operations on the queue until the open enumerator is Disposed");
			m_pendingDecumulatorCommit = true;
			return new PriorityQueueDecumulator(this);
		}

		internal void Clear()
		{
			m_priorityLevels.Clear();
		}
	}
}
