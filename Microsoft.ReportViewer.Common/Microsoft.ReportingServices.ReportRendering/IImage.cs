namespace Microsoft.ReportingServices.ReportRendering
{
	internal interface IImage
	{
		byte[] ImageData
		{
			get;
		}

		string MIMEType
		{
			get;
		}

		string StreamName
		{
			get;
		}
	}
}
