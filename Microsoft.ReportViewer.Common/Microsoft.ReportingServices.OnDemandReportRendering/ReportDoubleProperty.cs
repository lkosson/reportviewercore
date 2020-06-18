using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ReportDoubleProperty : ReportProperty
	{
		private double m_value;

		public double Value => m_value;

		internal ReportDoubleProperty(Microsoft.ReportingServices.ReportProcessing.ExpressionInfo expressionInfo)
			: base(expressionInfo?.IsExpression ?? false, expressionInfo?.OriginalText)
		{
			if (expressionInfo != null && !expressionInfo.IsExpression && !double.TryParse(expressionInfo.Value, out m_value))
			{
				m_value = 0.0;
			}
		}

		internal ReportDoubleProperty(Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo)
			: base(expressionInfo?.IsExpression ?? false, expressionInfo?.OriginalText)
		{
			if (expressionInfo != null && !expressionInfo.IsExpression)
			{
				if (expressionInfo.ConstantType == DataType.Float)
				{
					m_value = expressionInfo.FloatValue;
				}
				else if (!double.TryParse(expressionInfo.StringValue, out m_value))
				{
					m_value = 0.0;
				}
			}
		}
	}
}
