using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	internal sealed class SecureStoreMissingCredentialFieldsException : ReportCatalogException
	{
		public SecureStoreMissingCredentialFieldsException(string appId)
			: base(ErrorCode.rsSecureStoreMissingCredentialFields, ErrorStrings.rsSecureStoreMissingCredentialFields(appId), null, null)
		{
		}

		private SecureStoreMissingCredentialFieldsException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
