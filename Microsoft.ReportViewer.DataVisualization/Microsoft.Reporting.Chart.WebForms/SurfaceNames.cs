using System;

namespace Microsoft.Reporting.Chart.WebForms
{
	[Flags]
	internal enum SurfaceNames
	{
		Front = 0x1,
		Back = 0x2,
		Left = 0x4,
		Right = 0x8,
		Top = 0x10,
		Bottom = 0x20
	}
}
