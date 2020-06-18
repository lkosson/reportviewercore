using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class NotEnabledException : ReportCatalogException
	{
		public NotEnabledException()
			: base(ErrorCode.rsNotEnabled, ErrorStrings.rsNotEnabled, null, null)
		{
		}

		private NotEnabledException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
