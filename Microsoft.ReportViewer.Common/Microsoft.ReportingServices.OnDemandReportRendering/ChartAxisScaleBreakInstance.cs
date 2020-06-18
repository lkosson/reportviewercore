namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartAxisScaleBreakInstance : BaseInstance
	{
		private ChartAxisScaleBreak m_chartAxisScaleBreakDef;

		private StyleInstance m_style;

		private bool? m_enabled;

		private ChartBreakLineType? m_breakLineType;

		private int? m_collapsibleSpaceThreshold;

		private int? m_maxNumberOfBreaks;

		private double? m_spacing;

		private ChartAutoBool? m_includeZero;

		public StyleInstance Style
		{
			get
			{
				if (m_style == null)
				{
					m_style = new StyleInstance(m_chartAxisScaleBreakDef, m_chartAxisScaleBreakDef.ChartDef, m_chartAxisScaleBreakDef.ChartDef.RenderingContext);
				}
				return m_style;
			}
		}

		public bool Enabled
		{
			get
			{
				if (!m_enabled.HasValue && !m_chartAxisScaleBreakDef.ChartDef.IsOldSnapshot)
				{
					m_enabled = m_chartAxisScaleBreakDef.ChartAxisScaleBreakDef.EvaluateEnabled(ReportScopeInstance, m_chartAxisScaleBreakDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_enabled.Value;
			}
		}

		public ChartBreakLineType BreakLineType
		{
			get
			{
				if (!m_breakLineType.HasValue && !m_chartAxisScaleBreakDef.ChartDef.IsOldSnapshot)
				{
					m_breakLineType = m_chartAxisScaleBreakDef.ChartAxisScaleBreakDef.EvaluateBreakLineType(ReportScopeInstance, m_chartAxisScaleBreakDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_breakLineType.Value;
			}
		}

		public int CollapsibleSpaceThreshold
		{
			get
			{
				if (!m_collapsibleSpaceThreshold.HasValue && !m_chartAxisScaleBreakDef.ChartDef.IsOldSnapshot)
				{
					m_collapsibleSpaceThreshold = m_chartAxisScaleBreakDef.ChartAxisScaleBreakDef.EvaluateCollapsibleSpaceThreshold(ReportScopeInstance, m_chartAxisScaleBreakDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_collapsibleSpaceThreshold.Value;
			}
		}

		public int MaxNumberOfBreaks
		{
			get
			{
				if (!m_maxNumberOfBreaks.HasValue && !m_chartAxisScaleBreakDef.ChartDef.IsOldSnapshot)
				{
					m_maxNumberOfBreaks = m_chartAxisScaleBreakDef.ChartAxisScaleBreakDef.EvaluateMaxNumberOfBreaks(ReportScopeInstance, m_chartAxisScaleBreakDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_maxNumberOfBreaks.Value;
			}
		}

		public double Spacing
		{
			get
			{
				if (!m_spacing.HasValue && !m_chartAxisScaleBreakDef.ChartDef.IsOldSnapshot)
				{
					m_spacing = m_chartAxisScaleBreakDef.ChartAxisScaleBreakDef.EvaluateSpacing(ReportScopeInstance, m_chartAxisScaleBreakDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_spacing.Value;
			}
		}

		public ChartAutoBool IncludeZero
		{
			get
			{
				if (!m_includeZero.HasValue && !m_chartAxisScaleBreakDef.ChartDef.IsOldSnapshot)
				{
					string val = m_chartAxisScaleBreakDef.ChartAxisScaleBreakDef.EvaluateIncludeZero(ReportScopeInstance, m_chartAxisScaleBreakDef.ChartDef.RenderingContext.OdpContext);
					m_includeZero = EnumTranslator.TranslateChartAutoBool(val, m_chartAxisScaleBreakDef.ChartDef.RenderingContext.OdpContext.ReportRuntime);
				}
				return m_includeZero.Value;
			}
		}

		internal ChartAxisScaleBreakInstance(ChartAxisScaleBreak chartAxisScaleBreakDef)
			: base(chartAxisScaleBreakDef.ChartDef)
		{
			m_chartAxisScaleBreakDef = chartAxisScaleBreakDef;
		}

		protected override void ResetInstanceCache()
		{
			if (m_style != null)
			{
				m_style.SetNewContext();
			}
			m_enabled = null;
			m_breakLineType = null;
			m_collapsibleSpaceThreshold = null;
			m_maxNumberOfBreaks = null;
			m_spacing = null;
			m_includeZero = null;
		}
	}
}
