using System;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class WindowsAuthzApiException : ReportCatalogException
	{
		public WindowsAuthzApiException(string methodName, string errorCode, string username)
			: base(ErrorCode.rsWinAuthzError, ErrorStrings.rsWinAuthz(methodName, errorCode, username), null, null)
		{
		}
	}
}
