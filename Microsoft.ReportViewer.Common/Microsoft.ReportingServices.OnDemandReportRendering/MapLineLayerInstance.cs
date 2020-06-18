namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapLineLayerInstance : MapVectorLayerInstance
	{
		private MapLineLayer m_defObject;

		internal MapLineLayerInstance(MapLineLayer defObject)
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
