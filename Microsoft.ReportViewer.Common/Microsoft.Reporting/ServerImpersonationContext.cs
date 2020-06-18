using System;
using System.Security.Principal;

namespace Microsoft.Reporting
{
	internal sealed class ServerImpersonationContext : IDisposable
	{
		private WindowsIdentity m_oldUser;

		public ServerImpersonationContext(WindowsIdentity userToImpersonate)
		{
			try
			{
				if (userToImpersonate != null)
				{
					m_oldUser = WindowsIdentity.GetCurrent();
					userToImpersonate.Impersonate();
				}
			}
			catch
			{
				Dispose();
				throw;
			}
		}

		public void Dispose()
		{
			if (m_oldUser != null)
			{
				m_oldUser.Impersonate();
			}
		}
	}
}
