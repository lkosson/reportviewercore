using System;

namespace Microsoft.Reporting.Chart.WebForms
{
	[Flags]
	internal enum ProcessMode
	{
		Paint = 0x1,
		HotRegions = 0x2,
		ImageMaps = 0x4
	}
}
