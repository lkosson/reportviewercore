using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class OperationPreventsUnattendedExecutionException : ReportCatalogException
	{
		public OperationPreventsUnattendedExecutionException()
			: base(ErrorCode.rsOperationPreventsUnattendedExecution, ErrorStrings.rsOperationPreventsUnattendedExecution, null, null)
		{
		}

		private OperationPreventsUnattendedExecutionException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
