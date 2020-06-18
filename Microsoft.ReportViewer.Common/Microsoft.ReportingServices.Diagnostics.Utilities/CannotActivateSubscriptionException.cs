using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class CannotActivateSubscriptionException : ReportCatalogException
	{
		public CannotActivateSubscriptionException()
			: base(ErrorCode.rsCannotActivateSubscription, ErrorStrings.rsCannotActivateSubscription, null, null)
		{
		}

		private CannotActivateSubscriptionException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
