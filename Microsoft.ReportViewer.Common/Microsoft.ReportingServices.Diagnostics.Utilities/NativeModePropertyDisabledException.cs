using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class NativeModePropertyDisabledException : ReportCatalogException
	{
		public NativeModePropertyDisabledException()
			: base(ErrorCode.rsPropertyDisabledNativeMode, ErrorStrings.rsPropertyDisabledNativeMode, null, null)
		{
		}

		private NativeModePropertyDisabledException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
