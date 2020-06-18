namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal abstract class MapViewInstance : BaseInstance
	{
		private MapView m_defObject;

		private double? m_zoom;

		public double Zoom
		{
			get
			{
				if (!m_zoom.HasValue)
				{
					m_zoom = m_defObject.MapViewDef.EvaluateZoom(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return m_zoom.Value;
			}
		}

		internal MapViewInstance(MapView defObject)
			: base(defObject.MapDef.ReportScope)
		{
			m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			m_zoom = null;
		}
	}
}
