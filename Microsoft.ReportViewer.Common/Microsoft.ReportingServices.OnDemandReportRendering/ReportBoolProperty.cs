using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ReportBoolProperty : ReportProperty
	{
		private bool m_value;

		public bool Value => m_value;

		internal ReportBoolProperty()
		{
			m_value = false;
		}

		internal ReportBoolProperty(bool value)
		{
			m_value = value;
		}

		internal ReportBoolProperty(Microsoft.ReportingServices.ReportProcessing.ExpressionInfo expression)
			: base(expression?.IsExpression ?? false, expression?.OriginalText)
		{
			if (expression != null && !expression.IsExpression)
			{
				m_value = expression.BoolValue;
			}
		}

		internal ReportBoolProperty(Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
			: base(expression?.IsExpression ?? false, expression?.OriginalText)
		{
			if (expression != null && !expression.IsExpression)
			{
				m_value = expression.BoolValue;
			}
		}

		internal ReportBoolProperty(Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, bool value)
			: base(expression?.IsExpression ?? false, expression?.OriginalText)
		{
			if (expression != null && !expression.IsExpression)
			{
				m_value = value;
			}
		}
	}
}
