using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class OperationNotSupportedSharePointModeException : ReportCatalogException
	{
		public OperationNotSupportedSharePointModeException()
			: base(ErrorCode.rsOperationNotSupportedSharePointMode, ErrorStrings.rsOperationNotSupportedSharePointMode, null, null)
		{
		}

		private OperationNotSupportedSharePointModeException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
