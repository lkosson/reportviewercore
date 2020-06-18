using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class UnknownEventTypeException : ReportCatalogException
	{
		public UnknownEventTypeException(string eventType)
			: base(ErrorCode.rsUnknownEventType, ErrorStrings.rsUnknownEventType(eventType), null, null)
		{
		}

		private UnknownEventTypeException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
