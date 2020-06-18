using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapViewportInstance : MapSubItemInstance
	{
		private MapViewport m_defObject;

		private MapCoordinateSystem? m_mapCoordinateSystem;

		private MapProjection? m_mapProjection;

		private double? m_projectionCenterX;

		private double? m_projectionCenterY;

		private double? m_maximumZoom;

		private double? m_minimumZoom;

		private ReportSize m_contentMargin;

		private bool? m_gridUnderContent;

		private double? m_simplificationResolution;

		public MapCoordinateSystem MapCoordinateSystem
		{
			get
			{
				if (!m_mapCoordinateSystem.HasValue)
				{
					m_mapCoordinateSystem = ((Microsoft.ReportingServices.ReportIntermediateFormat.MapViewport)m_defObject.MapSubItemDef).EvaluateMapCoordinateSystem(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return m_mapCoordinateSystem.Value;
			}
		}

		public MapProjection MapProjection
		{
			get
			{
				if (!m_mapProjection.HasValue)
				{
					m_mapProjection = ((Microsoft.ReportingServices.ReportIntermediateFormat.MapViewport)m_defObject.MapSubItemDef).EvaluateMapProjection(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return m_mapProjection.Value;
			}
		}

		public double ProjectionCenterX
		{
			get
			{
				if (!m_projectionCenterX.HasValue)
				{
					m_projectionCenterX = ((Microsoft.ReportingServices.ReportIntermediateFormat.MapViewport)m_defObject.MapSubItemDef).EvaluateProjectionCenterX(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return m_projectionCenterX.Value;
			}
		}

		public double ProjectionCenterY
		{
			get
			{
				if (!m_projectionCenterY.HasValue)
				{
					m_projectionCenterY = ((Microsoft.ReportingServices.ReportIntermediateFormat.MapViewport)m_defObject.MapSubItemDef).EvaluateProjectionCenterY(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return m_projectionCenterY.Value;
			}
		}

		public double MaximumZoom
		{
			get
			{
				if (!m_maximumZoom.HasValue)
				{
					m_maximumZoom = ((Microsoft.ReportingServices.ReportIntermediateFormat.MapViewport)m_defObject.MapSubItemDef).EvaluateMaximumZoom(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return m_maximumZoom.Value;
			}
		}

		public double MinimumZoom
		{
			get
			{
				if (!m_minimumZoom.HasValue)
				{
					m_minimumZoom = ((Microsoft.ReportingServices.ReportIntermediateFormat.MapViewport)m_defObject.MapSubItemDef).EvaluateMinimumZoom(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return m_minimumZoom.Value;
			}
		}

		public double SimplificationResolution
		{
			get
			{
				if (!m_simplificationResolution.HasValue)
				{
					m_simplificationResolution = ((Microsoft.ReportingServices.ReportIntermediateFormat.MapViewport)m_defObject.MapSubItemDef).EvaluateSimplificationResolution(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return m_simplificationResolution.Value;
			}
		}

		public ReportSize ContentMargin
		{
			get
			{
				if (m_contentMargin == null)
				{
					m_contentMargin = new ReportSize(((Microsoft.ReportingServices.ReportIntermediateFormat.MapViewport)m_defObject.MapSubItemDef).EvaluateContentMargin(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext));
				}
				return m_contentMargin;
			}
		}

		public bool GridUnderContent
		{
			get
			{
				if (!m_gridUnderContent.HasValue)
				{
					m_gridUnderContent = ((Microsoft.ReportingServices.ReportIntermediateFormat.MapViewport)m_defObject.MapSubItemDef).EvaluateGridUnderContent(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return m_gridUnderContent.Value;
			}
		}

		internal MapViewportInstance(MapViewport defObject)
			: base(defObject)
		{
			m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			m_mapCoordinateSystem = null;
			m_mapProjection = null;
			m_projectionCenterX = null;
			m_projectionCenterY = null;
			m_maximumZoom = null;
			m_minimumZoom = null;
			m_contentMargin = null;
			m_gridUnderContent = null;
			m_simplificationResolution = null;
		}
	}
}
