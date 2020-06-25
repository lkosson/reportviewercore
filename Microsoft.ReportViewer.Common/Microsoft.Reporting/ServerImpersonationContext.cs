using System;
using System.Security.Principal;

namespace Microsoft.Reporting
{
	internal sealed class ServerImpersonationContext : IDisposable
	{

		public ServerImpersonationContext(object /* WindowsIdentity */ userToImpersonate)
		{
		}

		public void Dispose()
		{
		}
	}
}
