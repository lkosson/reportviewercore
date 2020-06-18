using System;

namespace Microsoft.Reporting.Map.WebForms
{
	[Flags]
	internal enum PanelButtonType
	{
		Unknown = 0x0,
		ZoomButton = 0x10,
		ZoomIn = 0x10,
		ZoomOut = 0x11,
		NavigationButton = 0x20,
		NaviagateNorth = 0x20,
		NaviagateSouth = 0x21,
		NaviagateEast = 0x22,
		NaviagateWest = 0x24,
		NaviagateCenter = 0x28
	}
}
