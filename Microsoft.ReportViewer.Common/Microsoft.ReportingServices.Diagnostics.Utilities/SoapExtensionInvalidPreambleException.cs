using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class SoapExtensionInvalidPreambleException : RSException
	{
		public SoapExtensionInvalidPreambleException(Exception innerException, string reason, string preamble)
			: base(ErrorCode.rsSoapExtensionInvalidPreambleError, ErrorStrings.rsSoapExtensionInvalidPreambleError, innerException, RSTrace.IsTraceInitialized ? RSTrace.WebServerTracer : null, string.Format(CultureInfo.InvariantCulture, "reason={0}{1}", reason, RSTrace.WebServerTracer.TraceVerbose ? (":\n" + preamble) : "."))
		{
		}

		private SoapExtensionInvalidPreambleException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
