using System;
using System.ComponentModel;

namespace Microsoft.Reporting.Gauge.WebForms
{
	internal class TickMark : CustomTickMark
	{
		private double interval = double.NaN;

		private double intervalOffset = double.NaN;

		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeInterval3")]
		[TypeConverter(typeof(DoubleAutoValueConverter))]
		[NotifyParentProperty(true)]
		[DefaultValue(double.NaN)]
		public virtual double Interval
		{
			get
			{
				return interval;
			}
			set
			{
				if (value < 0.0)
				{
					throw new ArgumentException(Utils.SRGetStr("ExceptionIntervalNegative"));
				}
				if (value == 0.0)
				{
					value = double.NaN;
				}
				interval = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeIntervalOffset")]
		[TypeConverter(typeof(DoubleAutoValueConverter))]
		[NotifyParentProperty(true)]
		[DefaultValue(double.NaN)]
		public virtual double IntervalOffset
		{
			get
			{
				return intervalOffset;
			}
			set
			{
				if (value < 0.0)
				{
					throw new ArgumentException(Utils.SRGetStr("ExceptionIntervalOffsetNegative"));
				}
				intervalOffset = value;
				Invalidate();
			}
		}

		public TickMark()
			: this(null)
		{
		}

		public TickMark(object parent)
			: base(parent)
		{
		}

		public TickMark(object parent, MarkerStyle shape, float length, float width)
			: base(parent, shape, length, width)
		{
		}
	}
}
