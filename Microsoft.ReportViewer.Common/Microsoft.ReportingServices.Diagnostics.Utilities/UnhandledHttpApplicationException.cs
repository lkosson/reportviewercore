using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	internal sealed class UnhandledHttpApplicationException : RSException
	{
		public UnhandledHttpApplicationException(Exception innerException)
			: base(ErrorCode.rsUnhandledHttpApplicationError, ErrorStrings.rsUnhandledHttpApplicationError, innerException, RSTrace.IsTraceInitialized ? RSTrace.WebServerTracer : null, null)
		{
		}

		private UnhandledHttpApplicationException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
