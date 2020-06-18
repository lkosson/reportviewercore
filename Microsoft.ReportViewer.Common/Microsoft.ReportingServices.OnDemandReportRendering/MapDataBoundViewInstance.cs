namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapDataBoundViewInstance : MapViewInstance
	{
		private MapDataBoundView m_defObject;

		internal MapDataBoundViewInstance(MapDataBoundView defObject)
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
