namespace Microsoft.ReportingServices.ReportRendering
{
	internal sealed class CustomProperty
	{
		private string m_name;

		private object m_value;

		public string Name => m_name;

		public object Value => m_value;

		public CustomProperty(string name, object value)
		{
			m_name = name;
			m_value = value;
		}
	}
}
