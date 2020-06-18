using System;

namespace Microsoft.Reporting.Gauge.WebForms
{
	internal class GaugeDuration : ICloneable
	{
		private DurationType durationType;

		private double count;

		private TimeSpan timeSpan;

		private bool ivalidated;

		public double Count
		{
			get
			{
				return count;
			}
			set
			{
				if (value < 0.0)
				{
					throw new ArgumentException(Utils.SRGetStr("ExceptionDurationNegative"));
				}
				count = value;
				ivalidated = true;
			}
		}

		internal DurationType DurationType
		{
			get
			{
				return durationType;
			}
			set
			{
				durationType = value;
				ivalidated = true;
			}
		}

		internal bool IsTimeBased
		{
			get
			{
				if (durationType != DurationType.Count)
				{
					return durationType != DurationType.Infinite;
				}
				return false;
			}
		}

		internal bool IsCountBased => durationType == DurationType.Count;

		internal bool IsInfinity => durationType == DurationType.Infinite;

		internal bool IsEmpty
		{
			get
			{
				if (IsInfinity)
				{
					return false;
				}
				if (!double.IsNaN(count))
				{
					return count == 0.0;
				}
				return true;
			}
		}

		internal GaugeDuration()
		{
			count = 0.0;
			durationType = DurationType.Count;
			ivalidated = true;
		}

		internal GaugeDuration(double count, DurationType durationType)
		{
			this.count = count;
			this.durationType = durationType;
			ivalidated = true;
		}

		internal static PeriodType MapToPeriodType(DurationType type)
		{
			switch (type)
			{
			case DurationType.Days:
				return PeriodType.Days;
			case DurationType.Hours:
				return PeriodType.Hours;
			case DurationType.Minutes:
				return PeriodType.Minutes;
			case DurationType.Seconds:
				return PeriodType.Seconds;
			case DurationType.Milliseconds:
				return PeriodType.Milliseconds;
			default:
				throw new ArgumentException(Utils.SRGetStr("ExceptionMapPeriodTypeArgument"));
			}
		}

		internal TimeSpan ToTimeSpan()
		{
			if (!IsTimeBased || IsEmpty)
			{
				return TimeSpan.Zero;
			}
			if (IsInfinity)
			{
				return TimeSpan.MaxValue;
			}
			if (ivalidated)
			{
				timeSpan = GaugePeriod.PeriodToTimeSpan(count, MapToPeriodType(durationType));
				ivalidated = false;
			}
			return timeSpan;
		}

		internal void Extend(GaugeDuration extend, DateTime topDate, DateTime btmDate)
		{
			if (extend.IsInfinity)
			{
				DurationType = DurationType.Infinite;
			}
			else if (extend.IsCountBased && Count < extend.Count)
			{
				Count = extend.Count;
			}
			else if (extend.IsTimeBased)
			{
				DateTime t = topDate - extend.ToTimeSpan();
				if (btmDate > t)
				{
					Count++;
				}
			}
		}

		public object Clone()
		{
			return MemberwiseClone();
		}
	}
}
