using System.Data.Common;
using System.Globalization;

namespace Microsoft.ReportingServices.ReportProcessing
{
	internal sealed class ConnectionKey
	{
		private readonly string m_dataSourceType;

		private readonly string m_connectionString;

		private readonly ConnectionSecurity m_connectionSecurity;

		private readonly string m_domainName;

		private readonly string m_userName;

		private readonly bool m_impersonateUser;

		private readonly string m_impersonateUserName;

		private int m_hashCode = -1;

		private string m_hashCodeString;

		public string DataSourceType => m_dataSourceType;

		public bool IsOnPremiseConnection
		{
			get
			{
				if (string.IsNullOrEmpty(m_connectionString))
				{
					return false;
				}
				return new DbConnectionStringBuilder
				{
					ConnectionString = m_connectionString
				}.ContainsKey("External Tenant Id");
			}
		}

		public ConnectionKey(string dataSourceType, string connectionString, ConnectionSecurity connectionSecurity, string domainName, string userName, bool impersonateUser, string impersonateUserName)
		{
			m_dataSourceType = dataSourceType;
			m_connectionString = connectionString;
			m_connectionSecurity = connectionSecurity;
			m_domainName = domainName;
			m_userName = userName;
			m_impersonateUser = impersonateUser;
			m_impersonateUserName = impersonateUserName;
		}

		public string GetKeyString()
		{
			if (m_hashCodeString == null)
			{
				m_hashCodeString = GetHashCode().ToString(CultureInfo.InvariantCulture);
			}
			return m_hashCodeString;
		}

		public override int GetHashCode()
		{
			if (m_hashCode == -1)
			{
				ConnectionSecurity connectionSecurity = m_connectionSecurity;
				m_hashCode = connectionSecurity.GetHashCode();
				HashCombine(ref m_hashCode, m_impersonateUser.GetHashCode());
				if (m_dataSourceType != null)
				{
					HashCombine(ref m_hashCode, m_dataSourceType.GetHashCode());
				}
				if (m_connectionString != null)
				{
					HashCombine(ref m_hashCode, m_connectionString.GetHashCode());
				}
				if (m_domainName != null)
				{
					HashCombine(ref m_hashCode, m_domainName.GetHashCode());
				}
				if (m_userName != null)
				{
					HashCombine(ref m_hashCode, m_userName.GetHashCode());
				}
				if (m_impersonateUserName != null)
				{
					HashCombine(ref m_hashCode, m_impersonateUserName.GetHashCode());
				}
			}
			return m_hashCode;
		}

		public override bool Equals(object obj)
		{
			ConnectionKey connectionKey = obj as ConnectionKey;
			if (connectionKey != null && m_dataSourceType == connectionKey.m_dataSourceType && m_connectionString == connectionKey.m_connectionString && m_connectionSecurity == connectionKey.m_connectionSecurity && m_domainName == connectionKey.m_domainName && m_userName == connectionKey.m_userName && m_impersonateUser == connectionKey.m_impersonateUser && m_impersonateUserName == connectionKey.m_impersonateUserName)
			{
				return true;
			}
			return false;
		}

		public bool ShouldCheckIsAlive()
		{
			if (DataSourceType == null)
			{
				return true;
			}
			if (!DataSourceType.EndsWith("-Native"))
			{
				return !DataSourceType.EndsWith("-Managed");
			}
			return false;
		}

		private static void HashCombine(ref int seed, int other)
		{
			uint num = (uint)seed;
			num = (uint)(seed = ((int)num ^ (other + -1640531527 + (int)(num << 6) + (int)(num >> 2))));
		}
	}
}
