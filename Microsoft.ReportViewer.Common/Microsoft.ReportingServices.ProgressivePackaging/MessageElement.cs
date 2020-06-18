namespace Microsoft.ReportingServices.ProgressivePackaging
{
	internal class MessageElement
	{
		private string m_name;

		private object m_value;

		internal string Name => m_name;

		internal object Value => m_value;

		internal MessageElement(string name, object value)
		{
			m_name = name;
			m_value = value;
		}
	}
}
