using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapColorRangeRuleInstance : MapColorRuleInstance
	{
		private MapColorRangeRule m_defObject;

		private ReportColor m_startColor;

		private ReportColor m_middleColor;

		private ReportColor m_endColor;

		public ReportColor StartColor
		{
			get
			{
				if (m_startColor == null)
				{
					m_startColor = new ReportColor(((Microsoft.ReportingServices.ReportIntermediateFormat.MapColorRangeRule)m_defObject.MapColorRuleDef).EvaluateStartColor(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext));
				}
				return m_startColor;
			}
		}

		public ReportColor MiddleColor
		{
			get
			{
				if (m_middleColor == null)
				{
					m_middleColor = new ReportColor(((Microsoft.ReportingServices.ReportIntermediateFormat.MapColorRangeRule)m_defObject.MapColorRuleDef).EvaluateMiddleColor(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext));
				}
				return m_middleColor;
			}
		}

		public ReportColor EndColor
		{
			get
			{
				if (m_endColor == null)
				{
					m_endColor = new ReportColor(((Microsoft.ReportingServices.ReportIntermediateFormat.MapColorRangeRule)m_defObject.MapColorRuleDef).EvaluateEndColor(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext));
				}
				return m_endColor;
			}
		}

		internal MapColorRangeRuleInstance(MapColorRangeRule defObject)
			: base(defObject)
		{
			m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			m_startColor = null;
			m_middleColor = null;
			m_endColor = null;
		}
	}
}
