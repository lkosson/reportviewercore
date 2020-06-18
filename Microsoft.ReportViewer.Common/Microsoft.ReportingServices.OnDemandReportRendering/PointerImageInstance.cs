using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class PointerImageInstance : BaseGaugeImageInstance
	{
		private ReportColor m_hueColor;

		private double? m_transparency;

		private ReportSize m_offsetX;

		private ReportSize m_offsetY;

		public ReportColor HueColor
		{
			get
			{
				if (m_hueColor == null)
				{
					m_hueColor = new ReportColor(((Microsoft.ReportingServices.ReportIntermediateFormat.PointerImage)m_defObject.BaseGaugeImageDef).EvaluateHueColor(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext), allowTransparency: true);
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
					m_transparency = ((Microsoft.ReportingServices.ReportIntermediateFormat.PointerImage)m_defObject.BaseGaugeImageDef).EvaluateTransparency(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_transparency.Value;
			}
		}

		public ReportSize OffsetX
		{
			get
			{
				if (m_offsetX == null)
				{
					m_offsetX = new ReportSize(((Microsoft.ReportingServices.ReportIntermediateFormat.PointerImage)m_defObject.BaseGaugeImageDef).EvaluateOffsetX(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext));
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
					m_offsetY = new ReportSize(((Microsoft.ReportingServices.ReportIntermediateFormat.PointerImage)m_defObject.BaseGaugeImageDef).EvaluateOffsetY(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext));
				}
				return m_offsetY;
			}
		}

		internal PointerImageInstance(PointerImage defObject)
			: base(defObject)
		{
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			m_hueColor = null;
			m_transparency = null;
			m_offsetX = null;
			m_offsetY = null;
		}
	}
}
