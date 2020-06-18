using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Flags]
	internal enum ReportProcessingFlags
	{
		NotSet = 0x0,
		OnDemandEngine = 0x1,
		YukonEngine = 0x10,
		UpgradedYukonSnapshot = 0x2,
		YukonSnapshot = 0x20
	}
}
