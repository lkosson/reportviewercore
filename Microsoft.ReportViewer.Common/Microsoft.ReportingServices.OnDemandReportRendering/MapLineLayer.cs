using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapLineLayer : MapVectorLayer
	{
		private MapLineTemplate m_mapLineTemplate;

		private MapLineRules m_mapLineRules;

		private MapLineCollection m_mapLines;

		public MapLineTemplate MapLineTemplate
		{
			get
			{
				if (m_mapLineTemplate == null && MapLineLayerDef.MapLineTemplate != null)
				{
					m_mapLineTemplate = new MapLineTemplate(MapLineLayerDef.MapLineTemplate, this, m_map);
				}
				return m_mapLineTemplate;
			}
		}

		public MapLineRules MapLineRules
		{
			get
			{
				if (m_mapLineRules == null && MapLineLayerDef.MapLineRules != null)
				{
					m_mapLineRules = new MapLineRules(MapLineLayerDef.MapLineRules, this, m_map);
				}
				return m_mapLineRules;
			}
		}

		public MapLineCollection MapLines
		{
			get
			{
				if (m_mapLines == null && MapLineLayerDef.MapLines != null)
				{
					m_mapLines = new MapLineCollection(this, m_map);
				}
				return m_mapLines;
			}
		}

		internal Microsoft.ReportingServices.ReportIntermediateFormat.MapLineLayer MapLineLayerDef => (Microsoft.ReportingServices.ReportIntermediateFormat.MapLineLayer)base.MapLayerDef;

		public new MapLineLayerInstance Instance => (MapLineLayerInstance)GetInstance();

		internal MapLineLayer(Microsoft.ReportingServices.ReportIntermediateFormat.MapLineLayer defObject, Map map)
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
				m_instance = new MapLineLayerInstance(this);
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
			if (m_mapLineTemplate != null)
			{
				m_mapLineTemplate.SetNewContext();
			}
			if (m_mapLineRules != null)
			{
				m_mapLineRules.SetNewContext();
			}
			if (m_mapLines != null)
			{
				m_mapLines.SetNewContext();
			}
		}
	}
}
