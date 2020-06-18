namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapMarkerRuleInstance : MapAppearanceRuleInstance
	{
		private MapMarkerRule m_defObject;

		internal MapMarkerRuleInstance(MapMarkerRule defObject)
			: base(defObject)
		{
			m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
		}
	}
}
