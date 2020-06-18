using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class RdceInvalidExecutionOptionException : RSException
	{
		public RdceInvalidExecutionOptionException()
			: base(ErrorCode.rsRdceInvalidExecutionOptionError, ErrorStrings.rsRdceInvalidExecutionOptionError, null, RSTrace.IsTraceInitialized ? RSTrace.CatalogTrace : null, null)
		{
		}

		private RdceInvalidExecutionOptionException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
