using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartGridLines : IROMStyleDefinitionContainer
	{
		private Chart m_chart;

		private Microsoft.ReportingServices.ReportIntermediateFormat.ChartGridLines m_gridLinesDef;

		private GridLines m_renderGridLinesDef;

		private object[] m_styleValues;

		private Style m_style;

		private ChartGridLinesInstance m_instance;

		private ReportEnumProperty<ChartAutoBool> m_enabled;

		private ReportDoubleProperty m_interval;

		private ReportDoubleProperty m_intervalOffset;

		private ReportEnumProperty<ChartIntervalType> m_intervalType;

		private ReportEnumProperty<ChartIntervalType> m_intervalOffsetType;

		public ReportEnumProperty<ChartAutoBool> Enabled
		{
			get
			{
				if (m_enabled == null && !m_chart.IsOldSnapshot && m_gridLinesDef.Enabled != null)
				{
					m_enabled = new ReportEnumProperty<ChartAutoBool>(m_gridLinesDef.Enabled.IsExpression, m_gridLinesDef.Enabled.OriginalText, EnumTranslator.TranslateChartAutoBool(m_gridLinesDef.Enabled.StringValue, null));
				}
				return m_enabled;
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
						m_style = new Style(m_renderGridLinesDef.StyleClass, m_styleValues, m_chart.RenderingContext);
					}
					else if (m_gridLinesDef.StyleClass != null)
					{
						m_style = new Style(m_chart, m_chart, m_gridLinesDef, m_chart.RenderingContext);
					}
				}
				return m_style;
			}
		}

		public ReportDoubleProperty Interval
		{
			get
			{
				if (m_interval == null && !m_chart.IsOldSnapshot && m_gridLinesDef.Interval != null)
				{
					m_interval = new ReportDoubleProperty(m_gridLinesDef.Interval);
				}
				return m_interval;
			}
		}

		public ReportDoubleProperty IntervalOffset
		{
			get
			{
				if (m_intervalOffset == null && !m_chart.IsOldSnapshot && m_gridLinesDef.IntervalOffset != null)
				{
					m_intervalOffset = new ReportDoubleProperty(m_gridLinesDef.IntervalOffset);
				}
				return m_intervalOffset;
			}
		}

		public ReportEnumProperty<ChartIntervalType> IntervalType
		{
			get
			{
				if (m_intervalType == null && !m_chart.IsOldSnapshot && m_gridLinesDef.IntervalType != null)
				{
					m_intervalType = new ReportEnumProperty<ChartIntervalType>(m_gridLinesDef.IntervalType.IsExpression, m_gridLinesDef.IntervalType.OriginalText, EnumTranslator.TranslateChartIntervalType(m_gridLinesDef.IntervalType.StringValue, null));
				}
				return m_intervalType;
			}
		}

		public ReportEnumProperty<ChartIntervalType> IntervalOffsetType
		{
			get
			{
				if (m_intervalOffsetType == null && !m_chart.IsOldSnapshot && m_gridLinesDef.IntervalOffsetType != null)
				{
					m_intervalOffsetType = new ReportEnumProperty<ChartIntervalType>(m_gridLinesDef.IntervalOffsetType.IsExpression, m_gridLinesDef.IntervalOffsetType.OriginalText, EnumTranslator.TranslateChartIntervalType(m_gridLinesDef.IntervalOffsetType.StringValue, null));
				}
				return m_intervalOffsetType;
			}
		}

		internal Chart ChartDef => m_chart;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.ChartGridLines ChartGridLinesDef => m_gridLinesDef;

		public ChartGridLinesInstance Instance
		{
			get
			{
				if (m_chart.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (m_instance == null)
				{
					m_instance = new ChartGridLinesInstance(this);
				}
				return m_instance;
			}
		}

		internal ChartGridLines(Microsoft.ReportingServices.ReportIntermediateFormat.ChartGridLines gridLinesDef, Chart chart)
		{
			m_chart = chart;
			m_gridLinesDef = gridLinesDef;
		}

		internal ChartGridLines(GridLines renderGridLinesDef, object[] styleValues, Chart chart)
		{
			m_chart = chart;
			m_renderGridLinesDef = renderGridLinesDef;
			m_styleValues = styleValues;
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
