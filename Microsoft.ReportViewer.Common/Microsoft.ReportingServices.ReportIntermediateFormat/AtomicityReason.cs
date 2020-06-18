namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	internal enum AtomicityReason
	{
		Filters,
		Sorts,
		NonNaturalSorts,
		NonNaturalGroup,
		DomainScope,
		RecursiveParent,
		Aggregates,
		RunningValues,
		Lookups,
		PeerChildScopes
	}
}
