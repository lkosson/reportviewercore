using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Microsoft.Reporting.WinForms
{
	[ComVisible(false)]
	public sealed class SearchEventArgs : CancelEventArgs
	{
		private string m_searchString;

		private int m_startPage;

		private bool m_isFindNext;

		public string SearchString => m_searchString;

		public int StartPage => m_startPage;

		public bool IsFindNext => m_isFindNext;

		public SearchEventArgs(string searchString, int startPage, bool isFindNext)
		{
			m_searchString = searchString;
			m_startPage = startPage;
			m_isFindNext = isFindNext;
		}
	}
}
