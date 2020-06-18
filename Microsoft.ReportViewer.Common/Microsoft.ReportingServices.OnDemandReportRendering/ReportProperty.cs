namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal abstract class ReportProperty
	{
		private bool m_isExpression;

		private string m_expressionString;

		public bool IsExpression => m_isExpression;

		public string ExpressionString => m_expressionString;

		internal ReportProperty()
		{
			m_isExpression = false;
			m_expressionString = null;
		}

		internal ReportProperty(bool isExpression, string expressionString)
		{
			m_isExpression = isExpression;
			m_expressionString = expressionString;
		}
	}
}
