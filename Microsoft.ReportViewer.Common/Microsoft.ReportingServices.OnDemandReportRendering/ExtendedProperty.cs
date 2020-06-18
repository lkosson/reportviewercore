namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ExtendedProperty
	{
		private string m_name;

		private object m_value;

		public string Name => m_name;

		public object Value => m_value;

		internal ExtendedProperty(string name, object value)
		{
			m_name = name;
			m_value = value;
		}

		internal void UpdateValue(object value)
		{
			m_value = value;
		}
	}
}
