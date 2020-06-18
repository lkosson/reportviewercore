using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class AuthenticationExtensionException : RSException
	{
		public AuthenticationExtensionException(Exception innerException, string parameterName)
			: base(ErrorCode.rsAuthorizationTokenInvalidOrExpired, ErrorStrings.rsAuthenticationExtensionError(parameterName), innerException, RSTrace.IsTraceInitialized ? RSTrace.CatalogTrace : null, null)
		{
		}

		private AuthenticationExtensionException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
