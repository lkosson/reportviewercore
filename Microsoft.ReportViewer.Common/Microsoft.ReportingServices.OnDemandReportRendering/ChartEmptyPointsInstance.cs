namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartEmptyPointsInstance : BaseInstance
	{
		private ChartEmptyPoints m_chartEmptyPointsDef;

		private StyleInstance m_style;

		private object m_axisLabel;

		private string m_toolTip;

		public StyleInstance Style
		{
			get
			{
				if (m_style == null)
				{
					m_style = new StyleInstance(m_chartEmptyPointsDef, m_chartEmptyPointsDef.ChartDef, m_chartEmptyPointsDef.ChartDef.RenderingContext);
				}
				return m_style;
			}
		}

		public object AxisLabel
		{
			get
			{
				if (m_axisLabel == null && !m_chartEmptyPointsDef.ChartDef.IsOldSnapshot)
				{
					m_axisLabel = m_chartEmptyPointsDef.ChartEmptyPointsDef.EvaluateAxisLabel(ReportScopeInstance, m_chartEmptyPointsDef.ChartDef.RenderingContext.OdpContext).Value;
				}
				return m_axisLabel;
			}
		}

		public string ToolTip
		{
			get
			{
				if (m_toolTip == null)
				{
					m_toolTip = m_chartEmptyPointsDef.ChartEmptyPointsDef.EvaluateToolTip(ReportScopeInstance, m_chartEmptyPointsDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_toolTip;
			}
		}

		internal ChartEmptyPointsInstance(ChartEmptyPoints chartEmptyPointsDef)
			: base(chartEmptyPointsDef.ReportScope)
		{
			m_chartEmptyPointsDef = chartEmptyPointsDef;
		}

		protected override void ResetInstanceCache()
		{
			if (m_style != null)
			{
				m_style.SetNewContext();
			}
			m_axisLabel = null;
			m_toolTip = null;
		}
	}
}
