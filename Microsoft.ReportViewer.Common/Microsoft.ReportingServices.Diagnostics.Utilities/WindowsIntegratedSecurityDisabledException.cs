using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class WindowsIntegratedSecurityDisabledException : ReportCatalogException
	{
		public WindowsIntegratedSecurityDisabledException()
			: base(ErrorCode.rsWindowsIntegratedSecurityDisabled, ErrorStrings.rsWindowsIntegratedSecurityDisabled, null, null)
		{
		}

		private WindowsIntegratedSecurityDisabledException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
