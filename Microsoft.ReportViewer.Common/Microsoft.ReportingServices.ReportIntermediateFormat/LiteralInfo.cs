namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	internal sealed class LiteralInfo
	{
		private readonly object m_value;

		public object Value => m_value;

		public LiteralInfo(object value)
		{
			m_value = value;
		}
	}
}
