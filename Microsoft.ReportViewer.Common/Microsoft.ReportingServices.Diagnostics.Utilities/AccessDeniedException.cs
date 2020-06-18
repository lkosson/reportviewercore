using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class AccessDeniedException : ReportCatalogException
	{
		public AccessDeniedException(string userName, ErrorCode errorCode = ErrorCode.rsAccessDenied)
			: base(errorCode, ErrorStrings.rsAccessDenied(userName), null, null)
		{
		}

		private AccessDeniedException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
