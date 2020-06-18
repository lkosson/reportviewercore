namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal abstract class ChartSeriesCollection : ReportElementCollectionBase<ChartSeries>, IDataRegionRowCollection
	{
		protected Chart m_owner;

		protected ChartSeries[] m_chartSeriesCollection;

		internal ChartSeriesCollection(Chart owner)
		{
			m_owner = owner;
		}

		IDataRegionRow IDataRegionRowCollection.GetIfExists(int seriesIndex)
		{
			if (m_chartSeriesCollection != null && seriesIndex >= 0 && seriesIndex < Count)
			{
				return m_chartSeriesCollection[seriesIndex];
			}
			return null;
		}
	}
}
