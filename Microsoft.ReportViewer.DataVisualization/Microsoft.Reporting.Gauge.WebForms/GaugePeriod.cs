using System;

namespace Microsoft.Reporting.Gauge.WebForms
{
	internal class GaugePeriod : ICloneable
	{
		private double duration;

		private PeriodType periodType;

		private TimeSpan timeSpan;

		private bool ivalidated;

		internal double Duration
		{
			get
			{
				return duration;
			}
			set
			{
				if (value < 0.0)
				{
					throw new ArgumentException(Utils.SRGetStr("ExceptionPeriodNegative"));
				}
				if (value == 0.0)
				{
					value = double.NaN;
				}
				duration = value;
				ivalidated = true;
			}
		}

		internal PeriodType PeriodType
		{
			get
			{
				return periodType;
			}
			set
			{
				periodType = value;
				ivalidated = true;
			}
		}

		internal GaugePeriod()
		{
			duration = double.NaN;
			periodType = PeriodType.Seconds;
			ivalidated = true;
		}

		internal GaugePeriod(double duration, PeriodType periodType)
		{
			this.duration = duration;
			this.periodType = periodType;
			ivalidated = true;
		}

		internal TimeSpan ToTimeSpan()
		{
			if (ivalidated)
			{
				timeSpan = PeriodToTimeSpan(duration, periodType);
				ivalidated = false;
			}
			return timeSpan;
		}

		internal static TimeSpan PeriodToTimeSpan(double timeTicks, PeriodType period)
		{
			if (double.IsNaN(timeTicks))
			{
				timeTicks = 0.0;
			}
			double num;
			switch (period)
			{
			case PeriodType.Days:
				num = timeTicks * 24.0 * 60.0 * 60.0 * 1000.0;
				break;
			case PeriodType.Hours:
				num = timeTicks * 60.0 * 60.0 * 1000.0;
				break;
			case PeriodType.Minutes:
				num = timeTicks * 60.0 * 1000.0;
				break;
			case PeriodType.Seconds:
				num = timeTicks * 1000.0;
				break;
			case PeriodType.Milliseconds:
				num = timeTicks;
				break;
			default:
				throw new ArgumentException(Utils.SRGetStr("ExceptionPeriodTimespanArgument"));
			}
			return new TimeSpan((long)Math.Floor(num * 10000.0));
		}

		internal GaugePeriod Clone()
		{
			return (GaugePeriod)MemberwiseClone();
		}

		object ICloneable.Clone()
		{
			return MemberwiseClone();
		}
	}
}
