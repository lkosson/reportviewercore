namespace Microsoft.ReportingServices.ReportProcessing
{
	internal enum DataFieldStatus
	{
		None = 0,
		Overflow = 1,
		UnSupportedDataType = 2,
		IsMissing = 4,
		IsError = 8
	}
}
