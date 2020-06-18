using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapPolygonRules
	{
		private Map m_map;

		private MapPolygonLayer m_mapPolygonLayer;

		private Microsoft.ReportingServices.ReportIntermediateFormat.MapPolygonRules m_defObject;

		private MapPolygonRulesInstance m_instance;

		private MapColorRule m_mapColorRule;

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
							m_mapColorRule = new MapColorRangeRule((Microsoft.ReportingServices.ReportIntermediateFormat.MapColorRangeRule)m_defObject.MapColorRule, m_mapPolygonLayer, m_map);
						}
						else if (mapColorRule is Microsoft.ReportingServices.ReportIntermediateFormat.MapColorPaletteRule)
						{
							m_mapColorRule = new MapColorPaletteRule((Microsoft.ReportingServices.ReportIntermediateFormat.MapColorPaletteRule)m_defObject.MapColorRule, m_mapPolygonLayer, m_map);
						}
						else if (mapColorRule is Microsoft.ReportingServices.ReportIntermediateFormat.MapCustomColorRule)
						{
							m_mapColorRule = new MapCustomColorRule((Microsoft.ReportingServices.ReportIntermediateFormat.MapCustomColorRule)m_defObject.MapColorRule, m_mapPolygonLayer, m_map);
						}
					}
				}
				return m_mapColorRule;
			}
		}

		internal Map MapDef => m_map;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.MapPolygonRules MapPolygonRulesDef => m_defObject;

		public MapPolygonRulesInstance Instance
		{
			get
			{
				if (m_map.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (m_instance == null)
				{
					m_instance = new MapPolygonRulesInstance(this);
				}
				return m_instance;
			}
		}

		internal MapPolygonRules(Microsoft.ReportingServices.ReportIntermediateFormat.MapPolygonRules defObject, MapPolygonLayer mapPolygonLayer, Map map)
		{
			m_defObject = defObject;
			m_mapPolygonLayer = mapPolygonLayer;
			m_map = map;
		}

		internal void SetNewContext()
		{
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
			if (m_mapColorRule != null)
			{
				m_mapColorRule.SetNewContext();
			}
		}
	}
}
