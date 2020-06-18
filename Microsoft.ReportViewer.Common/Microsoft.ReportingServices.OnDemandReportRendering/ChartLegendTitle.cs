using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartLegendTitle : IROMStyleDefinitionContainer
	{
		private Chart m_chart;

		private Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegendTitle m_chartLegendTitleDef;

		private ChartLegendTitleInstance m_instance;

		private Style m_style;

		private ReportStringProperty m_caption;

		private ReportEnumProperty<ChartSeparators> m_titleSeparator;

		public Style Style
		{
			get
			{
				if (m_style == null && !m_chart.IsOldSnapshot)
				{
					m_style = new Style(m_chart, m_chart, m_chartLegendTitleDef, m_chart.RenderingContext);
				}
				return m_style;
			}
		}

		public ReportStringProperty Caption
		{
			get
			{
				if (m_caption == null && !m_chart.IsOldSnapshot && m_chartLegendTitleDef.Caption != null)
				{
					m_caption = new ReportStringProperty(m_chartLegendTitleDef.Caption);
				}
				return m_caption;
			}
		}

		public ReportEnumProperty<ChartSeparators> TitleSeparator
		{
			get
			{
				if (m_titleSeparator == null && !m_chart.IsOldSnapshot && m_chartLegendTitleDef.TitleSeparator != null)
				{
					m_titleSeparator = new ReportEnumProperty<ChartSeparators>(m_chartLegendTitleDef.TitleSeparator.IsExpression, m_chartLegendTitleDef.TitleSeparator.OriginalText, EnumTranslator.TranslateChartSeparator(m_chartLegendTitleDef.TitleSeparator.StringValue, null));
				}
				return m_titleSeparator;
			}
		}

		internal Chart ChartDef => m_chart;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegendTitle ChartLegendTitleDef => m_chartLegendTitleDef;

		public ChartLegendTitleInstance Instance
		{
			get
			{
				if (m_chart.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (m_instance == null)
				{
					m_instance = new ChartLegendTitleInstance(this);
				}
				return m_instance;
			}
		}

		internal ChartLegendTitle(Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegendTitle chartLegendTitleDef, Chart chart)
		{
			m_chartLegendTitleDef = chartLegendTitleDef;
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
