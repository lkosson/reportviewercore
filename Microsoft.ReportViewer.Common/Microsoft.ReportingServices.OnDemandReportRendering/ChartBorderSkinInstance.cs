namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartBorderSkinInstance : BaseInstance
	{
		private ChartBorderSkin m_chartBorderSkinDef;

		private StyleInstance m_style;

		private ChartBorderSkinType? m_borderSkinType;

		public StyleInstance Style
		{
			get
			{
				if (m_style == null)
				{
					m_style = new StyleInstance(m_chartBorderSkinDef, m_chartBorderSkinDef.ChartDef, m_chartBorderSkinDef.ChartDef.RenderingContext);
				}
				return m_style;
			}
		}

		public ChartBorderSkinType BorderSkinType
		{
			get
			{
				if (!m_borderSkinType.HasValue && !m_chartBorderSkinDef.ChartDef.IsOldSnapshot)
				{
					m_borderSkinType = m_chartBorderSkinDef.ChartBorderSkinDef.EvaluateBorderSkinType(ReportScopeInstance, m_chartBorderSkinDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_borderSkinType.Value;
			}
		}

		internal ChartBorderSkinInstance(ChartBorderSkin chartBorderSkinDef)
			: base(chartBorderSkinDef.ChartDef)
		{
			m_chartBorderSkinDef = chartBorderSkinDef;
		}

		protected override void ResetInstanceCache()
		{
			if (m_style != null)
			{
				m_style.SetNewContext();
			}
			m_borderSkinType = null;
		}
	}
}
