using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapSizeRule : MapAppearanceRule
	{
		private ReportSizeProperty m_startSize;

		private ReportSizeProperty m_endSize;

		public ReportSizeProperty StartSize
		{
			get
			{
				if (m_startSize == null && MapSizeRuleDef.StartSize != null)
				{
					m_startSize = new ReportSizeProperty(MapSizeRuleDef.StartSize);
				}
				return m_startSize;
			}
		}

		public ReportSizeProperty EndSize
		{
			get
			{
				if (m_endSize == null && MapSizeRuleDef.EndSize != null)
				{
					m_endSize = new ReportSizeProperty(MapSizeRuleDef.EndSize);
				}
				return m_endSize;
			}
		}

		internal Microsoft.ReportingServices.ReportIntermediateFormat.MapSizeRule MapSizeRuleDef => (Microsoft.ReportingServices.ReportIntermediateFormat.MapSizeRule)base.MapAppearanceRuleDef;

		public new MapSizeRuleInstance Instance => (MapSizeRuleInstance)GetInstance();

		internal MapSizeRule(Microsoft.ReportingServices.ReportIntermediateFormat.MapSizeRule defObject, MapVectorLayer mapVectorLayer, Map map)
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
				m_instance = new MapSizeRuleInstance(this);
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
