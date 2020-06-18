using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class InternalRepublishingException : ReportCatalogException
	{
		public InternalRepublishingException(string itemPath, Exception innerException, byte[] contents)
			: base(ErrorCode.rsInternalRepublishingFailed, string.Format(CultureInfo.CurrentCulture, "Report upgrade failed for item '{0}'.", itemPath), innerException, null, contents)
		{
		}

		private InternalRepublishingException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
