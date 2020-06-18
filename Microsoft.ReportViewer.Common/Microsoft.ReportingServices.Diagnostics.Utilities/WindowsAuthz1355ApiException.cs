using System;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class WindowsAuthz1355ApiException : ReportCatalogException
	{
		public WindowsAuthz1355ApiException(string methodName, string username)
			: base(ErrorCode.rsWinAuthzError1355, ErrorStrings.rsWinAuthz1355(methodName, username), null, null)
		{
		}
	}
}
