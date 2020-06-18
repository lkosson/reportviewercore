using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapShapefile : MapSpatialData
	{
		private ReportStringProperty m_source;

		private MapFieldNameCollection m_mapFieldNames;

		public ReportStringProperty Source
		{
			get
			{
				if (m_source == null && MapShapefileDef.Source != null)
				{
					m_source = new ReportStringProperty(MapShapefileDef.Source);
				}
				return m_source;
			}
		}

		public MapFieldNameCollection MapFieldNames
		{
			get
			{
				if (m_mapFieldNames == null && MapShapefileDef.MapFieldNames != null)
				{
					m_mapFieldNames = new MapFieldNameCollection(MapShapefileDef.MapFieldNames, m_map);
				}
				return m_mapFieldNames;
			}
		}

		internal Microsoft.ReportingServices.ReportIntermediateFormat.MapShapefile MapShapefileDef => (Microsoft.ReportingServices.ReportIntermediateFormat.MapShapefile)base.MapSpatialDataDef;

		public new MapShapefileInstance Instance => (MapShapefileInstance)GetInstance();

		internal MapShapefile(MapVectorLayer mapVectorLayer, Map map)
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
				m_instance = new MapShapefileInstance(this);
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
			if (m_mapFieldNames != null)
			{
				m_mapFieldNames.SetNewContext();
			}
		}
	}
}
