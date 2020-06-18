using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class InternalResourceNotFoundException : RSException
	{
		public InternalResourceNotFoundException()
			: base(ErrorCode.rsInternalResourceNotSpecifiedError, ErrorStrings.rsInternalResourceNotSpecifiedError, null, RSTrace.IsTraceInitialized ? RSTrace.WebServerTracer : null, null)
		{
		}

		public InternalResourceNotFoundException(string imageId)
			: base(ErrorCode.rsInternalResourceNotFoundError, ErrorStrings.rsInternalResourceNotFoundError(imageId), null, RSTrace.IsTraceInitialized ? RSTrace.WebServerTracer : null, null)
		{
		}

		private InternalResourceNotFoundException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
