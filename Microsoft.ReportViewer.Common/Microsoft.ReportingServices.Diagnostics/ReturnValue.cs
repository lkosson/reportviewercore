namespace Microsoft.ReportingServices.Diagnostics
{
	internal sealed class ReturnValue
	{
		private readonly object m_value;

		public object Value => m_value;

		public ReturnValue(object value)
		{
			m_value = value;
		}
	}
}
