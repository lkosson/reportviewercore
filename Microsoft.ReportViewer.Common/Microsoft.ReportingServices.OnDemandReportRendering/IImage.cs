namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal interface IImage
	{
		Image.SourceType Source
		{
			get;
		}

		ReportStringProperty Value
		{
			get;
		}

		ReportStringProperty MIMEType
		{
			get;
		}
	}
}
