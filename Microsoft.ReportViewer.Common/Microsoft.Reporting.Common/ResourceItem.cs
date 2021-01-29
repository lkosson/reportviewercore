using System.Web;

namespace Microsoft.Reporting.Common
{
	internal sealed class ResourceItem
	{
		private readonly string m_name;

		private readonly string m_debugName;

		private readonly string m_mimeType;

		internal string EffectiveName
		{
			get
			{
				if (!IsDebuggingEnabled)
				{
					return m_name;
				}
				return m_debugName;
			}
		}

		private static bool IsDebuggingEnabled => false;

		internal string MimeType => m_mimeType;

		internal ResourceItem(string name, string debugName, string mimeType)
		{
			m_name = name;
			m_debugName = debugName;
			m_mimeType = mimeType;
		}

		internal ResourceItem(string name, string mimeType)
			: this(name, name, mimeType)
		{
		}
	}
}
