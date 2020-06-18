using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapSizeRuleInstance : MapAppearanceRuleInstance
	{
		private MapSizeRule m_defObject;

		private ReportSize m_startSize;

		private ReportSize m_endSize;

		public ReportSize StartSize
		{
			get
			{
				if (m_startSize == null)
				{
					m_startSize = new ReportSize(((Microsoft.ReportingServices.ReportIntermediateFormat.MapSizeRule)m_defObject.MapAppearanceRuleDef).EvaluateStartSize(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext));
				}
				return m_startSize;
			}
		}

		public ReportSize EndSize
		{
			get
			{
				if (m_endSize == null)
				{
					m_endSize = new ReportSize(((Microsoft.ReportingServices.ReportIntermediateFormat.MapSizeRule)m_defObject.MapAppearanceRuleDef).EvaluateEndSize(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext));
				}
				return m_endSize;
			}
		}

		internal MapSizeRuleInstance(MapSizeRule defObject)
			: base(defObject)
		{
			m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			m_startSize = null;
			m_endSize = null;
		}
	}
}
