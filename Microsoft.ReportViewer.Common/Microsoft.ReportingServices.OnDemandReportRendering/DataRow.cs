namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal abstract class DataRow : ReportElementCollectionBase<DataCell>, IDataRegionRow
	{
		protected CustomReportItem m_owner;

		protected int m_rowIndex;

		protected DataCell[] m_cachedDataCells;

		internal DataRow(CustomReportItem owner, int rowIndex)
		{
			m_owner = owner;
			m_rowIndex = rowIndex;
		}

		IDataRegionCell IDataRegionRow.GetIfExists(int cellIndex)
		{
			if (m_cachedDataCells != null && cellIndex >= 0 && cellIndex < Count)
			{
				return m_cachedDataCells[cellIndex];
			}
			return null;
		}
	}
}
