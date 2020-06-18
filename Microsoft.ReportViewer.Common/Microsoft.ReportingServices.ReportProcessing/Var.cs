using Microsoft.ReportingServices.Diagnostics.Utilities;

namespace Microsoft.ReportingServices.ReportProcessing
{
	internal class Var : VarBase
	{
		internal override object Result()
		{
			if (1 == m_currentCount)
			{
				return null;
			}
			switch (m_sumOfXType)
			{
			case DataTypeCode.Null:
				return null;
			case DataTypeCode.Double:
				return ((double)m_currentCount * (double)m_sumOfXSquared - (double)m_sumOfX * (double)m_sumOfX) / (double)(m_currentCount * (m_currentCount - 1));
			case DataTypeCode.Decimal:
				return ((decimal)m_currentCount * (decimal)m_sumOfXSquared - (decimal)m_sumOfX * (decimal)m_sumOfX) / (decimal)(m_currentCount * (m_currentCount - 1));
			default:
				Global.Tracer.Assert(condition: false);
				throw new ReportProcessingException(ErrorCode.rsInvalidOperation);
			}
		}
	}
}
