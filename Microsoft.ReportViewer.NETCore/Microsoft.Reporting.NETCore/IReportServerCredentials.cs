using System.Net;
using System.Security.Principal;

namespace Microsoft.Reporting.NETCore
{
	public interface IReportServerCredentials
	{
		System.Security.Principal.WindowsIdentity ImpersonationUser
		{
			get;
		}

		ICredentials NetworkCredentials
		{
			get;
		}

		bool GetFormsCredentials(out Cookie authCookie, out string userName, out string password, out string authority);
	}
}
