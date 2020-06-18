using Microsoft.ReportingServices.ReportIntermediateFormat;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapFieldNameCollection : MapObjectCollectionBase<MapFieldName>
	{
		private Map m_map;

		private List<Microsoft.ReportingServices.ReportIntermediateFormat.MapFieldName> m_mapFieldNames;

		public override int Count => m_mapFieldNames.Count;

		internal MapFieldNameCollection(List<Microsoft.ReportingServices.ReportIntermediateFormat.MapFieldName> mapFieldNames, Map map)
		{
			m_mapFieldNames = mapFieldNames;
			m_map = map;
		}

		protected override MapFieldName CreateMapObject(int index)
		{
			return new MapFieldName(m_mapFieldNames[index], m_map);
		}
	}
}
