using System;

namespace Microsoft.ReportingServices.Diagnostics
{
	[Flags]
	internal enum ItemPathOptions
	{
		None = 0x0,
		Validate = 0x1,
		Convert = 0x2,
		Translate = 0x4,
		AllowEditSessionSyntax = 0x8,
		IgnoreValidateEditSession = 0x10,
		Default = 0x7
	}
}
