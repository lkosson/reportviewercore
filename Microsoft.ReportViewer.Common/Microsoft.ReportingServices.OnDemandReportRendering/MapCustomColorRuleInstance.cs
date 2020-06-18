namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapCustomColorRuleInstance : MapColorRuleInstance
	{
		private MapCustomColorRule m_defObject;

		internal MapCustomColorRuleInstance(MapCustomColorRule defObject)
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
