using System;
using System.Globalization;
using System.Runtime.Loader;
using System.Security.Policy;

namespace Microsoft.ReportingServices
{
	internal sealed class AppDomainPool
	{
		private Evidence m_evidence;

		private AppDomainSetup m_setupInfo;

		private bool m_policyChanged;

		private PolicyManager m_policyManager;

		private RefCountedAppDomain m_lastDispensedAppDomain;

		private SandboxCasPolicySettings m_settingsForLastDispensedAppDomain;

		private DateTime m_lastAppDomainCreationTime = DateTime.MinValue;

		private bool m_areAppDomainsReusable;

		public PolicyManager PolicyManager => m_policyManager;

		public AppDomainPool(bool allowAppDomainReuse, Evidence evidence, AppDomainSetup setupInfo, PolicyManager policyManager)
		{
			m_areAppDomainsReusable = allowAppDomainReuse;
			m_evidence = evidence;
			m_setupInfo = setupInfo;
			m_policyManager = policyManager;
			m_policyManager.PolicyChanged += OnPolicyChanged;
			m_policyChanged = true;
		}

		public RefCountedAppDomain GetAppDomain(SandboxCasPolicySettings casSettings)
		{
			lock (this)
			{
				if (m_lastDispensedAppDomain != null && !IsLastAppDomainReusable(casSettings))
				{
					try
					{
						m_lastDispensedAppDomain.Dispose();
					}
					finally
					{
						m_lastDispensedAppDomain = null;
					}
				}
				if (m_lastDispensedAppDomain == null)
				{
					DateTime now = DateTime.Now;
					RefCountedAppDomain refCountedAppDomain = new RefCountedAppDomain(CreateAssemblyLoadContext(now, casSettings));
					if (m_areAppDomainsReusable)
					{
						m_lastDispensedAppDomain = refCountedAppDomain.CreateNewReference();
						m_lastAppDomainCreationTime = now;
						m_settingsForLastDispensedAppDomain = casSettings.Copy();
					}
					return refCountedAppDomain;
				}
				return m_lastDispensedAppDomain.CreateNewReference();
			}
		}

		private AssemblyLoadContext CreateAssemblyLoadContext(DateTime timeStamp, SandboxCasPolicySettings casSettings)
		{
			if (m_policyChanged)
			{
				lock (this)
				{
					m_policyChanged = false;
				}
			}
			string appDomainName = "Local Processing " + timeStamp.ToString(CultureInfo.InvariantCulture);
			return m_policyManager.CreateAssemblyLoadContextWithPolicy(appDomainName, m_evidence, m_setupInfo, casSettings);
		}

		private bool IsLastAppDomainReusable(SandboxCasPolicySettings casSettings)
		{
			if (m_policyChanged)
			{
				return false;
			}
			if (!object.Equals(m_settingsForLastDispensedAppDomain, casSettings))
			{
				return false;
			}
			TimeSpan t = DateTime.Now - m_lastAppDomainCreationTime;
			if (t < TimeSpan.FromMinutes(1.0))
			{
				return t >= TimeSpan.FromMinutes(0.0);
			}
			return false;
		}

		private void OnPolicyChanged(object sender, EventArgs e)
		{
			lock (this)
			{
				m_policyChanged = true;
			}
		}
	}
}
