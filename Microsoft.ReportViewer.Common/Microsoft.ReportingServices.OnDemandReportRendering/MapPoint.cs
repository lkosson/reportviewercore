using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapPoint : MapSpatialElement
	{
		private ReportBoolProperty m_useCustomPointTemplate;

		private MapPointTemplate m_mapPointTemplate;

		public ReportBoolProperty UseCustomPointTemplate
		{
			get
			{
				if (m_useCustomPointTemplate == null && MapPointDef.UseCustomPointTemplate != null)
				{
					m_useCustomPointTemplate = new ReportBoolProperty(MapPointDef.UseCustomPointTemplate);
				}
				return m_useCustomPointTemplate;
			}
		}

		public MapPointTemplate MapPointTemplate
		{
			get
			{
				if (m_mapPointTemplate == null)
				{
					Microsoft.ReportingServices.ReportIntermediateFormat.MapPointTemplate mapPointTemplate = MapPointDef.MapPointTemplate;
					if (mapPointTemplate != null && mapPointTemplate is Microsoft.ReportingServices.ReportIntermediateFormat.MapMarkerTemplate)
					{
						m_mapPointTemplate = new MapMarkerTemplate((Microsoft.ReportingServices.ReportIntermediateFormat.MapMarkerTemplate)mapPointTemplate, m_mapVectorLayer, m_map);
					}
				}
				return m_mapPointTemplate;
			}
		}

		internal Microsoft.ReportingServices.ReportIntermediateFormat.MapPoint MapPointDef => (Microsoft.ReportingServices.ReportIntermediateFormat.MapPoint)base.MapSpatialElementDef;

		public new MapPointInstance Instance => (MapPointInstance)GetInstance();

		internal MapPoint(Microsoft.ReportingServices.ReportIntermediateFormat.MapPoint defObject, MapPointLayer mapPointLayer, Map map)
			: base(defObject, mapPointLayer, map)
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
				m_instance = new MapPointInstance(this);
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
			if (m_mapPointTemplate != null)
			{
				m_mapPointTemplate.SetNewContext();
			}
		}
	}
}
