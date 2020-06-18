using Microsoft.ReportingServices.Diagnostics;

namespace Microsoft.ReportingServices.OnDemandProcessing
{
	internal sealed class TimeMetric
	{
		private bool m_isRunning;

		private long m_totalDurationMs;

		private readonly Timer m_timer;

		private readonly long[] m_otherMetricAdjustments;

		private readonly TimeMetricManager m_metricAdjuster;

		private readonly int m_indexInCollection;

		public bool IsRunning => m_isRunning;

		public long TotalDurationMs => m_totalDurationMs;

		public long[] OtherMetricAdjustments => m_otherMetricAdjustments;

		public TimeMetric(int indexInCollection, TimeMetricManager metricAdjuster, int otherMetricCount)
		{
			m_indexInCollection = indexInCollection;
			m_timer = new Timer();
			m_totalDurationMs = 0L;
			m_isRunning = false;
			m_otherMetricAdjustments = new long[otherMetricCount];
			m_metricAdjuster = metricAdjuster;
		}

		public TimeMetric(TimeMetric other)
		{
			m_indexInCollection = other.m_indexInCollection;
			m_timer = new Timer();
			m_totalDurationMs = other.m_totalDurationMs;
			m_isRunning = false;
			m_otherMetricAdjustments = (long[])other.m_otherMetricAdjustments.Clone();
			m_metricAdjuster = other.m_metricAdjuster;
		}

		public void StartTimer()
		{
			m_timer.StartTimer();
			m_isRunning = true;
		}

		public bool TryStartTimer()
		{
			if (m_isRunning)
			{
				return false;
			}
			StartTimer();
			return true;
		}

		public void StopTimer()
		{
			m_isRunning = false;
			long durationMs = m_timer.ElapsedTimeMs();
			AddTime(durationMs);
		}

		public void Add(TimeMetric otherMetric)
		{
			m_totalDurationMs += otherMetric.TotalDurationMs;
			for (int i = 0; i < m_otherMetricAdjustments.Length; i++)
			{
				m_otherMetricAdjustments[i] += otherMetric.m_otherMetricAdjustments[i];
			}
		}

		public void AddTime(long durationMs)
		{
			durationMs = ExecutionLogContext.TimerMeasurementAdjusted(durationMs);
			m_metricAdjuster.UpdateTimeMetricAdjustments(durationMs, m_otherMetricAdjustments);
			m_totalDurationMs += durationMs;
		}

		public void Subtract(TimeMetric other)
		{
			m_totalDurationMs = ExecutionLogContext.TimerMeasurementAdjusted(m_totalDurationMs - other.m_totalDurationMs);
			for (int i = 0; i < m_otherMetricAdjustments.Length; i++)
			{
				long durationMs = m_otherMetricAdjustments[i] - other.m_otherMetricAdjustments[i];
				m_otherMetricAdjustments[i] = ExecutionLogContext.TimerMeasurementAdjusted(durationMs);
			}
		}
	}
}
