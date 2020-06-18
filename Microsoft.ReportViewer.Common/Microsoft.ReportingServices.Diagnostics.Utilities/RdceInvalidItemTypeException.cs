using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class RdceInvalidItemTypeException : RSException
	{
		public RdceInvalidItemTypeException(string type)
			: base(ErrorCode.rsRdceInvalidItemTypeError, ErrorStrings.rsRdceInvalidItemTypeError(type), null, RSTrace.IsTraceInitialized ? RSTrace.CatalogTrace : null, null)
		{
		}

		private RdceInvalidItemTypeException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
