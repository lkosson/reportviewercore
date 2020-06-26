using System;

namespace Microsoft.Reporting.NETCore
{
	[Flags]
	internal enum ToolbarFlags
	{
		DocMap = 0x1,
		Params = 0x2,
		PageNav = 0x4,
		Refresh = 0x8,
		Print = 0x10,
		Export = 0x20,
		Zoom = 0x40,
		Back = 0x80,
		Stop = 0x100,
		Find = 0x200
	}
}
