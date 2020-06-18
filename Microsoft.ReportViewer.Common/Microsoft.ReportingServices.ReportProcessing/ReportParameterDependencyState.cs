namespace Microsoft.ReportingServices.ReportProcessing
{
	internal enum ReportParameterDependencyState
	{
		AllDependenciesSpecified,
		HasOutstandingDependencies,
		MissingUpstreamDataSourcePrompt
	}
}
