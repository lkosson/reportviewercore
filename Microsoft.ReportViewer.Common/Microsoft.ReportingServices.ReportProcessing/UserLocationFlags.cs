using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Flags]
	internal enum UserLocationFlags
	{
		None = 0x1,
		ReportBody = 0x2,
		ReportPageSection = 0x4,
		ReportQueries = 0x8
	}
}
