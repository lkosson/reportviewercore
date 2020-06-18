using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class InvalidItemNameException : ReportCatalogException
	{
		public InvalidItemNameException(string invalidName, int maxItemNameLength)
			: base(ErrorCode.rsInvalidItemName, ErrorStrings.rsInvalidItemName(invalidName, maxItemNameLength), null, null)
		{
		}

		private InvalidItemNameException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
