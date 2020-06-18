using System;

namespace Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing
{
	[Flags]
	internal enum SecondPassOperations
	{
		None = 0x0,
		Variables = 0x1,
		Sorting = 0x2,
		FilteringOrAggregatesOrDomainScope = 0x4
	}
}
