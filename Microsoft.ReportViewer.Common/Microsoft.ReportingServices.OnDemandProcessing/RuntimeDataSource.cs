using Microsoft.Cloud.Platform.Utils;
using Microsoft.ReportingServices.Common;
using Microsoft.ReportingServices.DataExtensions;
using Microsoft.ReportingServices.DataProcessing;
using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.ReportingServices.OnDemandProcessing
{
	internal abstract class RuntimeDataSource
	{
		protected readonly OnDemandProcessingContext m_odpContext;

		private readonly Microsoft.ReportingServices.ReportIntermediateFormat.Report m_report;

		protected readonly Microsoft.ReportingServices.ReportIntermediateFormat.DataSource m_dataSource;

		protected List<RuntimeDataSet> m_runtimeDataSets;

		private bool m_canAbort;

		protected TimeMetric m_totalDurationFromExistingQuery;

		protected DataProcessingMetrics m_executionMetrics;

		private readonly bool m_mergeTran;

		protected IDbConnection m_connection;

		protected Microsoft.ReportingServices.ReportProcessing.ReportProcessing.TransactionInfo m_transaction;

		private bool m_needToCloseConnection;

		private bool m_isGlobalConnection;

		private bool m_isTransactionOwner;

		private bool m_isGlobalTransaction;

		protected bool m_useConcurrentDataSetProcessing;

		internal DataProcessingMetrics ExecutionMetrics => m_executionMetrics;

		internal abstract bool NoRows
		{
			get;
		}

		protected Microsoft.ReportingServices.ReportIntermediateFormat.Report ReportDefinition => m_report;

		protected Microsoft.ReportingServices.ReportIntermediateFormat.DataSource DataSourceDefinition => m_dataSource;

		protected OnDemandProcessingContext OdpContext => m_odpContext;

		protected virtual bool AllowConcurrentProcessing => false;

		protected virtual bool NeedsExecutionLogging => true;

		protected virtual bool CreatesDataChunks => false;

		protected RuntimeDataSource(Microsoft.ReportingServices.ReportIntermediateFormat.Report report, Microsoft.ReportingServices.ReportIntermediateFormat.DataSource dataSource, OnDemandProcessingContext processingContext, bool mergeTransactions)
		{
			m_report = report;
			m_dataSource = dataSource;
			m_odpContext = processingContext;
			m_runtimeDataSets = null;
			m_mergeTran = mergeTransactions;
			m_executionMetrics = new DataProcessingMetrics(m_odpContext.JobContext, m_odpContext.ExecutionLogContext);
			Global.Tracer.Assert(m_dataSource.Name != null, "The name of a data source cannot be null.");
		}

		internal virtual void Abort()
		{
			if (Global.Tracer.TraceVerbose)
			{
				Global.Tracer.Trace(TraceLevel.Verbose, "Data source '{0}': Abort handler called. CanAbort = {1}.", m_dataSource.Name, m_canAbort);
			}
			if (m_canAbort && m_runtimeDataSets != null)
			{
				int count = m_runtimeDataSets.Count;
				for (int i = 0; i < count; i++)
				{
					m_runtimeDataSets[i].Abort();
				}
			}
		}

		internal void EraseDataChunk()
		{
			Global.Tracer.Assert(CreatesDataChunks, "EraseDataChunk is invalid for the current RuntimeDataSource implementation.");
			if (m_runtimeDataSets == null)
			{
				return;
			}
			foreach (RuntimeDataSet runtimeDataSet in m_runtimeDataSets)
			{
				runtimeDataSet.EraseDataChunk();
			}
		}

		protected bool InitializeDataSource(ExecutedQuery existingQuery)
		{
			if (m_dataSource.DataSets == null || 0 >= m_dataSource.DataSets.Count)
			{
				return false;
			}
			m_connection = null;
			m_transaction = null;
			m_needToCloseConnection = false;
			m_isGlobalConnection = false;
			m_isTransactionOwner = false;
			m_isGlobalTransaction = false;
			m_runtimeDataSets = CreateRuntimeDataSets();
			if (0 >= m_runtimeDataSets.Count)
			{
				return false;
			}
			m_canAbort = true;
			m_odpContext.CheckAndThrowIfAborted();
			m_useConcurrentDataSetProcessing = (m_runtimeDataSets.Count > 1 && AllowConcurrentProcessing);
			if (!m_dataSource.IsArtificialForSharedDataSets)
			{
				if (existingQuery != null)
				{
					InitializeFromExistingQuery(existingQuery);
				}
				else
				{
					OpenInitialConnectionAndTransaction();
				}
			}
			return true;
		}

		protected void TeardownDataSource()
		{
			Global.Tracer.Trace(TraceLevel.Verbose, "Data source '{0}': Processing of all data sets completed.", m_dataSource.Name.MarkAsModelInfo());
			m_odpContext.CheckAndThrowIfAborted();
			ComputeAndUpdateRowCounts();
			CommitTransaction();
		}

		protected void HandleException(Exception e)
		{
			if (!(e is ProcessingAbortedException))
			{
				Global.Tracer.Trace(TraceLevel.Error, "Data source '{0}': An error has occurred. Details: {1}", m_dataSource.Name.MarkAsModelInfo(), e.ToString());
			}
			RollbackTransaction();
		}

		protected virtual void FinalCleanup()
		{
			CloseConnection();
		}

		private void CloseConnection()
		{
			if (m_needToCloseConnection)
			{
				CloseConnection(m_connection, m_dataSource, m_odpContext, m_executionMetrics);
				if (NeedsExecutionLogging && m_odpContext.ExecutionLogContext != null)
				{
					int num = (m_runtimeDataSets != null) ? m_runtimeDataSets.Count : 0;
					List<DataProcessingMetrics> list = new List<DataProcessingMetrics>();
					for (int i = 0; i < num; i++)
					{
						if (m_runtimeDataSets[i].IsConnectionOwner)
						{
							m_odpContext.ExecutionLogContext.AddDataSourceParallelExecutionMetrics(m_dataSource.Name, m_dataSource.DataSourceReference, m_dataSource.Type, m_runtimeDataSets[i].DataSetExecutionMetrics);
						}
						else
						{
							list.Add(m_runtimeDataSets[i].DataSetExecutionMetrics);
						}
					}
					m_odpContext.ExecutionLogContext.AddDataSourceMetrics(m_dataSource.Name, m_dataSource.DataSourceReference, m_dataSource.Type, m_executionMetrics, list.ToArray());
				}
			}
			m_connection = null;
		}

		internal void RecordTimeDataRetrieval()
		{
			m_odpContext.ExecutionLogContext.AddDataProcessingTime(m_executionMetrics.TotalDuration);
		}

		internal static DataSourceInfo GetDataSourceInfo(Microsoft.ReportingServices.ReportIntermediateFormat.DataSource dataSource, OnDemandProcessingContext processingContext)
		{
			if (processingContext.CreateAndSetupDataExtensionFunction.MustResolveSharedDataSources)
			{
				return dataSource.GetDataSourceInfo(processingContext);
			}
			return null;
		}

		private void RollbackTransaction()
		{
			if (m_transaction == null)
			{
				return;
			}
			m_transaction.RollbackRequired = true;
			if (m_isGlobalTransaction)
			{
				m_odpContext.GlobalDataSourceInfo.Remove(m_dataSource.Name);
			}
			if (m_isTransactionOwner)
			{
				Global.Tracer.Trace(TraceLevel.Error, "Data source '{0}': Rolling the transaction back.", m_dataSource.Name.MarkAsModelInfo());
				try
				{
					m_transaction.Transaction.Rollback();
				}
				catch (Exception innerException)
				{
					throw new ReportProcessingException(ErrorCode.rsErrorRollbackTransaction, innerException, m_dataSource.Name.MarkAsModelInfo());
				}
			}
			m_transaction = null;
		}

		private void CommitTransaction()
		{
			if (m_isTransactionOwner)
			{
				if (m_isGlobalTransaction)
				{
					if (m_isGlobalConnection)
					{
						m_needToCloseConnection = false;
					}
				}
				else
				{
					Global.Tracer.Trace(TraceLevel.Verbose, "Data source '{0}': Committing transaction.", m_dataSource.Name.MarkAsModelInfo());
					try
					{
						m_transaction.Transaction.Commit();
					}
					catch (Exception innerException)
					{
						throw new ReportProcessingException(ErrorCode.rsErrorCommitTransaction, innerException, m_dataSource.Name.MarkAsModelInfo());
					}
				}
				m_isTransactionOwner = false;
			}
			m_transaction = null;
		}

		private void ComputeAndUpdateRowCounts()
		{
			for (int i = 0; i < m_runtimeDataSets.Count; i++)
			{
				m_executionMetrics.AddRowCount(m_runtimeDataSets[i].NumRowsRead);
			}
			IJobContext jobContext = m_odpContext.JobContext;
			if (NeedsExecutionLogging && jobContext != null)
			{
				lock (jobContext.SyncRoot)
				{
					jobContext.RowCount += m_executionMetrics.TotalRowsRead;
				}
			}
		}

		private void InitializeFromExistingQuery(ExecutedQuery query)
		{
			query.ReleaseOwnership(ref m_connection);
			m_needToCloseConnection = true;
			MergeAutoCollationSettings(m_connection);
			m_executionMetrics.Add(DataProcessingMetrics.MetricType.OpenConnection, query.ExecutionMetrics.OpenConnectionDurationMs);
			m_executionMetrics.ConnectionFromPool = query.ExecutionMetrics.ConnectionFromPool;
			m_totalDurationFromExistingQuery = new TimeMetric(query.ExecutionMetrics.TotalDuration);
		}

		protected virtual void OpenInitialConnectionAndTransaction()
		{
			if (m_dataSource.Transaction && m_mergeTran)
			{
				Microsoft.ReportingServices.ReportProcessing.ReportProcessing.DataSourceInfo dataSourceInfo = m_odpContext.GlobalDataSourceInfo[m_dataSource.Name];
				if (dataSourceInfo != null)
				{
					m_connection = dataSourceInfo.Connection;
					m_transaction = dataSourceInfo.TransactionInfo;
				}
			}
			Global.Tracer.Trace(TraceLevel.Verbose, "Data source '{0}': Transaction = {1}, MergeTran = {2}, NumDataSets = {3}", m_dataSource.Name.MarkAsModelInfo(), m_dataSource.Transaction, m_mergeTran, m_runtimeDataSets.Count);
			if (m_connection == null)
			{
				Microsoft.ReportingServices.ReportIntermediateFormat.DataSet dataSet = m_runtimeDataSets[0].DataSet;
				m_connection = OpenConnection(m_dataSource, dataSet, m_odpContext, m_executionMetrics);
				m_needToCloseConnection = true;
				Global.Tracer.Trace(TraceLevel.Verbose, "Data source '{0}': Created a connection.", m_dataSource.Name.MarkAsModelInfo());
			}
			bool flag = false;
			if (m_dataSource.Transaction)
			{
				if (m_transaction == null)
				{
					IDbTransaction transaction = m_connection.BeginTransaction();
					Global.Tracer.Trace(TraceLevel.Verbose, "Data source '{0}': Begun a transaction.", m_dataSource.Name.MarkAsModelInfo());
					m_transaction = new Microsoft.ReportingServices.ReportProcessing.ReportProcessing.TransactionInfo(transaction);
					m_isTransactionOwner = true;
				}
				flag = ((m_transaction.Transaction as IDbTransactionExtension)?.AllowMultiConnection ?? false);
				m_useConcurrentDataSetProcessing &= flag;
				Global.Tracer.Trace(TraceLevel.Verbose, "Data source '{0}': TransactionCanSpanConnections = {1}, ConcurrentDataSets = {2}", m_dataSource.Name.MarkAsModelInfo(), flag, m_useConcurrentDataSetProcessing);
			}
			MergeAutoCollationSettings(m_connection);
			if (m_isTransactionOwner && m_report.SubReportMergeTransactions && !m_odpContext.ProcessReportParameters)
			{
				IDbConnection connection;
				if (flag)
				{
					connection = null;
					m_isGlobalConnection = false;
				}
				else
				{
					connection = m_connection;
					m_isGlobalConnection = true;
				}
				Global.Tracer.Trace(TraceLevel.Verbose, "Data source '{0}': Storing trans+conn into GlobalDataSourceInfo. CloseConnection = {1}.", m_dataSource.Name.MarkAsModelInfo(), m_needToCloseConnection);
				DataSourceInfo dataSourceInfo2 = GetDataSourceInfo(m_dataSource, m_odpContext);
				m_odpContext.GlobalDataSourceInfo.Add(m_dataSource, connection, m_transaction, dataSourceInfo2);
				m_isGlobalTransaction = true;
			}
		}

		private void MergeAutoCollationSettings(IDbConnection connection)
		{
			if (!(connection is IDbCollationProperties) || !m_dataSource.AnyActiveDataSetNeedsAutoDetectCollation())
			{
				return;
			}
			try
			{
				if (((IDbCollationProperties)connection).GetCollationProperties(out string cultureName, out bool caseSensitive, out bool accentSensitive, out bool kanatypeSensitive, out bool widthSensitive))
				{
					m_dataSource.MergeCollationSettingsForAllDataSets(m_odpContext.ErrorContext, cultureName, caseSensitive, accentSensitive, kanatypeSensitive, widthSensitive);
				}
			}
			catch (Exception ex)
			{
				m_odpContext.ErrorContext.Register(ProcessingErrorCode.rsCollationDetectionFailed, Severity.Warning, ObjectType.DataSource, m_dataSource.Name, "Collation", ex.ToString());
			}
		}

		protected abstract List<RuntimeDataSet> CreateRuntimeDataSets();

		internal static IDbConnection OpenConnection(Microsoft.ReportingServices.ReportIntermediateFormat.DataSource dataSourceObj, Microsoft.ReportingServices.ReportIntermediateFormat.DataSet dataSetObj, OnDemandProcessingContext pc, DataProcessingMetrics metrics)
		{
			IDbConnection dbConnection = null;
			try
			{
				metrics.StartTimer(DataProcessingMetrics.MetricType.OpenConnection);
				DataSourceInfo dataSourceInfo = null;
				string text = null;
				if (pc.CreateAndSetupDataExtensionFunction.MustResolveSharedDataSources)
				{
					text = dataSourceObj.ResolveConnectionString(pc, out dataSourceInfo);
					if (pc.UseVerboseExecutionLogging)
					{
						metrics.ResolvedConnectionString = text;
					}
				}
				return pc.CreateAndSetupDataExtensionFunction.OpenDataSourceExtensionConnection(dataSourceObj, text, dataSourceInfo, dataSetObj.Name);
			}
			catch (RSException)
			{
				throw;
			}
			catch (Exception ex2)
			{
				if (AsynchronousExceptionDetection.IsStoppingException(ex2))
				{
					throw;
				}
				throw new ReportProcessingException(ErrorCode.rsErrorOpeningConnection, ex2, dataSourceObj.Name);
			}
			finally
			{
				long num = metrics.RecordTimerMeasurementWithUpdatedTotal(DataProcessingMetrics.MetricType.OpenConnection);
				Global.Tracer.Trace(TraceLevel.Verbose, "Opening a connection for DataSource: {0} took {1} ms.", dataSourceObj.Name.MarkAsModelInfo(), num);
			}
		}

		internal static void CloseConnection(IDbConnection connection, Microsoft.ReportingServices.ReportIntermediateFormat.DataSource dataSource, OnDemandProcessingContext odpContext, DataProcessingMetrics executionMetrics)
		{
			try
			{
				DataSourceInfo dataSourceInfo = GetDataSourceInfo(dataSource, odpContext);
				odpContext.CreateAndSetupDataExtensionFunction.CloseConnection(connection, dataSource, dataSourceInfo);
			}
			catch (Exception innerException)
			{
				throw new ReportProcessingException(ErrorCode.rsErrorClosingConnection, innerException, dataSource.Name);
			}
		}

		protected bool CheckNoRows(RuntimeDataSet runtimeDataSet)
		{
			return runtimeDataSet?.NoRows ?? false;
		}
	}
}
