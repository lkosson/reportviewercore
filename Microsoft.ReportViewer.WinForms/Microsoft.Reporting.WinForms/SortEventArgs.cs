using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Microsoft.Reporting.WinForms
{
	[ComVisible(false)]
	public sealed class SortEventArgs : CancelEventArgs
	{
		private string m_sortId;

		private SortOrder m_sortDirection;

		private bool m_clearSort;

		public string SortId => m_sortId;

		public SortOrder SortDirection => m_sortDirection;

		public bool ClearSort => m_clearSort;

		public SortEventArgs(string sortId, SortOrder sortDirection, bool clearSort)
		{
			m_sortId = sortId;
			m_sortDirection = sortDirection;
			m_clearSort = clearSort;
		}
	}
}
