namespace Microsoft.ReportingServices.ReportProcessing
{
	internal sealed class Count : DataAggregate
	{
		private int m_currentTotal;

		internal override void Init()
		{
			m_currentTotal = 0;
		}

		internal override void Update(object[] expressions, IErrorContext iErrorContext)
		{
			Global.Tracer.Assert(expressions != null);
			Global.Tracer.Assert(1 == expressions.Length);
			if (!DataAggregate.IsNull(DataAggregate.GetTypeCode(expressions[0])))
			{
				m_currentTotal++;
			}
		}

		internal override object Result()
		{
			return m_currentTotal;
		}
	}
}
