using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Microsoft.Reporting.WinForms
{
	[ComVisible(false)]
	public sealed class BackEventArgs : CancelEventArgs
	{
		private Report m_parentReport;

		public Report ParentReport => m_parentReport;

		public BackEventArgs(Report parentReport)
		{
			m_parentReport = parentReport;
		}
	}
}
