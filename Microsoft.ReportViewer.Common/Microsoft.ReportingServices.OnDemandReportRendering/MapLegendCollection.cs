namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapLegendCollection : MapObjectCollectionBase<MapLegend>
	{
		private Map m_map;

		public override int Count => m_map.MapDef.MapLegends.Count;

		internal MapLegendCollection(Map map)
		{
			m_map = map;
		}

		protected override MapLegend CreateMapObject(int index)
		{
			return new MapLegend(m_map.MapDef.MapLegends[index], m_map);
		}
	}
}
