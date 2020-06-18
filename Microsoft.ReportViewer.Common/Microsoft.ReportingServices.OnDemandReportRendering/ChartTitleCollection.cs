namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartTitleCollection : ChartObjectCollectionBase<ChartTitle, ChartTitleInstance>
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
				if (m_chart.ChartDef.Titles != null)
				{
					return m_chart.ChartDef.Titles.Count;
				}
				return 0;
			}
		}

		internal ChartTitleCollection(Chart chart)
		{
			m_chart = chart;
		}

		protected override ChartTitle CreateChartObject(int index)
		{
			if (m_chart.IsOldSnapshot)
			{
				return new ChartTitle(m_chart.RenderChartDef.Title, m_chart.ChartInstanceInfo.Title, m_chart);
			}
			return new ChartTitle(m_chart.ChartDef.Titles[index], m_chart);
		}
	}
}
