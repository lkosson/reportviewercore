namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartGridLinesInstance : BaseInstance
	{
		private ChartGridLines m_gridLinesDef;

		private StyleInstance m_style;

		private ChartAutoBool? m_enabled;

		private double? m_interval;

		private double? m_intervalOffset;

		private ChartIntervalType? m_intervalType;

		private ChartIntervalType? m_intervalOffsetType;

		public StyleInstance Style
		{
			get
			{
				if (m_style == null)
				{
					m_style = new StyleInstance(m_gridLinesDef, m_gridLinesDef.ChartDef, m_gridLinesDef.ChartDef.RenderingContext);
				}
				return m_style;
			}
		}

		public ChartAutoBool Enabled
		{
			get
			{
				if (!m_enabled.HasValue && !m_gridLinesDef.ChartDef.IsOldSnapshot)
				{
					m_enabled = m_gridLinesDef.ChartGridLinesDef.EvaluateEnabled(ReportScopeInstance, m_gridLinesDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_enabled.Value;
			}
		}

		public double Interval
		{
			get
			{
				if (!m_interval.HasValue && !m_gridLinesDef.ChartDef.IsOldSnapshot)
				{
					m_interval = m_gridLinesDef.ChartGridLinesDef.EvaluateInterval(ReportScopeInstance, m_gridLinesDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_interval.Value;
			}
		}

		public double IntervalOffset
		{
			get
			{
				if (!m_intervalOffset.HasValue && !m_gridLinesDef.ChartDef.IsOldSnapshot)
				{
					m_intervalOffset = m_gridLinesDef.ChartGridLinesDef.EvaluateIntervalOffset(ReportScopeInstance, m_gridLinesDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_intervalOffset.Value;
			}
		}

		public ChartIntervalType IntervalType
		{
			get
			{
				if (!m_intervalType.HasValue && !m_gridLinesDef.ChartDef.IsOldSnapshot)
				{
					m_intervalType = m_gridLinesDef.ChartGridLinesDef.EvaluateIntervalType(ReportScopeInstance, m_gridLinesDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_intervalType.Value;
			}
		}

		public ChartIntervalType IntervalOffsetType
		{
			get
			{
				if (!m_intervalOffsetType.HasValue && !m_gridLinesDef.ChartDef.IsOldSnapshot)
				{
					m_intervalOffsetType = m_gridLinesDef.ChartGridLinesDef.EvaluateIntervalOffsetType(ReportScopeInstance, m_gridLinesDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_intervalOffsetType.Value;
			}
		}

		internal ChartGridLinesInstance(ChartGridLines gridlinesDef)
			: base(gridlinesDef.ChartDef)
		{
			m_gridLinesDef = gridlinesDef;
		}

		protected override void ResetInstanceCache()
		{
			if (m_style != null)
			{
				m_style.SetNewContext();
			}
			m_enabled = null;
			m_interval = null;
			m_intervalOffset = null;
			m_intervalType = null;
			m_intervalOffsetType = null;
		}
	}
}
