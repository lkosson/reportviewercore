using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapPointRules
	{
		private Map m_map;

		private MapVectorLayer m_mapVectorLayer;

		private Microsoft.ReportingServices.ReportIntermediateFormat.MapPointRules m_defObject;

		private MapPointRulesInstance m_instance;

		private MapSizeRule m_mapSizeRule;

		private MapColorRule m_mapColorRule;

		private MapMarkerRule m_mapMarkerRule;

		public MapSizeRule MapSizeRule
		{
			get
			{
				if (m_mapSizeRule == null && m_defObject.MapSizeRule != null)
				{
					m_mapSizeRule = new MapSizeRule(m_defObject.MapSizeRule, m_mapVectorLayer, m_map);
				}
				return m_mapSizeRule;
			}
		}

		public MapColorRule MapColorRule
		{
			get
			{
				if (m_mapColorRule == null)
				{
					Microsoft.ReportingServices.ReportIntermediateFormat.MapColorRule mapColorRule = m_defObject.MapColorRule;
					if (mapColorRule != null)
					{
						if (mapColorRule is Microsoft.ReportingServices.ReportIntermediateFormat.MapColorRangeRule)
						{
							m_mapColorRule = new MapColorRangeRule((Microsoft.ReportingServices.ReportIntermediateFormat.MapColorRangeRule)m_defObject.MapColorRule, m_mapVectorLayer, m_map);
						}
						else if (mapColorRule is Microsoft.ReportingServices.ReportIntermediateFormat.MapColorPaletteRule)
						{
							m_mapColorRule = new MapColorPaletteRule((Microsoft.ReportingServices.ReportIntermediateFormat.MapColorPaletteRule)m_defObject.MapColorRule, m_mapVectorLayer, m_map);
						}
						else if (mapColorRule is Microsoft.ReportingServices.ReportIntermediateFormat.MapCustomColorRule)
						{
							m_mapColorRule = new MapCustomColorRule((Microsoft.ReportingServices.ReportIntermediateFormat.MapCustomColorRule)m_defObject.MapColorRule, m_mapVectorLayer, m_map);
						}
					}
				}
				return m_mapColorRule;
			}
		}

		public MapMarkerRule MapMarkerRule
		{
			get
			{
				if (m_mapMarkerRule == null && m_defObject.MapMarkerRule != null)
				{
					m_mapMarkerRule = new MapMarkerRule(m_defObject.MapMarkerRule, m_mapVectorLayer, m_map);
				}
				return m_mapMarkerRule;
			}
		}

		internal Map MapDef => m_map;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.MapPointRules MapMarkerRulesDef => m_defObject;

		public MapPointRulesInstance Instance
		{
			get
			{
				if (m_map.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (m_instance == null)
				{
					m_instance = new MapPointRulesInstance(this);
				}
				return m_instance;
			}
		}

		internal MapPointRules(Microsoft.ReportingServices.ReportIntermediateFormat.MapPointRules defObject, MapVectorLayer mapVectorLayer, Map map)
		{
			m_defObject = defObject;
			m_mapVectorLayer = mapVectorLayer;
			m_map = map;
		}

		internal void SetNewContext()
		{
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
			if (m_mapSizeRule != null)
			{
				m_mapSizeRule.SetNewContext();
			}
			if (m_mapColorRule != null)
			{
				m_mapColorRule.SetNewContext();
			}
			if (m_mapMarkerRule != null)
			{
				m_mapMarkerRule.SetNewContext();
			}
		}
	}
}
