using System;

namespace Microsoft.ReportingServices.Diagnostics
{
    internal sealed class Timer
	{
		private DateTime counter;

		public void StartTimer()
		{
			counter = DateTime.Now;
		}

		public long ElapsedTimeMs()
		{
			return (DateTime.Now - counter).Ticks;
		}
	}
}
