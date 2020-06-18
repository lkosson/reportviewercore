namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapLineCollection : MapObjectCollectionBase<MapLine>, ISpatialElementCollection
	{
		private Map m_map;

		private MapLineLayer m_mapLineLayer;

		public override int Count => m_mapLineLayer.MapLineLayerDef.MapLines.Count;

		internal MapLineCollection(MapLineLayer mapLineLayer, Map map)
		{
			m_mapLineLayer = mapLineLayer;
			m_map = map;
		}

		protected override MapLine CreateMapObject(int index)
		{
			return new MapLine(m_mapLineLayer.MapLineLayerDef.MapLines[index], m_mapLineLayer, m_map);
		}

		MapSpatialElement ISpatialElementCollection.GetItem(int index)
		{
			return this[index];
		}
	}
}
