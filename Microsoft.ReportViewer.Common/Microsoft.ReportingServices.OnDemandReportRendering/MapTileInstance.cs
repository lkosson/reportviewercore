namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapTileInstance : BaseInstance
	{
		private MapTile m_defObject;

		internal MapTileInstance(MapTile defObject)
			: base(defObject.MapDef.ReportScope)
		{
			m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
		}
	}
}
