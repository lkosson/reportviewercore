using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	internal sealed class DataCacheInvalidException : RSException
	{
		public DataCacheInvalidException(Exception innerException)
			: base(ErrorCode.rsDataCacheMismatch, ErrorStrings.rsDataCacheMismatch, innerException, RSTrace.IsTraceInitialized ? RSTrace.CatalogTrace : null, null)
		{
		}

		private DataCacheInvalidException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
