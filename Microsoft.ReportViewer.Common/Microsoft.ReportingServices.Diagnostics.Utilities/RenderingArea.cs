using System;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Flags]
	internal enum RenderingArea
	{
		All = 0x0,
		PageCreation = 0x1,
		KeepTogether = 0x2,
		RepeatOnNewPage = 0x4,
		RichText = 0x8
	}
}
