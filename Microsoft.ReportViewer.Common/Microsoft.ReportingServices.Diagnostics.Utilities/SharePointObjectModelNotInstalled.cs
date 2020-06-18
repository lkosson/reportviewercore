using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class SharePointObjectModelNotInstalled : ReportCatalogException
	{
		public SharePointObjectModelNotInstalled(Exception sharePointObjectModelLoadException)
			: base(ErrorCode.rsSharePointObjectModelNotInstalled, ErrorStrings.rsSharePointObjectModelNotInstalled((sharePointObjectModelLoadException != null) ? sharePointObjectModelLoadException.ToString() : string.Empty), sharePointObjectModelLoadException, null)
		{
		}

		private SharePointObjectModelNotInstalled(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
