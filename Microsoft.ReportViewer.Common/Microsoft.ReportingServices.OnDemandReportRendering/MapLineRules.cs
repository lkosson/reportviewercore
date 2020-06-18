using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapLineRules
	{
		private Map m_map;

		private MapLineLayer m_mapLineLayer;

		private Microsoft.ReportingServices.ReportIntermediateFormat.MapLineRules m_defObject;

		private MapLineRulesInstance m_instance;

		private MapSizeRule m_mapSizeRule;

		private MapColorRule m_mapColorRule;

		public MapSizeRule MapSizeRule
		{
			get
			{
				if (m_mapSizeRule == null && m_defObject.MapSizeRule != null)
				{
					m_mapSizeRule = new MapSizeRule(m_defObject.MapSizeRule, m_mapLineLayer, m_map);
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
							m_mapColorRule = new MapColorRangeRule((Microsoft.ReportingServices.ReportIntermediateFormat.MapColorRangeRule)m_defObject.MapColorRule, m_mapLineLayer, m_map);
						}
						else if (mapColorRule is Microsoft.ReportingServices.ReportIntermediateFormat.MapColorPaletteRule)
						{
							m_mapColorRule = new MapColorPaletteRule((Microsoft.ReportingServices.ReportIntermediateFormat.MapColorPaletteRule)m_defObject.MapColorRule, m_mapLineLayer, m_map);
						}
						else if (mapColorRule is Microsoft.ReportingServices.ReportIntermediateFormat.MapCustomColorRule)
						{
							m_mapColorRule = new MapCustomColorRule((Microsoft.ReportingServices.ReportIntermediateFormat.MapCustomColorRule)m_defObject.MapColorRule, m_mapLineLayer, m_map);
						}
					}
				}
				return m_mapColorRule;
			}
		}

		internal Map MapDef => m_map;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.MapLineRules MapLineRulesDef => m_defObject;

		public MapLineRulesInstance Instance
		{
			get
			{
				if (m_map.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (m_instance == null)
				{
					m_instance = new MapLineRulesInstance(this);
				}
				return m_instance;
			}
		}

		internal MapLineRules(Microsoft.ReportingServices.ReportIntermediateFormat.MapLineRules defObject, MapLineLayer mapLineLayer, Map map)
		{
			m_defObject = defObject;
			m_mapLineLayer = mapLineLayer;
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
		}
	}
}
