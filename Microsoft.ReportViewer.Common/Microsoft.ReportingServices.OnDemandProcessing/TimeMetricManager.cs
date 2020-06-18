using Microsoft.ReportingServices.ReportProcessing;
using System.Diagnostics;

namespace Microsoft.ReportingServices.OnDemandProcessing
{
	internal sealed class TimeMetricManager
	{
		private readonly TimeMetric[] m_timeMetrics;

		public TimeMetric this[int index] => m_timeMetrics[index];

		public TimeMetricManager(int metricCount)
		{
			m_timeMetrics = new TimeMetric[metricCount];
			for (int i = 0; i < m_timeMetrics.Length; i++)
			{
				m_timeMetrics[i] = CreateTimeMetric(i);
			}
		}

		public TimeMetric CreateTimeMetric(int index)
		{
			return new TimeMetric(index, this, m_timeMetrics.Length);
		}

		public long GetNormalizedAdjustedMetric(int targetIndex)
		{
			long num = m_timeMetrics[targetIndex].TotalDurationMs;
			for (int i = 0; i < m_timeMetrics.Length; i++)
			{
				if (i != targetIndex)
				{
					TimeMetric timeMetric = m_timeMetrics[i];
					num -= timeMetric.OtherMetricAdjustments[targetIndex];
				}
			}
			return ExecutionLogContext.NormalizeCalculatedDuration(num);
		}

		public void UpdateTimeMetricAdjustments(long lastDurationMs, long[] metricAdjustments)
		{
			if (lastDurationMs <= 0)
			{
				return;
			}
			int num = m_timeMetrics.Length - 1;
			while (true)
			{
				if (num >= 0)
				{
					if (m_timeMetrics[num].IsRunning)
					{
						break;
					}
					num--;
					continue;
				}
				return;
			}
			metricAdjustments[num] += lastDurationMs;
		}

		public void StopAllRunningTimers()
		{
			for (int num = m_timeMetrics.Length - 1; num >= 0; num--)
			{
				TimeMetric timeMetric = m_timeMetrics[num];
				if (timeMetric.IsRunning)
				{
					timeMetric.StopTimer();
				}
			}
		}

		[Conditional("DEBUG")]
		public void VerifyStartOrder(int index)
		{
			for (int i = index; i < m_timeMetrics.Length; i++)
			{
				Global.Tracer.Assert(!m_timeMetrics[i].IsRunning, "Later metric must not be running when starting an earlier metric or adjustments will not work.");
			}
		}
	}
}
