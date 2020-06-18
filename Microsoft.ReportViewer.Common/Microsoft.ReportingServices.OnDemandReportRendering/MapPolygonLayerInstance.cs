namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapPolygonLayerInstance : MapVectorLayerInstance
	{
		private MapPolygonLayer m_defObject;

		internal MapPolygonLayerInstance(MapPolygonLayer defObject)
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
