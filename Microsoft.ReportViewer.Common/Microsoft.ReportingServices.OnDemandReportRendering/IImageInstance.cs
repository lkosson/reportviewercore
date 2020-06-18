namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal interface IImageInstance
	{
		byte[] ImageData
		{
			get;
		}

		string StreamName
		{
			get;
		}

		string MIMEType
		{
			get;
		}
	}
}
