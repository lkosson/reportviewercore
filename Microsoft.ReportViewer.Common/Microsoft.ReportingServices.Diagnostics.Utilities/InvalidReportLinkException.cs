using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class InvalidReportLinkException : ReportCatalogException
	{
		public InvalidReportLinkException()
			: base(ErrorCode.rsInvalidReportLink, ErrorStrings.rsInvalidReportLink, null, null)
		{
		}

		private InvalidReportLinkException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
