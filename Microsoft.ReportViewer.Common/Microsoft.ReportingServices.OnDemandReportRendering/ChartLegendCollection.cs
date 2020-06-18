namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartLegendCollection : ChartObjectCollectionBase<ChartLegend, ChartLegendInstance>
	{
		private Chart m_chart;

		public override int Count
		{
			get
			{
				if (m_chart.IsOldSnapshot)
				{
					return 1;
				}
				if (m_chart.ChartDef.Legends != null)
				{
					return m_chart.ChartDef.Legends.Count;
				}
				return 0;
			}
		}

		internal ChartLegendCollection(Chart chart)
		{
			m_chart = chart;
		}

		protected override ChartLegend CreateChartObject(int index)
		{
			if (m_chart.IsOldSnapshot)
			{
				if (m_chart.RenderChartDef.Legend != null)
				{
					return new ChartLegend(m_chart.RenderChartDef.Legend, m_chart.ChartInstanceInfo.LegendStyleAttributeValues, m_chart);
				}
				return null;
			}
			return new ChartLegend(m_chart.ChartDef.Legends[index], m_chart);
		}
	}
}
