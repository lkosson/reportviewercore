using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Microsoft.Reporting.WinForms
{
	[ComVisible(false)]
	public sealed class ZoomChangeEventArgs : CancelEventArgs
	{
		private int m_zoomPercent;

		private ZoomMode m_zoomMode;

		public int ZoomPercent => m_zoomPercent;

		public ZoomMode ZoomMode => m_zoomMode;

		public ZoomChangeEventArgs(ZoomMode zoomMode, int zoomPercent)
		{
			m_zoomMode = zoomMode;
			m_zoomPercent = zoomPercent;
		}
	}
}
