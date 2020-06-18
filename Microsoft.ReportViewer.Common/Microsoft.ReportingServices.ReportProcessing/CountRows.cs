namespace Microsoft.ReportingServices.ReportProcessing
{
	internal sealed class CountRows : DataAggregate
	{
		private int m_currentTotal;

		internal override void Init()
		{
			m_currentTotal = 0;
		}

		internal override void Update(object[] expressions, IErrorContext iErrorContext)
		{
			m_currentTotal++;
		}

		internal override object Result()
		{
			return m_currentTotal;
		}
	}
}
