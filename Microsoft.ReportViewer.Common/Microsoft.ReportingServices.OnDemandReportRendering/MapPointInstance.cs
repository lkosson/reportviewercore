using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapPointInstance : MapSpatialElementInstance
	{
		private MapPoint m_defObject;

		private bool? m_useCustomPointTemplate;

		public bool UseCustomPointTemplate
		{
			get
			{
				if (!m_useCustomPointTemplate.HasValue)
				{
					m_useCustomPointTemplate = ((Microsoft.ReportingServices.ReportIntermediateFormat.MapPoint)m_defObject.MapSpatialElementDef).EvaluateUseCustomPointTemplate(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return m_useCustomPointTemplate.Value;
			}
		}

		internal MapPointInstance(MapPoint defObject)
			: base(defObject)
		{
			m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			m_useCustomPointTemplate = null;
		}
	}
}
