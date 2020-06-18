namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal abstract class MapVectorLayerInstance : MapLayerInstance
	{
		private MapVectorLayer m_defObject;

		internal MapVectorLayerInstance(MapVectorLayer defObject)
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
