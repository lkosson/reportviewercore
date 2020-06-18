using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapLineTemplateInstance : MapSpatialElementTemplateInstance
	{
		private MapLineTemplate m_defObject;

		private ReportSize m_width;

		private MapLineLabelPlacement? m_labelPlacement;

		public ReportSize Width
		{
			get
			{
				if (m_width == null)
				{
					m_width = new ReportSize(((Microsoft.ReportingServices.ReportIntermediateFormat.MapLineTemplate)m_defObject.MapSpatialElementTemplateDef).EvaluateWidth(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext));
				}
				return m_width;
			}
		}

		public MapLineLabelPlacement LabelPlacement
		{
			get
			{
				if (!m_labelPlacement.HasValue)
				{
					m_labelPlacement = ((Microsoft.ReportingServices.ReportIntermediateFormat.MapLineTemplate)m_defObject.MapSpatialElementTemplateDef).EvaluateLabelPlacement(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return m_labelPlacement.Value;
			}
		}

		internal MapLineTemplateInstance(MapLineTemplate defObject)
			: base(defObject)
		{
			m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			m_width = null;
			m_labelPlacement = null;
		}
	}
}
