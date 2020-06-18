using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Microsoft.Reporting.WinForms
{
	[ComVisible(false)]
	public sealed class DrillthroughEventArgs : CancelEventArgs
	{
		private string m_reportPath;

		private Report m_report;

		public string ReportPath => m_reportPath;

		public Report Report => m_report;

		public DrillthroughEventArgs(string reportPath, Report targetReport)
		{
			m_reportPath = reportPath;
			m_report = targetReport;
		}
	}
}
