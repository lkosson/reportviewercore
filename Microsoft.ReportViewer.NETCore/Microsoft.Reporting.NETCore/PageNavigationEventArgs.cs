using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Microsoft.Reporting.NETCore
{
	[ComVisible(false)]
	public sealed class PageNavigationEventArgs : CancelEventArgs
	{
		private int m_newPage;

		public int NewPage => m_newPage;

		public PageNavigationEventArgs(int newPage)
		{
			m_newPage = newPage;
		}
	}
}
