namespace Microsoft.ReportingServices.ReportProcessing
{
	internal sealed class Last : DataAggregate
	{
		private object m_value;

		internal override void Init()
		{
			m_value = null;
		}

		internal override void Update(object[] expressions, IErrorContext iErrorContext)
		{
			Global.Tracer.Assert(expressions != null);
			Global.Tracer.Assert(1 == expressions.Length);
			m_value = expressions[0];
		}

		internal override object Result()
		{
			return m_value;
		}
	}
}
