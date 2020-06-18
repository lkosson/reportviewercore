using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapMarkerTemplate : MapPointTemplate
	{
		private MapMarker m_mapMarker;

		public MapMarker MapMarker
		{
			get
			{
				if (m_mapMarker == null && MapMarkerTemplateDef.MapMarker != null)
				{
					m_mapMarker = new MapMarker(MapMarkerTemplateDef.MapMarker, m_map);
				}
				return m_mapMarker;
			}
		}

		internal Microsoft.ReportingServices.ReportIntermediateFormat.MapMarkerTemplate MapMarkerTemplateDef => (Microsoft.ReportingServices.ReportIntermediateFormat.MapMarkerTemplate)base.MapSpatialElementTemplateDef;

		public new MapMarkerTemplateInstance Instance => (MapMarkerTemplateInstance)GetInstance();

		internal MapMarkerTemplate(Microsoft.ReportingServices.ReportIntermediateFormat.MapMarkerTemplate defObject, MapVectorLayer mapVectorLayer, Map map)
			: base(defObject, mapVectorLayer, map)
		{
		}

		internal override MapSpatialElementTemplateInstance GetInstance()
		{
			if (m_map.RenderingContext.InstanceAccessDisallowed)
			{
				return null;
			}
			if (m_instance == null)
			{
				m_instance = new MapMarkerTemplateInstance(this);
			}
			return m_instance;
		}

		internal override void SetNewContext()
		{
			base.SetNewContext();
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
			if (m_mapMarker != null)
			{
				m_mapMarker.SetNewContext();
			}
		}
	}
}
