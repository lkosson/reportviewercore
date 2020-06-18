using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class AuthenticationExtensionUnauthorizedException : RSException
	{
		public KeyValuePair<string, string> HttpResponseHeader
		{
			get;
			private set;
		}

		public AuthenticationExtensionUnauthorizedException(string authorizationUri, string resourceId, string nativeClientId, string oauthLogoutUrl)
			: base(ErrorCode.rsAuthorizationHeaderNotFound, ErrorStrings.rsAuthorizationTokenInvalidOrExpired, null, RSTrace.IsTraceInitialized ? RSTrace.CatalogTrace : null, null)
		{
			HttpResponseHeader = new KeyValuePair<string, string>("WWW-authenticate", GetAuthenticateHeader(authorizationUri, resourceId, nativeClientId, oauthLogoutUrl));
		}

		public AuthenticationExtensionUnauthorizedException(Exception innerException, string authorizationUri, string resourceId, string nativeClientId, string oauthLogoutUrl)
			: base(ErrorCode.rsAuthorizationTokenInvalidOrExpired, ErrorStrings.rsAuthorizationTokenInvalidOrExpired, innerException, RSTrace.IsTraceInitialized ? RSTrace.CatalogTrace : null, null)
		{
			HttpResponseHeader = new KeyValuePair<string, string>("WWW-authenticate", GetAuthenticateHeader(authorizationUri, resourceId, nativeClientId, oauthLogoutUrl));
		}

		private AuthenticationExtensionUnauthorizedException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		private string GetAuthenticateHeader(string authorizationUri, string resourceId, string nativeClientId, string oauthLogoutUrl)
		{
			return $"Bearer authorization_uri={authorizationUri},resource_id={resourceId},nativeclient_id={nativeClientId},oauthLogoutUrl_uri={oauthLogoutUrl}";
		}
	}
}
