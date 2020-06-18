using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class RdceExtraElementException : RSException
	{
		public RdceExtraElementException(string nodeName)
			: base(ErrorCode.rsRdceExtraElementError, ErrorStrings.rsRdceExtraElementError(nodeName), null, RSTrace.IsTraceInitialized ? RSTrace.CatalogTrace : null, null)
		{
		}

		private RdceExtraElementException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
