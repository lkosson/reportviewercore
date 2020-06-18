using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapTile : MapObjectCollectionItem
	{
		private Map m_map;

		private Microsoft.ReportingServices.ReportIntermediateFormat.MapTile m_defObject;

		public string Name => m_defObject.Name;

		public string TileData => m_defObject.TileData;

		public string MIMEType => m_defObject.MIMEType;

		internal Map MapDef => m_map;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.MapTile MapTileDef => m_defObject;

		public MapTileInstance Instance
		{
			get
			{
				if (m_map.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (m_instance == null)
				{
					m_instance = new MapTileInstance(this);
				}
				return (MapTileInstance)m_instance;
			}
		}

		internal MapTile(Microsoft.ReportingServices.ReportIntermediateFormat.MapTile defObject, Map map)
		{
			m_defObject = defObject;
			m_map = map;
		}

		internal override void SetNewContext()
		{
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
		}
	}
}
