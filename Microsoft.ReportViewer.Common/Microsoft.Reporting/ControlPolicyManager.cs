using Microsoft.ReportingServices;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System;
using System.Reflection;
using System.Runtime.Loader;

namespace Microsoft.Reporting
{
	internal sealed class ControlPolicyManager : PolicyManager
	{
		private static StrongName[] m_baseFullTrustAssemblies;

		public override AssemblyLoadContext CreateAssemblyLoadContextWithPolicy(string appDomainName, Evidence evidence, Microsoft.ReportingServices.AppDomainSetup setupInfo, SandboxCasPolicySettings casSettings)
		{
#if NETSTANDARD2_1
			AssemblyLoadContext assemblyLoadContext = AssemblyLoadContext.Default;
#else
			AssemblyLoadContext assemblyLoadContext = new AssemblyLoadContext(appDomainName, true);
#endif
			return assemblyLoadContext;
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
			return new StrongName();
		}
	}
}
