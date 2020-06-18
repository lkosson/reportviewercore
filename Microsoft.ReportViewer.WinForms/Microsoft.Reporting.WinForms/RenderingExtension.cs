using Microsoft.Reporting.WinForms.Internal.Soap.ReportingServices2005.Execution;
using System;

namespace Microsoft.Reporting.WinForms
{
	[Serializable]
	public sealed class RenderingExtension
	{
		private string m_name;

		private string m_localizedName;

		private bool m_isVisible;

		public string Name => m_name;

		public string LocalizedName => m_localizedName;

		public bool Visible => m_isVisible;

		internal RenderingExtension(string name, string localizedName, bool isVisible)
		{
			m_name = name;
			m_localizedName = localizedName;
			m_isVisible = isVisible;
		}

		internal static RenderingExtension[] FromSoapExtensions(Extension[] soapExtensions)
		{
			if (soapExtensions == null)
			{
				return null;
			}
			RenderingExtension[] array = new RenderingExtension[soapExtensions.Length];
			for (int i = 0; i < soapExtensions.Length; i++)
			{
				array[i] = new RenderingExtension(soapExtensions[i].Name, soapExtensions[i].LocalizedName, soapExtensions[i].Visible);
			}
			return array;
		}
	}
}
