using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal class BaseGaugeImageInstance : BaseInstance
	{
		protected BaseGaugeImage m_defObject;

		private Image.SourceType? m_source;

		private ReportColor m_transparentColor;

		private ImageDataHandler m_imageDataHandler;

		public Image.SourceType Source
		{
			get
			{
				if (!m_source.HasValue)
				{
					m_source = m_defObject.BaseGaugeImageDef.EvaluateSource(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_source.Value;
			}
		}

		public string MIMEType => ImageHandler.MIMEType;

		public ReportColor TransparentColor
		{
			get
			{
				if (m_transparentColor == null)
				{
					m_transparentColor = new ReportColor(m_defObject.BaseGaugeImageDef.EvaluateTransparentColor(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext));
				}
				return m_transparentColor;
			}
		}

		public byte[] ImageData => ImageHandler.ImageData;

		private ImageDataHandler ImageHandler
		{
			get
			{
				if (m_imageDataHandler == null || Source != m_imageDataHandler.Source)
				{
					m_imageDataHandler = ImageDataHandlerFactory.Create(m_defObject.GaugePanelDef, m_defObject);
				}
				return m_imageDataHandler;
			}
		}

		internal BaseGaugeImageInstance(BaseGaugeImage defObject)
			: base(defObject.GaugePanelDef)
		{
			m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			m_source = null;
			m_transparentColor = null;
			if (m_imageDataHandler != null)
			{
				m_imageDataHandler.ClearCache();
			}
		}
	}
}
