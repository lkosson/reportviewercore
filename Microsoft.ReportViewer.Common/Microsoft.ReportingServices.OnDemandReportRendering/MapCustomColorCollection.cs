namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapCustomColorCollection : MapObjectCollectionBase<MapCustomColor>
	{
		private Map m_map;

		private MapCustomColorRule m_customColorRule;

		public override int Count => m_customColorRule.MapCustomColorRuleDef.MapCustomColors.Count;

		internal MapCustomColorCollection(MapCustomColorRule customColorRule, Map map)
		{
			m_customColorRule = customColorRule;
			m_map = map;
		}

		protected override MapCustomColor CreateMapObject(int index)
		{
			return new MapCustomColor(m_customColorRule.MapCustomColorRuleDef.MapCustomColors[index], m_map);
		}
	}
}
