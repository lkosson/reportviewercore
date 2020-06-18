using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartTickMarks : IROMStyleDefinitionContainer
	{
		private Chart m_chart;

		private Microsoft.ReportingServices.ReportIntermediateFormat.ChartTickMarks m_chartTickMarksDef;

		private ChartTickMarksInstance m_instance;

		private Style m_style;

		private ReportEnumProperty<ChartAutoBool> m_enabled;

		private ReportEnumProperty<ChartTickMarksType> m_type;

		private ReportDoubleProperty m_length;

		private ReportDoubleProperty m_interval;

		private ReportEnumProperty<ChartIntervalType> m_intervalType;

		private ReportDoubleProperty m_intervalOffset;

		private ReportEnumProperty<ChartIntervalType> m_intervalOffsetType;

		public Style Style
		{
			get
			{
				if (m_style == null && !m_chart.IsOldSnapshot && m_chartTickMarksDef.StyleClass != null)
				{
					m_style = new Style(m_chart, m_chart, m_chartTickMarksDef, m_chart.RenderingContext);
				}
				return m_style;
			}
		}

		public ReportEnumProperty<ChartAutoBool> Enabled
		{
			get
			{
				if (m_enabled == null)
				{
					if (m_chart.IsOldSnapshot)
					{
						if (m_type != null)
						{
							m_enabled = new ReportEnumProperty<ChartAutoBool>((m_type.Value != 0) ? ChartAutoBool.True : ChartAutoBool.False);
						}
					}
					else if (m_chartTickMarksDef.Enabled != null)
					{
						m_enabled = new ReportEnumProperty<ChartAutoBool>(m_chartTickMarksDef.Enabled.IsExpression, m_chartTickMarksDef.Enabled.OriginalText, (!m_chartTickMarksDef.Enabled.IsExpression) ? EnumTranslator.TranslateChartAutoBool(m_chartTickMarksDef.Enabled.StringValue, null) : ChartAutoBool.Auto);
					}
				}
				return m_enabled;
			}
		}

		public ReportEnumProperty<ChartTickMarksType> Type
		{
			get
			{
				if (m_type == null && !m_chart.IsOldSnapshot && m_chartTickMarksDef.Type != null)
				{
					m_type = new ReportEnumProperty<ChartTickMarksType>(m_chartTickMarksDef.Type.IsExpression, m_chartTickMarksDef.Type.OriginalText, EnumTranslator.TranslateChartTickMarksType(m_chartTickMarksDef.Type.StringValue, null));
				}
				return m_type;
			}
		}

		public ReportDoubleProperty Length
		{
			get
			{
				if (m_length == null && !m_chart.IsOldSnapshot && m_chartTickMarksDef.Length != null)
				{
					m_length = new ReportDoubleProperty(m_chartTickMarksDef.Length);
				}
				return m_length;
			}
		}

		public ReportDoubleProperty Interval
		{
			get
			{
				if (m_interval == null && !m_chart.IsOldSnapshot && m_chartTickMarksDef.Interval != null)
				{
					m_interval = new ReportDoubleProperty(m_chartTickMarksDef.Interval);
				}
				return m_interval;
			}
		}

		public ReportEnumProperty<ChartIntervalType> IntervalType
		{
			get
			{
				if (m_intervalType == null && !m_chart.IsOldSnapshot && m_chartTickMarksDef.IntervalType != null)
				{
					m_intervalType = new ReportEnumProperty<ChartIntervalType>(m_chartTickMarksDef.IntervalType.IsExpression, m_chartTickMarksDef.IntervalType.OriginalText, EnumTranslator.TranslateChartIntervalType(m_chartTickMarksDef.IntervalType.StringValue, null));
				}
				return m_intervalType;
			}
		}

		public ReportDoubleProperty IntervalOffset
		{
			get
			{
				if (m_intervalOffset == null && !m_chart.IsOldSnapshot && m_chartTickMarksDef.IntervalOffset != null)
				{
					m_intervalOffset = new ReportDoubleProperty(m_chartTickMarksDef.IntervalOffset);
				}
				return m_intervalOffset;
			}
		}

		public ReportEnumProperty<ChartIntervalType> IntervalOffsetType
		{
			get
			{
				if (m_intervalOffsetType == null && !m_chart.IsOldSnapshot && m_chartTickMarksDef.IntervalOffsetType != null)
				{
					m_intervalOffsetType = new ReportEnumProperty<ChartIntervalType>(m_chartTickMarksDef.IntervalOffsetType.IsExpression, m_chartTickMarksDef.IntervalOffsetType.OriginalText, EnumTranslator.TranslateChartIntervalType(m_chartTickMarksDef.IntervalOffsetType.StringValue, null));
				}
				return m_intervalOffsetType;
			}
		}

		internal Chart ChartDef => m_chart;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.ChartTickMarks ChartTickMarksDef => m_chartTickMarksDef;

		public ChartTickMarksInstance Instance
		{
			get
			{
				if (m_chart.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (m_instance == null)
				{
					m_instance = new ChartTickMarksInstance(this);
				}
				return m_instance;
			}
		}

		internal ChartTickMarks(Axis.TickMarks type, Chart chart)
		{
			m_type = new ReportEnumProperty<ChartTickMarksType>(GetTickMarksType(type));
			m_chart = chart;
		}

		internal ChartTickMarks(Microsoft.ReportingServices.ReportIntermediateFormat.ChartTickMarks chartTickMarksDef, Chart chart)
		{
			m_chartTickMarksDef = chartTickMarksDef;
			m_chart = chart;
		}

		private ChartTickMarksType GetTickMarksType(Axis.TickMarks tickMarks)
		{
			switch (tickMarks)
			{
			case Axis.TickMarks.Cross:
				return ChartTickMarksType.Cross;
			case Axis.TickMarks.Inside:
				return ChartTickMarksType.Inside;
			case Axis.TickMarks.Outside:
				return ChartTickMarksType.Outside;
			default:
				return ChartTickMarksType.None;
			}
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
