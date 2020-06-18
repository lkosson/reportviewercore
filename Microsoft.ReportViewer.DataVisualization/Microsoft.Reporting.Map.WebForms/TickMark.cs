using System;
using System.ComponentModel;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class TickMark : CustomTickMark
	{
		private double interval = double.NaN;

		private double intervalOffset = double.NaN;

		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeTickMark_Interval")]
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
					throw new ArgumentException(SR.interval_negative);
				}
				if (value == 0.0)
				{
					value = double.NaN;
				}
				interval = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeTickMark_IntervalOffset")]
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
					throw new ArgumentException(SR.interval_offset_negative);
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
