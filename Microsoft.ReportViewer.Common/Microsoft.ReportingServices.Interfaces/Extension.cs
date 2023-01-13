using System.Security.Permissions;

namespace Microsoft.ReportingServices.Interfaces
{
	public class Extension
	{
		private string m_name;

		private string m_localizedName;

		private bool m_visible;

		public string Name => m_name;

		public string LocalizedName => m_localizedName;

		public bool Visible => m_visible;

		public Extension(string name, string localizedName, bool visible)
		{
			m_name = name;
			m_localizedName = localizedName;
			m_visible = visible;
		}
	}
}
