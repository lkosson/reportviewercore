using System.ComponentModel;
using System.Drawing;

namespace Microsoft.Reporting.Gauge.WebForms
{
	[TypeConverter(typeof(NumericRangeConverter))]
	internal class NumericRange : Range
	{
		private const double DEFAULT_START_VALUE = 7000.0;

		private const double DEFAULT_END_VALUE = 10000.0;

		private Color digitColor = Color.Red;

		private Color decimalColor = Color.Red;

		[SRCategory("CategoryMisc")]
		[SRDescription("DescriptionAttributeName13")]
		public override string Name
		{
			get
			{
				return base.Name;
			}
			set
			{
				base.Name = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryLayout")]
		[SRDescription("DescriptionAttributeStartValue")]
		[DefaultValue(7000.0)]
		public override double StartValue
		{
			get
			{
				return base.StartValue;
			}
			set
			{
				base.StartValue = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryLayout")]
		[SRDescription("DescriptionAttributeEndValue3")]
		[DefaultValue(10000.0)]
		public override double EndValue
		{
			get
			{
				return base.EndValue;
			}
			set
			{
				base.EndValue = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeNumericRange_DigitColor")]
		[DefaultValue(typeof(Color), "Red")]
		public Color DigitColor
		{
			get
			{
				return digitColor;
			}
			set
			{
				digitColor = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeNumericRange_DecimalColor")]
		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Color), "Red")]
		public Color DecimalColor
		{
			get
			{
				return decimalColor;
			}
			set
			{
				decimalColor = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeInRangeTimeout")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DefaultValue(0.0)]
		public override double InRangeTimeout
		{
			get
			{
				return base.InRangeTimeout;
			}
			set
			{
				base.InRangeTimeout = value;
			}
		}

		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeInRangeTimeoutType")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DefaultValue(PeriodType.Seconds)]
		public override PeriodType InRangeTimeoutType
		{
			get
			{
				return base.InRangeTimeoutType;
			}
			set
			{
				base.InRangeTimeoutType = value;
			}
		}

		public NumericRange()
			: base(7000.0, 10000.0)
		{
		}

		public override string ToString()
		{
			return Name;
		}

		internal override void OnAdded()
		{
			((IValueConsumer)((NumericIndicator)ParentElement).Data).Refresh();
			base.OnAdded();
		}
	}
}
