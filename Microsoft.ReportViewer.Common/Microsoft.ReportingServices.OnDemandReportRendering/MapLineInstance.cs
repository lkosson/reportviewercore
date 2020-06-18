using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapLineInstance : MapSpatialElementInstance
	{
		private MapLine m_defObject;

		private bool? m_useCustomLineTemplate;

		public bool UseCustomLineTemplate
		{
			get
			{
				if (!m_useCustomLineTemplate.HasValue)
				{
					m_useCustomLineTemplate = ((Microsoft.ReportingServices.ReportIntermediateFormat.MapLine)m_defObject.MapSpatialElementDef).EvaluateUseCustomLineTemplate(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return m_useCustomLineTemplate.Value;
			}
		}

		internal MapLineInstance(MapLine defObject)
			: base(defObject)
		{
			m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			m_useCustomLineTemplate = null;
		}
	}
}
