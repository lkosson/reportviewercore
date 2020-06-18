using System;

namespace Microsoft.ReportingServices.Rendering.RichText
{
	[Flags]
	internal enum TextRunState : byte
	{
		Clear = 0x0,
		HasEastAsianChars = 0x1,
		FallbackFont = 0x2
	}
}
