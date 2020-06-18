namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class CompiledGaugePointerInstance
	{
		private CompiledGaugeInputValueInstance m_gaugeInputValue;

		public CompiledGaugeInputValueInstance GaugeInputValue
		{
			get
			{
				return m_gaugeInputValue;
			}
			internal set
			{
				m_gaugeInputValue = value;
			}
		}
	}
}
