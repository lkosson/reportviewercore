namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapFieldInstance : BaseInstance
	{
		private MapField m_defObject;

		internal MapFieldInstance(MapField defObject)
			: base(defObject.MapDef.ReportScope)
		{
			m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
		}
	}
}
