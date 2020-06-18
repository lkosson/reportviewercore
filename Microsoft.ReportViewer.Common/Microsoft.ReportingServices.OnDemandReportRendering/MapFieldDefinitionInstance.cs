namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapFieldDefinitionInstance : BaseInstance
	{
		private MapFieldDefinition m_defObject;

		internal MapFieldDefinitionInstance(MapFieldDefinition defObject)
			: base(defObject.MapDef.ReportScope)
		{
			m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
		}
	}
}
