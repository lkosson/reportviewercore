using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class CapImageInstance : BaseGaugeImageInstance
	{
		private ReportColor m_hueColor;

		private ReportSize m_offsetX;

		private ReportSize m_offsetY;

		public ReportColor HueColor
		{
			get
			{
				if (m_hueColor == null)
				{
					m_hueColor = new ReportColor(((Microsoft.ReportingServices.ReportIntermediateFormat.CapImage)m_defObject.BaseGaugeImageDef).EvaluateHueColor(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext), allowTransparency: true);
				}
				return m_hueColor;
			}
		}

		public ReportSize OffsetX
		{
			get
			{
				if (m_offsetX == null)
				{
					m_offsetX = new ReportSize(((Microsoft.ReportingServices.ReportIntermediateFormat.CapImage)m_defObject.BaseGaugeImageDef).EvaluateOffsetX(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext));
				}
				return m_offsetX;
			}
		}

		public ReportSize OffsetY
		{
			get
			{
				if (m_offsetY == null)
				{
					m_offsetY = new ReportSize(((Microsoft.ReportingServices.ReportIntermediateFormat.CapImage)m_defObject.BaseGaugeImageDef).EvaluateOffsetY(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext));
				}
				return m_offsetY;
			}
		}

		internal CapImageInstance(CapImage defObject)
			: base(defObject)
		{
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			m_hueColor = null;
			m_offsetX = null;
			m_offsetY = null;
		}
	}
}
