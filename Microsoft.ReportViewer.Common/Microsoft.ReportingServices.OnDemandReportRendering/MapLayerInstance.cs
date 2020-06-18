namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal abstract class MapLayerInstance : BaseInstance
	{
		private MapLayer m_defObject;

		private MapVisibilityMode? m_visibilityMode;

		private double? m_minimumZoom;

		private double? m_maximumZoom;

		private double? m_transparency;

		public MapVisibilityMode VisibilityMode
		{
			get
			{
				if (!m_visibilityMode.HasValue)
				{
					m_visibilityMode = m_defObject.MapLayerDef.EvaluateVisibilityMode(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return m_visibilityMode.Value;
			}
		}

		public double MinimumZoom
		{
			get
			{
				if (!m_minimumZoom.HasValue)
				{
					m_minimumZoom = m_defObject.MapLayerDef.EvaluateMinimumZoom(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return m_minimumZoom.Value;
			}
		}

		public double MaximumZoom
		{
			get
			{
				if (!m_maximumZoom.HasValue)
				{
					m_maximumZoom = m_defObject.MapLayerDef.EvaluateMaximumZoom(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return m_maximumZoom.Value;
			}
		}

		public double Transparency
		{
			get
			{
				if (!m_transparency.HasValue)
				{
					m_transparency = m_defObject.MapLayerDef.EvaluateTransparency(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return m_transparency.Value;
			}
		}

		internal MapLayerInstance(MapLayer defObject)
			: base(defObject.MapDef.ReportScope)
		{
			m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			m_visibilityMode = null;
			m_minimumZoom = null;
			m_maximumZoom = null;
			m_transparency = null;
		}
	}
}
