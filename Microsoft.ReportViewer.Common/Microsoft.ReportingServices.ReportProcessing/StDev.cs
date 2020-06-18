using Microsoft.ReportingServices.Diagnostics.Utilities;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	internal sealed class StDev : Var
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
				return Math.Sqrt((double)base.Result());
			case DataTypeCode.Decimal:
				return Math.Sqrt(Convert.ToDouble((decimal)base.Result()));
			default:
				Global.Tracer.Assert(condition: false);
				throw new ReportProcessingException(ErrorCode.rsInvalidOperation);
			}
		}
	}
}
