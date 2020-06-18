using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class SecurityZoneNotSupportedException : ReportCatalogException
	{
		public SecurityZoneNotSupportedException()
			: base(ErrorCode.rsSecurityZoneNotSupported, ErrorStrings.rsSecurityZoneNotSupported, null, null)
		{
		}

		private SecurityZoneNotSupportedException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
