namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapLineRulesInstance : BaseInstance
	{
		private MapLineRules m_defObject;

		internal MapLineRulesInstance(MapLineRules defObject)
			: base(defObject.MapDef.ReportScope)
		{
			m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
		}
	}
}
