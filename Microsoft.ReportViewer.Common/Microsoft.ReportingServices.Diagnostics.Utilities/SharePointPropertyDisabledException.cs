using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class SharePointPropertyDisabledException : ReportCatalogException
	{
		public SharePointPropertyDisabledException()
			: base(ErrorCode.rsPropertyDisabled, ErrorStrings.rsPropertyDisabled, null, null)
		{
		}

		private SharePointPropertyDisabledException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
