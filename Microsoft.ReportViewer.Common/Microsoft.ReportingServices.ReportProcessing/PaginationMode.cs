using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Flags]
	internal enum PaginationMode
	{
		Progressive = 0x0,
		TotalPages = 0x1,
		Estimate = 0x2
	}
}
