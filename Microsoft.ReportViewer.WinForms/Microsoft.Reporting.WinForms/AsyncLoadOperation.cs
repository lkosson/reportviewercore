using System.IO;

namespace Microsoft.Reporting.WinForms
{
	internal class AsyncLoadOperation : AsyncReportOperation
	{
		private Stream m_reportDefinition;

		public AsyncLoadOperation(Report report, Stream reportDefinition)
			: base(report)
		{
			m_reportDefinition = reportDefinition;
		}

		protected override void PerformOperation()
		{
			base.Report.LoadReportDefinition(m_reportDefinition);
		}
	}
}
