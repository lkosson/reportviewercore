namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartLegendColumnCollection : ChartObjectCollectionBase<ChartLegendColumn, ChartLegendColumnInstance>
	{
		private Chart m_chart;

		private ChartLegend m_legend;

		public override int Count
		{
			get
			{
				if (m_chart.IsOldSnapshot)
				{
					return 0;
				}
				return m_legend.ChartLegendDef.LegendColumns.Count;
			}
		}

		internal ChartLegendColumnCollection(ChartLegend legend, Chart chart)
		{
			m_legend = legend;
			m_chart = chart;
		}

		protected override ChartLegendColumn CreateChartObject(int index)
		{
			if (m_chart.IsOldSnapshot)
			{
				return null;
			}
			return new ChartLegendColumn(m_legend.ChartLegendDef.LegendColumns[index], m_chart);
		}
	}
}
