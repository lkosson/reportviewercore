using Microsoft.ReportingServices.DataExtensions;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.ReportingServices.OnDemandProcessing
{
	internal class RetrievalManager
	{
		private Microsoft.ReportingServices.ReportIntermediateFormat.Report m_report;

		private DataSetDefinition m_dataSetDefinition;

		private bool m_noRows;

		private OnDemandProcessingContext m_odpContext;

		private List<RuntimeAtomicDataSource> m_runtimeDataSources = new List<RuntimeAtomicDataSource>();

		internal bool NoRows => m_noRows;

		internal RetrievalManager(Microsoft.ReportingServices.ReportIntermediateFormat.Report report, OnDemandProcessingContext context)
		{
			m_report = report;
			m_odpContext = context;
		}

		internal RetrievalManager(DataSetDefinition dataSetDefinition, OnDemandProcessingContext context)
		{
			m_dataSetDefinition = dataSetDefinition;
			m_odpContext = context;
		}

		internal void FetchParameterData(ReportParameterDataSetCache aCache, int aDataSourceIndex, int aDataSetIndex)
		{
			RuntimeDataSourceParameters item = new RuntimeDataSourceParameters(m_report, m_report.DataSources[aDataSourceIndex], m_odpContext, aDataSetIndex, aCache);
			m_runtimeDataSources.Add(item);
			FetchData();
		}

		internal bool FetchSharedDataSet(ParameterInfoCollection parameters)
		{
			if (parameters != null && parameters.Count != 0)
			{
				m_odpContext.ReportObjectModel.ParametersImpl.Clear();
				m_odpContext.ReportObjectModel.Initialize(parameters);
			}
			if (m_odpContext.ExternalDataSetContext.CachedDataChunkName == null)
			{
				return FetchSharedDataSetLive();
			}
			return FetchSharedDataSetCached();
		}

		private bool FetchSharedDataSetCached()
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.DataSet dataSet = new Microsoft.ReportingServices.ReportIntermediateFormat.DataSet(m_dataSetDefinition.DataSetCore);
			ProcessingDataReader processingDataReader = new ProcessingDataReader(new DataSetInstance(dataSet), dataSet, m_odpContext, overrideWithSharedDataSetChunkSettings: true);
			IRowConsumer consumerRequest = m_odpContext.ExternalDataSetContext.ConsumerRequest;
			consumerRequest.SetProcessingDataReader(processingDataReader);
			long num = 0L;
			try
			{
				while (processingDataReader.GetNextRow())
				{
					Microsoft.ReportingServices.ReportIntermediateFormat.RecordRow underlyingRecordRowObject = processingDataReader.GetUnderlyingRecordRowObject();
					consumerRequest.NextRow(underlyingRecordRowObject);
					num++;
				}
			}
			finally
			{
				if (m_odpContext.JobContext != null)
				{
					lock (m_odpContext.JobContext.SyncRoot)
					{
						m_odpContext.JobContext.RowCount += num;
					}
				}
			}
			return true;
		}

		private bool FetchSharedDataSetLive()
		{
			m_runtimeDataSources.Add(new RuntimeDataSourceSharedDataSet(m_dataSetDefinition, m_odpContext));
			try
			{
				return FetchData();
			}
			catch
			{
				m_runtimeDataSources[0].EraseDataChunk();
				throw;
			}
			finally
			{
				FinallyBlockForDataSetExecution();
			}
		}

		internal bool PrefetchData(Microsoft.ReportingServices.ReportIntermediateFormat.ReportInstance reportInstance, ParameterInfoCollection parameters, bool mergeTran)
		{
			if (m_report.DataSourceCount == 0)
			{
				return true;
			}
			try
			{
				bool flag = true;
				for (int i = 0; i < m_report.DataSourceCount; i++)
				{
					m_runtimeDataSources.Add(new RuntimeDataSourcePrefetch(m_report, reportInstance, m_report.DataSources[i], m_odpContext, mergeTran));
				}
				flag &= FetchData();
				if (m_report.ParametersNotUsedInQuery && m_odpContext.ErrorSavingSnapshotData)
				{
					for (int j = 0; j < parameters.Count; j++)
					{
						parameters[j].UsedInQuery = true;
					}
					return false;
				}
				return flag;
			}
			catch
			{
				foreach (RuntimeAtomicDataSource runtimeDataSource in m_runtimeDataSources)
				{
					runtimeDataSource.EraseDataChunk();
				}
				throw;
			}
			finally
			{
				FinallyBlockForDataSetExecution();
			}
		}

		private void FinallyBlockForDataSetExecution()
		{
			m_noRows = true;
			DataProcessingMetrics dataProcessingMetrics = null;
			foreach (RuntimeAtomicDataSource runtimeDataSource in m_runtimeDataSources)
			{
				if (dataProcessingMetrics == null || runtimeDataSource.ExecutionMetrics.TotalDurationMs > dataProcessingMetrics.TotalDurationMs)
				{
					dataProcessingMetrics = runtimeDataSource.ExecutionMetrics;
				}
				if (!runtimeDataSource.NoRows)
				{
					m_noRows = false;
				}
			}
			if (dataProcessingMetrics != null)
			{
				m_odpContext.ExecutionLogContext.AddDataProcessingTime(dataProcessingMetrics.TotalDuration);
			}
			m_runtimeDataSources.Clear();
		}

		private bool FetchData()
		{
			EventHandler eventHandler = null;
			int count = m_runtimeDataSources.Count;
			ThreadSet threadSet = null;
			try
			{
				if (m_odpContext.AbortInfo != null)
				{
					eventHandler = AbortHandler;
					m_odpContext.AbortInfo.ProcessingAbortEvent += eventHandler;
				}
				if (count != 0)
				{
					RuntimeAtomicDataSource @object;
					if (count > 1)
					{
						threadSet = new ThreadSet(count - 1);
						try
						{
							for (int i = 1; i < count; i++)
							{
								@object = m_runtimeDataSources[i];
								threadSet.TryQueueWorkItem(m_odpContext, @object.ProcessConcurrent);
							}
						}
						catch (Exception e)
						{
							if (m_odpContext.AbortInfo != null)
							{
								m_odpContext.AbortInfo.SetError(e, m_odpContext.ProcessingAbortItemUniqueIdentifier);
							}
							throw;
						}
					}
					@object = m_runtimeDataSources[0];
					@object.ProcessConcurrent(null);
				}
			}
			finally
			{
				if (threadSet != null && count > 1)
				{
					threadSet.WaitForCompletion();
					threadSet.Dispose();
				}
				if (eventHandler != null)
				{
					m_odpContext.AbortInfo.ProcessingAbortEvent -= eventHandler;
				}
			}
			m_odpContext.CheckAndThrowIfAborted();
			return true;
		}

		private void AbortHandler(object sender, EventArgs e)
		{
			if (e is ProcessingAbortEventArgs && ((ProcessingAbortEventArgs)e).UniqueName == m_odpContext.ProcessingAbortItemUniqueIdentifier)
			{
				if (Global.Tracer.TraceInfo)
				{
					Global.Tracer.Trace(TraceLevel.Info, "DataPrefetch abort handler called for Report with ID={0}. Aborting data sources ...", m_odpContext.ProcessingAbortItemUniqueIdentifier);
				}
				int count = m_runtimeDataSources.Count;
				for (int i = 0; i < count; i++)
				{
					m_runtimeDataSources[i].Abort();
				}
			}
		}
	}
}
