namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapPointRulesInstance : BaseInstance
	{
		private MapPointRules m_defObject;

		internal MapPointRulesInstance(MapPointRules defObject)
			: base(defObject.MapDef.ReportScope)
		{
			m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
		}
	}
}
