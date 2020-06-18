using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class ReportSnapshotNotEnabledException : ReportCatalogException
	{
		public ReportSnapshotNotEnabledException()
			: base(ErrorCode.rsReportSnapshotNotEnabled, ErrorStrings.rsReportSnapshotNotEnabled, null, null)
		{
		}

		private ReportSnapshotNotEnabledException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
