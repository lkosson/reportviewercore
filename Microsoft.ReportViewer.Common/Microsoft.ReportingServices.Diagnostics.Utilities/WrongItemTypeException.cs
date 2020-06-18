using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class WrongItemTypeException : ReportCatalogException
	{
		public WrongItemTypeException(string itemPathOrType)
			: base(ErrorCode.rsWrongItemType, ErrorStrings.rsWrongItemType(itemPathOrType), null, null)
		{
		}

		private WrongItemTypeException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
