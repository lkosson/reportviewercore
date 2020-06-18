using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapColorScaleTitle : IROMStyleDefinitionContainer
	{
		private Map m_map;

		private Microsoft.ReportingServices.ReportIntermediateFormat.MapColorScaleTitle m_defObject;

		private MapColorScaleTitleInstance m_instance;

		private Style m_style;

		private ReportStringProperty m_caption;

		public Style Style
		{
			get
			{
				if (m_style == null)
				{
					m_style = new Style(m_map, m_map.ReportScope, m_defObject, m_map.RenderingContext);
				}
				return m_style;
			}
		}

		public ReportStringProperty Caption
		{
			get
			{
				if (m_caption == null && m_defObject.Caption != null)
				{
					m_caption = new ReportStringProperty(m_defObject.Caption);
				}
				return m_caption;
			}
		}

		internal Map MapDef => m_map;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.MapColorScaleTitle MapColorScaleTitleDef => m_defObject;

		public MapColorScaleTitleInstance Instance
		{
			get
			{
				if (m_map.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (m_instance == null)
				{
					m_instance = new MapColorScaleTitleInstance(this);
				}
				return m_instance;
			}
		}

		internal MapColorScaleTitle(Microsoft.ReportingServices.ReportIntermediateFormat.MapColorScaleTitle defObject, Map map)
		{
			m_defObject = defObject;
			m_map = map;
		}

		internal void SetNewContext()
		{
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
			if (m_style != null)
			{
				m_style.SetNewContext();
			}
		}
	}
}
