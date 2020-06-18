namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class IndicatorStateInstance : BaseInstance
	{
		private IndicatorState m_defObject;

		private ReportColor m_color;

		private double? m_scaleFactor;

		private GaugeStateIndicatorStyles? m_indicatorStyle;

		public ReportColor Color
		{
			get
			{
				if (m_color == null)
				{
					m_color = new ReportColor(m_defObject.IndicatorStateDef.EvaluateColor(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext));
				}
				return m_color;
			}
		}

		public double ScaleFactor
		{
			get
			{
				if (!m_scaleFactor.HasValue)
				{
					m_scaleFactor = m_defObject.IndicatorStateDef.EvaluateScaleFactor(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_scaleFactor.Value;
			}
		}

		public GaugeStateIndicatorStyles IndicatorStyle
		{
			get
			{
				if (!m_indicatorStyle.HasValue)
				{
					m_indicatorStyle = m_defObject.IndicatorStateDef.EvaluateIndicatorStyle(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_indicatorStyle.Value;
			}
		}

		internal IndicatorStateInstance(IndicatorState defObject)
			: base(defObject.GaugePanelDef.ReportScope)
		{
			m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			m_color = null;
			m_scaleFactor = null;
			m_indicatorStyle = null;
		}
	}
}
