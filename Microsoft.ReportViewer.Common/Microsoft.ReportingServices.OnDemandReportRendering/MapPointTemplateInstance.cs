using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal abstract class MapPointTemplateInstance : MapSpatialElementTemplateInstance
	{
		private MapPointTemplate m_defObject;

		private ReportSize m_size;

		private MapPointLabelPlacement? m_labelPlacement;

		public ReportSize Size
		{
			get
			{
				if (m_size == null)
				{
					m_size = new ReportSize(((Microsoft.ReportingServices.ReportIntermediateFormat.MapPointTemplate)m_defObject.MapSpatialElementTemplateDef).EvaluateSize(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext));
				}
				return m_size;
			}
		}

		public MapPointLabelPlacement LabelPlacement
		{
			get
			{
				if (!m_labelPlacement.HasValue)
				{
					m_labelPlacement = ((Microsoft.ReportingServices.ReportIntermediateFormat.MapPointTemplate)m_defObject.MapSpatialElementTemplateDef).EvaluateLabelPlacement(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return m_labelPlacement.Value;
			}
		}

		internal MapPointTemplateInstance(MapPointTemplate defObject)
			: base(defObject)
		{
			m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			m_size = null;
			m_labelPlacement = null;
		}
	}
}
