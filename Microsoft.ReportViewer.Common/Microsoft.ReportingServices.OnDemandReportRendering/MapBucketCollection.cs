namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapBucketCollection : MapObjectCollectionBase<MapBucket>
	{
		private Map m_map;

		private MapAppearanceRule m_mapApperanceRule;

		public override int Count => m_mapApperanceRule.MapAppearanceRuleDef.MapBuckets.Count;

		internal MapBucketCollection(MapAppearanceRule mapApperanceRule, Map map)
		{
			m_mapApperanceRule = mapApperanceRule;
			m_map = map;
		}

		protected override MapBucket CreateMapObject(int index)
		{
			return new MapBucket(m_mapApperanceRule.MapAppearanceRuleDef.MapBuckets[index], m_map);
		}
	}
}
