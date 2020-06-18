using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Diagnostics;
using System.Threading;

namespace Microsoft.ReportingServices.OnDemandProcessing
{
	internal sealed class ThreadSet : IDisposable
	{
		private int m_runningThreadCount;

		private ManualResetEvent m_allThreadsDone;

		private bool m_disposed;

		private bool m_waitCalled;

		private object m_counterLock = new object();

		internal ThreadSet(int expectedThreadCount)
		{
			if (Global.Tracer.TraceVerbose)
			{
				Global.Tracer.Trace(TraceLevel.Verbose, "ThreadSet object created. {0} threads remaining.", expectedThreadCount);
			}
			m_allThreadsDone = new ManualResetEvent(initialState: true);
		}

		internal void TryQueueWorkItem(OnDemandProcessingContext processingContext, WaitCallback workItemCallback)
		{
			try
			{
				Interlocked.Increment(ref m_runningThreadCount);
				processingContext.JobContext.TryQueueWorkItem(workItemCallback, this);
			}
			catch (Exception)
			{
				Interlocked.Decrement(ref m_runningThreadCount);
				throw;
			}
		}

		internal void QueueWorkItem(OnDemandProcessingContext processingContext, WaitCallback workItemCallback)
		{
			try
			{
				Interlocked.Increment(ref m_runningThreadCount);
				processingContext.JobContext.QueueWorkItem(workItemCallback, this);
			}
			catch (Exception)
			{
				Interlocked.Decrement(ref m_runningThreadCount);
				throw;
			}
		}

		internal void ThreadCompleted()
		{
			int num;
			lock (m_counterLock)
			{
				num = Interlocked.Decrement(ref m_runningThreadCount);
				if (num <= 0)
				{
					m_allThreadsDone.Set();
				}
			}
			if (Global.Tracer.TraceVerbose)
			{
				Global.Tracer.Trace(TraceLevel.Verbose, "Thread completed. {0} thread remaining.", num);
			}
		}

		internal void WaitForCompletion()
		{
			m_waitCalled = true;
			lock (m_counterLock)
			{
				if (Thread.VolatileRead(ref m_runningThreadCount) > 0)
				{
					m_allThreadsDone.Reset();
				}
			}
			m_allThreadsDone.WaitOne();
			if (Global.Tracer.TraceVerbose)
			{
				Global.Tracer.Trace(TraceLevel.Verbose, "All the processing threads have completed.");
			}
		}

		public void Dispose()
		{
			if (!m_disposed)
			{
				m_disposed = true;
				m_allThreadsDone.Close();
			}
		}
	}
}
