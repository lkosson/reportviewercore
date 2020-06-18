using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class ScheduleNotFoundException : ReportCatalogException
	{
		public ScheduleNotFoundException(string idOrData)
			: base(ErrorCode.rsScheduleNotFound, ErrorStrings.rsScheduleNotFound(idOrData), null, null)
		{
		}

		private ScheduleNotFoundException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
