using Microsoft.ReportingServices.DataProcessing;
using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.Diagnostics.Internal;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.ReportingServices.OnDemandProcessing
{
	internal sealed class DataProcessingMetrics
	{
		internal enum MetricType
		{
			ExecuteReader,
			DataReaderMapping,
			Query,
			OpenConnection,
			DisposeDataReader,
			CancelCommand
		}

		private IJobContext m_jobContext;

		private Timer[] m_timers;

		private TimeMetric m_totalTimeMetric;

		private long m_totalRowsRead;

		private long m_totalRowsSkipped;

		private long m_queryDurationMs;

		private long m_dataReaderMappingDurationMs;

		private long m_executeReaderDurationMs;

		private long m_openConnectionDurationMs;

		private long m_disposeDataReaderDurationMs;

		private long m_cancelCommandDurationMs = -2L;

		private string m_commandText;

		private Microsoft.ReportingServices.ReportIntermediateFormat.DataSet m_specificDataSet;

		private IDataParameter[] m_startAtParameters;

		private IDataParameterCollection m_queryParameters;

		private string m_resolvedConnectionString;

		private bool? m_connectionFromPool;

		private const long DurationNotMeasured = -2L;

		internal long TotalRowsRead => m_totalRowsRead;

		internal long TotalRowsSkipped => m_totalRowsSkipped;

		internal long TotalDurationMs
		{
			get
			{
				if (m_totalTimeMetric == null)
				{
					return 0L;
				}
				return m_totalTimeMetric.TotalDurationMs;
			}
		}

		internal TimeMetric TotalDuration => m_totalTimeMetric;

		internal long QueryDurationMs => m_queryDurationMs;

		internal long ExecuteReaderDurationMs => m_executeReaderDurationMs;

		internal long DataReaderMappingDurationMs => m_dataReaderMappingDurationMs;

		internal long OpenConnectionDurationMs => m_openConnectionDurationMs;

		internal long DisposeDataReaderDurationMs => m_disposeDataReaderDurationMs;

		internal long CancelCommandDurationMs => m_cancelCommandDurationMs;

		public string CommandText
		{
			get
			{
				return m_commandText;
			}
			set
			{
				m_commandText = value;
			}
		}

		internal string ResolvedConnectionString
		{
			get
			{
				return m_resolvedConnectionString;
			}
			set
			{
				m_resolvedConnectionString = value;
			}
		}

		internal bool? ConnectionFromPool
		{
			get
			{
				return m_connectionFromPool;
			}
			set
			{
				m_connectionFromPool = value;
			}
		}

		internal DataProcessingMetrics(Microsoft.ReportingServices.ReportIntermediateFormat.DataSet dataSet, IJobContext jobContext, ExecutionLogContext executionLogContext)
			: this(jobContext, executionLogContext)
		{
			m_specificDataSet = dataSet;
		}

		internal DataProcessingMetrics(IJobContext jobContext, ExecutionLogContext executionLogContext)
		{
			m_jobContext = jobContext;
			if (jobContext != null)
			{
				m_timers = new Timer[6];
				m_totalTimeMetric = executionLogContext.CreateDataRetrievalWorkerTimer();
			}
			else
			{
				m_timers = null;
			}
		}

		internal void Add(DataProcessingMetrics metrics)
		{
			if (metrics != null)
			{
				if (m_totalTimeMetric != null)
				{
					m_totalTimeMetric.Add(metrics.m_totalTimeMetric);
				}
				Add(MetricType.ExecuteReader, metrics.m_executeReaderDurationMs);
				Add(MetricType.DataReaderMapping, metrics.m_dataReaderMappingDurationMs);
				Add(MetricType.Query, metrics.m_queryDurationMs);
				Add(MetricType.OpenConnection, metrics.m_openConnectionDurationMs);
				Add(MetricType.DisposeDataReader, metrics.m_disposeDataReaderDurationMs);
				Add(MetricType.CancelCommand, metrics.m_cancelCommandDurationMs);
			}
		}

		internal long Add(MetricType type, Timer timer)
		{
			if (timer == null)
			{
				return -1L;
			}
			long num = timer.ElapsedTimeMs();
			Add(type, num);
			return num;
		}

		internal void Add(MetricType type, long elapsedTimeMs)
		{
			switch (type)
			{
			case MetricType.ExecuteReader:
				Add(ref m_executeReaderDurationMs, elapsedTimeMs);
				break;
			case MetricType.DataReaderMapping:
				Add(ref m_dataReaderMappingDurationMs, elapsedTimeMs);
				break;
			case MetricType.Query:
				Add(ref m_queryDurationMs, elapsedTimeMs);
				break;
			case MetricType.OpenConnection:
				Add(ref m_openConnectionDurationMs, elapsedTimeMs);
				break;
			case MetricType.DisposeDataReader:
				Add(ref m_disposeDataReaderDurationMs, elapsedTimeMs);
				break;
			case MetricType.CancelCommand:
				Add(ref m_cancelCommandDurationMs, elapsedTimeMs);
				break;
			}
		}

		private void Add(ref long currentDurationMs, long elapsedTimeMs)
		{
			if (currentDurationMs == -2)
			{
				currentDurationMs = 0L;
			}
			currentDurationMs += ExecutionLogContext.TimerMeasurementAdjusted(elapsedTimeMs);
		}

		internal void AddRowCount(long rowCount)
		{
			m_totalRowsRead += rowCount;
		}

		internal void AddSkippedRowCount(long rowCount)
		{
			m_totalRowsSkipped += rowCount;
		}

		internal void StartTimer(MetricType type)
		{
			if (m_jobContext != null)
			{
				(m_timers[(int)type] = new Timer()).StartTimer();
			}
		}

		internal long RecordTimerMeasurement(MetricType type)
		{
			if (m_jobContext == null)
			{
				return 0L;
			}
			if (m_timers[(int)type] == null)
			{
				return 0L;
			}
			long result = Add(type, m_timers[(int)type]);
			m_timers[(int)type] = null;
			return result;
		}

		internal long RecordTimerMeasurementWithUpdatedTotal(MetricType type)
		{
			long num = RecordTimerMeasurement(type);
			if (m_totalTimeMetric != null && !m_totalTimeMetric.IsRunning)
			{
				m_totalTimeMetric.AddTime(num);
			}
			return num;
		}

		public void StartTotalTimer()
		{
			if (m_totalTimeMetric != null)
			{
				m_totalTimeMetric.StartTimer();
			}
		}

		public void RecordTotalTimerMeasurement()
		{
			if (m_totalTimeMetric != null)
			{
				m_totalTimeMetric.StopTimer();
			}
		}

		internal void SetStartAtParameters(IDataParameter[] startAtParameters)
		{
			m_startAtParameters = startAtParameters;
		}

		internal void SetQueryParameters(IDataParameterCollection queryParameters)
		{
			m_queryParameters = queryParameters;
		}

		internal Microsoft.ReportingServices.Diagnostics.Internal.DataSet ToAdditionalInfoDataSet(IJobContext jobContext)
		{
			if (jobContext == null || m_specificDataSet == null)
			{
				return null;
			}
			Microsoft.ReportingServices.Diagnostics.Internal.DataSet dataSet = new Microsoft.ReportingServices.Diagnostics.Internal.DataSet();
			dataSet.Name = m_specificDataSet.Name;
			if (jobContext.ExecutionLogLevel == ExecutionLogLevel.Verbose)
			{
				dataSet.CommandText = m_commandText;
				if (m_startAtParameters != null || m_queryParameters != null)
				{
					List<QueryParameter> list = new List<QueryParameter>();
					AddStartAtParameters(list);
					AddQueryParameters(list);
					if (list.Count > 0)
					{
						dataSet.QueryParameters = list;
					}
				}
			}
			dataSet.RowsRead = m_totalRowsRead;
			dataSet.TotalTimeDataRetrieval = m_totalTimeMetric.TotalDurationMs;
			if (jobContext.ExecutionLogLevel == ExecutionLogLevel.Verbose)
			{
				dataSet.QueryPrepareAndExecutionTime = m_queryDurationMs;
			}
			dataSet.ExecuteReaderTime = m_executeReaderDurationMs;
			if (jobContext.ExecutionLogLevel == ExecutionLogLevel.Verbose)
			{
				dataSet.DataReaderMappingTime = m_dataReaderMappingDurationMs;
				dataSet.DisposeDataReaderTime = m_disposeDataReaderDurationMs;
			}
			if (m_cancelCommandDurationMs != -2)
			{
				dataSet.CancelCommandTime = m_cancelCommandDurationMs.ToString(CultureInfo.InvariantCulture);
			}
			return dataSet;
		}

		private void AddStartAtParameters(List<QueryParameter> queryParams)
		{
			if (m_startAtParameters == null || m_startAtParameters.Length == 0)
			{
				return;
			}
			IDataParameter[] startAtParameters = m_startAtParameters;
			foreach (IDataParameter dataParameter in startAtParameters)
			{
				if (dataParameter != null)
				{
					queryParams.Add(CreateAdditionalInfoQueryParameter(dataParameter.ParameterName, dataParameter.Value));
				}
			}
		}

		private void AddQueryParameters(List<QueryParameter> queryParams)
		{
			if (m_queryParameters == null)
			{
				return;
			}
			foreach (IDataParameter queryParameter in m_queryParameters)
			{
				if (queryParameter != null)
				{
					IDataMultiValueParameter dataMultiValueParameter = queryParameter as IDataMultiValueParameter;
					if (dataMultiValueParameter != null)
					{
						AddMultiValueQueryParameter(queryParams, dataMultiValueParameter);
					}
					else
					{
						queryParams.Add(CreateAdditionalInfoQueryParameter(queryParameter.ParameterName, queryParameter.Value));
					}
				}
			}
		}

		private static void AddMultiValueQueryParameter(List<QueryParameter> queryParams, IDataMultiValueParameter parameter)
		{
			if (parameter.Values != null)
			{
				object[] values = parameter.Values;
				foreach (object parameterValue in values)
				{
					queryParams.Add(CreateAdditionalInfoQueryParameter(parameter.ParameterName, parameterValue));
				}
			}
		}

		private static QueryParameter CreateAdditionalInfoQueryParameter(string parameterName, object parameterValue)
		{
			QueryParameter queryParameter = new QueryParameter();
			queryParameter.Name = parameterName;
			if (parameterValue != null)
			{
				queryParameter.Value = Convert.ToString(parameterValue, CultureInfo.InvariantCulture);
				queryParameter.TypeName = parameterValue.GetType().ToString();
			}
			return queryParameter;
		}
	}
}
