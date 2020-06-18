using System;

namespace Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing
{
	[Flags]
	internal enum DataActions
	{
		None = 0x0,
		RecursiveAggregates = 0x1,
		PostSortAggregates = 0x2,
		UserSort = 0x4,
		AggregatesOfAggregates = 0x8,
		PostSortAggregatesOfAggregates = 0x10
	}
}
