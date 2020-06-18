namespace Microsoft.ReportingServices.ReportProcessing
{
	internal sealed class Previous : DataAggregate
	{
		private object m_value;

		private object m_previous;

		internal override void Init()
		{
			m_value = null;
			m_previous = null;
		}

		internal override void Update(object[] expressions, IErrorContext iErrorContext)
		{
			Global.Tracer.Assert(expressions != null);
			Global.Tracer.Assert(1 == expressions.Length);
			m_previous = m_value;
			m_value = expressions[0];
		}

		internal override object Result()
		{
			return m_previous;
		}
	}
}
