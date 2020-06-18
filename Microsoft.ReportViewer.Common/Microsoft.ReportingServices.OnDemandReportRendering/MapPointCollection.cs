namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapPointCollection : MapObjectCollectionBase<MapPoint>, ISpatialElementCollection
	{
		private Map m_map;

		private MapPointLayer m_mapPointLayer;

		public override int Count => m_mapPointLayer.MapPointLayerDef.MapPoints.Count;

		internal MapPointCollection(MapPointLayer mapPointLayer, Map map)
		{
			m_mapPointLayer = mapPointLayer;
			m_map = map;
		}

		protected override MapPoint CreateMapObject(int index)
		{
			return new MapPoint(m_mapPointLayer.MapPointLayerDef.MapPoints[index], m_mapPointLayer, m_map);
		}

		MapSpatialElement ISpatialElementCollection.GetItem(int index)
		{
			return this[index];
		}
	}
}
