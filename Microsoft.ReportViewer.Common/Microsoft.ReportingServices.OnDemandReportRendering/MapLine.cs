using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapLine : MapSpatialElement
	{
		private ReportBoolProperty m_useCustomLineTemplate;

		private MapLineTemplate m_mapLineTemplate;

		public ReportBoolProperty UseCustomLineTemplate
		{
			get
			{
				if (m_useCustomLineTemplate == null && MapLineDef.UseCustomLineTemplate != null)
				{
					m_useCustomLineTemplate = new ReportBoolProperty(MapLineDef.UseCustomLineTemplate);
				}
				return m_useCustomLineTemplate;
			}
		}

		public MapLineTemplate MapLineTemplate
		{
			get
			{
				if (m_mapLineTemplate == null && MapLineDef.MapLineTemplate != null)
				{
					m_mapLineTemplate = new MapLineTemplate(MapLineDef.MapLineTemplate, (MapLineLayer)m_mapVectorLayer, m_map);
				}
				return m_mapLineTemplate;
			}
		}

		internal Microsoft.ReportingServices.ReportIntermediateFormat.MapLine MapLineDef => (Microsoft.ReportingServices.ReportIntermediateFormat.MapLine)base.MapSpatialElementDef;

		public new MapLineInstance Instance => (MapLineInstance)GetInstance();

		internal MapLine(Microsoft.ReportingServices.ReportIntermediateFormat.MapLine defObject, MapLineLayer mapVectorLayer, Map map)
			: base(defObject, mapVectorLayer, map)
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
				m_instance = new MapLineInstance(this);
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
			if (m_mapLineTemplate != null)
			{
				m_mapLineTemplate.SetNewContext();
			}
		}
	}
}
