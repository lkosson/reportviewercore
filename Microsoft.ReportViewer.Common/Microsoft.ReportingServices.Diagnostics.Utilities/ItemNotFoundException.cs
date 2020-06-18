using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class ItemNotFoundException : ReportCatalogException
	{
		public ItemNotFoundException(string itemPath, string parameterName)
			: base(ErrorCode.rsItemNotFound, ErrorStrings.rsItemNotFound(itemPath), null, null)
		{
		}

		public ItemNotFoundException(string itemPath)
			: base(ErrorCode.rsItemNotFound, ErrorStrings.rsItemNotFound(itemPath), null, null)
		{
		}

		private ItemNotFoundException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
