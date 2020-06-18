namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartTickMarksInstance : BaseInstance
	{
		private ChartTickMarks m_chartTickMarksDef;

		private StyleInstance m_style;

		private ChartAutoBool? m_enabled;

		private ChartTickMarksType? m_type;

		private double? m_length;

		private double? m_interval;

		private ChartIntervalType? m_intervalType;

		private double? m_intervalOffset;

		private ChartIntervalType? m_intervalOffsetType;

		public StyleInstance Style
		{
			get
			{
				if (m_style == null)
				{
					m_style = new StyleInstance(m_chartTickMarksDef, m_chartTickMarksDef.ChartDef, m_chartTickMarksDef.ChartDef.RenderingContext);
				}
				return m_style;
			}
		}

		public ChartAutoBool Enabled
		{
			get
			{
				if (!m_enabled.HasValue && !m_chartTickMarksDef.ChartDef.IsOldSnapshot)
				{
					string val = m_chartTickMarksDef.ChartTickMarksDef.EvaluateEnabled(ReportScopeInstance, m_chartTickMarksDef.ChartDef.RenderingContext.OdpContext);
					m_enabled = EnumTranslator.TranslateChartAutoBool(val, m_chartTickMarksDef.ChartDef.RenderingContext.OdpContext.ReportRuntime);
				}
				return m_enabled.Value;
			}
		}

		public ChartTickMarksType Type
		{
			get
			{
				if (!m_type.HasValue && !m_chartTickMarksDef.ChartDef.IsOldSnapshot)
				{
					m_type = m_chartTickMarksDef.ChartTickMarksDef.EvaluateType(ReportScopeInstance, m_chartTickMarksDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_type.Value;
			}
		}

		public double Length
		{
			get
			{
				if (!m_length.HasValue && !m_chartTickMarksDef.ChartDef.IsOldSnapshot)
				{
					m_length = m_chartTickMarksDef.ChartTickMarksDef.EvaluateLength(ReportScopeInstance, m_chartTickMarksDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_length.Value;
			}
		}

		public double Interval
		{
			get
			{
				if (!m_interval.HasValue && !m_chartTickMarksDef.ChartDef.IsOldSnapshot)
				{
					m_interval = m_chartTickMarksDef.ChartTickMarksDef.EvaluateInterval(ReportScopeInstance, m_chartTickMarksDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_interval.Value;
			}
		}

		public ChartIntervalType IntervalType
		{
			get
			{
				if (!m_intervalType.HasValue && !m_chartTickMarksDef.ChartDef.IsOldSnapshot)
				{
					m_intervalType = m_chartTickMarksDef.ChartTickMarksDef.EvaluateIntervalType(ReportScopeInstance, m_chartTickMarksDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_intervalType.Value;
			}
		}

		public double IntervalOffset
		{
			get
			{
				if (!m_intervalOffset.HasValue && !m_chartTickMarksDef.ChartDef.IsOldSnapshot)
				{
					m_intervalOffset = m_chartTickMarksDef.ChartTickMarksDef.EvaluateIntervalOffset(ReportScopeInstance, m_chartTickMarksDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_intervalOffset.Value;
			}
		}

		public ChartIntervalType IntervalOffsetType
		{
			get
			{
				if (!m_intervalOffsetType.HasValue && !m_chartTickMarksDef.ChartDef.IsOldSnapshot)
				{
					m_intervalOffsetType = m_chartTickMarksDef.ChartTickMarksDef.EvaluateIntervalOffsetType(ReportScopeInstance, m_chartTickMarksDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_intervalOffsetType.Value;
			}
		}

		internal ChartTickMarksInstance(ChartTickMarks chartTickMarksDef)
			: base(chartTickMarksDef.ChartDef)
		{
			m_chartTickMarksDef = chartTickMarksDef;
		}

		protected override void ResetInstanceCache()
		{
			if (m_style != null)
			{
				m_style.SetNewContext();
			}
			m_enabled = null;
			m_type = null;
			m_length = null;
			m_interval = null;
			m_intervalType = null;
			m_intervalOffset = null;
			m_intervalOffsetType = null;
		}
	}
}
