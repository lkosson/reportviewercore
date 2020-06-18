using System;

namespace Microsoft.ReportingServices.Library
{
	[Flags]
	internal enum ChunkFlags
	{
		None = 0x0,
		Compressed = 0x1,
		FileSystem = 0x2,
		CrossDatabaseSharing = 0x4
	}
}
