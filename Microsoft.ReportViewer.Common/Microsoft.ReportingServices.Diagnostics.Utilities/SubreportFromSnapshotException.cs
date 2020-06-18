using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class SubreportFromSnapshotException : ReportCatalogException
	{
		public SubreportFromSnapshotException()
			: base(ErrorCode.rsSubreportFromSnapshot, ErrorStrings.rsSubreportFromSnapshot, null, null)
		{
		}

		private SubreportFromSnapshotException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
