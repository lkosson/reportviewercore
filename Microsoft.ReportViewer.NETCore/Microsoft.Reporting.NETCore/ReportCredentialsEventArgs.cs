using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Microsoft.Reporting.NETCore
{
	[ComVisible(false)]
	public class ReportCredentialsEventArgs : CancelEventArgs
	{
		private DataSourceCredentialsCollection m_credentials;

		public DataSourceCredentialsCollection Credentials => m_credentials;

		internal ReportCredentialsEventArgs(DataSourceCredentialsCollection credentials)
		{
			m_credentials = credentials;
		}
	}
}
