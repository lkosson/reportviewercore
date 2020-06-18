using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class RdceInvalidRdlException : RSException
	{
		public RdceInvalidRdlException(Exception innerException)
			: base(ErrorCode.rsRdceInvalidRdlError, ErrorStrings.rsRdceInvalidRdlError, innerException, RSTrace.IsTraceInitialized ? RSTrace.CatalogTrace : null, null)
		{
		}

		private RdceInvalidRdlException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
