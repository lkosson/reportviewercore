namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal abstract class DataRowCollection : ReportElementCollectionBase<DataRow>, IDataRegionRowCollection
	{
		protected CustomReportItem m_owner;

		protected DataRow[] m_cachedDataRows;

		internal DataRowCollection(CustomReportItem owner)
		{
			m_owner = owner;
		}

		IDataRegionRow IDataRegionRowCollection.GetIfExists(int rowIndex)
		{
			if (m_cachedDataRows != null && rowIndex >= 0 && rowIndex < Count)
			{
				return m_cachedDataRows[rowIndex];
			}
			return null;
		}
	}
}
