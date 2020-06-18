namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartLegendCustomItemInstance : BaseInstance
	{
		private ChartLegendCustomItem m_chartLegendCustomItemDef;

		private StyleInstance m_style;

		private ChartSeparators? m_separator;

		private ReportColor m_separatorColor;

		private string m_toolTip;

		public StyleInstance Style
		{
			get
			{
				if (m_style == null)
				{
					m_style = new StyleInstance(m_chartLegendCustomItemDef, m_chartLegendCustomItemDef.ChartDef, m_chartLegendCustomItemDef.ChartDef.RenderingContext);
				}
				return m_style;
			}
		}

		public ChartSeparators Separator
		{
			get
			{
				if (!m_separator.HasValue && !m_chartLegendCustomItemDef.ChartDef.IsOldSnapshot)
				{
					m_separator = m_chartLegendCustomItemDef.ChartLegendCustomItemDef.EvaluateSeparator(ReportScopeInstance, m_chartLegendCustomItemDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_separator.Value;
			}
		}

		public ReportColor SeparatorColor
		{
			get
			{
				if (m_separatorColor == null && !m_chartLegendCustomItemDef.ChartDef.IsOldSnapshot)
				{
					m_separatorColor = new ReportColor(m_chartLegendCustomItemDef.ChartLegendCustomItemDef.EvaluateSeparatorColor(ReportScopeInstance, m_chartLegendCustomItemDef.ChartDef.RenderingContext.OdpContext), allowTransparency: true);
				}
				return m_separatorColor;
			}
		}

		public string ToolTip
		{
			get
			{
				if (m_toolTip == null && !m_chartLegendCustomItemDef.ChartDef.IsOldSnapshot)
				{
					m_toolTip = m_chartLegendCustomItemDef.ChartLegendCustomItemDef.EvaluateToolTip(ReportScopeInstance, m_chartLegendCustomItemDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_toolTip;
			}
		}

		internal ChartLegendCustomItemInstance(ChartLegendCustomItem chartLegendCustomItemDef)
			: base(chartLegendCustomItemDef.ChartDef)
		{
			m_chartLegendCustomItemDef = chartLegendCustomItemDef;
		}

		protected override void ResetInstanceCache()
		{
			if (m_style != null)
			{
				m_style.SetNewContext();
			}
			m_separator = null;
			m_separatorColor = null;
			m_toolTip = null;
		}
	}
}
