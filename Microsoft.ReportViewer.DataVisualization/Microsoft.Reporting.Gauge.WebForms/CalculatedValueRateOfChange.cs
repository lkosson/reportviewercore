using System;
using System.ComponentModel;

namespace Microsoft.Reporting.Gauge.WebForms
{
	[TypeConverter(typeof(CalculatedValueRateOfChangeConverter))]
	internal class CalculatedValueRateOfChange : CalculatedValue
	{
		private DataSampleRC[] oldValues = new DataSampleRC[2]
		{
			new DataSampleRC(),
			new DataSampleRC()
		};

		private GaugePeriod rateOfChange = new GaugePeriod(double.NaN, Microsoft.Reporting.Gauge.WebForms.PeriodType.Seconds);

		[SRCategory("CategoryBehavior")]
		[Bindable(true)]
		[DefaultValue(double.NaN)]
		[SRDescription("DescriptionAttributeCalculatedValueRateOfChange_RateOfChangePeriod")]
		[TypeConverter(typeof(DoubleNanValueConverter))]
		public virtual double RateOfChangePeriod
		{
			get
			{
				return rateOfChange.Duration;
			}
			set
			{
				rateOfChange.Duration = value;
			}
		}

		[SRCategory("CategoryBehavior")]
		[Bindable(true)]
		[DefaultValue(Microsoft.Reporting.Gauge.WebForms.PeriodType.Seconds)]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeCalculatedValueRateOfChange_RateOfChangePeriodType")]
		public virtual PeriodType RateOfChangePeriodType
		{
			get
			{
				return rateOfChange.PeriodType;
			}
			set
			{
				rateOfChange.PeriodType = value;
			}
		}

		[Browsable(false)]
		public override long Period
		{
			get
			{
				return base.Period;
			}
			set
			{
			}
		}

		[Browsable(false)]
		public override DurationType PeriodType
		{
			get
			{
				return base.PeriodType;
			}
			set
			{
			}
		}

		internal override void CalculateValue(double value, DateTime timestamp)
		{
			TimeSpan period = rateOfChange.ToTimeSpan();
			double num = double.NaN;
			noMoreData = true;
			if (double.IsNaN(value))
			{
				base.CalculateValue(num, timestamp);
				return;
			}
			if (period.Ticks == 0L)
			{
				base.CalculateValue(num, timestamp);
				return;
			}
			if (!oldValues[0].Invalid)
			{
				if (oldValues[0].Timestamp == timestamp)
				{
					if (!oldValues[1].Invalid)
					{
						num = GetResult(value, timestamp, oldValues[1], period);
					}
					oldValues[0].Value = value;
				}
				else
				{
					num = GetResult(value, timestamp, oldValues[0], period);
					oldValues[1].Assign(oldValues[0]);
					oldValues[0].Value = value;
					oldValues[0].Timestamp = timestamp;
				}
			}
			else
			{
				oldValues[0].Value = value;
				oldValues[0].Timestamp = timestamp;
			}
			noMoreData = (num == 0.0);
			base.CalculateValue(num, timestamp);
		}

		private double GetResult(double value, DateTime timestamp, DataSampleRC rc, TimeSpan period)
		{
			double result = 0.0;
			TimeSpan timeSpan = timestamp - rc.Timestamp;
			double num = value - rc.Value;
			double num2 = (double)period.Ticks / (double)timeSpan.Ticks;
			if (num2 != 0.0)
			{
				result = num * num2;
			}
			else if (num > 0.0)
			{
				result = double.PositiveInfinity;
			}
			else if (num < 0.0)
			{
				result = double.NegativeInfinity;
			}
			return result;
		}

		internal override object CloneInternals(object copy)
		{
			CalculatedValueRateOfChange obj = (CalculatedValueRateOfChange)base.CloneInternals(copy);
			obj.rateOfChange = rateOfChange.Clone();
			return obj;
		}
	}
}
