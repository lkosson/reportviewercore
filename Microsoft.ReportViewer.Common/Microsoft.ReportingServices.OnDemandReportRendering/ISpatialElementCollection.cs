namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal interface ISpatialElementCollection
	{
		int Count
		{
			get;
		}

		MapSpatialElement GetItem(int index);
	}
}
