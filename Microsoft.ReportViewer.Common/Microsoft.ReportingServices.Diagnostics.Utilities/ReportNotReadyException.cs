using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class ReportNotReadyException : ReportCatalogException
	{
		public ReportNotReadyException()
			: base(ErrorCode.rsReportNotReady, ErrorStrings.rsReportNotReady, null, null)
		{
		}

		private ReportNotReadyException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
