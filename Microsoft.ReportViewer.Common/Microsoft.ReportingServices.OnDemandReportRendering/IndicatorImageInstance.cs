using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class IndicatorImageInstance : BaseGaugeImageInstance
	{
		private ReportColor m_hueColor;

		private double? m_transparency;

		public ReportColor HueColor
		{
			get
			{
				if (m_hueColor == null)
				{
					m_hueColor = new ReportColor(((Microsoft.ReportingServices.ReportIntermediateFormat.IndicatorImage)m_defObject.BaseGaugeImageDef).EvaluateHueColor(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext));
				}
				return m_hueColor;
			}
		}

		public double Transparency
		{
			get
			{
				if (!m_transparency.HasValue)
				{
					m_transparency = ((Microsoft.ReportingServices.ReportIntermediateFormat.IndicatorImage)m_defObject.BaseGaugeImageDef).EvaluateTransparency(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_transparency.Value;
			}
		}

		internal IndicatorImageInstance(IndicatorImage defObject)
			: base(defObject)
		{
			m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			m_hueColor = null;
			m_transparency = null;
		}
	}
}
