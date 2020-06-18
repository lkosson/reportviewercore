using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Microsoft.Reporting.WinForms
{
	[ComVisible(false)]
	public sealed class BookmarkNavigationEventArgs : CancelEventArgs
	{
		private string m_bookmarkId;

		public string BookmarkId => m_bookmarkId;

		public BookmarkNavigationEventArgs(string bookmarkId)
		{
			m_bookmarkId = bookmarkId;
		}
	}
}
