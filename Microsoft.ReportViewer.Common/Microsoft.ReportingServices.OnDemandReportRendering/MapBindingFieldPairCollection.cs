using Microsoft.ReportingServices.ReportIntermediateFormat;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapBindingFieldPairCollection : MapObjectCollectionBase<MapBindingFieldPair>
	{
		private Map m_map;

		private MapVectorLayer m_mapVectorLayer;

		private List<Microsoft.ReportingServices.ReportIntermediateFormat.MapBindingFieldPair> m_mapBindingFieldCollectionDef;

		public override int Count => m_mapBindingFieldCollectionDef.Count;

		internal MapBindingFieldPairCollection(MapVectorLayer mapVectorLayer, Map map)
		{
			m_mapBindingFieldCollectionDef = mapVectorLayer.MapVectorLayerDef.MapBindingFieldPairs;
			m_mapVectorLayer = mapVectorLayer;
			m_map = map;
		}

		internal MapBindingFieldPairCollection(List<Microsoft.ReportingServices.ReportIntermediateFormat.MapBindingFieldPair> mapBindingFieldCollectionDef, Map map)
		{
			m_mapBindingFieldCollectionDef = mapBindingFieldCollectionDef;
			m_mapVectorLayer = null;
			m_map = map;
		}

		protected override MapBindingFieldPair CreateMapObject(int index)
		{
			return new MapBindingFieldPair(m_mapBindingFieldCollectionDef[index], m_mapVectorLayer, m_map);
		}
	}
}
