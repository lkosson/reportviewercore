using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapPolygonInstance : MapSpatialElementInstance
	{
		private MapPolygon m_defObject;

		private bool? m_useCustomPolygonTemplate;

		private bool? m_useCustomCenterPointTemplate;

		public bool UseCustomPolygonTemplate
		{
			get
			{
				if (!m_useCustomPolygonTemplate.HasValue)
				{
					m_useCustomPolygonTemplate = ((Microsoft.ReportingServices.ReportIntermediateFormat.MapPolygon)m_defObject.MapSpatialElementDef).EvaluateUseCustomPolygonTemplate(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return m_useCustomPolygonTemplate.Value;
			}
		}

		public bool UseCustomCenterPointTemplate
		{
			get
			{
				if (!m_useCustomCenterPointTemplate.HasValue)
				{
					m_useCustomCenterPointTemplate = ((Microsoft.ReportingServices.ReportIntermediateFormat.MapPolygon)m_defObject.MapSpatialElementDef).EvaluateUseCustomCenterPointTemplate(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return m_useCustomCenterPointTemplate.Value;
			}
		}

		internal MapPolygonInstance(MapPolygon defObject)
			: base(defObject)
		{
			m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			m_useCustomPolygonTemplate = null;
			m_useCustomCenterPointTemplate = null;
		}
	}
}
