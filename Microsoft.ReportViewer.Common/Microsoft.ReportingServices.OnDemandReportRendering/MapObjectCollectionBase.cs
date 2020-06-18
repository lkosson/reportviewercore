using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal abstract class MapObjectCollectionBase<T> : ReportElementCollectionBase<T> where T : IMapObjectCollectionItem
	{
		private T[] m_collection;

		public override T this[int index]
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
					m_collection[index] = CreateMapObject(index);
				}
				return m_collection[index];
			}
		}

		protected abstract T CreateMapObject(int index);

		internal void SetNewContext()
		{
			if (m_collection != null)
			{
				for (int i = 0; i < m_collection.Length; i++)
				{
					T val = m_collection[i];
					val?.SetNewContext();
				}
			}
		}
	}
}
