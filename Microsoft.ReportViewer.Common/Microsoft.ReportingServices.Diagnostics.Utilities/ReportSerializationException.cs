using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class ReportSerializationException : ReportCatalogException
	{
		public ReportSerializationException()
			: this(null)
		{
		}

		public ReportSerializationException(Exception innerException)
			: base(ErrorCode.rsReportSerializationError, ErrorStrings.rsReportSerializationError, innerException, null)
		{
		}

		private ReportSerializationException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
