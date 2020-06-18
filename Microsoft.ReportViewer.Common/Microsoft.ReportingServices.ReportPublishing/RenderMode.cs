using System;

namespace Microsoft.ReportingServices.ReportPublishing
{
	[Flags]
	internal enum RenderMode
	{
		RenderEdit = 0x1,
		FullOdp = 0x2,
		Both = 0x3
	}
}
