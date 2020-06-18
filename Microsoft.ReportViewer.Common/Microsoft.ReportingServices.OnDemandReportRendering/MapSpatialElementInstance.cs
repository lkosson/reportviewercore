namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal abstract class MapSpatialElementInstance : BaseInstance
	{
		private MapSpatialElement m_defObject;

		internal MapSpatialElementInstance(MapSpatialElement defObject)
			: base(defObject.ReportScope)
		{
			m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
		}
	}
}
