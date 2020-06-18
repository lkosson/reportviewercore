using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class ReportBuilderFileTransmissionException : RSException
	{
		public ReportBuilderFileTransmissionException(Exception innerException, string fileName)
			: base(ErrorCode.rsReportBuilderFileTransmissionError, ErrorStrings.rsReportBuilderFileTransmissionError(fileName), innerException, RSTrace.IsTraceInitialized ? RSTrace.WebServerTracer : null, null)
		{
		}

		private ReportBuilderFileTransmissionException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
