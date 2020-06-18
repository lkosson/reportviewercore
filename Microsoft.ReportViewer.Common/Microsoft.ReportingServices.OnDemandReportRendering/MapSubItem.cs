using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal abstract class MapSubItem : MapObjectCollectionItem, IROMStyleDefinitionContainer
	{
		protected Map m_map;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.MapSubItem m_defObject;

		private Style m_style;

		private MapLocation m_mapLocation;

		private MapSize m_mapSize;

		private ReportSizeProperty m_leftMargin;

		private ReportSizeProperty m_rightMargin;

		private ReportSizeProperty m_topMargin;

		private ReportSizeProperty m_bottomMargin;

		private ReportIntProperty m_zIndex;

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

		public MapLocation MapLocation
		{
			get
			{
				if (m_mapLocation == null && m_defObject.MapLocation != null)
				{
					m_mapLocation = new MapLocation(m_defObject.MapLocation, m_map);
				}
				return m_mapLocation;
			}
		}

		public MapSize MapSize
		{
			get
			{
				if (m_mapSize == null && m_defObject.MapSize != null)
				{
					m_mapSize = new MapSize(m_defObject.MapSize, m_map);
				}
				return m_mapSize;
			}
		}

		public ReportSizeProperty LeftMargin
		{
			get
			{
				if (m_leftMargin == null && m_defObject.LeftMargin != null)
				{
					m_leftMargin = new ReportSizeProperty(m_defObject.LeftMargin);
				}
				return m_leftMargin;
			}
		}

		public ReportSizeProperty RightMargin
		{
			get
			{
				if (m_rightMargin == null && m_defObject.RightMargin != null)
				{
					m_rightMargin = new ReportSizeProperty(m_defObject.RightMargin);
				}
				return m_rightMargin;
			}
		}

		public ReportSizeProperty TopMargin
		{
			get
			{
				if (m_topMargin == null && m_defObject.TopMargin != null)
				{
					m_topMargin = new ReportSizeProperty(m_defObject.TopMargin);
				}
				return m_topMargin;
			}
		}

		public ReportSizeProperty BottomMargin
		{
			get
			{
				if (m_bottomMargin == null && m_defObject.BottomMargin != null)
				{
					m_bottomMargin = new ReportSizeProperty(m_defObject.BottomMargin);
				}
				return m_bottomMargin;
			}
		}

		public ReportIntProperty ZIndex
		{
			get
			{
				if (m_zIndex == null && m_defObject.ZIndex != null)
				{
					m_zIndex = new ReportIntProperty(m_defObject.ZIndex.IsExpression, m_defObject.ZIndex.OriginalText, m_defObject.ZIndex.IntValue, 0);
				}
				return m_zIndex;
			}
		}

		internal Map MapDef => m_map;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.MapSubItem MapSubItemDef => m_defObject;

		internal MapSubItemInstance Instance => GetInstance();

		internal MapSubItem(Microsoft.ReportingServices.ReportIntermediateFormat.MapSubItem defObject, Map map)
		{
			m_defObject = defObject;
			m_map = map;
		}

		internal abstract MapSubItemInstance GetInstance();

		internal override void SetNewContext()
		{
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
			if (m_style != null)
			{
				m_style.SetNewContext();
			}
			if (m_mapLocation != null)
			{
				m_mapLocation.SetNewContext();
			}
			if (m_mapSize != null)
			{
				m_mapSize.SetNewContext();
			}
		}
	}
}
