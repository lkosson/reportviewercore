using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class SchedulerNotRespondingPreventsPinningException : ReportCatalogException
	{
		public SchedulerNotRespondingPreventsPinningException()
			: base(ErrorCode.rsSchedulerNotResponding, ErrorStrings.rsSchedulerNotRespondingPreventsPinning, null, null)
		{
		}

		private SchedulerNotRespondingPreventsPinningException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
