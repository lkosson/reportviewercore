namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartAreaInstance : BaseInstance
	{
		private ChartArea m_chartAreaDef;

		private StyleInstance m_style;

		private bool? m_hidden;

		private ChartAreaAlignOrientations? m_alignOrientation;

		private bool? m_equallySizedAxesFont;

		public StyleInstance Style
		{
			get
			{
				if (m_style == null)
				{
					m_style = new StyleInstance(m_chartAreaDef, m_chartAreaDef.ChartDef, m_chartAreaDef.ChartDef.RenderingContext);
				}
				return m_style;
			}
		}

		public bool Hidden
		{
			get
			{
				if (!m_hidden.HasValue && !m_chartAreaDef.ChartDef.IsOldSnapshot)
				{
					m_hidden = m_chartAreaDef.ChartAreaDef.EvaluateHidden(ReportScopeInstance, m_chartAreaDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_hidden.Value;
			}
		}

		public ChartAreaAlignOrientations AlignOrientation
		{
			get
			{
				if (!m_alignOrientation.HasValue && !m_chartAreaDef.ChartDef.IsOldSnapshot)
				{
					m_alignOrientation = m_chartAreaDef.ChartAreaDef.EvaluateAlignOrientation(ReportScopeInstance, m_chartAreaDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_alignOrientation.Value;
			}
		}

		public bool EquallySizedAxesFont
		{
			get
			{
				if (!m_equallySizedAxesFont.HasValue && !m_chartAreaDef.ChartDef.IsOldSnapshot)
				{
					m_equallySizedAxesFont = m_chartAreaDef.ChartAreaDef.EvaluateEquallySizedAxesFont(ReportScopeInstance, m_chartAreaDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_equallySizedAxesFont.Value;
			}
		}

		internal ChartAreaInstance(ChartArea chartAreaDef)
			: base(chartAreaDef.ChartDef)
		{
			m_chartAreaDef = chartAreaDef;
		}

		protected override void ResetInstanceCache()
		{
			if (m_style != null)
			{
				m_style.SetNewContext();
			}
			m_hidden = null;
			m_alignOrientation = null;
			m_equallySizedAxesFont = null;
		}
	}
}
