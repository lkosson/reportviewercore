using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapMarkerImageInstance : BaseInstance
	{
		private MapMarkerImage m_defObject;

		private Image.SourceType? m_source;

		private ReportColor m_transparentColor;

		private MapResizeMode? m_resizeMode;

		private ImageDataHandler m_imageDataHandler;

		public Image.SourceType Source
		{
			get
			{
				if (!m_source.HasValue)
				{
					m_source = m_defObject.MapMarkerImageDef.EvaluateSource(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return m_source.Value;
			}
		}

		public byte[] ImageData => ImageHandler.ImageData;

		public string MIMEType => ImageHandler.MIMEType;

		public ReportColor TransparentColor
		{
			get
			{
				if (m_transparentColor == null)
				{
					m_transparentColor = new ReportColor(m_defObject.MapMarkerImageDef.EvaluateTransparentColor(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext));
				}
				return m_transparentColor;
			}
		}

		public MapResizeMode ResizeMode
		{
			get
			{
				if (!m_resizeMode.HasValue)
				{
					m_resizeMode = m_defObject.MapMarkerImageDef.EvaluateResizeMode(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return m_resizeMode.Value;
			}
		}

		private ImageDataHandler ImageHandler
		{
			get
			{
				if (m_imageDataHandler == null || Source != m_imageDataHandler.Source)
				{
					m_imageDataHandler = ImageDataHandlerFactory.Create(m_defObject.MapDef, m_defObject);
				}
				return m_imageDataHandler;
			}
		}

		internal MapMarkerImageInstance(MapMarkerImage defObject)
			: base(defObject.MapDef.ReportScope)
		{
			m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			m_source = null;
			m_transparentColor = null;
			m_resizeMode = null;
			if (m_imageDataHandler != null)
			{
				m_imageDataHandler.ClearCache();
			}
		}
	}
}
