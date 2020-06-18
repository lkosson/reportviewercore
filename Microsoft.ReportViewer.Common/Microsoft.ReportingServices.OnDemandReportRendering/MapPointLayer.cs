using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapPointLayer : MapVectorLayer
	{
		private MapPointTemplate m_mapPointTemplate;

		private MapPointRules m_mapPointRules;

		private MapPointCollection m_mapPoints;

		public MapPointTemplate MapPointTemplate
		{
			get
			{
				if (m_mapPointTemplate == null)
				{
					Microsoft.ReportingServices.ReportIntermediateFormat.MapPointTemplate mapPointTemplate = MapPointLayerDef.MapPointTemplate;
					if (mapPointTemplate != null && mapPointTemplate is Microsoft.ReportingServices.ReportIntermediateFormat.MapMarkerTemplate)
					{
						m_mapPointTemplate = new MapMarkerTemplate((Microsoft.ReportingServices.ReportIntermediateFormat.MapMarkerTemplate)mapPointTemplate, this, m_map);
					}
				}
				return m_mapPointTemplate;
			}
		}

		public MapPointRules MapPointRules
		{
			get
			{
				if (m_mapPointRules == null && MapPointLayerDef.MapPointRules != null)
				{
					m_mapPointRules = new MapPointRules(MapPointLayerDef.MapPointRules, this, m_map);
				}
				return m_mapPointRules;
			}
		}

		public MapPointCollection MapPoints
		{
			get
			{
				if (m_mapPoints == null && MapPointLayerDef.MapPoints != null)
				{
					m_mapPoints = new MapPointCollection(this, m_map);
				}
				return m_mapPoints;
			}
		}

		internal Microsoft.ReportingServices.ReportIntermediateFormat.MapPointLayer MapPointLayerDef => (Microsoft.ReportingServices.ReportIntermediateFormat.MapPointLayer)base.MapLayerDef;

		public new MapPointLayerInstance Instance => (MapPointLayerInstance)GetInstance();

		internal MapPointLayer(Microsoft.ReportingServices.ReportIntermediateFormat.MapPointLayer defObject, Map map)
			: base(defObject, map)
		{
		}

		internal override MapLayerInstance GetInstance()
		{
			if (m_map.RenderingContext.InstanceAccessDisallowed)
			{
				return null;
			}
			if (m_instance == null)
			{
				m_instance = new MapPointLayerInstance(this);
			}
			return (MapVectorLayerInstance)m_instance;
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
			if (m_mapPointRules != null)
			{
				m_mapPointRules.SetNewContext();
			}
			if (m_mapPoints != null)
			{
				m_mapPoints.SetNewContext();
			}
		}
	}
}
