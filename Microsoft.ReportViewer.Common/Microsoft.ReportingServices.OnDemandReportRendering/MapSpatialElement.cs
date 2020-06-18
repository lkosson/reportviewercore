using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal abstract class MapSpatialElement : MapObjectCollectionItem
	{
		protected Map m_map;

		protected MapVectorLayer m_mapVectorLayer;

		private Microsoft.ReportingServices.ReportIntermediateFormat.MapSpatialElement m_defObject;

		private MapFieldCollection m_mapFields;

		public string VectorData => m_defObject.VectorData;

		public MapFieldCollection MapFields
		{
			get
			{
				if (m_mapFields == null && m_defObject.MapFields != null)
				{
					m_mapFields = new MapFieldCollection(this, m_map);
				}
				return m_mapFields;
			}
		}

		internal IReportScope ReportScope => m_mapVectorLayer.ReportScope;

		internal Map MapDef => m_map;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.MapSpatialElement MapSpatialElementDef => m_defObject;

		internal MapSpatialElementInstance Instance => GetInstance();

		internal MapSpatialElement(Microsoft.ReportingServices.ReportIntermediateFormat.MapSpatialElement defObject, MapVectorLayer mapVectorLayer, Map map)
		{
			m_defObject = defObject;
			m_mapVectorLayer = mapVectorLayer;
			m_map = map;
		}

		internal abstract MapSpatialElementInstance GetInstance();

		internal override void SetNewContext()
		{
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
			if (m_mapFields != null)
			{
				m_mapFields.SetNewContext();
			}
		}
	}
}
