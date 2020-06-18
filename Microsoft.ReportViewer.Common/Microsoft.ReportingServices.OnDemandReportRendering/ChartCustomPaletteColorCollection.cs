namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartCustomPaletteColorCollection : ChartObjectCollectionBase<ChartCustomPaletteColor, ChartCustomPaletteColorInstance>
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
				if (m_chart.ChartDef.CustomPaletteColors != null)
				{
					return m_chart.ChartDef.CustomPaletteColors.Count;
				}
				return 0;
			}
		}

		internal ChartCustomPaletteColorCollection(Chart chart)
		{
			m_chart = chart;
		}

		protected override ChartCustomPaletteColor CreateChartObject(int index)
		{
			if (m_chart.IsOldSnapshot)
			{
				return null;
			}
			return new ChartCustomPaletteColor(m_chart.ChartDef.CustomPaletteColors[index], m_chart);
		}
	}
}
