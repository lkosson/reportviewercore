using System;
using System.Security;
using System.Security.Permissions;
using System.Threading;

namespace Microsoft.Reporting.WinForms
{
	internal sealed class ProcessingThread
	{
		private AsyncReportOperation m_operation;

		private Thread m_backgroundThread;

		private bool IsRendering
		{
			get
			{
				if (m_backgroundThread != null)
				{
					return m_backgroundThread.IsAlive;
				}
				return false;
			}
		}

		public bool Cancel(int millisecondsTimeout)
		{
			if (IsRendering)
			{
				try
				{
					AsyncReportOperation operation = m_operation;
					if (operation != null && !operation.Abort())
					{
						millisecondsTimeout = 0;
					}
				}
				catch (ThreadStateException)
				{
					if (IsRendering)
					{
						throw;
					}
				}
				if (millisecondsTimeout != 0)
				{
					return m_backgroundThread.Join(millisecondsTimeout);
				}
				return false;
			}
			return true;
		}

		public void BeginBackgroundOperation(AsyncReportOperation operation)
		{
			if (m_backgroundThread != null)
			{
				m_backgroundThread.Join();
			}
			operation.ClearAbortFlag();
			m_operation = operation;
			m_backgroundThread = new Thread(ProcessThreadMain);
			try
			{
				PropagateThreadCulture();
			}
			catch (SecurityException)
			{
			}
			m_backgroundThread.Name = "Rendering";
			m_backgroundThread.IsBackground = true;
			m_backgroundThread.Start(operation);
		}

		[SecurityPermission(SecurityAction.Assert, ControlThread = true)]
		private void PropagateThreadCulture()
		{
			m_backgroundThread.CurrentCulture = Thread.CurrentThread.CurrentCulture;
			m_backgroundThread.CurrentUICulture = Thread.CurrentThread.CurrentUICulture;
		}

		private void ProcessThreadMain(object arg)
		{
			Exception e = null;
			try
			{
				m_operation.BeginAsyncExecution();
			}
			catch (Exception ex)
			{
				e = ex;
				Exception ex2 = ex;
				while (true)
				{
					if (ex2 != null)
					{
						if (ex2 is ThreadAbortException || ex2 is OperationCanceledException)
						{
							break;
						}
						ex2 = ex2.InnerException;
						continue;
					}
					return;
				}
				e = new OperationCanceledException();
			}
			finally
			{
				m_operation.EndAsyncExecution(e);
				m_operation = null;
			}
		}
	}
}
