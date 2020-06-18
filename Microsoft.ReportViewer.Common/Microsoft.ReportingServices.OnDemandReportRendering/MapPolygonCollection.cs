namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapPolygonCollection : MapObjectCollectionBase<MapPolygon>, ISpatialElementCollection
	{
		private Map m_map;

		private MapPolygonLayer m_mapPolygonLayer;

		public override int Count => m_mapPolygonLayer.MapPolygonLayerDef.MapPolygons.Count;

		internal MapPolygonCollection(MapPolygonLayer mapPolygonLayer, Map map)
		{
			m_mapPolygonLayer = mapPolygonLayer;
			m_map = map;
		}

		protected override MapPolygon CreateMapObject(int index)
		{
			return new MapPolygon(m_mapPolygonLayer.MapPolygonLayerDef.MapPolygons[index], m_mapPolygonLayer, m_map);
		}

		MapSpatialElement ISpatialElementCollection.GetItem(int index)
		{
			return this[index];
		}
	}
}
