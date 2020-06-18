using System;
using System.Security;
using System.Security.Permissions;
using System.Security.Principal;

namespace Microsoft.ReportingServices.Diagnostics
{
	internal static class RevertImpersonationContext
	{
		internal const int SkipStackFrames = 8;

		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.Infrastructure)]
		public static void Run(ContextBody callback)
		{
			callback();
		}

		public static void RunFromRestrictedCasContext(ContextBody callback)
		{
			callback();
		}
	}
}
