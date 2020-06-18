namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ReportUrlProperty : ReportProperty
	{
		private ReportUrl m_reportUrl;

		public ReportUrl Value => m_reportUrl;

		internal ReportUrlProperty(bool isExpression, string expressionString, ReportUrl reportUrl)
			: base(isExpression, expressionString)
		{
			if (!isExpression)
			{
				m_reportUrl = reportUrl;
			}
		}
	}
}
