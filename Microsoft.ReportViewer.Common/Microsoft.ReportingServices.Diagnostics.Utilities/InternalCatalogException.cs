using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class InternalCatalogException : ReportCatalogException
	{
		public InternalCatalogException(Exception innerException, string additionalTraceMessage)
			: base(ErrorCode.rsInternalError, ErrorStrings.rsInternalError, innerException, additionalTraceMessage)
		{
		}

		public InternalCatalogException(string additionalTraceMessage)
			: this(null, additionalTraceMessage)
		{
		}

		public InternalCatalogException(Exception innerException, string additionalTraceMessage, params object[] exceptionData)
			: base(ErrorCode.rsInternalError, ErrorStrings.rsInternalError, innerException, additionalTraceMessage, exceptionData)
		{
		}

		public InternalCatalogException(string additionalTraceMessage, params object[] exceptionData)
			: this(null, additionalTraceMessage, exceptionData)
		{
		}

		private InternalCatalogException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
