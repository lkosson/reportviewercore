using System;

namespace Microsoft.Reporting.Chart.WebForms
{
	[Flags]
	internal enum LabelAlignmentTypes
	{
		Top = 0x1,
		Bottom = 0x2,
		Right = 0x4,
		Left = 0x8,
		TopLeft = 0x10,
		TopRight = 0x20,
		BottomLeft = 0x40,
		BottomRight = 0x80,
		Center = 0x100
	}
}
