namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartLegendCustomItemCellCollection : ChartObjectCollectionBase<ChartLegendCustomItemCell, ChartLegendCustomItemCellInstance>
	{
		private Chart m_chart;

		private ChartLegendCustomItem m_legendCustomItem;

		public override int Count
		{
			get
			{
				if (m_chart.IsOldSnapshot)
				{
					return 0;
				}
				return m_legendCustomItem.ChartLegendCustomItemDef.LegendCustomItemCells.Count;
			}
		}

		internal ChartLegendCustomItemCellCollection(ChartLegendCustomItem legendCustomItem, Chart chart)
		{
			m_legendCustomItem = legendCustomItem;
			m_chart = chart;
		}

		protected override ChartLegendCustomItemCell CreateChartObject(int index)
		{
			if (m_chart.IsOldSnapshot)
			{
				return null;
			}
			return new ChartLegendCustomItemCell(m_legendCustomItem.ChartLegendCustomItemDef.LegendCustomItemCells[index], m_chart);
		}
	}
}
