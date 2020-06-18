using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapMarker : MapObjectCollectionItem
	{
		private Map m_map;

		private Microsoft.ReportingServices.ReportIntermediateFormat.MapMarker m_defObject;

		private ReportEnumProperty<MapMarkerStyle> m_mapMarkerStyle;

		private MapMarkerImage m_mapMarkerImage;

		public ReportEnumProperty<MapMarkerStyle> MapMarkerStyle
		{
			get
			{
				if (m_mapMarkerStyle == null && m_defObject.MapMarkerStyle != null)
				{
					m_mapMarkerStyle = new ReportEnumProperty<MapMarkerStyle>(m_defObject.MapMarkerStyle.IsExpression, m_defObject.MapMarkerStyle.OriginalText, EnumTranslator.TranslateMapMarkerStyle(m_defObject.MapMarkerStyle.StringValue, null));
				}
				return m_mapMarkerStyle;
			}
		}

		public MapMarkerImage MapMarkerImage
		{
			get
			{
				if (m_mapMarkerImage == null && m_defObject.MapMarkerImage != null)
				{
					m_mapMarkerImage = new MapMarkerImage(m_defObject.MapMarkerImage, m_map);
				}
				return m_mapMarkerImage;
			}
		}

		internal Map MapDef => m_map;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.MapMarker MapMarkerDef => m_defObject;

		public MapMarkerInstance Instance
		{
			get
			{
				if (m_map.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (m_instance == null)
				{
					m_instance = new MapMarkerInstance(this);
				}
				return (MapMarkerInstance)m_instance;
			}
		}

		internal MapMarker(Microsoft.ReportingServices.ReportIntermediateFormat.MapMarker defObject, Map map)
		{
			m_defObject = defObject;
			m_map = map;
		}

		internal override void SetNewContext()
		{
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
			if (m_mapMarkerImage != null)
			{
				m_mapMarkerImage.SetNewContext();
			}
		}
	}
}
