using System;

namespace Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing
{
	[Flags]
	internal enum AggregateUpdateFlags
	{
		None = 0x0,
		ScopedAggregates = 0x1,
		RowAggregates = 0x2,
		Both = 0x3
	}
}
