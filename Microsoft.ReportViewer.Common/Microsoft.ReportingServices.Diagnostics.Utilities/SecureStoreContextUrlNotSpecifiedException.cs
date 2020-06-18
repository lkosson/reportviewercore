using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	internal sealed class SecureStoreContextUrlNotSpecifiedException : ReportCatalogException
	{
		public SecureStoreContextUrlNotSpecifiedException()
			: base(ErrorCode.rsSecureStoreContextUrlNotSpecified, ErrorStrings.rsSecureStoreContextUrlNotSpecified, null, null)
		{
		}

		private SecureStoreContextUrlNotSpecifiedException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
