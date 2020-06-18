using Microsoft.Reporting.WinForms.Internal.Soap.ReportingServices2005.Execution;

namespace Microsoft.Reporting.WinForms
{
	public sealed class DataSourceCredentials
	{
		private string m_name = "";

		private string m_userID = "";

		private string m_password = "";

		public string Name
		{
			get
			{
				return m_name;
			}
			set
			{
				m_name = value;
			}
		}

		public string UserId
		{
			get
			{
				return m_userID;
			}
			set
			{
				m_userID = value;
			}
		}

		public string Password
		{
			get
			{
				return m_password;
			}
			set
			{
				m_password = value;
			}
		}

		internal Microsoft.Reporting.WinForms.Internal.Soap.ReportingServices2005.Execution.DataSourceCredentials ToSoapCredentials()
		{
			return new Microsoft.Reporting.WinForms.Internal.Soap.ReportingServices2005.Execution.DataSourceCredentials
			{
				DataSourceName = Name,
				UserName = UserId,
				Password = Password
			};
		}
	}
}
