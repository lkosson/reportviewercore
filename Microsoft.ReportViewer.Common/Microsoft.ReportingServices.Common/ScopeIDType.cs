using System;

namespace Microsoft.ReportingServices.Common
{
	[Flags]
	internal enum ScopeIDType
	{
		None = 0x0,
		SortValues = 0x1,
		GroupValues = 0x2,
		SortGroup = 0x3
	}
}
