using Microsoft.ReportingServices.Diagnostics.Utilities;
using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics
{
	[Serializable]
	internal abstract class ReportProcessingExceptionBase : RSException
	{
		protected ReportProcessingExceptionBase(ErrorCode errorCode, string localizedMessage, Exception innerException, RSTrace tracer, string additionalTraceMessage, params object[] exceptionData)
			: base(errorCode, localizedMessage, innerException, tracer, additionalTraceMessage, exceptionData)
		{
		}

		protected ReportProcessingExceptionBase(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
