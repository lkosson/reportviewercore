using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ReportIntProperty : ReportProperty
	{
		private int m_value;

		public int Value => m_value;

		internal ReportIntProperty(int value)
		{
			m_value = value;
		}

		internal ReportIntProperty(bool isExpression, string expressionString, int value, int defaultValue)
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

		internal ReportIntProperty(ExpressionInfo expression)
			: base(expression?.IsExpression ?? false, expression?.OriginalText)
		{
			if (expression != null && !expression.IsExpression)
			{
				m_value = expression.IntValue;
			}
		}
	}
}
