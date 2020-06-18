using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ReportSizeProperty : ReportProperty
	{
		private ReportSize m_value;

		public ReportSize Value => m_value;

		internal ReportSizeProperty(bool isExpression, string expressionString, ReportSize value)
			: this(isExpression, expressionString, value, null)
		{
		}

		internal ReportSizeProperty(bool isExpression, string expressionString, ReportSize value, ReportSize defaultValue)
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

		internal ReportSizeProperty(ExpressionInfo expressionInfo)
			: base(expressionInfo?.IsExpression ?? false, expressionInfo?.OriginalText)
		{
			if (expressionInfo != null && !expressionInfo.IsExpression)
			{
				m_value = new ReportSize(expressionInfo.StringValue);
			}
		}

		internal ReportSizeProperty(ExpressionInfo expressionInfo, bool allowNegative)
			: base(expressionInfo?.IsExpression ?? false, expressionInfo?.OriginalText)
		{
			if (expressionInfo != null && !expressionInfo.IsExpression)
			{
				m_value = new ReportSize(expressionInfo.StringValue, validate: true, allowNegative);
			}
		}

		internal ReportSizeProperty(ExpressionInfo expressionInfo, ReportSize defaultValue)
			: base(expressionInfo?.IsExpression ?? false, (expressionInfo == null) ? defaultValue.ToString() : expressionInfo.OriginalText)
		{
			if (expressionInfo != null && !expressionInfo.IsExpression)
			{
				m_value = new ReportSize(expressionInfo.StringValue);
			}
			else
			{
				m_value = defaultValue;
			}
		}
	}
}
