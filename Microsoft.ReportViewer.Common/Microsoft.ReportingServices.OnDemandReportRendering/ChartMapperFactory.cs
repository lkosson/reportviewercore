namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal static class ChartMapperFactory
	{
		internal static IChartMapper CreateChartMapperInstance(Chart chart, string defaultFontFamily)
		{
			return new ChartMapper(chart, defaultFontFamily);
		}
	}
}
