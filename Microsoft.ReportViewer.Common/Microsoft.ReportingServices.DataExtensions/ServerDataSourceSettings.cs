namespace Microsoft.ReportingServices.DataExtensions
{
	internal sealed class ServerDataSourceSettings
	{
		private bool m_allowIntegratedSecurity = true;

		private bool m_isSurrogatePresent;

		public bool IsSurrogatePresent => m_isSurrogatePresent;

		public bool AllowIntegratedSecurity => m_allowIntegratedSecurity;

		public ServerDataSourceSettings(bool isSurrogatePresent, bool allowIntegratedSecurity)
		{
			m_isSurrogatePresent = isSurrogatePresent;
			m_allowIntegratedSecurity = allowIntegratedSecurity;
		}
	}
}
