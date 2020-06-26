using System.Collections.Generic;
using System.Collections.Specialized;

namespace Microsoft.Reporting.NETCore
{
	internal sealed class ExtensionSettings
	{
		private readonly string m_extensionName;

		private readonly NameValueCollection m_extensionParameters;

		public NameValueCollection Settings => m_extensionParameters;

		public ExtensionSettings(string name, NameValueCollection extensionParameters)
		{
			m_extensionName = name;
			m_extensionParameters = extensionParameters;
		}
	}
}
