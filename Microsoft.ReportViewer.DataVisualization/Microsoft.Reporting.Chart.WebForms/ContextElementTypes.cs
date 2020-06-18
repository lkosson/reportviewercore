using System;

namespace Microsoft.Reporting.Chart.WebForms
{
	[Flags]
	internal enum ContextElementTypes
	{
		None = 0x0,
		ChartArea = 0x1,
		Series = 0x2,
		Axis = 0x8,
		Title = 0x10,
		Annotation = 0x20,
		Legend = 0x40,
		AxisLabel = 0x80,
		Any = 0xFB
	}
}
