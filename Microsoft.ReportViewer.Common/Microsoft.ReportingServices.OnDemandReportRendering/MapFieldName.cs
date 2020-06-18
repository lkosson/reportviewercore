using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapFieldName : MapObjectCollectionItem
	{
		private Map m_map;

		private Microsoft.ReportingServices.ReportIntermediateFormat.MapFieldName m_defObject;

		private ReportStringProperty m_name;

		public ReportStringProperty Name
		{
			get
			{
				if (m_name == null && m_defObject.Name != null)
				{
					m_name = new ReportStringProperty(m_defObject.Name);
				}
				return m_name;
			}
		}

		internal Map MapDef => m_map;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.MapFieldName MapFieldNameDef => m_defObject;

		public MapFieldNameInstance Instance
		{
			get
			{
				if (m_map.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (m_instance == null)
				{
					m_instance = new MapFieldNameInstance(this);
				}
				return (MapFieldNameInstance)m_instance;
			}
		}

		internal MapFieldName(Microsoft.ReportingServices.ReportIntermediateFormat.MapFieldName defObject, Map map)
		{
			m_defObject = defObject;
			m_map = map;
		}

		internal override void SetNewContext()
		{
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
		}
	}
}
