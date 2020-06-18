using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class OperationNotSupportedException : ReportCatalogException
	{
		public OperationNotSupportedException(string operation)
			: base(ErrorCode.rsOperationNotSupported, ErrorStrings.rsOperationNotSupported(operation), null, null)
		{
		}

		private OperationNotSupportedException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
