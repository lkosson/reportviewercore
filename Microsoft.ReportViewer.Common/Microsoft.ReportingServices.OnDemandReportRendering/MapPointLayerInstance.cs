namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapPointLayerInstance : MapVectorLayerInstance
	{
		private MapPointLayer m_defObject;

		internal MapPointLayerInstance(MapPointLayer defObject)
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
