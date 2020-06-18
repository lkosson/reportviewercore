using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class FrameImageInstance : BaseGaugeImageInstance
	{
		private ReportColor m_hueColor;

		private double? m_transparency;

		private bool? m_clipImage;

		public ReportColor HueColor
		{
			get
			{
				if (m_hueColor == null)
				{
					m_hueColor = new ReportColor(((Microsoft.ReportingServices.ReportIntermediateFormat.FrameImage)m_defObject.BaseGaugeImageDef).EvaluateHueColor(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext), allowTransparency: true);
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
					m_transparency = ((Microsoft.ReportingServices.ReportIntermediateFormat.FrameImage)m_defObject.BaseGaugeImageDef).EvaluateTransparency(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_transparency.Value;
			}
		}

		public bool ClipImage
		{
			get
			{
				if (!m_clipImage.HasValue)
				{
					m_clipImage = ((Microsoft.ReportingServices.ReportIntermediateFormat.FrameImage)m_defObject.BaseGaugeImageDef).EvaluateClipImage(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_clipImage.Value;
			}
		}

		internal FrameImageInstance(FrameImage defObject)
			: base(defObject)
		{
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			m_hueColor = null;
			m_transparency = null;
			m_clipImage = null;
		}
	}
}
