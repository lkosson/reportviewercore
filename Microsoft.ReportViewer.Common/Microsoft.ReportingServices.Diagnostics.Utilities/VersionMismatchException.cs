using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class VersionMismatchException : ReportCatalogException
	{
		private Guid m_reportID;

		private bool m_isPermanentSnapshot;

		public Guid ReportID => m_reportID;

		public bool IsPermanentSnapshot => m_isPermanentSnapshot;

		protected override bool TraceFullException => false;

		public VersionMismatchException(Guid reportID, bool isPermanentSnapshot)
			: base(ErrorCode.rsSnapshotVersionMismatch, ErrorStrings.rsSnapshotVersionMismatch, null, "version mismatch found", TraceLevel.Verbose)
		{
			m_reportID = reportID;
			m_isPermanentSnapshot = isPermanentSnapshot;
		}

		private VersionMismatchException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
