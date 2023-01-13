using System;
using System.Security.Permissions;
using System.Security.Principal;

namespace Microsoft.ReportingServices.Interfaces
{
	public interface IAuthenticationExtension : IExtension
	{
		void GetUserInfo(out IIdentity userIdentity, out IntPtr userId);

		bool LogonUser(string userName, string password, string authority);

		bool IsValidPrincipalName(string principalName);
	}
}
