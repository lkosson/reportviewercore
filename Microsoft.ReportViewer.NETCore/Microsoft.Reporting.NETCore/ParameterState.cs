namespace Microsoft.Reporting.NETCore
{
	public enum ParameterState
	{
		HasValidValue,
		MissingValidValue,
		HasOutstandingDependencies,
		DynamicValuesUnavailable
	}
}
