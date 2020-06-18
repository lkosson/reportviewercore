using System;
using System.Collections.Specialized;
using System.Security.Policy;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ReportRuntimeSetup
	{
		private static ReportRuntimeSetup DefaultRuntimeSetup;

		[NonSerialized]
		private readonly AppDomain m_exprHostAppDomain;

		private readonly Evidence m_exprHostEvidence;

		private readonly bool m_restrictCodeModulesInCurrentAppDomain;

		private StringCollection m_currentAppDomainTrustedCodeModules;

		private readonly bool m_requireExpressionHostWithRefusedPermissions;

		public bool ExecutesInSeparateAppDomain => ExprHostAppDomain != null;

		internal AppDomain ExprHostAppDomain => m_exprHostAppDomain;

		internal Evidence ExprHostEvidence => m_exprHostEvidence;

		public bool RequireExpressionHostWithRefusedPermissions => m_requireExpressionHostWithRefusedPermissions;

		public ReportRuntimeSetup(ReportRuntimeSetup originalSetup, AppDomain newAppDomain)
			: this(newAppDomain, originalSetup.m_exprHostEvidence, originalSetup.m_restrictCodeModulesInCurrentAppDomain, originalSetup.m_requireExpressionHostWithRefusedPermissions)
		{
			if (originalSetup.m_currentAppDomainTrustedCodeModules == null)
			{
				return;
			}
			StringEnumerator enumerator = originalSetup.m_currentAppDomainTrustedCodeModules.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					string current = enumerator.Current;
					AddTrustedCodeModuleInCurrentAppDomain(current);
				}
			}
			finally
			{
				(enumerator as IDisposable)?.Dispose();
			}
		}

		public static ReportRuntimeSetup GetDefault()
		{
			if (DefaultRuntimeSetup == null)
			{
				DefaultRuntimeSetup = new ReportRuntimeSetup(null, null, restrictCodeModulesInCurrentAppDomain: false, requireExpressionHostWithRefusedPermissions: false);
			}
			return DefaultRuntimeSetup;
		}

		public static ReportRuntimeSetup CreateForSeparateAppDomainExecution(AppDomain exprHostAppDomain)
		{
			return new ReportRuntimeSetup(exprHostAppDomain, null, restrictCodeModulesInCurrentAppDomain: false, requireExpressionHostWithRefusedPermissions: false);
		}

		public static ReportRuntimeSetup CreateForCurrentAppDomainExecution()
		{
			return new ReportRuntimeSetup(null, null, restrictCodeModulesInCurrentAppDomain: true, requireExpressionHostWithRefusedPermissions: true);
		}

		public static ReportRuntimeSetup CreateForCurrentAppDomainExecution(Evidence exprHostEvidence)
		{
			return new ReportRuntimeSetup(null, exprHostEvidence, restrictCodeModulesInCurrentAppDomain: true, requireExpressionHostWithRefusedPermissions: false);
		}

		public void AddTrustedCodeModuleInCurrentAppDomain(string assemblyName)
		{
			if (m_currentAppDomainTrustedCodeModules == null)
			{
				m_currentAppDomainTrustedCodeModules = new StringCollection();
			}
			if (!m_currentAppDomainTrustedCodeModules.Contains(assemblyName))
			{
				m_currentAppDomainTrustedCodeModules.Add(assemblyName);
			}
		}

		internal bool CheckCodeModuleIsTrustedInCurrentAppDomain(string assemblyName)
		{
			if (m_restrictCodeModulesInCurrentAppDomain)
			{
				if (m_currentAppDomainTrustedCodeModules != null)
				{
					return m_currentAppDomainTrustedCodeModules.Contains(assemblyName);
				}
				return false;
			}
			return true;
		}

		private ReportRuntimeSetup(AppDomain exprHostAppDomain, Evidence exprHostEvidence, bool restrictCodeModulesInCurrentAppDomain, bool requireExpressionHostWithRefusedPermissions)
		{
			m_exprHostAppDomain = exprHostAppDomain;
			m_exprHostEvidence = exprHostEvidence;
			m_restrictCodeModulesInCurrentAppDomain = restrictCodeModulesInCurrentAppDomain;
			m_requireExpressionHostWithRefusedPermissions = requireExpressionHostWithRefusedPermissions;
		}
	}
}
