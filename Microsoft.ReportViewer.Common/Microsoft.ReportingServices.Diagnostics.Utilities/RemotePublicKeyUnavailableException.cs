using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class RemotePublicKeyUnavailableException : ReportCatalogException
	{
		public RemotePublicKeyUnavailableException()
			: base(ErrorCode.rsRemotePublicKeyUnavailable, ErrorStrings.rsRemotePublicKeyUnavailable, null, null)
		{
		}

		private RemotePublicKeyUnavailableException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
