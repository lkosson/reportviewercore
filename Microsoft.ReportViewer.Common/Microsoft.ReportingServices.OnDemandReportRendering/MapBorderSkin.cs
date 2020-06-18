using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapBorderSkin : IROMStyleDefinitionContainer
	{
		private Map m_map;

		private Microsoft.ReportingServices.ReportIntermediateFormat.MapBorderSkin m_defObject;

		private MapBorderSkinInstance m_instance;

		private Style m_style;

		private ReportEnumProperty<MapBorderSkinType> m_mapBorderSkinType;

		public Style Style
		{
			get
			{
				if (m_style == null)
				{
					m_style = new Style(m_map, m_map.ReportScope, m_defObject, m_map.RenderingContext);
				}
				return m_style;
			}
		}

		public ReportEnumProperty<MapBorderSkinType> MapBorderSkinType
		{
			get
			{
				if (m_mapBorderSkinType == null && m_defObject.MapBorderSkinType != null)
				{
					m_mapBorderSkinType = new ReportEnumProperty<MapBorderSkinType>(m_defObject.MapBorderSkinType.IsExpression, m_defObject.MapBorderSkinType.OriginalText, EnumTranslator.TranslateMapBorderSkinType(m_defObject.MapBorderSkinType.StringValue, null));
				}
				return m_mapBorderSkinType;
			}
		}

		internal Map MapDef => m_map;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.MapBorderSkin MapBorderSkinDef => m_defObject;

		public MapBorderSkinInstance Instance
		{
			get
			{
				if (m_map.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (m_instance == null)
				{
					m_instance = new MapBorderSkinInstance(this);
				}
				return m_instance;
			}
		}

		internal MapBorderSkin(Microsoft.ReportingServices.ReportIntermediateFormat.MapBorderSkin defObject, Map map)
		{
			m_defObject = defObject;
			m_map = map;
		}

		internal void SetNewContext()
		{
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
			if (m_style != null)
			{
				m_style.SetNewContext();
			}
		}
	}
}
