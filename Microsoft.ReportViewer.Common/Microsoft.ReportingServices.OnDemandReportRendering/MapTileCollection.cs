namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapTileCollection : MapObjectCollectionBase<MapTile>
	{
		private Map m_map;

		private MapTileLayer m_mapTileLayer;

		public override int Count => m_mapTileLayer.MapTileLayerDef.MapTiles.Count;

		internal MapTileCollection(MapTileLayer mapTileLayer, Map map)
		{
			m_mapTileLayer = mapTileLayer;
			m_map = map;
		}

		protected override MapTile CreateMapObject(int index)
		{
			return new MapTile(m_mapTileLayer.MapTileLayerDef.MapTiles[index], m_map);
		}
	}
}
