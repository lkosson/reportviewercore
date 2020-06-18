using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class CachingNotEnabledException : ReportCatalogException
	{
		public CachingNotEnabledException(string itemPath)
			: base(ErrorCode.rsCachingNotEnabled, ErrorStrings.rsCachingNotEnabled(itemPath), null, null)
		{
		}

		private CachingNotEnabledException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
