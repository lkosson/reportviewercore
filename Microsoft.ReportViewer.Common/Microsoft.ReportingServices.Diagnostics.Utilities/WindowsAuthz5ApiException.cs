using System;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class WindowsAuthz5ApiException : ReportCatalogException
	{
		public WindowsAuthz5ApiException(string methodName, string username)
			: base(ErrorCode.rsWinAuthzError5, ErrorStrings.rsWinAuthz5(methodName, username), null, null)
		{
		}
	}
}
