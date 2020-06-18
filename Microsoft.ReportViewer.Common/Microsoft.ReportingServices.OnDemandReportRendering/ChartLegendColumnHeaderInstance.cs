namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartLegendColumnHeaderInstance : BaseInstance
	{
		private ChartLegendColumnHeader m_chartLegendColumnHeaderDef;

		private StyleInstance m_style;

		private string m_value;

		public StyleInstance Style
		{
			get
			{
				if (m_style == null)
				{
					m_style = new StyleInstance(m_chartLegendColumnHeaderDef, m_chartLegendColumnHeaderDef.ChartDef, m_chartLegendColumnHeaderDef.ChartDef.RenderingContext);
				}
				return m_style;
			}
		}

		public string Value
		{
			get
			{
				if (m_value == null && !m_chartLegendColumnHeaderDef.ChartDef.IsOldSnapshot)
				{
					m_value = m_chartLegendColumnHeaderDef.ChartLegendColumnHeaderDef.EvaluateValue(ReportScopeInstance, m_chartLegendColumnHeaderDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_value;
			}
		}

		internal ChartLegendColumnHeaderInstance(ChartLegendColumnHeader chartLegendColumnHeaderDef)
			: base(chartLegendColumnHeaderDef.ChartDef)
		{
			m_chartLegendColumnHeaderDef = chartLegendColumnHeaderDef;
		}

		protected override void ResetInstanceCache()
		{
			if (m_style != null)
			{
				m_style.SetNewContext();
			}
			m_value = null;
		}
	}
}
