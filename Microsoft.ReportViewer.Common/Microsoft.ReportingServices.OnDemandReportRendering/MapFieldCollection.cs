namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapFieldCollection : MapObjectCollectionBase<MapField>
	{
		private Map m_map;

		private MapSpatialElement m_mapSpatialElement;

		public override int Count => m_mapSpatialElement.MapSpatialElementDef.MapFields.Count;

		internal MapFieldCollection(MapSpatialElement mapSpatialElement, Map map)
		{
			m_mapSpatialElement = mapSpatialElement;
			m_map = map;
		}

		protected override MapField CreateMapObject(int index)
		{
			return new MapField(m_mapSpatialElement.MapSpatialElementDef.MapFields[index], m_map);
		}
	}
}
