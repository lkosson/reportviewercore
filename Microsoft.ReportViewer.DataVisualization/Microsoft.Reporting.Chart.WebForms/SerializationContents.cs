using System;

namespace Microsoft.Reporting.Chart.WebForms
{
	[Flags]
	internal enum SerializationContents
	{
		Default = 0x1,
		Data = 0x2,
		Appearance = 0x4,
		All = 0x7
	}
}
