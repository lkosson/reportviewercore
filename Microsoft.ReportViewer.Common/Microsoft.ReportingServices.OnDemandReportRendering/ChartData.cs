namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartData
	{
		private Chart m_owner;

		private ChartSeriesCollection m_seriesCollection;

		private ChartDerivedSeriesCollection m_chartDerivedSeriesCollection;

		internal bool HasSeriesCollection => m_seriesCollection != null;

		public ChartSeriesCollection SeriesCollection
		{
			get
			{
				if (m_seriesCollection == null)
				{
					if (m_owner.IsOldSnapshot)
					{
						m_seriesCollection = new ShimChartSeriesCollection(m_owner);
					}
					else
					{
						m_seriesCollection = new InternalChartSeriesCollection(m_owner, m_owner.ChartDef.ChartSeriesCollection);
					}
				}
				return m_seriesCollection;
			}
		}

		public ChartDerivedSeriesCollection DerivedSeriesCollection
		{
			get
			{
				if (m_chartDerivedSeriesCollection == null && !m_owner.IsOldSnapshot && m_owner.ChartDef.DerivedSeriesCollection != null)
				{
					m_chartDerivedSeriesCollection = new ChartDerivedSeriesCollection(m_owner);
				}
				return m_chartDerivedSeriesCollection;
			}
		}

		internal ChartData(Chart owner)
		{
			m_owner = owner;
		}
	}
}
