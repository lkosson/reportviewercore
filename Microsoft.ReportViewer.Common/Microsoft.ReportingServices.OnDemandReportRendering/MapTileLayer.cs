using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapTileLayer : MapLayer
	{
		private ReportStringProperty m_serviceUrl;

		private ReportEnumProperty<MapTileStyle> m_tileStyle;

		private MapTileCollection m_mapTiles;

		private ReportBoolProperty m_useSecureConnection;

		public ReportStringProperty ServiceUrl
		{
			get
			{
				if (m_serviceUrl == null && MapTileLayerDef.ServiceUrl != null)
				{
					m_serviceUrl = new ReportStringProperty(MapTileLayerDef.ServiceUrl);
				}
				return m_serviceUrl;
			}
		}

		public ReportEnumProperty<MapTileStyle> TileStyle
		{
			get
			{
				if (m_tileStyle == null && MapTileLayerDef.TileStyle != null)
				{
					m_tileStyle = new ReportEnumProperty<MapTileStyle>(MapTileLayerDef.TileStyle.IsExpression, MapTileLayerDef.TileStyle.OriginalText, EnumTranslator.TranslateMapTileStyle(MapTileLayerDef.TileStyle.StringValue, null));
				}
				return m_tileStyle;
			}
		}

		public ReportBoolProperty UseSecureConnection
		{
			get
			{
				if (m_useSecureConnection == null && MapTileLayerDef.UseSecureConnection != null)
				{
					m_useSecureConnection = new ReportBoolProperty(MapTileLayerDef.UseSecureConnection);
				}
				return m_useSecureConnection;
			}
		}

		public MapTileCollection MapTiles
		{
			get
			{
				if (m_mapTiles == null && MapTileLayerDef.MapTiles != null)
				{
					m_mapTiles = new MapTileCollection(this, m_map);
				}
				return m_mapTiles;
			}
		}

		internal Microsoft.ReportingServices.ReportIntermediateFormat.MapTileLayer MapTileLayerDef => (Microsoft.ReportingServices.ReportIntermediateFormat.MapTileLayer)base.MapLayerDef;

		public new MapTileLayerInstance Instance => (MapTileLayerInstance)GetInstance();

		internal MapTileLayer(Microsoft.ReportingServices.ReportIntermediateFormat.MapTileLayer defObject, Map map)
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
				m_instance = new MapTileLayerInstance(this);
			}
			return (MapLayerInstance)m_instance;
		}

		internal override void SetNewContext()
		{
			base.SetNewContext();
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
			if (m_mapTiles != null)
			{
				m_mapTiles.SetNewContext();
			}
		}
	}
}
