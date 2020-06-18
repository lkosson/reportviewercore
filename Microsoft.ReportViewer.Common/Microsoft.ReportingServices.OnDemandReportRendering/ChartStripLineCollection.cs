namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartStripLineCollection : ChartObjectCollectionBase<ChartStripLine, ChartStripLineInstance>
	{
		private Chart m_chart;

		private ChartAxis m_axis;

		public override int Count
		{
			get
			{
				if (m_chart.IsOldSnapshot)
				{
					return 0;
				}
				return m_axis.AxisDef.StripLines.Count;
			}
		}

		internal ChartStripLineCollection(ChartAxis axis, Chart chart)
		{
			m_axis = axis;
			m_chart = chart;
		}

		protected override ChartStripLine CreateChartObject(int index)
		{
			if (m_chart.IsOldSnapshot)
			{
				return null;
			}
			return new ChartStripLine(m_axis.AxisDef.StripLines[index], m_chart);
		}
	}
}
