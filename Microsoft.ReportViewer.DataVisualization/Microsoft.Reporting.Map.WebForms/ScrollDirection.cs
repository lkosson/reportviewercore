using System;

namespace Microsoft.Reporting.Map.WebForms
{
	[Flags]
	internal enum ScrollDirection
	{
		None = 0x0,
		North = 0x1,
		South = 0x2,
		West = 0x4,
		East = 0x8
	}
}
