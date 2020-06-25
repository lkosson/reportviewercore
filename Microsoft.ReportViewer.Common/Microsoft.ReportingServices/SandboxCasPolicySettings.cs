using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Security;
using Microsoft.Reporting;

namespace Microsoft.ReportingServices
{
	[Serializable]
	internal class SandboxCasPolicySettings
	{
		private PermissionSet m_basePermissions;

		private List<StrongName> m_fullTrustAssemblies;

		private ReadOnlyCollection<StrongName> m_fullTrustAssembliesReadOnly;

		public PermissionSet BasePermissions
		{
			get
			{
				return m_basePermissions;
			}
			set
			{
				m_basePermissions = value;
			}
		}

		public ReadOnlyCollection<StrongName> FullTrustAssemblies => m_fullTrustAssembliesReadOnly;

		public void AddFullTrustAssembly(StrongName assemblyName)
		{
			if (m_fullTrustAssemblies == null)
			{
				m_fullTrustAssemblies = new List<StrongName>();
				m_fullTrustAssembliesReadOnly = new ReadOnlyCollection<StrongName>(m_fullTrustAssemblies);
			}
			m_fullTrustAssemblies.Add(assemblyName);
		}

		public SandboxCasPolicySettings Copy()
		{
			SandboxCasPolicySettings sandboxCasPolicySettings = new SandboxCasPolicySettings();
			if (m_basePermissions != null)
			{
				sandboxCasPolicySettings.m_basePermissions = m_basePermissions.Copy();
			}
			if (m_fullTrustAssemblies != null)
			{
				foreach (StrongName fullTrustAssembly in m_fullTrustAssemblies)
				{
					sandboxCasPolicySettings.AddFullTrustAssembly(fullTrustAssembly);
				}
				return sandboxCasPolicySettings;
			}
			return sandboxCasPolicySettings;
		}

		public override bool Equals(object obj)
		{
			SandboxCasPolicySettings sandboxCasPolicySettings = obj as SandboxCasPolicySettings;
			if (sandboxCasPolicySettings == null)
			{
				return false;
			}
			if (!object.Equals(m_basePermissions, sandboxCasPolicySettings.m_basePermissions))
			{
				return false;
			}
			if (m_fullTrustAssemblies == null)
			{
				if (sandboxCasPolicySettings.m_fullTrustAssemblies != null)
				{
					return false;
				}
			}
			else
			{
				if (sandboxCasPolicySettings.m_fullTrustAssemblies == null)
				{
					return false;
				}
				int count = m_fullTrustAssemblies.Count;
				if (count != sandboxCasPolicySettings.m_fullTrustAssemblies.Count)
				{
					return false;
				}
				for (int i = 0; i < count; i++)
				{
					if (!object.Equals(m_fullTrustAssemblies[i], sandboxCasPolicySettings.m_fullTrustAssemblies[i]))
					{
						return false;
					}
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int num = 0;
			if (m_basePermissions != null)
			{
				num ^= m_basePermissions.GetHashCode();
			}
			if (m_fullTrustAssemblies != null)
			{
				num ^= m_fullTrustAssemblies.GetHashCode();
			}
			return num;
		}
	}
}
