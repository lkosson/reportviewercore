namespace Microsoft.Reporting.WinForms
{
	public sealed class ValidValue
	{
		private string m_label;

		private string m_value;

		public string Label => m_label;

		public string Value => m_value;

		internal ValidValue(string label, string value)
		{
			m_label = label;
			m_value = value;
		}
	}
}
