namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapTitleCollection : MapObjectCollectionBase<MapTitle>
	{
		private Map m_map;

		public override int Count => m_map.MapDef.MapTitles.Count;

		internal MapTitleCollection(Map map)
		{
			m_map = map;
		}

		protected override MapTitle CreateMapObject(int index)
		{
			return new MapTitle(m_map.MapDef.MapTitles[index], m_map);
		}
	}
}
