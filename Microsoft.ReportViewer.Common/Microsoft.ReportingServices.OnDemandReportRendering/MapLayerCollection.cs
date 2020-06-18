using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapLayerCollection : MapObjectCollectionBase<MapLayer>
	{
		private Map m_map;

		public override int Count => m_map.MapDef.MapLayers.Count;

		internal MapLayerCollection(Map map)
		{
			m_map = map;
		}

		protected override MapLayer CreateMapObject(int index)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.MapLayer mapLayer = m_map.MapDef.MapLayers[index];
			if (mapLayer is Microsoft.ReportingServices.ReportIntermediateFormat.MapTileLayer)
			{
				return new MapTileLayer((Microsoft.ReportingServices.ReportIntermediateFormat.MapTileLayer)mapLayer, m_map);
			}
			if (mapLayer is Microsoft.ReportingServices.ReportIntermediateFormat.MapPolygonLayer)
			{
				return new MapPolygonLayer((Microsoft.ReportingServices.ReportIntermediateFormat.MapPolygonLayer)mapLayer, m_map);
			}
			if (mapLayer is Microsoft.ReportingServices.ReportIntermediateFormat.MapPointLayer)
			{
				return new MapPointLayer((Microsoft.ReportingServices.ReportIntermediateFormat.MapPointLayer)mapLayer, m_map);
			}
			if (mapLayer is Microsoft.ReportingServices.ReportIntermediateFormat.MapLineLayer)
			{
				return new MapLineLayer((Microsoft.ReportingServices.ReportIntermediateFormat.MapLineLayer)mapLayer, m_map);
			}
			return null;
		}
	}
}
