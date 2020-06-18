using System;

namespace Microsoft.Reporting.Gauge.WebForms
{
	internal class TimerData : ICloneable
	{
		public TimeSpan ticks;

		public DateTime timestamp;

		public TimerData()
			: this(TimeSpan.Zero, DateTime.Now)
		{
		}

		public TimerData(TimeSpan ticks, DateTime timestamp)
		{
			this.timestamp = timestamp;
			this.ticks = ticks;
		}

		public object Clone()
		{
			return MemberwiseClone();
		}
	}
}
