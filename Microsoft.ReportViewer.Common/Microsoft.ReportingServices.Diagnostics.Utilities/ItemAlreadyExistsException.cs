using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class ItemAlreadyExistsException : ReportCatalogException
	{
		public ItemAlreadyExistsException(string itemPath)
			: base(ErrorCode.rsItemAlreadyExists, ErrorStrings.rsItemAlreadyExists(itemPath), null, null)
		{
		}

		private ItemAlreadyExistsException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
