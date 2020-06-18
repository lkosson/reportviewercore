using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapMarkerRule : MapAppearanceRule
	{
		private MapMarkerCollection m_mapMarkers;

		public MapMarkerCollection MapMarkers
		{
			get
			{
				if (m_mapMarkers == null && MapMarkerRuleDef.MapMarkers != null)
				{
					m_mapMarkers = new MapMarkerCollection(this, m_map);
				}
				return m_mapMarkers;
			}
		}

		internal Microsoft.ReportingServices.ReportIntermediateFormat.MapMarkerRule MapMarkerRuleDef => (Microsoft.ReportingServices.ReportIntermediateFormat.MapMarkerRule)base.MapAppearanceRuleDef;

		public new MapMarkerRuleInstance Instance => (MapMarkerRuleInstance)GetInstance();

		internal MapMarkerRule(Microsoft.ReportingServices.ReportIntermediateFormat.MapMarkerRule defObject, MapVectorLayer mapVectorLayer, Map map)
			: base(defObject, mapVectorLayer, map)
		{
		}

		internal override MapAppearanceRuleInstance GetInstance()
		{
			if (m_map.RenderingContext.InstanceAccessDisallowed)
			{
				return null;
			}
			if (m_instance == null)
			{
				m_instance = new MapMarkerRuleInstance(this);
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
			if (m_mapMarkers != null)
			{
				m_mapMarkers.SetNewContext();
			}
		}
	}
}
