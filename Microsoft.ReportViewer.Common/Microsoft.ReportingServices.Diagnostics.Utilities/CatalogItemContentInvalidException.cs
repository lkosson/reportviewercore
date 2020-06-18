using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class CatalogItemContentInvalidException : ReportCatalogException
	{
		public CatalogItemContentInvalidException(string itemPath)
			: base(ErrorCode.rsItemContentInvalid, ErrorStrings.rsItemContentInvalid(itemPath), null, null)
		{
		}

		private CatalogItemContentInvalidException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
