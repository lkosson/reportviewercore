using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Diagnostics;
using System.Threading;

namespace Microsoft.ReportingServices.OnDemandProcessing
{
	internal abstract class AbortHelper : IAbortHelper, IDisposable
	{
		private ProcessingStatus m_overallStatus;

		private Exception m_exception;

		private readonly IJobContext m_jobContext;

		private bool m_enforceSingleAbortException;

		private bool m_hasThrownAbortedException;

		private readonly bool m_requireEventHandler;

		private readonly object m_syncRoot;

		protected ProcessingStatus Status
		{
			get
			{
				return m_overallStatus;
			}
			set
			{
				m_overallStatus = value;
			}
		}

		internal bool EnforceSingleAbortException
		{
			get
			{
				return m_enforceSingleAbortException;
			}
			set
			{
				m_enforceSingleAbortException = value;
			}
		}

		public event EventHandler ProcessingAbortEvent;

		internal AbortHelper(IJobContext jobContext, bool enforceSingleAbortException, bool requireEventHandler)
		{
			m_enforceSingleAbortException = enforceSingleAbortException;
			m_requireEventHandler = requireEventHandler;
			m_syncRoot = new object();
			if (jobContext != null)
			{
				m_jobContext = jobContext;
				jobContext.AddAbortHelper(this);
			}
		}

		public void Dispose()
		{
			Dispose(disposing: true);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing && m_jobContext != null)
			{
				m_jobContext.RemoveAbortHelper();
			}
		}

		public virtual bool Abort(ProcessingStatus status)
		{
			return Abort(status, null);
		}

		protected abstract ProcessingStatus GetStatus(string uniqueName);

		protected abstract void SetStatus(ProcessingStatus newStatus, string uniqueName);

		internal abstract void AddSubreportInstanceOrSharedDataSet(string uniqueName);

		internal bool SetError(Exception e, string uniqueName)
		{
			Global.Tracer.Trace(TraceLevel.Verbose, "An exception has occurred. Trying to abort processing. Details: {0}", (e == null) ? "" : e.ToString());
			if (m_exception == null)
			{
				m_exception = e;
			}
			else if (e is DataSetExecutionException && e.InnerException == m_exception)
			{
				m_exception = e;
			}
			if (!Abort(ProcessingStatus.AbnormalTermination, uniqueName))
			{
				return false;
			}
			return true;
		}

		private bool Abort(ProcessingStatus status, string uniqueName)
		{
			bool result = !m_requireEventHandler;
			if (!Monitor.TryEnter(m_syncRoot))
			{
				Global.Tracer.Trace(TraceLevel.Info, "Some other thread is aborting processing.");
				return result;
			}
			try
			{
				if (GetStatus(uniqueName) != 0)
				{
					Global.Tracer.Trace(TraceLevel.Info, "Some other thread has already aborted processing.");
					return result;
				}
				SetStatus(status, uniqueName);
				if (this.ProcessingAbortEvent != null)
				{
					try
					{
						this.ProcessingAbortEvent(this, new ProcessingAbortEventArgs(uniqueName));
						result = true;
						Global.Tracer.Trace(TraceLevel.Verbose, "Abort callback successful.");
						return result;
					}
					catch (Exception ex)
					{
						Global.Tracer.Trace(TraceLevel.Error, "Exception in abort callback. Details: {0}", ex.ToString());
						return result;
					}
				}
				Global.Tracer.Trace(TraceLevel.Verbose, "No abort callback.");
				return result;
			}
			finally
			{
				Monitor.Exit(m_syncRoot);
			}
		}

		internal virtual void ThrowIfAborted(CancelationTrigger cancelationTrigger, string uniqueName)
		{
			ProcessingStatus status;
			lock (m_syncRoot)
			{
				status = GetStatus(uniqueName);
			}
			if (status != 0 && (!m_hasThrownAbortedException || !m_enforceSingleAbortException))
			{
				m_hasThrownAbortedException = true;
				if (status == ProcessingStatus.AbnormalTermination)
				{
					throw new ProcessingAbortedException(cancelationTrigger, m_exception);
				}
				throw new ProcessingAbortedException(cancelationTrigger);
			}
		}
	}
}
