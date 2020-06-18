using Microsoft.ReportingServices.ReportProcessing;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal abstract class GaugePanelObjectCollectionBase<T> : IEnumerable<T>, IEnumerable where T : GaugePanelObjectCollectionItem
	{
		private T[] m_collection;

		public T this[int index]
		{
			get
			{
				if (index < 0 || index >= Count)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, Count);
				}
				if (m_collection == null)
				{
					m_collection = new T[Count];
				}
				if (m_collection[index] == null)
				{
					m_collection[index] = CreateGaugePanelObject(index);
				}
				return m_collection[index];
			}
		}

		public abstract int Count
		{
			get;
		}

		protected abstract T CreateGaugePanelObject(int index);

		public IEnumerator<T> GetEnumerator()
		{
			for (int i = 0; i < Count; i++)
			{
				yield return this[i];
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		internal void SetNewContext()
		{
			if (m_collection != null)
			{
				for (int i = 0; i < m_collection.Length; i++)
				{
					m_collection[i]?.SetNewContext();
				}
			}
		}
	}
}
