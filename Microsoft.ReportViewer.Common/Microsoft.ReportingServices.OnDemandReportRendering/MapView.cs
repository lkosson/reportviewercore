using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal abstract class MapView
	{
		protected Map m_map;

		private Microsoft.ReportingServices.ReportIntermediateFormat.MapView m_defObject;

		protected MapViewInstance m_instance;

		private ReportDoubleProperty m_zoom;

		public ReportDoubleProperty Zoom
		{
			get
			{
				if (m_zoom == null && m_defObject.Zoom != null)
				{
					m_zoom = new ReportDoubleProperty(m_defObject.Zoom);
				}
				return m_zoom;
			}
		}

		internal Map MapDef => m_map;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.MapView MapViewDef => m_defObject;

		internal MapViewInstance Instance => GetInstance();

		internal MapView(Microsoft.ReportingServices.ReportIntermediateFormat.MapView defObject, Map map)
		{
			m_defObject = defObject;
			m_map = map;
		}

		internal abstract MapViewInstance GetInstance();

		internal virtual void SetNewContext()
		{
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
		}
	}
}
