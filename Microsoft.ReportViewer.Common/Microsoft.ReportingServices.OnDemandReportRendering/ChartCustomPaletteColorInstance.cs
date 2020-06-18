namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartCustomPaletteColorInstance : BaseInstance
	{
		private ChartCustomPaletteColor m_chartCustomPaletteColorDef;

		private bool m_colorEvaluated;

		private ReportColor m_color;

		public ReportColor Color
		{
			get
			{
				if (!m_colorEvaluated)
				{
					m_colorEvaluated = true;
					if (!m_chartCustomPaletteColorDef.ChartDef.IsOldSnapshot)
					{
						m_color = new ReportColor(m_chartCustomPaletteColorDef.ChartCustomPaletteColorDef.EvaluateColor(ReportScopeInstance, m_chartCustomPaletteColorDef.ChartDef.RenderingContext.OdpContext), allowTransparency: true);
					}
				}
				return m_color;
			}
		}

		internal ChartCustomPaletteColorInstance(ChartCustomPaletteColor chartCustomPaletteColorDef)
			: base(chartCustomPaletteColorDef.ChartDef)
		{
			m_chartCustomPaletteColorDef = chartCustomPaletteColorDef;
		}

		protected override void ResetInstanceCache()
		{
			m_colorEvaluated = false;
		}
	}
}
