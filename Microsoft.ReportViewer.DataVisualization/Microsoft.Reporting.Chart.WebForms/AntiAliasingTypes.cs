using System;

namespace Microsoft.Reporting.Chart.WebForms
{
	[Flags]
	internal enum AntiAliasingTypes
	{
		None = 0x0,
		Text = 0x1,
		Graphics = 0x2,
		All = 0x3
	}
}
