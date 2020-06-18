using Microsoft.ReportingServices.Diagnostics;

namespace Microsoft.ReportingServices.ReportProcessing
{
	internal sealed class ConnectionContext
	{
		internal string DataSourceType
		{
			get;
			set;
		}

		internal ConnectionSecurity ConnectionSecurity
		{
			get;
			set;
		}

		internal string ConnectionString
		{
			get;
			set;
		}

		internal string DomainName
		{
			get;
			set;
		}

		internal string UserName
		{
			get;
			set;
		}

		internal bool ImpersonateUser
		{
			get;
			set;
		}

		internal string ImpersonateUserName
		{
			get;
			set;
		}

		internal SecureStringWrapper Password
		{
			get;
			set;
		}

		internal string DecryptedPassword
		{
			get
			{
				if (Password != null)
				{
					return Password.ToString();
				}
				return string.Empty;
			}
		}

		internal ConnectionContext()
		{
			ConnectionSecurity = ConnectionSecurity.None;
		}

		internal ConnectionKey CreateConnectionKey()
		{
			return new ConnectionKey(DataSourceType, ConnectionString, ConnectionSecurity, DomainName, UserName, ImpersonateUser, ImpersonateUserName);
		}
	}
}
