using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class SubscriptionNotFoundException : ReportCatalogException
	{
		public SubscriptionNotFoundException(string idOrData)
			: base(ErrorCode.rsSubscriptionNotFound, ErrorStrings.rsSubscriptionNotFound(idOrData), null, null)
		{
		}

		private SubscriptionNotFoundException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
