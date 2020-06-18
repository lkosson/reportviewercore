using System;

namespace Microsoft.Reporting.Chart.WebForms
{
	[Flags]
	internal enum AreaAlignTypes
	{
		None = 0x0,
		Position = 0x1,
		PlotPosition = 0x2,
		Cursor = 0x4,
		AxesView = 0x8,
		All = 0xF
	}
}
