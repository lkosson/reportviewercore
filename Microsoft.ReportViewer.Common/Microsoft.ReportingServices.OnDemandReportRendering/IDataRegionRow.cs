namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal interface IDataRegionRow
	{
		int Count
		{
			get;
		}

		IDataRegionCell GetIfExists(int index);
	}
}
