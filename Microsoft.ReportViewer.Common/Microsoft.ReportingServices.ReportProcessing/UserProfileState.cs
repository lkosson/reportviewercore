using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Flags]
	internal enum UserProfileState
	{
		None = 0x0,
		InQuery = 0x1,
		InReport = 0x2,
		Both = 0x3,
		OnDemandExpressions = 0x8
	}
}
