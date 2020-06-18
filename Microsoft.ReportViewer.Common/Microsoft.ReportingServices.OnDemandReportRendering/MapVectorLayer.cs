using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal abstract class MapVectorLayer : MapLayer
	{
		private IReportScope m_reportScope;

		private MapBindingFieldPairCollection m_mapBindingFieldPairs;

		private MapFieldDefinitionCollection m_mapFieldDefinitions;

		private MapSpatialData m_mapSpatialData;

		private MapDataRegion m_mapDataRegion;

		public string DataElementName => MapVectorLayerDef.DataElementName;

		public DataElementOutputTypes DataElementOutput => MapVectorLayerDef.DataElementOutput;

		public string MapDataRegionName => MapVectorLayerDef.MapDataRegionName;

		public MapBindingFieldPairCollection MapBindingFieldPairs
		{
			get
			{
				if (m_mapBindingFieldPairs == null && MapVectorLayerDef.MapBindingFieldPairs != null)
				{
					m_mapBindingFieldPairs = new MapBindingFieldPairCollection(this, m_map);
				}
				return m_mapBindingFieldPairs;
			}
		}

		public MapFieldDefinitionCollection MapFieldDefinitions
		{
			get
			{
				if (m_mapFieldDefinitions == null && MapVectorLayerDef.MapFieldDefinitions != null)
				{
					m_mapFieldDefinitions = new MapFieldDefinitionCollection(this, m_map);
				}
				return m_mapFieldDefinitions;
			}
		}

		public MapSpatialData MapSpatialData
		{
			get
			{
				if (m_mapSpatialData == null)
				{
					Microsoft.ReportingServices.ReportIntermediateFormat.MapSpatialData mapSpatialData = MapVectorLayerDef.MapSpatialData;
					if (mapSpatialData != null)
					{
						if (mapSpatialData is Microsoft.ReportingServices.ReportIntermediateFormat.MapShapefile)
						{
							m_mapSpatialData = new MapShapefile(this, m_map);
						}
						else if (mapSpatialData is Microsoft.ReportingServices.ReportIntermediateFormat.MapSpatialDataSet)
						{
							m_mapSpatialData = new MapSpatialDataSet(this, m_map);
						}
						else if (mapSpatialData is Microsoft.ReportingServices.ReportIntermediateFormat.MapSpatialDataRegion)
						{
							m_mapSpatialData = new MapSpatialDataRegion(this, m_map);
						}
					}
				}
				return m_mapSpatialData;
			}
		}

		internal IReportScope ReportScope
		{
			get
			{
				if (m_reportScope == null)
				{
					if (MapDataRegionName != null)
					{
						m_reportScope = m_map.MapDataRegions[MapDataRegionName].InnerMostMapMember;
					}
					else
					{
						m_reportScope = m_map.ReportScope;
					}
				}
				return m_reportScope;
			}
		}

		public MapDataRegion MapDataRegion
		{
			get
			{
				if (MapDataRegionName != null && m_mapDataRegion == null)
				{
					m_mapDataRegion = base.MapDef.MapDataRegions[MapDataRegionName];
				}
				return m_mapDataRegion;
			}
		}

		internal Microsoft.ReportingServices.ReportIntermediateFormat.MapVectorLayer MapVectorLayerDef => (Microsoft.ReportingServices.ReportIntermediateFormat.MapVectorLayer)base.MapLayerDef;

		internal new MapVectorLayerInstance Instance => (MapVectorLayerInstance)GetInstance();

		internal MapVectorLayer(Microsoft.ReportingServices.ReportIntermediateFormat.MapVectorLayer defObject, Map map)
			: base(defObject, map)
		{
		}

		internal override void SetNewContext()
		{
			base.SetNewContext();
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
			if (m_mapBindingFieldPairs != null)
			{
				m_mapBindingFieldPairs.SetNewContext();
			}
			if (m_mapFieldDefinitions != null)
			{
				m_mapFieldDefinitions.SetNewContext();
			}
			if (m_mapSpatialData != null)
			{
				m_mapSpatialData.SetNewContext();
			}
		}
	}
}
