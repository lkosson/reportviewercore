using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapSpatialDataRegion : MapSpatialData
	{
		private ReportVariantProperty m_vectorData;

		public ReportVariantProperty VectorData
		{
			get
			{
				if (m_vectorData == null && MapSpatialDataRegionDef.VectorData != null)
				{
					m_vectorData = new ReportVariantProperty(MapSpatialDataRegionDef.VectorData);
				}
				return m_vectorData;
			}
		}

		internal IReportScope ReportScope => m_mapVectorLayer.ReportScope;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.MapSpatialDataRegion MapSpatialDataRegionDef => (Microsoft.ReportingServices.ReportIntermediateFormat.MapSpatialDataRegion)base.MapSpatialDataDef;

		public new MapSpatialDataRegionInstance Instance => (MapSpatialDataRegionInstance)GetInstance();

		internal MapSpatialDataRegion(MapVectorLayer mapVectorLayer, Map map)
			: base(mapVectorLayer, map)
		{
		}

		internal override MapSpatialDataInstance GetInstance()
		{
			if (m_map.RenderingContext.InstanceAccessDisallowed)
			{
				return null;
			}
			if (m_instance == null)
			{
				m_instance = new MapSpatialDataRegionInstance(this);
			}
			return m_instance;
		}

		internal override void SetNewContext()
		{
			base.SetNewContext();
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
		}
	}
}
