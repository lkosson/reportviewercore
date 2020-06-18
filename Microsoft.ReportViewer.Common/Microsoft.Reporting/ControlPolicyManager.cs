using Microsoft.ReportingServices;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;

namespace Microsoft.Reporting
{
	internal sealed class ControlPolicyManager : PolicyManager
	{
		private static StrongName[] m_baseFullTrustAssemblies;

		public override AppDomain CreateAppDomainWithPolicy(string appDomainName, Evidence evidence, AppDomainSetup setupInfo, SandboxCasPolicySettings casSettings)
		{
			PermissionSet permissionSet = casSettings.BasePermissions;
			if (permissionSet == null)
			{
				permissionSet = new PermissionSet(PermissionState.None);
				permissionSet.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));
			}
			StrongName[] baseFullTrustAssemblies = GetBaseFullTrustAssemblies();
			int num = baseFullTrustAssemblies.Length;
			if (casSettings.FullTrustAssemblies != null)
			{
				num += casSettings.FullTrustAssemblies.Count;
			}
			StrongName[] array = new StrongName[num];
			Array.Copy(baseFullTrustAssemblies, array, baseFullTrustAssemblies.Length);
			if (casSettings.FullTrustAssemblies != null)
			{
				casSettings.FullTrustAssemblies.CopyTo(array, baseFullTrustAssemblies.Length);
			}
			return AppDomain.CreateDomain(appDomainName, evidence, setupInfo, permissionSet, array);
		}

		private static StrongName[] GetBaseFullTrustAssemblies()
		{
			if (m_baseFullTrustAssemblies == null)
			{
				m_baseFullTrustAssemblies = new StrongName[2]
				{
					CreateStrongName(typeof(ReportRuntime).Assembly),
					CreateStrongName(typeof(ObjectModel).Assembly)
				};
			}
			return m_baseFullTrustAssemblies;
		}

		private static StrongName CreateStrongName(Assembly assembly)
		{
			AssemblyName name = assembly.GetName();
			if (name == null)
			{
				throw new InvalidOperationException("Could not get assembly name");
			}
			byte[] publicKey = name.GetPublicKey();
			if (publicKey == null || publicKey.Length == 0)
			{
				throw new InvalidOperationException("Assembly is not strongly named");
			}
			return new StrongName(new StrongNamePublicKeyBlob(publicKey), name.Name, name.Version);
		}
	}
}
