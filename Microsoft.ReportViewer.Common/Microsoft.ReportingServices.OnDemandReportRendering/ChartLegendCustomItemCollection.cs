namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartLegendCustomItemCollection : ChartObjectCollectionBase<ChartLegendCustomItem, ChartLegendCustomItemInstance>
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
				return m_legend.ChartLegendDef.LegendCustomItems.Count;
			}
		}

		internal ChartLegendCustomItemCollection(ChartLegend legend, Chart chart)
		{
			m_legend = legend;
			m_chart = chart;
		}

		protected override ChartLegendCustomItem CreateChartObject(int index)
		{
			if (m_chart.IsOldSnapshot)
			{
				return null;
			}
			return new ChartLegendCustomItem(m_legend.ChartLegendDef.LegendCustomItems[index], m_chart);
		}
	}
}
