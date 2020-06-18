namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal static class MapMapperFactory
	{
		internal static IMapMapper CreateMapMapperInstance(Map map, string defaultFontFamily)
		{
			return new MapMapper(map, defaultFontFamily);
		}
	}
}
