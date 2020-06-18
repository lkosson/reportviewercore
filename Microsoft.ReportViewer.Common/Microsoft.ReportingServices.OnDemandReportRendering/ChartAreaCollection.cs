using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartAreaCollection : ChartObjectCollectionBase<ChartArea, ChartAreaInstance>
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
				if (m_chart.ChartDef.ChartAreas != null)
				{
					return m_chart.ChartDef.ChartAreas.Count;
				}
				return 0;
			}
		}

		internal ChartAreaCollection(Chart chart)
		{
			m_chart = chart;
		}

		protected override ChartArea CreateChartObject(int index)
		{
			if (m_chart.IsOldSnapshot)
			{
				return new ChartArea(m_chart);
			}
			return new ChartArea(m_chart.ChartDef.ChartAreas[index], m_chart);
		}

		internal ChartArea GetByName(string areaName)
		{
			for (int i = 0; i < Count; i++)
			{
				Microsoft.ReportingServices.ReportIntermediateFormat.ChartArea chartArea = m_chart.ChartDef.ChartAreas[i];
				if (string.CompareOrdinal(areaName, chartArea.ChartAreaName) == 0)
				{
					return base[i];
				}
			}
			return null;
		}
	}
}
