using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal abstract class MapColorRule : MapAppearanceRule
	{
		private ReportBoolProperty m_showInColorScale;

		public ReportBoolProperty ShowInColorScale
		{
			get
			{
				if (m_showInColorScale == null && MapColorRuleDef.ShowInColorScale != null)
				{
					m_showInColorScale = new ReportBoolProperty(MapColorRuleDef.ShowInColorScale);
				}
				return m_showInColorScale;
			}
		}

		internal Microsoft.ReportingServices.ReportIntermediateFormat.MapColorRule MapColorRuleDef => (Microsoft.ReportingServices.ReportIntermediateFormat.MapColorRule)base.MapAppearanceRuleDef;

		internal new MapColorRuleInstance Instance => (MapColorRuleInstance)GetInstance();

		internal MapColorRule(Microsoft.ReportingServices.ReportIntermediateFormat.MapColorRule defObject, MapVectorLayer mapVectorLayer, Map map)
			: base(defObject, mapVectorLayer, map)
		{
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
