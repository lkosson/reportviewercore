namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ReportColorProperty : ReportProperty
	{
		private ReportColor m_value;

		public ReportColor Value => m_value;

		internal ReportColorProperty(bool isExpression, string expressionString, ReportColor value, ReportColor defaultValue)
			: base(isExpression, expressionString)
		{
			if (!isExpression)
			{
				m_value = value;
			}
			else
			{
				m_value = defaultValue;
			}
		}
	}
}
