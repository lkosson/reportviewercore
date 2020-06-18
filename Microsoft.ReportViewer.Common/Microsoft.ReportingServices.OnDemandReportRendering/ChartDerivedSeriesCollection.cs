namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartDerivedSeriesCollection : ChartObjectCollectionBase<ChartDerivedSeries, BaseInstance>
	{
		private Chart m_chart;

		public override int Count
		{
			get
			{
				if (m_chart.IsOldSnapshot)
				{
					return 0;
				}
				return m_chart.ChartDef.DerivedSeriesCollection.Count;
			}
		}

		internal ChartDerivedSeriesCollection(Chart chart)
		{
			m_chart = chart;
		}

		protected override ChartDerivedSeries CreateChartObject(int index)
		{
			if (m_chart.IsOldSnapshot)
			{
				return null;
			}
			return new ChartDerivedSeries(m_chart.ChartDef.DerivedSeriesCollection[index], m_chart);
		}
	}
}
