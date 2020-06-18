using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class AccessDeniedToSecureDataException : ReportCatalogException
	{
		public AccessDeniedToSecureDataException()
			: base(ErrorCode.rsAccessDeniedToSecureData, ErrorStrings.rsAccessDeniedToSecureData, null, null)
		{
		}

		private AccessDeniedToSecureDataException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
