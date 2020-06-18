using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class LogClientTraceEventsException : ReportCatalogException
	{
		public LogClientTraceEventsException(string message, ErrorCode errorCode)
			: this(message, errorCode, null)
		{
		}

		public LogClientTraceEventsException(string message, ErrorCode errorCode, Exception innerException)
			: base(errorCode, message, innerException, null)
		{
		}

		private LogClientTraceEventsException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
