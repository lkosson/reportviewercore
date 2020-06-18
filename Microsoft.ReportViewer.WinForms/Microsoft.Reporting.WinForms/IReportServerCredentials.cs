using System.Net;
using System.Security.Principal;

namespace Microsoft.Reporting.WinForms
{
	public interface IReportServerCredentials
	{
		WindowsIdentity ImpersonationUser
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
