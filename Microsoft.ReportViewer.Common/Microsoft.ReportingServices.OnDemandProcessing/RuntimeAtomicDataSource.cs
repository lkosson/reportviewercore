using Microsoft.Cloud.Platform.Utils;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Diagnostics;

namespace Microsoft.ReportingServices.OnDemandProcessing
{
	internal abstract class RuntimeAtomicDataSource : RuntimeDataSource
	{
		protected RuntimeAtomicDataSource(Microsoft.ReportingServices.ReportIntermediateFormat.Report report, Microsoft.ReportingServices.ReportIntermediateFormat.DataSource dataSource, OnDemandProcessingContext processingContext, bool mergeTransactions)
			: base(report, dataSource, processingContext, mergeTransactions)
		{
		}

		internal void ProcessConcurrent(object threadSet)
		{
			try
			{
				if (Global.Tracer.TraceVerbose)
				{
					Global.Tracer.Trace(TraceLevel.Verbose, "Thread has started processing data source '{0}'", base.DataSourceDefinition.Name.MarkAsModelInfo());
				}
				Process(fromOdp: false);
			}
			catch (ProcessingAbortedException)
			{
				if (Global.Tracer.TraceWarning)
				{
					Global.Tracer.Trace(TraceLevel.Warning, "Data source '{0}': Report processing has been aborted.", base.DataSourceDefinition.Name.MarkAsModelInfo());
				}
				if (m_odpContext.StreamingMode)
				{
					throw;
				}
			}
			catch (Exception ex2)
			{
				if (Global.Tracer.TraceError)
				{
					Global.Tracer.Trace(TraceLevel.Error, "An exception has occurred in data source '{0}'. Details: {1}", base.DataSourceDefinition.Name.MarkAsModelInfo(), ex2.ToString());
				}
				if (base.OdpContext.AbortInfo != null)
				{
					base.OdpContext.AbortInfo.SetError(ex2, base.OdpContext.ProcessingAbortItemUniqueIdentifier);
					return;
				}
				throw;
			}
			finally
			{
				if (Global.Tracer.TraceVerbose)
				{
					Global.Tracer.Trace(TraceLevel.Verbose, "Processing of data source '{0}' completed.", base.DataSourceDefinition.Name.MarkAsModelInfo());
				}
				(threadSet as ThreadSet)?.ThreadCompleted();
			}
		}

		private void Process(bool fromOdp)
		{
			try
			{
				if (InitializeDataSource(null))
				{
					if (m_useConcurrentDataSetProcessing)
					{
						ExecuteParallelDataSets();
					}
					else
					{
						ExecuteSequentialDataSets();
					}
					TeardownDataSource();
				}
			}
			catch (Exception e)
			{
				HandleException(e);
				throw;
			}
			finally
			{
				FinalCleanup();
			}
		}

		private void ExecuteSequentialDataSets()
		{
			for (int i = 0; i < m_runtimeDataSets.Count; i++)
			{
				m_odpContext.CheckAndThrowIfAborted();
				RuntimeAtomicDataSet runtimeAtomicDataSet = (RuntimeAtomicDataSet)m_runtimeDataSets[i];
				runtimeAtomicDataSet.InitProcessingParams(m_connection, m_transaction);
				runtimeAtomicDataSet.ProcessConcurrent(null);
				m_executionMetrics.Add(runtimeAtomicDataSet.DataSetExecutionMetrics);
			}
		}

		private void ExecuteParallelDataSets()
		{
			ThreadSet threadSet = new ThreadSet(m_runtimeDataSets.Count - 1);
			try
			{
				for (int i = 1; i < m_runtimeDataSets.Count; i++)
				{
					RuntimeAtomicDataSet runtimeAtomicDataSet = (RuntimeAtomicDataSet)m_runtimeDataSets[i];
					runtimeAtomicDataSet.InitProcessingParams(null, m_transaction);
					threadSet.TryQueueWorkItem(m_odpContext, runtimeAtomicDataSet.ProcessConcurrent);
				}
				RuntimeAtomicDataSet obj = (RuntimeAtomicDataSet)m_runtimeDataSets[0];
				obj.InitProcessingParams(m_connection, m_transaction);
				obj.ProcessConcurrent(null);
			}
			catch (Exception e)
			{
				if (m_odpContext.AbortInfo != null)
				{
					m_odpContext.AbortInfo.SetError(e, m_odpContext.ProcessingAbortItemUniqueIdentifier);
				}
				throw;
			}
			finally
			{
				threadSet.WaitForCompletion();
				threadSet.Dispose();
			}
			if (!NeedsExecutionLogging || m_odpContext.JobContext == null)
			{
				return;
			}
			DataProcessingMetrics dataProcessingMetrics = null;
			for (int j = 0; j < m_runtimeDataSets.Count; j++)
			{
				RuntimeDataSet runtimeDataSet = m_runtimeDataSets[j];
				if (dataProcessingMetrics == null || runtimeDataSet.DataSetExecutionMetrics.TotalDurationMs > dataProcessingMetrics.TotalDurationMs)
				{
					dataProcessingMetrics = runtimeDataSet.DataSetExecutionMetrics;
				}
			}
			m_executionMetrics.Add(dataProcessingMetrics);
		}
	}
}
