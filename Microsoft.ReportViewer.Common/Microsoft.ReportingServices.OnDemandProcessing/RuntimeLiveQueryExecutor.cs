using Microsoft.Cloud.Platform.Utils;
using Microsoft.ReportingServices.Common;
using Microsoft.ReportingServices.DataExtensions;
using Microsoft.ReportingServices.DataProcessing;
using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.RdlExpressions;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace Microsoft.ReportingServices.OnDemandProcessing
{
	internal abstract class RuntimeLiveQueryExecutor
	{
		protected readonly Microsoft.ReportingServices.ReportIntermediateFormat.DataSource m_dataSource;

		protected readonly Microsoft.ReportingServices.ReportIntermediateFormat.DataSet m_dataSet;

		protected readonly OnDemandProcessingContext m_odpContext;

		protected DataProcessingMetrics m_executionMetrics;

		protected IDbConnection m_dataSourceConnection;

		protected Microsoft.ReportingServices.ReportProcessing.ReportProcessing.TransactionInfo m_transInfo;

		protected bool m_isConnectionOwner;

		protected IDbCommand m_command;

		protected IDbCommand m_commandWrappedForCancel;

		internal DataProcessingMetrics DataSetExecutionMetrics => m_executionMetrics;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.DataSet DataSet => m_dataSet;

		internal bool IsConnectionOwner => m_isConnectionOwner;

		internal RuntimeLiveQueryExecutor(Microsoft.ReportingServices.ReportIntermediateFormat.DataSource dataSource, Microsoft.ReportingServices.ReportIntermediateFormat.DataSet dataSet, OnDemandProcessingContext odpContext)
		{
			m_dataSource = dataSource;
			m_dataSet = dataSet;
			m_odpContext = odpContext;
			m_executionMetrics = new DataProcessingMetrics(dataSet, m_odpContext.JobContext, m_odpContext.ExecutionLogContext);
		}

		internal void Abort()
		{
			IDbCommand command = m_command;
			IDbCommand commandWrappedForCancel = m_commandWrappedForCancel;
			if (command != null)
			{
				if (Global.Tracer.TraceVerbose)
				{
					Global.Tracer.Trace(TraceLevel.Verbose, "Data set '{0}': Cancelling command.", m_dataSet.Name.MarkAsPrivate());
				}
				if (commandWrappedForCancel != null)
				{
					commandWrappedForCancel.Cancel();
				}
				else
				{
					command.Cancel();
				}
			}
		}

		protected void CloseConnection()
		{
			if (m_isConnectionOwner && m_dataSourceConnection != null)
			{
				RuntimeDataSource.CloseConnection(m_dataSourceConnection, m_dataSource, m_odpContext, m_executionMetrics);
				m_dataSourceConnection = null;
			}
		}

		protected IDataReader RunLiveQuery(List<Microsoft.ReportingServices.ReportIntermediateFormat.ParameterValue> queryParams, object[] paramValues)
		{
			IDataReader reader = null;
			IDbCommand command = null;
			IJobContext jobContext = m_odpContext.JobContext;
			if (m_dataSourceConnection == null)
			{
				m_dataSourceConnection = RuntimeDataSource.OpenConnection(m_dataSource, m_dataSet, m_odpContext, m_executionMetrics);
			}
			try
			{
				m_executionMetrics.StartTimer(DataProcessingMetrics.MetricType.Query);
				command = CreateCommand();
				SetCommandParameters(command, queryParams, paramValues);
				string commandText = SetCommandText(command);
				StoreCommandText(commandText);
				SetCommandType(command);
				SetTransaction(command);
				m_odpContext.CheckAndThrowIfAborted();
				SetCommandTimeout(command);
				ExtractRewrittenCommandText(command);
				SetRestartPosition(command);
				DataSourceInfo dataSourceInfo = null;
				if (command is IDbImpersonationNeededForCommandCancel)
				{
					dataSourceInfo = m_dataSource.GetDataSourceInfo(m_odpContext);
				}
				m_command = command;
				m_commandWrappedForCancel = new CommandWrappedForCancel(m_command, m_odpContext.CreateAndSetupDataExtensionFunction, m_dataSource, dataSourceInfo, m_dataSet.Name, m_dataSourceConnection);
				if (jobContext != null)
				{
					jobContext.SetAdditionalCorrelation(m_command);
					jobContext.ApplyCommandMemoryLimit(m_command);
				}
				DataSourceErrorInspector errorInspector = CreateErrorInspector();
				reader = ExecuteReader(jobContext, errorInspector, commandText);
				StoreDataReader(reader, errorInspector);
				return reader;
			}
			catch (RSException)
			{
				EagerInlineCommandAndReaderCleanup(ref reader, ref command);
				throw;
			}
			catch (Exception e)
			{
				if (AsynchronousExceptionDetection.IsStoppingException(e))
				{
					throw;
				}
				EagerInlineCommandAndReaderCleanup(ref reader, ref command);
				throw;
			}
			finally
			{
				m_executionMetrics.RecordTimerMeasurement(DataProcessingMetrics.MetricType.Query);
			}
		}

		protected abstract void StoreDataReader(IDataReader dataReader, DataSourceErrorInspector errorInspector);

		protected abstract void ExtractRewrittenCommandText(IDbCommand command);

		private IDbCommand CreateCommand()
		{
			try
			{
				return m_dataSourceConnection.CreateCommand();
			}
			catch (Exception innerException)
			{
				throw new ReportProcessingException(ErrorCode.rsErrorCreatingCommand, innerException, m_dataSource.Name.MarkAsModelInfo());
			}
		}

		private IDataReader ExecuteReader(IJobContext jobContext, DataSourceErrorInspector errorInspector, string commandText)
		{
			IDataReader dataReader = null;
			try
			{
				jobContext?.AddCommand(m_commandWrappedForCancel);
				m_executionMetrics.StartTimer(DataProcessingMetrics.MetricType.ExecuteReader);
				try
				{
					dataReader = m_command.ExecuteReader(CommandBehavior.SingleResult);
				}
				catch (Exception ex)
				{
					if (m_odpContext.ContextMode == OnDemandProcessingContext.Mode.Streaming)
					{
						ErrorCode errorCode = ErrorCode.rsSuccess;
						bool flag = errorInspector?.TryInterpretProviderErrorCode(ex, out errorCode) ?? false;
						TraceExecuteReaderFailed(ex, commandText, flag ? new ErrorCode?(errorCode) : null);
						if (flag)
						{
							string text = string.Format(CultureInfo.CurrentCulture, RPRes.Keys.GetString(ErrorCode.rsErrorExecutingCommand.ToString()), m_dataSet.Name.MarkAsPrivate());
							throw new ReportProcessingQueryException(errorCode, ex, text);
						}
						if (errorInspector != null && errorInspector.IsOnPremiseServiceException(ex))
						{
							throw new ReportProcessingQueryOnPremiseServiceException(ErrorCode.rsErrorExecutingCommand, ex, m_dataSet.Name.MarkAsPrivate());
						}
					}
					throw new ReportProcessingException(ErrorCode.rsErrorExecutingCommand, ex, m_dataSet.Name.MarkAsPrivate());
				}
				finally
				{
					m_executionMetrics.RecordTimerMeasurement(DataProcessingMetrics.MetricType.ExecuteReader);
				}
			}
			finally
			{
				jobContext?.RemoveCommand(m_commandWrappedForCancel);
			}
			if (dataReader == null)
			{
				if (Global.Tracer.TraceError)
				{
					Global.Tracer.Trace(TraceLevel.Error, "The source data reader is null. Cannot read results.");
				}
				throw new ReportProcessingException(ErrorCode.rsErrorCreatingDataReader, m_dataSet.Name.MarkAsPrivate());
			}
			return dataReader;
		}

		private void TraceExecuteReaderFailed(Exception e, string commandText, ErrorCode? specificErrorCode)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("An error occured running the query for DataSet: \"");
			stringBuilder.Append(m_dataSet.Name.MarkAsPrivate());
			stringBuilder.Append("\"");
			if (specificErrorCode.HasValue)
			{
				stringBuilder.Append(" ErrorCode: \"").Append(specificErrorCode.Value).Append("\"");
			}
			if (m_dataSet.Query != null && m_dataSet.Query.TimeOut > 0)
			{
				stringBuilder.Append(" Timeout: \"").Append(m_dataSet.Query.TimeOut).Append("\"");
			}
			if (!string.IsNullOrEmpty(m_dataSource.ConnectionCategory))
			{
				stringBuilder.Append(" ConnectionCategory: \"").Append(m_dataSource.ConnectionCategory).Append("\"");
			}
			stringBuilder.Append("Error: \"").Append(e.Message).Append("\"");
			if (!string.IsNullOrEmpty(commandText))
			{
				stringBuilder.Append("Query: ");
				if (commandText.Length > 2048)
				{
					stringBuilder.Append(commandText.Substring(0, 2048).MarkAsPrivate());
					stringBuilder.Append(" ...");
				}
				else
				{
					stringBuilder.Append(commandText.MarkAsPrivate());
				}
			}
			Global.Tracer.Trace(TraceLevel.Error, stringBuilder.ToString());
		}

		protected abstract void SetRestartPosition(IDbCommand command);

		private void SetCommandTimeout(IDbCommand command)
		{
			try
			{
				if (m_dataSet.Query.TimeOut == 0 && command is CommandWrapper && ((CommandWrapper)command).UnderlyingCommand is SqlCommand)
				{
					command.CommandTimeout = 2147483646;
				}
				else
				{
					command.CommandTimeout = m_dataSet.Query.TimeOut;
				}
			}
			catch (Exception innerException)
			{
				throw new ReportProcessingException(ErrorCode.rsErrorSettingQueryTimeout, innerException, m_dataSet.Name.MarkAsPrivate());
			}
		}

		private void SetTransaction(IDbCommand command)
		{
			if (m_transInfo != null)
			{
				try
				{
					command.Transaction = m_transInfo.Transaction;
				}
				catch (Exception innerException)
				{
					throw new ReportProcessingException(ErrorCode.rsErrorSettingTransaction, innerException, m_dataSet.Name.MarkAsPrivate());
				}
			}
		}

		private void SetCommandType(IDbCommand command)
		{
			try
			{
				command.CommandType = (CommandType)m_dataSet.Query.CommandType;
			}
			catch (Exception innerException)
			{
				throw new ReportProcessingException(ErrorCode.rsErrorSettingCommandType, innerException, m_dataSet.Name.MarkAsPrivate());
			}
		}

		private string SetCommandText(IDbCommand command)
		{
			try
			{
				if (m_dataSet.Query.CommandText != null)
				{
					Microsoft.ReportingServices.RdlExpressions.StringResult stringResult = m_odpContext.ReportRuntime.EvaluateCommandText(m_dataSet);
					if (stringResult.ErrorOccurred)
					{
						throw new ReportProcessingException(ErrorCode.rsQueryCommandTextProcessingError, m_dataSet.Name.MarkAsPrivate());
					}
					command.CommandText = stringResult.Value;
					if (m_odpContext.UseVerboseExecutionLogging)
					{
						m_executionMetrics.CommandText = stringResult.Value;
					}
					return stringResult.Value;
				}
				return null;
			}
			catch (Exception innerException)
			{
				throw new ReportProcessingException(ErrorCode.rsErrorSettingCommandText, innerException, m_dataSet.Name.MarkAsPrivate());
			}
		}

		protected abstract void StoreCommandText(string commandText);

		private void SetCommandParameters(IDbCommand command, List<Microsoft.ReportingServices.ReportIntermediateFormat.ParameterValue> queryParams, object[] paramValues)
		{
			if (queryParams == null)
			{
				return;
			}
			int num = 0;
			IDataParameter dataParameter;
			while (true)
			{
				if (num >= paramValues.Length)
				{
					return;
				}
				if (!m_odpContext.IsSharedDataSetExecutionOnly || !((DataSetParameterValue)queryParams[num]).OmitFromQuery)
				{
					try
					{
						dataParameter = command.CreateParameter();
					}
					catch (Exception innerException)
					{
						throw new ReportProcessingException(ErrorCode.rsErrorCreatingQueryParameter, innerException, m_dataSet.Name.MarkAsPrivate());
					}
					dataParameter.ParameterName = queryParams[num].Name;
					bool flag = dataParameter is IDataMultiValueParameter && paramValues[num] is ICollection;
					object obj = paramValues[num];
					if (obj == null)
					{
						obj = DBNull.Value;
					}
					if (!(dataParameter is IDataMultiValueParameter) && paramValues[num] is ICollection)
					{
						break;
					}
					if (flag)
					{
						int count = ((ICollection)obj).Count;
						if (1 == count)
						{
							try
							{
								Global.Tracer.Assert(obj is object[], "(paramValue is object[])");
								dataParameter.Value = (obj as object[])[0];
							}
							catch (Exception innerException2)
							{
								throw new ReportProcessingException(ErrorCode.rsErrorAddingQueryParameter, innerException2, m_dataSource.Name.MarkAsModelInfo());
							}
						}
						else
						{
							object[] array = new object[count];
							((ICollection)obj).CopyTo(array, 0);
							((IDataMultiValueParameter)dataParameter).Values = array;
						}
					}
					else
					{
						try
						{
							dataParameter.Value = obj;
						}
						catch (Exception innerException3)
						{
							throw new ReportProcessingException(ErrorCode.rsErrorAddingQueryParameter, innerException3, m_dataSource.Name.MarkAsModelInfo());
						}
					}
					try
					{
						command.Parameters.Add(dataParameter);
					}
					catch (Exception innerException4)
					{
						throw new ReportProcessingException(ErrorCode.rsErrorAddingQueryParameter, innerException4, m_dataSource.Name.MarkAsModelInfo());
					}
					if (m_odpContext.UseVerboseExecutionLogging)
					{
						m_executionMetrics.SetQueryParameters(command.Parameters);
					}
				}
				num++;
			}
			throw new ReportProcessingException(ErrorCode.rsErrorAddingMultiValueQueryParameter, null, m_dataSet.Name.MarkAsPrivate(), dataParameter.ParameterName);
		}

		protected void EagerInlineCommandAndReaderCleanup(ref IDataReader reader, ref IDbCommand command)
		{
			EagerInlineReaderCleanup(ref reader);
			EagerInlineCommandCleanup(ref command);
		}

		protected abstract void EagerInlineReaderCleanup(ref IDataReader reader);

		private void EagerInlineCommandCleanup(ref IDbCommand command)
		{
			if (m_command != null)
			{
				command = null;
				DisposeCommand();
			}
			else
			{
				DisposeDataExtensionObject(ref command, "command");
			}
		}

		protected void DisposeCommand()
		{
			m_commandWrappedForCancel = null;
			DisposeDataExtensionObject(ref m_command, "command");
		}

		protected void CancelCommand()
		{
			if (m_commandWrappedForCancel == null)
			{
				return;
			}
			try
			{
				m_executionMetrics.StartTimer(DataProcessingMetrics.MetricType.CancelCommand);
				m_commandWrappedForCancel.Cancel();
				m_executionMetrics.RecordTimerMeasurementWithUpdatedTotal(DataProcessingMetrics.MetricType.CancelCommand);
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
				Global.Tracer.Trace(TraceLevel.Warning, "Error occurred while canceling the command for DataSet '" + m_dataSet.Name.MarkAsPrivate() + "'. Details: " + ex2.ToString());
			}
		}

		protected void DisposeDataExtensionObject<T>(ref T obj, string objectType) where T : class, IDisposable
		{
			QueryExecutionUtils.DisposeDataExtensionObject(ref obj, objectType, m_dataSet.Name.MarkAsPrivate());
		}

		protected void DisposeDataExtensionObject<T>(ref T obj, string objectType, DataProcessingMetrics.MetricType? metricType) where T : class, IDisposable
		{
			QueryExecutionUtils.DisposeDataExtensionObject(ref obj, objectType, m_dataSet.Name.MarkAsPrivate(), m_executionMetrics, metricType);
		}

		private DataSourceErrorInspector CreateErrorInspector()
		{
			return null;
		}
	}
}
