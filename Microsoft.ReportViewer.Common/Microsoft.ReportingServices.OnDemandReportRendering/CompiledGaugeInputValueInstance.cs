namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class CompiledGaugeInputValueInstance
	{
		private object m_value;

		public object Value => m_value;

		internal CompiledGaugeInputValueInstance(object value)
		{
			m_value = value;
		}
	}
}
