using Microsoft.ReportingServices.OnDemandReportRendering;
using System;

namespace Microsoft.Reporting
{
	[Serializable]
	internal sealed class LocalRenderingExtensionInfo
	{
		private string m_name;

		private string m_localizedName;

		private bool m_isVisible;

		private Type m_type;

		private bool m_isExposedExternally = true;

		public string Name => m_name;

		public string LocalizedName => m_localizedName;

		public bool IsVisible => m_isVisible;

		internal bool IsExposedExternally => m_isExposedExternally;

		internal LocalRenderingExtensionInfo(string name, string localizedName, bool isVisible)
		{
			m_name = name;
			m_localizedName = localizedName;
			m_isVisible = isVisible;
		}

		internal LocalRenderingExtensionInfo(string name, string localizedName, bool isVisible, Type type, bool isExposedExternally)
			: this(name, localizedName, isVisible)
		{
			m_type = type;
			m_isExposedExternally = isExposedExternally;
		}

		internal IRenderingExtension Instantiate()
		{
			if (m_type == null)
			{
				throw new Exception("Internal Error: Direct instantiation is only available during standalone local mode");
			}
			return (IRenderingExtension)Activator.CreateInstance(m_type);
		}
	}
}
