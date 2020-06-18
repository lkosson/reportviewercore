using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartAxisScaleBreak : IROMStyleDefinitionContainer
	{
		private Chart m_chart;

		private Microsoft.ReportingServices.ReportIntermediateFormat.ChartAxisScaleBreak m_chartAxisScaleBreakDef;

		private ChartAxisScaleBreakInstance m_instance;

		private Style m_style;

		private ReportBoolProperty m_enabled;

		private ReportEnumProperty<ChartBreakLineType> m_breakLineType;

		private ReportIntProperty m_collapsibleSpaceThreshold;

		private ReportIntProperty m_maxNumberOfBreaks;

		private ReportDoubleProperty m_spacing;

		private ReportEnumProperty<ChartAutoBool> m_includeZero;

		public Style Style
		{
			get
			{
				if (m_style == null && !m_chart.IsOldSnapshot && m_chartAxisScaleBreakDef.StyleClass != null)
				{
					m_style = new Style(m_chart, m_chart, m_chartAxisScaleBreakDef, m_chart.RenderingContext);
				}
				return m_style;
			}
		}

		public ReportBoolProperty Enabled
		{
			get
			{
				if (m_enabled == null && !m_chart.IsOldSnapshot && m_chartAxisScaleBreakDef.Enabled != null)
				{
					m_enabled = new ReportBoolProperty(m_chartAxisScaleBreakDef.Enabled);
				}
				return m_enabled;
			}
		}

		public ReportEnumProperty<ChartBreakLineType> BreakLineType
		{
			get
			{
				if (m_breakLineType == null && !m_chart.IsOldSnapshot && m_chartAxisScaleBreakDef.BreakLineType != null)
				{
					m_breakLineType = new ReportEnumProperty<ChartBreakLineType>(m_chartAxisScaleBreakDef.BreakLineType.IsExpression, m_chartAxisScaleBreakDef.BreakLineType.OriginalText, EnumTranslator.TranslateChartBreakLineType(m_chartAxisScaleBreakDef.BreakLineType.StringValue, null));
				}
				return m_breakLineType;
			}
		}

		public ReportIntProperty CollapsibleSpaceThreshold
		{
			get
			{
				if (m_collapsibleSpaceThreshold == null && !m_chart.IsOldSnapshot && m_chartAxisScaleBreakDef.CollapsibleSpaceThreshold != null)
				{
					m_collapsibleSpaceThreshold = new ReportIntProperty(m_chartAxisScaleBreakDef.CollapsibleSpaceThreshold.IsExpression, m_chartAxisScaleBreakDef.CollapsibleSpaceThreshold.OriginalText, m_chartAxisScaleBreakDef.CollapsibleSpaceThreshold.IntValue, 25);
				}
				return m_collapsibleSpaceThreshold;
			}
		}

		public ReportIntProperty MaxNumberOfBreaks
		{
			get
			{
				if (m_maxNumberOfBreaks == null && !m_chart.IsOldSnapshot && m_chartAxisScaleBreakDef.MaxNumberOfBreaks != null)
				{
					m_maxNumberOfBreaks = new ReportIntProperty(m_chartAxisScaleBreakDef.MaxNumberOfBreaks.IsExpression, m_chartAxisScaleBreakDef.MaxNumberOfBreaks.OriginalText, m_chartAxisScaleBreakDef.MaxNumberOfBreaks.IntValue, 5);
				}
				return m_maxNumberOfBreaks;
			}
		}

		public ReportDoubleProperty Spacing
		{
			get
			{
				if (m_spacing == null && !m_chart.IsOldSnapshot && m_chartAxisScaleBreakDef.Spacing != null)
				{
					m_spacing = new ReportDoubleProperty(m_chartAxisScaleBreakDef.Spacing);
				}
				return m_spacing;
			}
		}

		public ReportEnumProperty<ChartAutoBool> IncludeZero
		{
			get
			{
				if (m_includeZero == null && !m_chart.IsOldSnapshot && m_chartAxisScaleBreakDef.IncludeZero != null)
				{
					m_includeZero = new ReportEnumProperty<ChartAutoBool>(m_chartAxisScaleBreakDef.IncludeZero.IsExpression, m_chartAxisScaleBreakDef.IncludeZero.OriginalText, (!m_chartAxisScaleBreakDef.IncludeZero.IsExpression) ? EnumTranslator.TranslateChartAutoBool(m_chartAxisScaleBreakDef.IncludeZero.StringValue, null) : ChartAutoBool.Auto);
				}
				return m_includeZero;
			}
		}

		internal Chart ChartDef => m_chart;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.ChartAxisScaleBreak ChartAxisScaleBreakDef => m_chartAxisScaleBreakDef;

		public ChartAxisScaleBreakInstance Instance
		{
			get
			{
				if (m_chart.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (m_instance == null)
				{
					m_instance = new ChartAxisScaleBreakInstance(this);
				}
				return m_instance;
			}
		}

		internal ChartAxisScaleBreak(Microsoft.ReportingServices.ReportIntermediateFormat.ChartAxisScaleBreak chartAxisScaleBreakDef, Chart chart)
		{
			m_chartAxisScaleBreakDef = chartAxisScaleBreakDef;
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
