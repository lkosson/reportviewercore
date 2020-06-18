namespace Microsoft.ReportingServices.ReportProcessing
{
	internal sealed class First : DataAggregate
	{
		private object m_value;

		private bool m_updated;

		internal override void Init()
		{
			m_value = null;
			m_updated = false;
		}

		internal override void Update(object[] expressions, IErrorContext iErrorContext)
		{
			Global.Tracer.Assert(expressions != null);
			Global.Tracer.Assert(1 == expressions.Length);
			if (!m_updated)
			{
				m_value = expressions[0];
				m_updated = true;
			}
		}

		internal override object Result()
		{
			return m_value;
		}
	}
}
