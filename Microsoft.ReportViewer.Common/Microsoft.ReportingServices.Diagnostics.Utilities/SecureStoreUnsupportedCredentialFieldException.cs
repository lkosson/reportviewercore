using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	internal sealed class SecureStoreUnsupportedCredentialFieldException : ReportCatalogException
	{
		public SecureStoreUnsupportedCredentialFieldException(string appId)
			: base(ErrorCode.rsSecureStoreUnsupportedCredentialField, ErrorStrings.rsSecureStoreUnsupportedCredentialField(appId), null, null)
		{
		}

		private SecureStoreUnsupportedCredentialFieldException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
