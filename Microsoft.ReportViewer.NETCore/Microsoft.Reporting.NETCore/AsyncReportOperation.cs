using System;
using System.ComponentModel;

namespace Microsoft.Reporting.NETCore
{
	internal abstract class AsyncReportOperation
	{
		private Report m_report;

		public Report Report => m_report;

		public event AsyncCompletedEventHandler Completed;

		protected AsyncReportOperation(Report report)
		{
			m_report = report;
		}

		public void BeginAsyncExecution()
		{
			PerformOperation();
		}

		public void EndAsyncExecution(Exception e)
		{
			if (this.Completed != null)
			{
				this.Completed(this, new AsyncCompletedEventArgs(e, e is OperationCanceledException, null));
			}
		}

		public bool Abort()
		{
			if (m_report.CanSelfCancel)
			{
				m_report.SetCancelState(shouldCancel: true);
				return true;
			}
			return false;
		}

		public void ClearAbortFlag()
		{
			if (m_report.CanSelfCancel)
			{
				m_report.SetCancelState(shouldCancel: false);
			}
		}

		protected abstract void PerformOperation();
	}
}
