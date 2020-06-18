using Microsoft.ReportingServices.ReportProcessing;
using System.Collections;

namespace Microsoft.ReportingServices.ReportRendering
{
	internal sealed class OWCChartColumnCollection
	{
		private ReportItem m_owner;

		private Microsoft.ReportingServices.ReportProcessing.OWCChart m_chartDef;

		private OWCChartInstance m_chartInstance;

		private OWCChartColumn[] m_chartData;

		public OWCChartColumn this[int index]
		{
			get
			{
				if (index < 0 || index >= Count)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, Count);
				}
				OWCChartColumn oWCChartColumn = null;
				if (m_chartData == null || m_chartData[index] == null)
				{
					ArrayList columnData = null;
					if (m_chartInstance != null)
					{
						columnData = ((OWCChartInstanceInfo)m_owner.InstanceInfo)[index];
					}
					oWCChartColumn = new OWCChartColumn(m_chartDef.ChartData[index], columnData);
					if (m_owner.RenderingContext.CacheState)
					{
						if (m_chartData == null)
						{
							m_chartData = new OWCChartColumn[m_chartDef.ChartData.Count];
						}
						m_chartData[index] = oWCChartColumn;
					}
				}
				else
				{
					oWCChartColumn = m_chartData[index];
				}
				return oWCChartColumn;
			}
		}

		public int Count => m_chartDef.ChartData.Count;

		internal OWCChartColumnCollection(Microsoft.ReportingServices.ReportProcessing.OWCChart chartDef, OWCChartInstance chartInstance, ReportItem owner)
		{
			m_owner = owner;
			m_chartInstance = chartInstance;
			m_chartDef = chartDef;
		}
	}
}
