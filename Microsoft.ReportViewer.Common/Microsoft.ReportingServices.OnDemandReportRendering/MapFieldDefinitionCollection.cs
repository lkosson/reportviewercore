using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapFieldDefinitionCollection : MapObjectCollectionBase<MapFieldDefinition>
	{
		private Map m_map;

		private MapVectorLayer m_mapVectorLayer;

		public override int Count => m_mapVectorLayer.MapVectorLayerDef.MapFieldDefinitions.Count;

		internal MapFieldDefinitionCollection(MapVectorLayer mapVectorLayer, Map map)
		{
			m_mapVectorLayer = mapVectorLayer;
			m_map = map;
		}

		protected override MapFieldDefinition CreateMapObject(int index)
		{
			return new MapFieldDefinition(m_mapVectorLayer.MapVectorLayerDef.MapFieldDefinitions[index], m_map);
		}

		internal MapFieldDefinition GetFieldDefinition(string name)
		{
			using (IEnumerator<MapFieldDefinition> enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					MapFieldDefinition current = enumerator.Current;
					if (string.CompareOrdinal(name, current.Name) == 0)
					{
						return current;
					}
				}
			}
			return null;
		}
	}
}
