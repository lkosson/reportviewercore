using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartAxisCollection : ChartObjectCollectionBase<ChartAxis, ChartAxisInstance>
	{
		private Chart m_chart;

		private ChartArea m_chartArea;

		private bool m_isCategory;

		public override int Count
		{
			get
			{
				if (m_chart.IsOldSnapshot)
				{
					return 1;
				}
				if (!m_isCategory)
				{
					return m_chartArea.ChartAreaDef.ValueAxes.Count;
				}
				return m_chartArea.ChartAreaDef.CategoryAxes.Count;
			}
		}

		internal ChartAxisCollection(ChartArea chartArea, Chart chart, bool isCategory)
		{
			m_chartArea = chartArea;
			m_chart = chart;
			m_isCategory = isCategory;
		}

		protected override ChartAxis CreateChartObject(int index)
		{
			if (m_chart.IsOldSnapshot)
			{
				if (!m_isCategory)
				{
					return new ChartAxis(m_chart.RenderChartDef.ValueAxis, m_chart.ChartInstanceInfo.ValueAxis, m_chart, m_isCategory);
				}
				return new ChartAxis(m_chart.RenderChartDef.CategoryAxis, m_chart.ChartInstanceInfo.CategoryAxis, m_chart, m_isCategory);
			}
			if (!m_isCategory)
			{
				return new ChartAxis(m_chartArea.ChartAreaDef.ValueAxes[index], m_chart);
			}
			return new ChartAxis(m_chartArea.ChartAreaDef.CategoryAxes[index], m_chart);
		}

		internal ChartAxis GetByName(string axisName)
		{
			for (int i = 0; i < Count; i++)
			{
				Microsoft.ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis = m_isCategory ? m_chartArea.ChartAreaDef.CategoryAxes[i] : m_chartArea.ChartAreaDef.ValueAxes[i];
				if (string.CompareOrdinal(axisName, chartAxis.AxisName) == 0)
				{
					return base[i];
				}
			}
			return null;
		}
	}
}
