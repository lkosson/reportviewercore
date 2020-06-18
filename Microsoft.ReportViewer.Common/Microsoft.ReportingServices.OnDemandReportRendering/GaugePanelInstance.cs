using Microsoft.ReportingServices.ReportIntermediateFormat;
using System.IO;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class GaugePanelInstance : DynamicImageInstance, IDynamicImageInstance
	{
		private GaugeAntiAliasings? m_antiAliasing;

		private bool? m_autoLayout;

		private double? m_shadowIntensity;

		private TextAntiAliasingQualities? m_textAntiAliasingQuality;

		public GaugeAntiAliasings AntiAliasing
		{
			get
			{
				if (!m_antiAliasing.HasValue)
				{
					m_antiAliasing = ((Microsoft.ReportingServices.ReportIntermediateFormat.GaugePanel)m_reportElementDef.ReportItemDef).EvaluateAntiAliasing(this, m_reportElementDef.RenderingContext.OdpContext);
				}
				return m_antiAliasing.Value;
			}
		}

		public bool AutoLayout
		{
			get
			{
				if (!m_autoLayout.HasValue)
				{
					m_autoLayout = ((Microsoft.ReportingServices.ReportIntermediateFormat.GaugePanel)m_reportElementDef.ReportItemDef).EvaluateAutoLayout(this, m_reportElementDef.RenderingContext.OdpContext);
				}
				return m_autoLayout.Value;
			}
		}

		public double ShadowIntensity
		{
			get
			{
				if (!m_shadowIntensity.HasValue)
				{
					m_shadowIntensity = ((Microsoft.ReportingServices.ReportIntermediateFormat.GaugePanel)m_reportElementDef.ReportItemDef).EvaluateShadowIntensity(this, m_reportElementDef.RenderingContext.OdpContext);
				}
				return m_shadowIntensity.Value;
			}
		}

		public TextAntiAliasingQualities TextAntiAliasingQuality
		{
			get
			{
				if (!m_textAntiAliasingQuality.HasValue)
				{
					m_textAntiAliasingQuality = ((Microsoft.ReportingServices.ReportIntermediateFormat.GaugePanel)m_reportElementDef.ReportItemDef).EvaluateTextAntiAliasingQuality(this, m_reportElementDef.RenderingContext.OdpContext);
				}
				return m_textAntiAliasingQuality.Value;
			}
		}

		internal GaugePanelInstance(GaugePanel reportItemDef)
			: base(reportItemDef)
		{
		}

		protected override void GetImage(ImageType type, out ActionInfoWithDynamicImageMapCollection actionImageMaps, out Stream image)
		{
			using (IGaugeMapper gaugeMapper = GaugeMapperFactory.CreateGaugeMapperInstance((GaugePanel)m_reportElementDef, GetDefaultFontFamily()))
			{
				gaugeMapper.DpiX = m_dpiX;
				gaugeMapper.DpiY = m_dpiY;
				gaugeMapper.WidthOverride = m_widthOverride;
				gaugeMapper.HeightOverride = m_heightOverride;
				gaugeMapper.RenderGaugePanel();
				image = gaugeMapper.GetImage(type);
				actionImageMaps = gaugeMapper.GetImageMaps();
			}
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			m_antiAliasing = null;
			m_autoLayout = null;
			m_shadowIntensity = null;
			m_textAntiAliasingQuality = null;
		}

		public Stream GetCoreXml()
		{
			Stream stream = null;
			using (IGaugeMapper gaugeMapper = GaugeMapperFactory.CreateGaugeMapperInstance((GaugePanel)m_reportElementDef, GetDefaultFontFamily()))
			{
				gaugeMapper.DpiX = m_dpiX;
				gaugeMapper.DpiY = m_dpiY;
				gaugeMapper.WidthOverride = m_widthOverride;
				gaugeMapper.HeightOverride = m_heightOverride;
				gaugeMapper.RenderGaugePanel();
				return gaugeMapper.GetCoreXml();
			}
		}
	}
}
