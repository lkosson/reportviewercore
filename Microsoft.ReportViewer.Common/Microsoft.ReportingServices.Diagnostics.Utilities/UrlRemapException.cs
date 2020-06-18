using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class UrlRemapException : RSException
	{
		public UrlRemapException(Exception innerException, string url)
			: base(ErrorCode.rsUrlRemapError, ErrorStrings.rsUrlRemapError(url), innerException, RSTrace.IsTraceInitialized ? RSTrace.WebServerTracer : null, null)
		{
		}

		private UrlRemapException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
