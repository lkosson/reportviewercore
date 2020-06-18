using System;

namespace Microsoft.ReportingServices.Diagnostics
{
	[Serializable]
	internal sealed class DatasourceCredentials
	{
		private string m_userName;

		private string m_password;

		private string m_promptID;

		public string UserName => m_userName;

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

		public string PromptID => m_promptID;

		public DatasourceCredentials(string promptID, string userName, string password)
		{
			m_promptID = promptID;
			m_userName = userName;
			m_password = password;
		}
	}
}
