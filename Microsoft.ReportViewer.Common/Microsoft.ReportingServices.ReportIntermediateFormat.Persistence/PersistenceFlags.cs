using System;

namespace Microsoft.ReportingServices.ReportIntermediateFormat.Persistence
{
	[Flags]
	internal enum PersistenceFlags
	{
		None = 0x0,
		Seekable = 0x1,
		CompatVersioned = 0x2
	}
}
