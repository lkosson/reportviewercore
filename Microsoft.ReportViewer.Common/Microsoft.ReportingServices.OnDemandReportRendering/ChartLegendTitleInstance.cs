namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartLegendTitleInstance : BaseInstance
	{
		private ChartLegendTitle m_chartLegendTitleDef;

		private StyleInstance m_style;

		private string m_caption;

		private ChartSeparators? m_titleSeparator;

		public StyleInstance Style
		{
			get
			{
				if (m_style == null)
				{
					m_style = new StyleInstance(m_chartLegendTitleDef, m_chartLegendTitleDef.ChartDef, m_chartLegendTitleDef.ChartDef.RenderingContext);
				}
				return m_style;
			}
		}

		public string Caption
		{
			get
			{
				if (m_caption == null && !m_chartLegendTitleDef.ChartDef.IsOldSnapshot)
				{
					m_caption = m_chartLegendTitleDef.ChartLegendTitleDef.EvaluateCaption(ReportScopeInstance, m_chartLegendTitleDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_caption;
			}
		}

		public ChartSeparators TitleSeparator
		{
			get
			{
				if (!m_titleSeparator.HasValue && !m_chartLegendTitleDef.ChartDef.IsOldSnapshot)
				{
					m_titleSeparator = m_chartLegendTitleDef.ChartLegendTitleDef.EvaluateTitleSeparator(ReportScopeInstance, m_chartLegendTitleDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_titleSeparator.Value;
			}
		}

		internal ChartLegendTitleInstance(ChartLegendTitle chartLegendTitleDef)
			: base(chartLegendTitleDef.ChartDef)
		{
			m_chartLegendTitleDef = chartLegendTitleDef;
		}

		protected override void ResetInstanceCache()
		{
			if (m_style != null)
			{
				m_style.SetNewContext();
			}
			m_caption = null;
			m_titleSeparator = null;
		}
	}
}
