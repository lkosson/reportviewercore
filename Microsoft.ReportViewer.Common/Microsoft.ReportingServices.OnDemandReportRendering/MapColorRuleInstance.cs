using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal abstract class MapColorRuleInstance : MapAppearanceRuleInstance
	{
		private MapColorRule m_defObject;

		private bool? m_showInColorScale;

		public bool ShowInColorScale
		{
			get
			{
				if (!m_showInColorScale.HasValue)
				{
					m_showInColorScale = ((Microsoft.ReportingServices.ReportIntermediateFormat.MapColorRule)m_defObject.MapAppearanceRuleDef).EvaluateShowInColorScale(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return m_showInColorScale.Value;
			}
		}

		internal MapColorRuleInstance(MapColorRule defObject)
			: base(defObject)
		{
			m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			m_showInColorScale = null;
		}
	}
}
