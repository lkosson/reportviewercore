using System;
using System.ComponentModel;

namespace Microsoft.Reporting.Gauge.WebForms
{
	[TypeConverter(typeof(CalculatedValueIntegralConverter))]
	internal class CalculatedValueIntegral : CalculatedValue
	{
		private GaugePeriod interval = new GaugePeriod(double.NaN, Microsoft.Reporting.Gauge.WebForms.PeriodType.Seconds);

		private double integralBase;

		private double integralResult;

		private DataSampleRC oldValue = new DataSampleRC();

		[SRCategory("CategoryBehavior")]
		[Bindable(true)]
		[DefaultValue(double.NaN)]
		[SRDescription("DescriptionAttributeCalculatedValueIntegral_IntegralInterval")]
		[TypeConverter(typeof(DoubleNanValueConverter))]
		public double IntegralInterval
		{
			get
			{
				return interval.Duration;
			}
			set
			{
				interval.Duration = value;
			}
		}

		[SRCategory("CategoryBehavior")]
		[Bindable(true)]
		[DefaultValue(Microsoft.Reporting.Gauge.WebForms.PeriodType.Seconds)]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeCalculatedValueIntegral_IntegralIntervalType")]
		public PeriodType IntegralIntervalType
		{
			get
			{
				return interval.PeriodType;
			}
			set
			{
				interval.PeriodType = value;
			}
		}

		[SRCategory("CategoryBehavior")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeCalculatedValueIntegral_IntegralBase")]
		[DefaultValue(0.0)]
		public double IntegralBase
		{
			get
			{
				return integralBase;
			}
			set
			{
				integralBase = value;
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
			TimeSpan timeSpan = interval.ToTimeSpan();
			noMoreData = false;
			if (double.IsNaN(value))
			{
				base.CalculateValue(integralBase + integralResult, timestamp);
				return;
			}
			if (timeSpan.Ticks == 0L)
			{
				integralResult += value;
				base.CalculateValue(integralBase + integralResult, timestamp);
				return;
			}
			if (!oldValue.Invalid)
			{
				integralResult += value * ((double)(timestamp.Ticks - oldValue.Timestamp.Ticks) / (double)timeSpan.Ticks);
			}
			oldValue.Timestamp = timestamp;
			oldValue.Value = value;
			base.CalculateValue(integralBase + integralResult, timestamp);
		}

		private void RegenerateIntegralResult()
		{
			if (((IValueConsumer)this).GetProvider() is ValueBase)
			{
				TimeSpan timeSpan = interval.ToTimeSpan();
				ValueBase valueBase = (ValueBase)((IValueConsumer)this).GetProvider();
				HistoryCollection history = valueBase.History;
				integralResult = history.AccumulatedValue / (double)timeSpan.Ticks;
				int num = history.Locate(valueBase.Date);
				for (int i = 1; i < num; i++)
				{
					integralResult += history[i].Value * (double)(history[i].Timestamp.Ticks - history[i - 1].Timestamp.Ticks) / (double)timeSpan.Ticks;
					oldValue.Timestamp = history[i].Timestamp;
					oldValue.Value = history[i].Value;
				}
			}
		}

		public override void Reset()
		{
			base.Reset();
			integralResult = 0.0;
			oldValue = new DataSampleRC();
		}

		internal override void RefreshConsumers()
		{
			RegenerateIntegralResult();
			base.RefreshConsumers();
		}

		internal override object CloneInternals(object copy)
		{
			copy = base.CloneInternals(copy);
			((CalculatedValueIntegral)copy).interval = interval.Clone();
			((CalculatedValueIntegral)copy).oldValue = (DataSampleRC)oldValue.Clone();
			return copy;
		}
	}
}
