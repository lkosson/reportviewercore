namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartItemInLegendInstance : BaseInstance
	{
		private ChartItemInLegend m_chartItemInLegendDef;

		private string m_legendText;

		private string m_toolTip;

		private bool? m_hidden;

		public string LegendText
		{
			get
			{
				if (m_legendText == null && !m_chartItemInLegendDef.ChartDef.IsOldSnapshot)
				{
					m_legendText = m_chartItemInLegendDef.ChartItemInLegendDef.EvaluateLegendText(ReportScopeInstance, m_chartItemInLegendDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_legendText;
			}
		}

		public string ToolTip
		{
			get
			{
				if (m_toolTip == null)
				{
					m_toolTip = m_chartItemInLegendDef.ChartItemInLegendDef.EvaluateToolTip(ReportScopeInstance, m_chartItemInLegendDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_toolTip;
			}
		}

		public bool Hidden
		{
			get
			{
				if (!m_hidden.HasValue)
				{
					m_hidden = m_chartItemInLegendDef.ChartItemInLegendDef.EvaluateHidden(ReportScopeInstance, m_chartItemInLegendDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_hidden.Value;
			}
		}

		internal ChartItemInLegendInstance(ChartItemInLegend chartItemInLegendDef)
			: base(chartItemInLegendDef.ReportScope)
		{
			m_chartItemInLegendDef = chartItemInLegendDef;
		}

		protected override void ResetInstanceCache()
		{
			m_legendText = null;
			m_toolTip = null;
			m_hidden = null;
		}
	}
}
