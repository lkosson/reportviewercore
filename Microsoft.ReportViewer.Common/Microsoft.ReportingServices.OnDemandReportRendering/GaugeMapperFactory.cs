namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal static class GaugeMapperFactory
	{
		internal static IGaugeMapper CreateGaugeMapperInstance(GaugePanel gaugePanel, string defaultFontFamily)
		{
			return new GaugeMapper(gaugePanel, defaultFontFamily);
		}
	}
}
