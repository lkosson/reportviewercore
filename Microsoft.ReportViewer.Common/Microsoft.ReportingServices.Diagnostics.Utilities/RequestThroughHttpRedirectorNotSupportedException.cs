using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class RequestThroughHttpRedirectorNotSupportedException : RSException
	{
		public RequestThroughHttpRedirectorNotSupportedException(string message)
			: base(ErrorCode.rsRequestThroughHttpRedirectorNotSupportedError, ErrorStrings.rsRequestThroughHttpRedirectorNotSupportedError, null, RSTrace.IsTraceInitialized ? RSTrace.WebServerTracer : null, message)
		{
		}

		private RequestThroughHttpRedirectorNotSupportedException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
