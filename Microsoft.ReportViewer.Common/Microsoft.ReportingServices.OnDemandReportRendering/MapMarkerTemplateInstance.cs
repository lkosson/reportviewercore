namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapMarkerTemplateInstance : MapPointTemplateInstance
	{
		private MapMarkerTemplate m_defObject;

		internal MapMarkerTemplateInstance(MapMarkerTemplate defObject)
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
