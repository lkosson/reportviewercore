using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class ReservedItemException : ReportCatalogException
	{
		public ReservedItemException(string itemPath)
			: base(ErrorCode.rsReservedItem, ErrorStrings.rsReservedItem(itemPath), null, null)
		{
		}

		private ReservedItemException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
