using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapDataRegionCollection : MapObjectCollectionBase<MapDataRegion>
	{
		private Map m_map;

		public MapDataRegion this[string name]
		{
			get
			{
				for (int i = 0; i < Count; i++)
				{
					Microsoft.ReportingServices.ReportIntermediateFormat.MapDataRegion mapDataRegion = m_map.MapDef.MapDataRegions[i];
					if (string.CompareOrdinal(name, mapDataRegion.Name) == 0)
					{
						return this[i];
					}
				}
				throw new RenderingObjectModelException(ProcessingErrorCode.rsNotInCollection, name);
			}
		}

		public override int Count
		{
			get
			{
				if (m_map.MapDef.MapDataRegions != null)
				{
					return m_map.MapDef.MapDataRegions.Count;
				}
				return 0;
			}
		}

		internal MapDataRegionCollection(Map map)
		{
			m_map = map;
		}

		protected override MapDataRegion CreateMapObject(int index)
		{
			return new MapDataRegion(m_map, index, m_map.MapDef.MapDataRegions[index], m_map.RenderingContext);
		}
	}
}
