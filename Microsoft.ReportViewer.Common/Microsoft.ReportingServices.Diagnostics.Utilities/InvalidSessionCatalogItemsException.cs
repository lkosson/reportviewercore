using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class InvalidSessionCatalogItemsException : ReportCatalogException
	{
		public InvalidSessionCatalogItemsException(Exception innerException, string errorString)
			: base(ErrorCode.rsInvalidSessionCatalogItems, ErrorStrings.rsInvalidSessionCatalogItems(errorString), innerException, null)
		{
		}

		private InvalidSessionCatalogItemsException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
