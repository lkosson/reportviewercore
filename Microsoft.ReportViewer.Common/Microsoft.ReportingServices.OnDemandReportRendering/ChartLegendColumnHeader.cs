using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartLegendColumnHeader : IROMStyleDefinitionContainer
	{
		private Chart m_chart;

		private Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegendColumnHeader m_chartLegendColumnHeaderDef;

		private ChartLegendColumnHeaderInstance m_instance;

		private Style m_style;

		private ReportStringProperty m_value;

		public Style Style
		{
			get
			{
				if (m_style == null && !m_chart.IsOldSnapshot && m_chartLegendColumnHeaderDef.StyleClass != null)
				{
					m_style = new Style(m_chart, m_chart, m_chartLegendColumnHeaderDef, m_chart.RenderingContext);
				}
				return m_style;
			}
		}

		public ReportStringProperty Value
		{
			get
			{
				if (m_value == null && !m_chart.IsOldSnapshot && m_chartLegendColumnHeaderDef.Value != null)
				{
					m_value = new ReportStringProperty(m_chartLegendColumnHeaderDef.Value);
				}
				return m_value;
			}
		}

		internal Chart ChartDef => m_chart;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegendColumnHeader ChartLegendColumnHeaderDef => m_chartLegendColumnHeaderDef;

		public ChartLegendColumnHeaderInstance Instance
		{
			get
			{
				if (m_chart.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (m_instance == null)
				{
					m_instance = new ChartLegendColumnHeaderInstance(this);
				}
				return m_instance;
			}
		}

		internal ChartLegendColumnHeader(Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegendColumnHeader chartLegendColumnHeaderDef, Chart chart)
		{
			m_chartLegendColumnHeaderDef = chartLegendColumnHeaderDef;
			m_chart = chart;
		}

		internal void SetNewContext()
		{
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
			if (m_style != null)
			{
				m_style.SetNewContext();
			}
		}
	}
}
