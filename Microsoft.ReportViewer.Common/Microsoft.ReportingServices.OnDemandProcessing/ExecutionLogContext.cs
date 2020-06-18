using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.Diagnostics.Internal;
using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandProcessing
{
	internal sealed class ExecutionLogContext
	{
		private enum TimeMetricType
		{
			Processing,
			Rendering,
			TablixProcessing,
			DataRetrieval
		}

		private sealed class ScaleCacheInfo
		{
			private long m_childPeakMemoryUsageKB;

			private readonly int m_reportGlobalId;

			public long ChildPeakMemoryUsageKB
			{
				get
				{
					return m_childPeakMemoryUsageKB;
				}
				set
				{
					m_childPeakMemoryUsageKB = value;
				}
			}

			public int ReportGlobalId => m_reportGlobalId;

			public ScaleCacheInfo(int reportGlobalId)
			{
				m_reportGlobalId = reportGlobalId;
				m_childPeakMemoryUsageKB = 0L;
			}
		}

		private long m_processingScalabilityDurationMs;

		private long m_peakGroupTreeMemoryUsageKB;

		private long m_externalImageDurationMs;

		private long m_externalImageCount;

		private long m_externalImageBytes;

		private List<Pair<string, DataProcessingMetrics>> m_dataSetMetrics = new List<Pair<string, DataProcessingMetrics>>();

		private List<DataSourceMetrics> m_dataSourceConnectionMetrics = new List<DataSourceMetrics>();

		private Timer m_externalImageTimer;

		private TimeMetricManager m_metricManager;

		private static readonly int TimeMetricCount = Enum.GetValues(typeof(TimeMetricType)).Length;

		private readonly IJobContext m_jobContext;

		private readonly Stack<ScaleCacheInfo> m_activeScaleCaches = new Stack<ScaleCacheInfo>();

		private const int RootScaleCacheInfoId = int.MinValue;

		internal bool IsProcessingTimerRunning
		{
			get
			{
				if (m_metricManager != null)
				{
					return m_metricManager[0].IsRunning;
				}
				return false;
			}
		}

		internal bool IsRenderingTimerRunning
		{
			get
			{
				if (m_metricManager != null)
				{
					return m_metricManager[1].IsRunning;
				}
				return false;
			}
		}

		internal long PeakTablixProcessingMemoryUsageKB
		{
			get
			{
				Global.Tracer.Assert(m_activeScaleCaches.Count > 0, "Missing root of active cache tree");
				return m_activeScaleCaches.Peek().ChildPeakMemoryUsageKB;
			}
		}

		internal long PeakGroupTreeMemoryUsageKB => m_peakGroupTreeMemoryUsageKB;

		internal long PeakProcesssingMemoryUsage => PeakTablixProcessingMemoryUsageKB + PeakGroupTreeMemoryUsageKB;

		internal long DataProcessingDurationMsNormalized => GetNormalizedAdjustedMetric(TimeMetricType.DataRetrieval);

		internal long ProcessingScalabilityDurationMsNormalized => NormalizeCalculatedDuration(m_processingScalabilityDurationMs);

		internal long ReportRenderingDurationMsNormalized => GetNormalizedAdjustedMetric(TimeMetricType.Rendering);

		internal long ReportProcessingDurationMsNormalized
		{
			get
			{
				long normalizedAdjustedMetric = GetNormalizedAdjustedMetric(TimeMetricType.Processing);
				long normalizedAdjustedMetric2 = GetNormalizedAdjustedMetric(TimeMetricType.TablixProcessing);
				return NormalizeCalculatedDuration(normalizedAdjustedMetric + normalizedAdjustedMetric2);
			}
		}

		internal long ExternalImageDurationMs => m_externalImageDurationMs;

		internal long ExternalImageCount
		{
			get
			{
				return m_externalImageCount;
			}
			set
			{
				m_externalImageCount = value;
			}
		}

		internal long ExternalImageBytes
		{
			get
			{
				return m_externalImageBytes;
			}
			set
			{
				m_externalImageBytes = value;
			}
		}

		internal List<Pair<string, DataProcessingMetrics>> DataSetMetrics => m_dataSetMetrics;

		internal List<DataSourceMetrics> DataSourceConnectionMetrics => m_dataSourceConnectionMetrics;

		internal ExecutionLogContext(IJobContext jobContext)
		{
			m_activeScaleCaches.Push(new ScaleCacheInfo(int.MinValue));
			m_jobContext = jobContext;
			if (m_jobContext != null)
			{
				m_metricManager = new TimeMetricManager(TimeMetricCount);
			}
		}

		public static long TimerMeasurementAdjusted(long durationMs)
		{
			return Math.Max(0L, durationMs);
		}

		public static long NormalizeCalculatedDuration(long durationMs)
		{
			return Math.Max(-1L, durationMs);
		}

		internal List<Connection> GetConnectionMetrics()
		{
			if (DataSourceConnectionMetrics != null)
			{
				List<Connection> list = new List<Connection>();
				{
					foreach (DataSourceMetrics dataSourceConnectionMetric in DataSourceConnectionMetrics)
					{
						Connection connection = dataSourceConnectionMetric.ToAdditionalInfoConnection(m_jobContext);
						if (connection != null)
						{
							list.Add(connection);
						}
					}
					return list;
				}
			}
			return null;
		}

		public void StopAllRunningTimers()
		{
			if (m_metricManager != null)
			{
				m_metricManager.StopAllRunningTimers();
			}
		}

		public void UpdateForTreeScaleCache(long scaleTimeDurationMs, long peakGroupTreeMemoryUsageKB)
		{
			m_processingScalabilityDurationMs += scaleTimeDurationMs;
			m_peakGroupTreeMemoryUsageKB += peakGroupTreeMemoryUsageKB;
		}

		internal void AddLegacyDataProcessingTime(long durationMs)
		{
			if (m_metricManager != null)
			{
				m_metricManager[3].AddTime(durationMs);
			}
		}

		internal void AddDataProcessingTime(TimeMetric childMetric)
		{
			if (m_metricManager != null && childMetric != null)
			{
				m_metricManager[3].Add(childMetric);
			}
		}

		internal void AddDataSetMetrics(string dataSetName, DataProcessingMetrics metrics)
		{
			lock (m_dataSetMetrics)
			{
				m_dataSetMetrics.Add(new Pair<string, DataProcessingMetrics>(dataSetName, metrics));
			}
		}

		internal void AddDataSourceParallelExecutionMetrics(string dataSourceName, string dataSourceReference, string dataSourceType, DataProcessingMetrics parallelDataSetMetrics)
		{
			lock (m_dataSourceConnectionMetrics)
			{
				m_dataSourceConnectionMetrics.Add(new DataSourceMetrics(dataSourceName, dataSourceReference, dataSourceType, parallelDataSetMetrics));
			}
		}

		internal void AddDataSourceMetrics(string dataSourceName, string dataSourceReference, string dataSourceType, DataProcessingMetrics aggregatedMetrics, DataProcessingMetrics[] dataSetsMetrics)
		{
			lock (m_dataSourceConnectionMetrics)
			{
				m_dataSourceConnectionMetrics.Add(new DataSourceMetrics(dataSourceName, dataSourceReference, dataSourceType, aggregatedMetrics, dataSetsMetrics));
			}
		}

		internal void StartProcessingTimer()
		{
			StartTimer(TimeMetricType.Processing);
		}

		internal void StopProcessingTimer()
		{
			StopTimer(TimeMetricType.Processing);
		}

		internal void StartRenderingTimer()
		{
			StartTimer(TimeMetricType.Rendering);
		}

		internal void StopRenderingTimer()
		{
			StopTimer(TimeMetricType.Rendering);
		}

		internal void StartTablixProcessingTimer()
		{
			StartTimer(TimeMetricType.TablixProcessing);
		}

		internal bool TryStartTablixProcessingTimer()
		{
			return TryStartTimer(TimeMetricType.TablixProcessing);
		}

		internal void StopTablixProcessingTimer()
		{
			StopTimer(TimeMetricType.TablixProcessing);
		}

		internal void StartExternalImageTimer()
		{
			if (m_jobContext != null)
			{
				m_externalImageTimer = new Timer();
				m_externalImageTimer.StartTimer();
			}
		}

		internal void StopExternalImageTimer()
		{
			if (m_externalImageTimer != null)
			{
				m_externalImageDurationMs += m_externalImageTimer.ElapsedTimeMs();
				m_externalImageTimer = null;
			}
		}

		public TimeMetric CreateDataRetrievalWorkerTimer()
		{
			return m_metricManager.CreateTimeMetric(3);
		}

		private void StartTimer(TimeMetricType metricType)
		{
			if (m_metricManager != null)
			{
				m_metricManager[(int)metricType].StartTimer();
			}
		}

		private bool TryStartTimer(TimeMetricType metricType)
		{
			if (m_metricManager != null)
			{
				return m_metricManager[(int)metricType].TryStartTimer();
			}
			return false;
		}

		private void StopTimer(TimeMetricType metricType)
		{
			if (m_metricManager != null)
			{
				m_metricManager[(int)metricType].StopTimer();
			}
		}

		private long GetNormalizedAdjustedMetric(TimeMetricType metricType)
		{
			if (m_metricManager == null)
			{
				return 0L;
			}
			return m_metricManager.GetNormalizedAdjustedMetric((int)metricType);
		}

		internal void RegisterTablixProcessingScaleCache(int reportId)
		{
			m_activeScaleCaches.Push(new ScaleCacheInfo(reportId));
		}

		internal void UnRegisterTablixProcessingScaleCache(int reportId, IScalabilityCache tablixProcessingCache)
		{
			m_processingScalabilityDurationMs += tablixProcessingCache.ScalabilityDurationMs;
			long num = tablixProcessingCache.PeakMemoryUsageKBytes;
			ScaleCacheInfo scaleCacheInfo = m_activeScaleCaches.Peek();
			if (scaleCacheInfo.ReportGlobalId == reportId && reportId != int.MinValue)
			{
				num += scaleCacheInfo.ChildPeakMemoryUsageKB;
				m_activeScaleCaches.Pop();
				scaleCacheInfo = m_activeScaleCaches.Peek();
			}
			scaleCacheInfo.ChildPeakMemoryUsageKB = Math.Max(scaleCacheInfo.ChildPeakMemoryUsageKB, num);
		}
	}
}
