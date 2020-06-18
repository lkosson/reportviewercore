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
			SecurityContext.Run(SecurityContext.Capture(), delegate
			{
				WindowsImpersonationContext windowsImpersonationContext = null;
				try
				{
					try
					{
						windowsImpersonationContext = WindowsIdentity.Impersonate(IntPtr.Zero);
						callback();
					}
					finally
					{
						windowsImpersonationContext?.Undo();
					}
				}
				catch
				{
					throw;
				}
			}, null);
		}

		public static void RunFromRestrictedCasContext(ContextBody callback)
		{
			SecurityContext.Run(SecurityContext.Capture(), delegate
			{
				new SecurityPermission(SecurityPermissionFlag.UnmanagedCode | SecurityPermissionFlag.ControlPrincipal).Assert();
				WindowsImpersonationContext windowsImpersonationContext = null;
				try
				{
					try
					{
						windowsImpersonationContext = WindowsIdentity.Impersonate(IntPtr.Zero);
						CodeAccessPermission.RevertAssert();
						callback();
					}
					finally
					{
						windowsImpersonationContext?.Undo();
					}
				}
				catch
				{
					throw;
				}
			}, null);
		}
	}
}
