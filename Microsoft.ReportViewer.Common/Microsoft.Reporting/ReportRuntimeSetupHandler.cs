using Microsoft.ReportingServices;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Security;
using System.Security.Permissions;

namespace Microsoft.Reporting
{
	[Serializable]
	internal sealed class ReportRuntimeSetupHandler : IDisposable
	{
		private enum TriState
		{
			Unknown,
			True,
			False
		}

		private ReportRuntimeSetup m_reportRuntimeSetup;

		private TriState m_executeInSandbox;

		private TriState m_isAppDomainCasPolicyEnabled;

		private SandboxCasPolicySettings m_sandboxCasSettings;

		[NonSerialized]
		private RefCountedAppDomain m_exprHostSandboxAppDomain;

		[NonSerialized]
		private bool m_isExprHostSandboxAppDomainDirty;

		private static AppDomainPool m_appDomainPool;

		private static readonly object m_appDomainPoolLoaderLock = new object();

		internal ReportRuntimeSetup ReportRuntimeSetup
		{
			get
			{
				if (m_isExprHostSandboxAppDomainDirty)
				{
					ReleaseSandboxAppDomain();
					m_isExprHostSandboxAppDomainDirty = false;
				}
				EnsureSandboxAppDomainIfNeeded();
				return GetReportRuntimeSetup();
			}
		}

		internal bool RequireExpressionHostWithRefusedPermissions
		{
			get
			{
				return GetReportRuntimeSetup().RequireExpressionHostWithRefusedPermissions;
			}
		}

		private bool IsAppDomainCasPolicyEnabled
		{
			get
			{
				if (m_isAppDomainCasPolicyEnabled == TriState.Unknown)
				{
					m_isAppDomainCasPolicyEnabled = TriState.False;
				}
				return m_isAppDomainCasPolicyEnabled == TriState.True;
			}
		}

		private bool ExecuteInSandbox
		{
			get
			{
				if (m_executeInSandbox == TriState.Unknown)
				{
					if (IsAppDomainCasPolicyEnabled)
					{
						m_executeInSandbox = TriState.False;
					}
					else
					{
						m_executeInSandbox = TriState.True;
					}
				}
				return m_executeInSandbox == TriState.True;
			}
		}

		internal ReportRuntimeSetupHandler()
		{
			m_executeInSandbox = TriState.Unknown;
			m_isAppDomainCasPolicyEnabled = TriState.Unknown;
			m_sandboxCasSettings = new SandboxCasPolicySettings();
		}

		private ReportRuntimeSetup GetReportRuntimeSetup()
		{
			if (m_reportRuntimeSetup == null)
			{
				if (ExecuteInSandbox)
				{
					ExecuteReportInSandboxAppDomain();
				}
				else
				{
					ExecuteReportInCurrentAppDomain();
				}
			}
			return m_reportRuntimeSetup;
		}

		public void Dispose()
		{
			ReleaseSandboxAppDomain();
		}

		public void ReleaseSandboxAppDomain()
		{
			if (m_exprHostSandboxAppDomain != null)
			{
				m_exprHostSandboxAppDomain.Dispose();
				m_exprHostSandboxAppDomain = null;
				m_appDomainPool.PolicyManager.PolicyChanged -= OnPolicyChanged;
			}
		}

		internal void ExecuteReportInCurrentAppDomain()
		{
			if (!IsAppDomainCasPolicyEnabled)
			{
				throw new InvalidOperationException(ProcessingStrings.CasPolicyUnavailableForCurrentAppDomain);
			}
			SetAppDomain(useSandBoxAppDomain: false);
			EnsureSandboxAppDomainIfNeeded(); // ???
			m_reportRuntimeSetup = ReportRuntimeSetup.CreateForCurrentAppDomainExecution();
		}

		internal void ExecuteReportInCurrentAppDomain(ReportingServices.Evidence reportEvidence)
		{
			if (!IsAppDomainCasPolicyEnabled)
			{
				throw new InvalidOperationException(ProcessingStrings.CasPolicyUnavailableForCurrentAppDomain);
			}
			SetAppDomain(useSandBoxAppDomain: false);
			EnsureSandboxAppDomainIfNeeded(); // ???
			m_reportRuntimeSetup = ReportRuntimeSetup.CreateForCurrentAppDomainExecution(reportEvidence);
		}

		internal void ExecuteReportInSandboxAppDomain()
		{
			SetAppDomain(useSandBoxAppDomain: true);
			EnsureSandboxAppDomainIfNeeded(); // ???
			m_reportRuntimeSetup = ReportRuntimeSetup.CreateForSeparateAppDomainExecution(m_exprHostSandboxAppDomain.AssemblyLoadContext);
		}

		internal void AddTrustedCodeModuleInCurrentAppDomain(string assemblyName)
		{
			if (!IsAppDomainCasPolicyEnabled)
			{
				throw new InvalidOperationException(ProcessingStrings.CasPolicyUnavailableForCurrentAppDomain);
			}
			GetReportRuntimeSetup().AddTrustedCodeModuleInCurrentAppDomain(assemblyName);
		}

		internal void AddFullTrustModuleInSandboxAppDomain(StrongName assemblyName)
		{
			m_isExprHostSandboxAppDomainDirty = true;
			m_sandboxCasSettings.AddFullTrustAssembly(assemblyName);
		}

		private void SetAppDomain(bool useSandBoxAppDomain)
		{
			if (useSandBoxAppDomain)
			{
				m_executeInSandbox = TriState.True;
				return;
			}
			m_executeInSandbox = TriState.False;
			ReleaseSandboxAppDomain();
		}

		private void EnsureSandboxAppDomainIfNeeded()
		{
			if (ExecuteInSandbox && m_exprHostSandboxAppDomain == null)
			{
				m_exprHostSandboxAppDomain = m_appDomainPool.GetAppDomain(m_sandboxCasSettings);
				m_appDomainPool.PolicyManager.PolicyChanged += OnPolicyChanged;
				m_reportRuntimeSetup = new ReportRuntimeSetup(GetReportRuntimeSetup(), m_exprHostSandboxAppDomain.AssemblyLoadContext);
			}
		}

		public static void InitAppDomainPool(ReportingServices.Evidence sandboxEvidence, PolicyManager policyManager)
		{
			if (m_appDomainPool != null)
			{
				return;
			}
			lock (m_appDomainPoolLoaderLock)
			{
				if (m_appDomainPool == null)
				{
					Microsoft.ReportingServices.AppDomainSetup setupInformation = null;
					m_appDomainPool = new AppDomainPool(allowAppDomainReuse: false, sandboxEvidence, setupInformation, policyManager);
				}
			}
		}

		private void OnPolicyChanged(object sender, EventArgs e)
		{
			m_isExprHostSandboxAppDomainDirty = true;
		}
	}
}
