using Microsoft.ReportingServices.DataProcessing;
using Microsoft.ReportingServices.Diagnostics.Internal;
using System;
using System.Threading;

namespace Microsoft.ReportingServices.Diagnostics
{
	internal interface IJobContext
	{
		object SyncRoot
		{
			get;
		}

		ExecutionLogLevel ExecutionLogLevel
		{
			get;
		}

		TimeSpan TimeDataRetrieval
		{
			get;
			set;
		}

		TimeSpan TimeProcessing
		{
			get;
			set;
		}

		TimeSpan TimeRendering
		{
			get;
			set;
		}

		AdditionalInfo AdditionalInfo
		{
			get;
		}

		long RowCount
		{
			get;
			set;
		}

		string ExecutionId
		{
			get;
		}

		void AddAbortHelper(IAbortHelper abortHelper);

		IAbortHelper GetAbortHelper();

		void RemoveAbortHelper();

		void AddCommand(IDbCommand cmd);

		void RemoveCommand(IDbCommand cmd);

		bool ApplyCommandMemoryLimit(IDbCommand cmd);

		bool SetAdditionalCorrelation(IDbCommand cmd);

		void TryQueueWorkItem(WaitCallback callback, object state);

		void QueueWorkItem(WaitCallback callback, object state);
	}
}
