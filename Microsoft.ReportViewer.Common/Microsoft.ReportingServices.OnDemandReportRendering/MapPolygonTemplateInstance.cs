using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapPolygonTemplateInstance : MapSpatialElementTemplateInstance
	{
		private MapPolygonTemplate m_defObject;

		private double? m_scaleFactor;

		private double? m_centerPointOffsetX;

		private double? m_centerPointOffsetY;

		private MapAutoBool? m_showLabel;

		private MapPolygonLabelPlacement? m_labelPlacement;

		public double ScaleFactor
		{
			get
			{
				if (!m_scaleFactor.HasValue)
				{
					m_scaleFactor = ((Microsoft.ReportingServices.ReportIntermediateFormat.MapPolygonTemplate)m_defObject.MapSpatialElementTemplateDef).EvaluateScaleFactor(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return m_scaleFactor.Value;
			}
		}

		public double CenterPointOffsetX
		{
			get
			{
				if (!m_centerPointOffsetX.HasValue)
				{
					m_centerPointOffsetX = ((Microsoft.ReportingServices.ReportIntermediateFormat.MapPolygonTemplate)m_defObject.MapSpatialElementTemplateDef).EvaluateCenterPointOffsetX(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return m_centerPointOffsetX.Value;
			}
		}

		public double CenterPointOffsetY
		{
			get
			{
				if (!m_centerPointOffsetY.HasValue)
				{
					m_centerPointOffsetY = ((Microsoft.ReportingServices.ReportIntermediateFormat.MapPolygonTemplate)m_defObject.MapSpatialElementTemplateDef).EvaluateCenterPointOffsetY(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return m_centerPointOffsetY.Value;
			}
		}

		public MapAutoBool ShowLabel
		{
			get
			{
				if (!m_showLabel.HasValue)
				{
					m_showLabel = ((Microsoft.ReportingServices.ReportIntermediateFormat.MapPolygonTemplate)m_defObject.MapSpatialElementTemplateDef).EvaluateShowLabel(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return m_showLabel.Value;
			}
		}

		public MapPolygonLabelPlacement LabelPlacement
		{
			get
			{
				if (!m_labelPlacement.HasValue)
				{
					m_labelPlacement = ((Microsoft.ReportingServices.ReportIntermediateFormat.MapPolygonTemplate)m_defObject.MapSpatialElementTemplateDef).EvaluateLabelPlacement(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return m_labelPlacement.Value;
			}
		}

		internal MapPolygonTemplateInstance(MapPolygonTemplate defObject)
			: base(defObject)
		{
			m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			m_scaleFactor = null;
			m_centerPointOffsetX = null;
			m_centerPointOffsetY = null;
			m_showLabel = null;
			m_labelPlacement = null;
		}
	}
}
