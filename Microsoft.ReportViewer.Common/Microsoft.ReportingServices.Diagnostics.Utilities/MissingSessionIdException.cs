using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class MissingSessionIdException : ReportCatalogException
	{
		public MissingSessionIdException()
			: base(ErrorCode.rsMissingSessionId, ErrorStrings.rsMissingSessionId, null, null)
		{
		}

		private MissingSessionIdException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
