using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class RdceWrappedException : RSException
	{
		public RdceWrappedException(Exception innerException)
			: this(innerException, null)
		{
		}

		public RdceWrappedException(Exception innerException, string additionalTraceMessage)
			: base(ErrorCode.rsRdceInvalidCacheOptionError, ErrorStrings.rsRdceWrappedException, innerException, RSTrace.IsTraceInitialized ? RSTrace.CatalogTrace : null, additionalTraceMessage)
		{
		}

		private RdceWrappedException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
