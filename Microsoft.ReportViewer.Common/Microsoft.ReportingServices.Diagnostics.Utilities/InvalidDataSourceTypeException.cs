using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class InvalidDataSourceTypeException : ReportCatalogException
	{
		public InvalidDataSourceTypeException(string datasourcePath)
			: base(ErrorCode.rsInvalidDataSourceType, ErrorStrings.rsInvalidDataSourceType(datasourcePath), null, null)
		{
		}

		private InvalidDataSourceTypeException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
