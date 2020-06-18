namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapMarkerInstance : BaseInstance
	{
		private MapMarker m_defObject;

		private MapMarkerStyle? m_mapMarkerStyle;

		public MapMarkerStyle MapMarkerStyle
		{
			get
			{
				if (!m_mapMarkerStyle.HasValue)
				{
					m_mapMarkerStyle = m_defObject.MapMarkerDef.EvaluateMapMarkerStyle(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return m_mapMarkerStyle.Value;
			}
		}

		internal MapMarkerInstance(MapMarker defObject)
			: base(defObject.MapDef.ReportScope)
		{
			m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			m_mapMarkerStyle = null;
		}
	}
}
