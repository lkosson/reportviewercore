using Microsoft.ReportingServices.DataProcessing;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using System;
using System.Threading;

namespace Microsoft.ReportingServices.OnDemandProcessing
{
	internal sealed class ExecutedQuery
	{
		private readonly DataSource m_dataSource;

		private readonly DataSet m_dataSet;

		private readonly OnDemandProcessingContext m_odpContext;

		private readonly DataProcessingMetrics m_executionMetrics;

		private readonly string m_commandText;

		private readonly DateTime m_queryExecutionTimestamp;

		private readonly DataSourceErrorInspector m_errorInspector;

		private IDbConnection m_connection;

		private IDbCommand m_command;

		private IDbCommand m_commandWrappedForCancel;

		private IDataReader m_dataReader;

		internal DataSet DataSet => m_dataSet;

		internal DateTime QueryExecutionTimestamp => m_queryExecutionTimestamp;

		internal string CommandText => m_commandText;

		internal DataSourceErrorInspector ErrorInspector => m_errorInspector;

		internal DataProcessingMetrics ExecutionMetrics => m_executionMetrics;

		internal ExecutedQuery(DataSource dataSource, DataSet dataSet, OnDemandProcessingContext odpContext, DataProcessingMetrics executionMetrics, string commandText, DateTime queryExecutionTimestamp, DataSourceErrorInspector errorInspector)
		{
			m_dataSource = dataSource;
			m_dataSet = dataSet;
			m_odpContext = odpContext;
			m_executionMetrics = executionMetrics;
			m_commandText = commandText;
			m_queryExecutionTimestamp = queryExecutionTimestamp;
			m_errorInspector = errorInspector;
		}

		internal void AssumeOwnership(ref IDbConnection connection, ref IDbCommand command, ref IDbCommand commandWrappedForCancel, ref IDataReader dataReader)
		{
			AssignAndClear(ref m_connection, ref connection);
			AssignAndClear(ref m_command, ref command);
			AssignAndClear(ref m_commandWrappedForCancel, ref commandWrappedForCancel);
			AssignAndClear(ref m_dataReader, ref dataReader);
		}

		internal void ReleaseOwnership(ref IDbConnection connection)
		{
			AssignAndClear(ref connection, ref m_connection);
		}

		internal void ReleaseOwnership(ref IDbCommand command, ref IDbCommand commandWrappedForCancel, ref IDataReader dataReader)
		{
			AssignAndClear(ref dataReader, ref m_dataReader);
			AssignAndClear(ref commandWrappedForCancel, ref m_commandWrappedForCancel);
			AssignAndClear(ref command, ref m_command);
		}

		private static void AssignAndClear<T>(ref T target, ref T source) where T : class
		{
			Interlocked.Exchange(ref target, source);
			Interlocked.Exchange(ref source, null);
		}

		internal void Close()
		{
			IDataReader obj = Interlocked.Exchange(ref m_dataReader, null);
			if (obj != null)
			{
				QueryExecutionUtils.DisposeDataExtensionObject(ref obj, "data reader", m_dataSet.Name, m_executionMetrics, DataProcessingMetrics.MetricType.DisposeDataReader);
			}
			m_commandWrappedForCancel = null;
			IDbCommand obj2 = Interlocked.Exchange(ref m_command, null);
			if (obj2 != null)
			{
				QueryExecutionUtils.DisposeDataExtensionObject(ref obj2, "command", m_dataSet.Name);
			}
			IDbConnection dbConnection = Interlocked.Exchange(ref m_connection, null);
			if (dbConnection != null)
			{
				RuntimeDataSource.CloseConnection(dbConnection, m_dataSource, m_odpContext, m_executionMetrics);
			}
		}
	}
}
