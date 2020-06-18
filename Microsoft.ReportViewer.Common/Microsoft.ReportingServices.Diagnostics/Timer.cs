using Microsoft.ReportingServices.Diagnostics.Utilities;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;

namespace Microsoft.ReportingServices.Diagnostics
{
	internal sealed class Timer
	{
		private static int m_valuesLessThanZero;

		private bool m_start;

		private long m_LastValue;

		private long Value
		{
			get
			{
				long lpPerformanceCount = 0L;
				QueryPerformanceCounter(ref lpPerformanceCount);
				return lpPerformanceCount;
			}
		}

		private long Frequency
		{
			get
			{
				long lpFrequency = 0L;
				QueryPerformanceFrequency(ref lpFrequency);
				return lpFrequency;
			}
		}

		public void StartTimer()
		{
			m_start = true;
			m_LastValue = Value;
		}

		public long ElapsedTimeMs()
		{
			if (!m_start)
			{
				return 0L;
			}
			m_start = false;
			long lastValue = m_LastValue;
			long num = Value - lastValue;
			long num2 = (long)(1000f * (float)num / (float)Frequency);
			if (num2 < 0 && RSTrace.RunningJobsTrace.TraceWarning && Interlocked.Increment(ref m_valuesLessThanZero) == 1)
			{
				RSTrace.RunningJobsTrace.Trace(TraceLevel.Warning, "Timestamp values retrieved from current CPU are not synchronized with other CPUs");
			}
			return num2;
		}

		[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
		[SuppressUnmanagedCodeSecurity]
		private static extern bool QueryPerformanceCounter([In] [Out] ref long lpPerformanceCount);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
		[SuppressUnmanagedCodeSecurity]
		private static extern bool QueryPerformanceFrequency([In] [Out] ref long lpFrequency);
	}
}
