using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartAxisTitle : ChartObjectCollectionItem<ChartAxisTitleInstance>, IROMStyleDefinitionContainer
	{
		private Chart m_chart;

		private Microsoft.ReportingServices.ReportIntermediateFormat.ChartAxisTitle m_chartAxisTitleDef;

		private Style m_style;

		private ReportStringProperty m_caption;

		private Microsoft.ReportingServices.ReportProcessing.ChartTitle m_renderChartTitleDef;

		private Microsoft.ReportingServices.ReportProcessing.ChartTitleInstance m_renderChartTitleInstance;

		private ReportEnumProperty<ChartAxisTitlePositions> m_position;

		private ReportEnumProperty<TextOrientations> m_textOrientation;

		public ReportStringProperty Caption
		{
			get
			{
				if (m_caption == null)
				{
					if (m_chart.IsOldSnapshot)
					{
						if (m_renderChartTitleDef.Caption != null)
						{
							m_caption = new ReportStringProperty(m_renderChartTitleDef.Caption);
						}
					}
					else if (m_chartAxisTitleDef.Caption != null)
					{
						m_caption = new ReportStringProperty(m_chartAxisTitleDef.Caption);
					}
				}
				return m_caption;
			}
		}

		public Style Style
		{
			get
			{
				if (m_style == null)
				{
					if (m_chart.IsOldSnapshot)
					{
						m_style = new Style(m_renderChartTitleDef.StyleClass, m_renderChartTitleInstance.StyleAttributeValues, m_chart.RenderingContext);
					}
					else if (m_chartAxisTitleDef.StyleClass != null)
					{
						m_style = new Style(m_chart, m_chart, m_chartAxisTitleDef, m_chart.RenderingContext);
					}
				}
				return m_style;
			}
		}

		public ReportEnumProperty<ChartAxisTitlePositions> Position
		{
			get
			{
				if (m_position == null)
				{
					if (m_chart.IsOldSnapshot)
					{
						ChartAxisTitlePositions value = ChartAxisTitlePositions.Center;
						switch (m_renderChartTitleDef.Position)
						{
						case Microsoft.ReportingServices.ReportProcessing.ChartTitle.Positions.Center:
							value = ChartAxisTitlePositions.Center;
							break;
						case Microsoft.ReportingServices.ReportProcessing.ChartTitle.Positions.Near:
							value = ChartAxisTitlePositions.Near;
							break;
						case Microsoft.ReportingServices.ReportProcessing.ChartTitle.Positions.Far:
							value = ChartAxisTitlePositions.Far;
							break;
						}
						m_position = new ReportEnumProperty<ChartAxisTitlePositions>(value);
					}
					else if (m_chartAxisTitleDef.Position != null)
					{
						m_position = new ReportEnumProperty<ChartAxisTitlePositions>(m_chartAxisTitleDef.Position.IsExpression, m_chartAxisTitleDef.Position.OriginalText, EnumTranslator.TranslateChartAxisTitlePosition(m_chartAxisTitleDef.Position.StringValue, null));
					}
				}
				return m_position;
			}
		}

		public ReportEnumProperty<TextOrientations> TextOrientation
		{
			get
			{
				if (m_textOrientation == null && !m_chart.IsOldSnapshot && m_chartAxisTitleDef.TextOrientation != null)
				{
					m_textOrientation = new ReportEnumProperty<TextOrientations>(m_chartAxisTitleDef.TextOrientation.IsExpression, m_chartAxisTitleDef.TextOrientation.OriginalText, EnumTranslator.TranslateTextOrientations(m_chartAxisTitleDef.TextOrientation.StringValue, null));
				}
				return m_textOrientation;
			}
		}

		internal Chart ChartDef => m_chart;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.ChartAxisTitle ChartAxisTitleDef => m_chartAxisTitleDef;

		internal Microsoft.ReportingServices.ReportProcessing.ChartTitleInstance RenderChartTitleInstance => m_renderChartTitleInstance;

		public ChartAxisTitleInstance Instance
		{
			get
			{
				if (m_chart.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (m_instance == null)
				{
					m_instance = new ChartAxisTitleInstance(this);
				}
				return m_instance;
			}
		}

		internal ChartAxisTitle(Microsoft.ReportingServices.ReportIntermediateFormat.ChartAxisTitle chartAxisTitleDef, Chart chart)
		{
			m_chart = chart;
			m_chartAxisTitleDef = chartAxisTitleDef;
		}

		internal ChartAxisTitle(Microsoft.ReportingServices.ReportProcessing.ChartTitle renderChartTitleDef, Microsoft.ReportingServices.ReportProcessing.ChartTitleInstance renderChartTitleInstance, Chart chart)
		{
			m_chart = chart;
			m_renderChartTitleDef = renderChartTitleDef;
			m_renderChartTitleInstance = renderChartTitleInstance;
		}

		internal override void SetNewContext()
		{
			base.SetNewContext();
			if (m_style != null)
			{
				m_style.SetNewContext();
			}
		}
	}
}
