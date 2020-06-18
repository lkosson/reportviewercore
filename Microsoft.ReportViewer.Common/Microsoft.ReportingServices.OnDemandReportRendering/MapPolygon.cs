using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapPolygon : MapSpatialElement
	{
		private ReportBoolProperty m_useCustomPolygonTemplate;

		private MapPolygonTemplate m_mapPolygonTemplate;

		private ReportBoolProperty m_useCustomCenterPointTemplate;

		private MapPointTemplate m_mapCenterPointTemplate;

		public ReportBoolProperty UseCustomPolygonTemplate
		{
			get
			{
				if (m_useCustomPolygonTemplate == null && MapPolygonDef.UseCustomPolygonTemplate != null)
				{
					m_useCustomPolygonTemplate = new ReportBoolProperty(MapPolygonDef.UseCustomPolygonTemplate);
				}
				return m_useCustomPolygonTemplate;
			}
		}

		public MapPolygonTemplate MapPolygonTemplate
		{
			get
			{
				if (m_mapPolygonTemplate == null && MapPolygonDef.MapPolygonTemplate != null)
				{
					m_mapPolygonTemplate = new MapPolygonTemplate(MapPolygonDef.MapPolygonTemplate, (MapPolygonLayer)m_mapVectorLayer, m_map);
				}
				return m_mapPolygonTemplate;
			}
		}

		public ReportBoolProperty UseCustomCenterPointTemplate
		{
			get
			{
				if (m_useCustomCenterPointTemplate == null && MapPolygonDef.UseCustomCenterPointTemplate != null)
				{
					m_useCustomCenterPointTemplate = new ReportBoolProperty(MapPolygonDef.UseCustomCenterPointTemplate);
				}
				return m_useCustomCenterPointTemplate;
			}
		}

		public MapPointTemplate MapCenterPointTemplate
		{
			get
			{
				if (m_mapCenterPointTemplate == null)
				{
					Microsoft.ReportingServices.ReportIntermediateFormat.MapPointTemplate mapCenterPointTemplate = MapPolygonDef.MapCenterPointTemplate;
					if (mapCenterPointTemplate != null && mapCenterPointTemplate is Microsoft.ReportingServices.ReportIntermediateFormat.MapMarkerTemplate)
					{
						m_mapCenterPointTemplate = new MapMarkerTemplate((Microsoft.ReportingServices.ReportIntermediateFormat.MapMarkerTemplate)mapCenterPointTemplate, m_mapVectorLayer, m_map);
					}
				}
				return m_mapCenterPointTemplate;
			}
		}

		internal Microsoft.ReportingServices.ReportIntermediateFormat.MapPolygon MapPolygonDef => (Microsoft.ReportingServices.ReportIntermediateFormat.MapPolygon)base.MapSpatialElementDef;

		public new MapPolygonInstance Instance => (MapPolygonInstance)GetInstance();

		internal MapPolygon(Microsoft.ReportingServices.ReportIntermediateFormat.MapPolygon defObject, MapPolygonLayer mapPolygonLayer, Map map)
			: base(defObject, mapPolygonLayer, map)
		{
		}

		internal override MapSpatialElementInstance GetInstance()
		{
			if (m_map.RenderingContext.InstanceAccessDisallowed)
			{
				return null;
			}
			if (m_instance == null)
			{
				m_instance = new MapPolygonInstance(this);
			}
			return (MapSpatialElementInstance)m_instance;
		}

		internal override void SetNewContext()
		{
			base.SetNewContext();
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
			if (m_mapPolygonTemplate != null)
			{
				m_mapPolygonTemplate.SetNewContext();
			}
			if (m_mapCenterPointTemplate != null)
			{
				m_mapCenterPointTemplate.SetNewContext();
			}
		}
	}
}
