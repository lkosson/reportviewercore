using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ReportVariantProperty : ReportProperty
	{
		private object m_value;

		public object Value => m_value;

		internal ReportVariantProperty()
		{
			m_value = null;
		}

		internal ReportVariantProperty(bool isExpression)
			: base(isExpression, null)
		{
			m_value = null;
		}

		internal ReportVariantProperty(Microsoft.ReportingServices.ReportProcessing.ExpressionInfo expression)
			: base(expression?.IsExpression ?? false, expression?.OriginalText)
		{
			if (expression != null && !expression.IsExpression)
			{
				m_value = expression.Value;
			}
		}

		internal ReportVariantProperty(Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
			: base(expression?.IsExpression ?? false, expression?.OriginalText)
		{
			if (expression != null && !expression.IsExpression)
			{
				m_value = expression.Value;
			}
		}
	}
}
