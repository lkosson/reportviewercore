using System;
using System.ComponentModel;

namespace Microsoft.Reporting.Gauge.WebForms
{
	[TypeConverter(typeof(CalculatedValueLinearConverter))]
	internal class CalculatedValueLinear : CalculatedValue
	{
		private double multiplier = 1.0;

		private double addend;

		[SRCategory("CategoryBehavior")]
		[Bindable(true)]
		[DefaultValue(1.0)]
		[SRDescription("DescriptionAttributeCalculatedValueLinear_Multiplier")]
		public double Multiplier
		{
			get
			{
				return multiplier;
			}
			set
			{
				multiplier = value;
			}
		}

		[SRCategory("CategoryBehavior")]
		[Bindable(true)]
		[DefaultValue(0.0)]
		[SRDescription("DescriptionAttributeCalculatedValueLinear_Adder")]
		public double AddConstant
		{
			get
			{
				return addend;
			}
			set
			{
				addend = value;
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

		[Browsable(false)]
		public override double RefreshRate
		{
			get
			{
				return base.RefreshRate;
			}
			set
			{
			}
		}

		[Browsable(false)]
		public override PeriodType RefreshRateType
		{
			get
			{
				return base.RefreshRateType;
			}
			set
			{
			}
		}

		internal override void CalculateValue(double value, DateTime timestamp)
		{
			noMoreData = true;
			value = inputValue * multiplier + addend;
			base.CalculateValue(value, timestamp);
		}
	}
}
