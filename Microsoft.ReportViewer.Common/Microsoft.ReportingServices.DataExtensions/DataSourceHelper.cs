using Microsoft.ReportingServices.Diagnostics;
using System;

namespace Microsoft.ReportingServices.DataExtensions
{
	internal sealed class DataSourceHelper
	{
		private readonly byte[] m_encryptedDomainAndUserName;

		private readonly byte[] m_encryptedPassword;

		private readonly IDataProtection m_dp;

		public DataSourceHelper(byte[] encryptedDomainAndUserName, byte[] encryptedPassword, IDataProtection dataProtection)
		{
			m_encryptedDomainAndUserName = encryptedDomainAndUserName;
			m_encryptedPassword = encryptedPassword;
			if (dataProtection == null)
			{
				throw new ArgumentNullException("dataProtection");
			}
			m_dp = dataProtection;
		}

		public string GetPassword()
		{
			return m_dp.UnprotectDataToString(m_encryptedPassword, "Password");
		}

		public string GetUserName()
		{
			return DataSourceInfo.GetUserNameOnly(m_dp.UnprotectDataToString(m_encryptedDomainAndUserName, "UserName"));
		}

		public string GetDomainName()
		{
			return DataSourceInfo.GetDomainOnly(m_dp.UnprotectDataToString(m_encryptedDomainAndUserName, "UserName"));
		}
	}
}
