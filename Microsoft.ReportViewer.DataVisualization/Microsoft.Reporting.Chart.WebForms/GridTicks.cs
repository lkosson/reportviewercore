using System;

namespace Microsoft.Reporting.Chart.WebForms
{
	[Flags]
	internal enum GridTicks
	{
		None = 0x0,
		TickMark = 0x1,
		Gridline = 0x2,
		All = 0x3
	}
}
