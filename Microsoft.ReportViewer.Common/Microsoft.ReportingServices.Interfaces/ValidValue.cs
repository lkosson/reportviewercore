namespace Microsoft.ReportingServices.Interfaces
{
	public class ValidValue
	{
		private string m_value;

		private string m_label;

		public string Value
		{
			get
			{
				return m_value;
			}
			set
			{
				m_value = value;
			}
		}

		public string Label
		{
			get
			{
				return m_label;
			}
			set
			{
				m_label = value;
			}
		}
	}
}
