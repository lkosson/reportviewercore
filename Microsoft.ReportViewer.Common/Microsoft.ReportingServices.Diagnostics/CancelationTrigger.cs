namespace Microsoft.ReportingServices.Diagnostics
{
	internal enum CancelationTrigger
	{
		None,
		AfterDsqParsing,
		AfterDataSourceResolution,
		DsqtAfterValidation,
		DsqtAfterQueryGeneration,
		DsqtAfterDsdGeneration,
		ReportProcessing
	}
}
