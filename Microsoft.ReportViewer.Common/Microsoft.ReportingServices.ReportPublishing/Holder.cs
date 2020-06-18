namespace Microsoft.ReportingServices.ReportPublishing
{
	internal class Holder<T> where T : struct
	{
		private T m_t;

		internal T Value
		{
			get
			{
				return m_t;
			}
			set
			{
				m_t = value;
			}
		}
	}
}
