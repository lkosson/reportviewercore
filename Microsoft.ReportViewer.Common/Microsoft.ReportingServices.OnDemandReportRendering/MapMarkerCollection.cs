namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapMarkerCollection : MapObjectCollectionBase<MapMarker>
	{
		private Map m_map;

		private MapMarkerRule m_markerRule;

		public override int Count => m_markerRule.MapMarkerRuleDef.MapMarkers.Count;

		internal MapMarkerCollection(MapMarkerRule markerRule, Map map)
		{
			m_markerRule = markerRule;
			m_map = map;
		}

		protected override MapMarker CreateMapObject(int index)
		{
			return new MapMarker(m_markerRule.MapMarkerRuleDef.MapMarkers[index], m_map);
		}
	}
}
