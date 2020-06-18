using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class UnknownUserNameException : ReportCatalogException
	{
		public UnknownUserNameException(string userName)
			: base(ErrorCode.rsUnknownUserName, ErrorStrings.rsUnknownUserName(userName), null, null)
		{
		}

		public UnknownUserNameException(string userName, Exception innerException)
			: base(ErrorCode.rsUnknownUserName, ErrorStrings.rsUnknownUserName(userName), innerException, null)
		{
		}

		private UnknownUserNameException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
