using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ReportStringProperty : ReportProperty
	{
		private string m_value;

		public string Value => m_value;

		internal ReportStringProperty()
		{
			m_value = null;
		}

		internal ReportStringProperty(bool isExpression, string expressionString, string value)
			: this(isExpression, expressionString, value, null)
		{
		}

		internal ReportStringProperty(bool isExpression, string expressionString, string value, string defaultValue)
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

		internal ReportStringProperty(Microsoft.ReportingServices.ReportProcessing.ExpressionInfo expression)
			: base(expression?.IsExpression ?? false, expression?.OriginalText)
		{
			if (expression != null && !expression.IsExpression)
			{
				m_value = expression.Value;
			}
		}

		internal ReportStringProperty(Microsoft.ReportingServices.ReportProcessing.ExpressionInfo expression, string formulaText)
			: base(expression?.IsExpression ?? false, (expression == null) ? null : ((expression.IsExpression && expression.OriginalText == null) ? formulaText : expression.OriginalText))
		{
			if (expression != null && !expression.IsExpression)
			{
				m_value = expression.Value;
			}
		}

		internal ReportStringProperty(Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
			: base(expression?.IsExpression ?? false, expression?.OriginalText)
		{
			if (expression != null && !expression.IsExpression)
			{
				if (expression.ConstantType != DataType.String)
				{
					m_value = expression.OriginalText;
				}
				else
				{
					m_value = expression.StringValue;
				}
			}
		}
	}
}
