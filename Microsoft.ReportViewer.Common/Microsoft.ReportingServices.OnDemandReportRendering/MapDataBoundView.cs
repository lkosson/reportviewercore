using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapDataBoundView : MapView
	{
		internal Microsoft.ReportingServices.ReportIntermediateFormat.MapDataBoundView MapDataBoundViewDef => (Microsoft.ReportingServices.ReportIntermediateFormat.MapDataBoundView)base.MapViewDef;

		public new MapDataBoundViewInstance Instance => (MapDataBoundViewInstance)GetInstance();

		internal MapDataBoundView(Microsoft.ReportingServices.ReportIntermediateFormat.MapDataBoundView defObject, Map map)
			: base(defObject, map)
		{
		}

		internal override MapViewInstance GetInstance()
		{
			if (m_map.RenderingContext.InstanceAccessDisallowed)
			{
				return null;
			}
			if (m_instance == null)
			{
				m_instance = new MapDataBoundViewInstance(this);
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
