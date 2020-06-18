using Microsoft.ReportingServices.DataProcessing;
using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.Diagnostics.Internal;
using System;
using System.Threading;

namespace Microsoft.ReportingServices
{
	internal class ViewerJobContextImpl : IJobContext
	{
		private readonly object m_sync = new object();

		private readonly AdditionalInfo m_additionalInfo = new AdditionalInfo();

		public object SyncRoot => m_sync;

		public ExecutionLogLevel ExecutionLogLevel => ExecutionLogLevel.Normal;

		public TimeSpan TimeDataRetrieval
		{
			get
			{
				return TimeSpan.Zero;
			}
			set
			{
			}
		}

		public TimeSpan TimeProcessing
		{
			get
			{
				return TimeSpan.Zero;
			}
			set
			{
			}
		}

		public TimeSpan TimeRendering
		{
			get
			{
				return TimeSpan.Zero;
			}
			set
			{
			}
		}

		public long RowCount
		{
			get
			{
				return 0L;
			}
			set
			{
			}
		}

		public AdditionalInfo AdditionalInfo
		{
			get
			{
				return m_additionalInfo;
			}
			set
			{
			}
		}

		public string ExecutionId => string.Empty;

		public void AddAbortHelper(IAbortHelper abortHelper)
		{
		}

		public IAbortHelper GetAbortHelper()
		{
			return null;
		}

		public void RemoveAbortHelper()
		{
		}

		public void AddCommand(IDbCommand cmd)
		{
		}

		public void RemoveCommand(IDbCommand cmd)
		{
		}

		public bool ApplyCommandMemoryLimit(IDbCommand cmd)
		{
			return false;
		}

		public bool SetAdditionalCorrelation(IDbCommand cmd)
		{
			return false;
		}

		public void TryQueueWorkItem(WaitCallback callback, object state)
		{
			callback?.Invoke(state);
		}

		public void QueueWorkItem(WaitCallback callback, object state)
		{
			throw new NotImplementedException();
		}
	}
}
