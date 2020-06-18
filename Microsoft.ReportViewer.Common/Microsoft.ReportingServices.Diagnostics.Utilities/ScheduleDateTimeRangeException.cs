using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class ScheduleDateTimeRangeException : ReportCatalogException
	{
		public ScheduleDateTimeRangeException()
			: base(ErrorCode.rsScheduleDateTimeRangeException, ErrorStrings.rsScheduleDateTimeRangeException, null, null)
		{
		}

		private ScheduleDateTimeRangeException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
