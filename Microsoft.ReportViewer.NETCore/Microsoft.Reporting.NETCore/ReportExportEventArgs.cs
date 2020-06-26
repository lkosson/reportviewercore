using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Microsoft.Reporting.NETCore
{
	[ComVisible(false)]
	public sealed class ReportExportEventArgs : CancelEventArgs
	{
		private RenderingExtension m_extension;

		private string m_deviceInfo;

		public RenderingExtension Extension => m_extension;

		public string DeviceInfo
		{
			get
			{
				return m_deviceInfo;
			}
			set
			{
				m_deviceInfo = value;
			}
		}

		public ReportExportEventArgs(RenderingExtension extension)
		{
			m_extension = extension;
		}
	}
}
