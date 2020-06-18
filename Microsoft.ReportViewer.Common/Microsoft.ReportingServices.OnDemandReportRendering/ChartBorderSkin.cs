using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartBorderSkin : IROMStyleDefinitionContainer
	{
		private Chart m_chart;

		private Microsoft.ReportingServices.ReportIntermediateFormat.ChartBorderSkin m_chartBorderSkinDef;

		private ChartBorderSkinInstance m_instance;

		private Style m_style;

		private ReportEnumProperty<ChartBorderSkinType> m_borderSkinType;

		public Style Style
		{
			get
			{
				if (m_style == null && !m_chart.IsOldSnapshot && m_chartBorderSkinDef.StyleClass != null)
				{
					m_style = new Style(m_chart, m_chart, m_chartBorderSkinDef, m_chart.RenderingContext);
				}
				return m_style;
			}
		}

		public ReportEnumProperty<ChartBorderSkinType> BorderSkinType
		{
			get
			{
				if (m_borderSkinType == null && !m_chart.IsOldSnapshot && m_chartBorderSkinDef.BorderSkinType != null)
				{
					m_borderSkinType = new ReportEnumProperty<ChartBorderSkinType>(m_chartBorderSkinDef.BorderSkinType.IsExpression, m_chartBorderSkinDef.BorderSkinType.OriginalText, EnumTranslator.TranslateChartBorderSkinType(m_chartBorderSkinDef.BorderSkinType.StringValue, null));
				}
				return m_borderSkinType;
			}
		}

		internal Chart ChartDef => m_chart;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.ChartBorderSkin ChartBorderSkinDef => m_chartBorderSkinDef;

		public ChartBorderSkinInstance Instance
		{
			get
			{
				if (m_chart.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (m_instance == null)
				{
					m_instance = new ChartBorderSkinInstance(this);
				}
				return m_instance;
			}
		}

		internal ChartBorderSkin(Microsoft.ReportingServices.ReportIntermediateFormat.ChartBorderSkin chartBorderSkinDef, Chart chart)
		{
			m_chartBorderSkinDef = chartBorderSkinDef;
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
