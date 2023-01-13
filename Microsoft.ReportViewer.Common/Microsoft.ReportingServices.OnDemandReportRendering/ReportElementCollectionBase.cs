using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal abstract class ReportElementCollectionBase<T> : IEnumerable<T>, IEnumerable
	{
		public class ReportElementEnumerator : IEnumerator<T>, IDisposable, IEnumerator
		{
			private ReportElementCollectionBase<T> m_collection;

			private int m_currentIndex = -1;

			public T Current
			{
				get
				{
					if (m_currentIndex < 0 || m_currentIndex >= m_collection.Count)
					{
						return default(T);
					}
					return m_collection[m_currentIndex];
				}
			}

			object IEnumerator.Current => Current;

			internal ReportElementEnumerator(ReportElementCollectionBase<T> collection)
			{
				m_collection = collection;
			}

			public void Dispose()
			{
			}

			public bool MoveNext()
			{
				m_currentIndex++;
				return m_currentIndex < m_collection.Count;
			}

			public void Reset()
			{
				m_currentIndex = -1;
			}
		}

		public virtual T this[int i]
		{
			get
			{
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
			}
			set
			{
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
			}
		}

		public abstract int Count
		{
			get;
		}

		public IEnumerator<T> GetEnumerator()
		{
			return new ReportElementEnumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
