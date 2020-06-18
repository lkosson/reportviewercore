namespace Microsoft.ReportingServices.ReportProcessing
{
	internal enum ReportParameterState
	{
		HasValidValue,
		InvalidValueProvided,
		DefaultValueInvalid,
		MissingValidValue,
		HasOutstandingDependencies,
		DynamicValuesUnavailable
	}
}
