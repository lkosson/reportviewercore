using System;

namespace Microsoft.Reporting.Map.WebForms
{
	[Flags]
	internal enum AntiAliasing
	{
		None = 0x0,
		Text = 0x1,
		Graphics = 0x2,
		All = 0x3
	}
}
