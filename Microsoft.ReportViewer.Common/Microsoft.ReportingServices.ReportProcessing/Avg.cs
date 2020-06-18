using Microsoft.ReportingServices.Diagnostics.Utilities;

namespace Microsoft.ReportingServices.ReportProcessing
{
	internal sealed class Avg : Sum
	{
		private uint m_currentCount;

		internal override void Init()
		{
			base.Init();
			m_currentCount = 0u;
		}

		internal override void Update(object[] expressions, IErrorContext iErrorContext)
		{
			Global.Tracer.Assert(expressions != null);
			Global.Tracer.Assert(1 == expressions.Length);
			if (!DataAggregate.IsNull(DataAggregate.GetTypeCode(expressions[0])))
			{
				base.Update(expressions, iErrorContext);
				m_currentCount++;
			}
		}

		internal override object Result()
		{
			switch (m_currentTotalType)
			{
			case DataTypeCode.Null:
				return null;
			case DataTypeCode.Double:
				return (double)m_currentTotal / (double)m_currentCount;
			case DataTypeCode.Decimal:
				return (decimal)m_currentTotal / (decimal)m_currentCount;
			default:
				Global.Tracer.Assert(condition: false);
				throw new ReportProcessingException(ErrorCode.rsInvalidOperation);
			}
		}
	}
}
