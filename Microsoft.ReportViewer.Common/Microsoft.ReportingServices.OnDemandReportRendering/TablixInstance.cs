using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class TablixInstance : DataRegionInstance
	{
		private Tablix m_owner;

		private ReportSize m_topMargin;

		private ReportSize m_bottomMargin;

		private ReportSize m_leftMargin;

		private ReportSize m_rightMargin;

		public ReportSize TopMargin => GetOrEvaluateMarginProperty(ref m_topMargin, Microsoft.ReportingServices.ReportIntermediateFormat.Tablix.MarginPosition.TopMargin);

		public ReportSize BottomMargin => GetOrEvaluateMarginProperty(ref m_bottomMargin, Microsoft.ReportingServices.ReportIntermediateFormat.Tablix.MarginPosition.BottomMargin);

		public ReportSize LeftMargin => GetOrEvaluateMarginProperty(ref m_leftMargin, Microsoft.ReportingServices.ReportIntermediateFormat.Tablix.MarginPosition.LeftMargin);

		public ReportSize RightMargin => GetOrEvaluateMarginProperty(ref m_rightMargin, Microsoft.ReportingServices.ReportIntermediateFormat.Tablix.MarginPosition.RightMargin);

		internal TablixInstance(Tablix reportItemDef)
			: base(reportItemDef)
		{
			m_owner = reportItemDef;
		}

		private ReportSize GetOrEvaluateMarginProperty(ref ReportSize property, Microsoft.ReportingServices.ReportIntermediateFormat.Tablix.MarginPosition marginPosition)
		{
			if (m_owner.IsOldSnapshot)
			{
				return null;
			}
			if (property == null)
			{
				property = new ReportSize(m_owner.TablixDef.EvaluateTablixMargin(ReportScopeInstance, marginPosition, m_reportElementDef.RenderingContext.OdpContext));
			}
			return property;
		}

		internal override void SetNewContext()
		{
			m_topMargin = (m_bottomMargin = (m_leftMargin = (m_rightMargin = null)));
			base.SetNewContext();
		}
	}
}
