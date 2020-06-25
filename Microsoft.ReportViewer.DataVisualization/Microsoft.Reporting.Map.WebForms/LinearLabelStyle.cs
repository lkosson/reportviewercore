using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class LinearLabelStyle : MapObject
	{
		private string formatStr = string.Empty;

		private bool visible = true;

		private Placement placement;

		private Font font = new Font("Microsoft Sans Serif", 14f);

		private FontUnit fontUnit;

		private float fontAngle;

		private Color textColor = Color.Black;

		private double interval = double.NaN;

		private double intervalOffset = double.NaN;

		private bool showEndLabels = true;

		private float scaleOffset = 2f;

		private string formatString = string.Empty;

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeLinearLabelStyle_Visible")]
		[ParenthesizePropertyName(true)]
		[DefaultValue(true)]
		public bool Visible
		{
			get
			{
				return visible;
			}
			set
			{
				visible = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeLinearLabelStyle_Placement")]
		[DefaultValue(Placement.Inside)]
		public Placement Placement
		{
			get
			{
				return placement;
			}
			set
			{
				placement = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeLinearLabelStyle_Font")]
		[DefaultValue(typeof(Font), "Microsoft Sans Serif, 14pt")]
		public virtual Font Font
		{
			get
			{
				return font;
			}
			set
			{
				font = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeLinearLabelStyle_FontUnit")]
		[DefaultValue(FontUnit.Percent)]
		public FontUnit FontUnit
		{
			get
			{
				return fontUnit;
			}
			set
			{
				fontUnit = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeLinearLabelStyle_FontAngle")]
		[DefaultValue(0f)]
		public float FontAngle
		{
			get
			{
				return fontAngle;
			}
			set
			{
				if (value > 360f || value < 0f)
				{
					throw new ArgumentOutOfRangeException(SR.out_of_range(0.0, 360.0));
				}
				fontAngle = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeLinearLabelStyle_TextColor")]
		[DefaultValue(typeof(Color), "Black")]
		public Color TextColor
		{
			get
			{
				return textColor;
			}
			set
			{
				textColor = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeLinearLabelStyle_Interval")]
		[TypeConverter(typeof(DoubleAutoValueConverter))]
		[DefaultValue(double.NaN)]
		public double Interval
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
		[SRDescription("DescriptionAttributeLinearLabelStyle_IntervalOffset")]
		[TypeConverter(typeof(DoubleAutoValueConverter))]
		[NotifyParentProperty(true)]
		[DefaultValue(double.NaN)]
		public double IntervalOffset
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

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeLinearLabelStyle_ShowEndLabels")]
		[DefaultValue(true)]
		public bool ShowEndLabels
		{
			get
			{
				return showEndLabels;
			}
			set
			{
				showEndLabels = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeLinearLabelStyle_DistanceFromScale")]
		[DefaultValue(2f)]
		public float DistanceFromScale
		{
			get
			{
				return scaleOffset;
			}
			set
			{
				if (value < -100f || value > 100f)
				{
					throw new ArgumentException(SR.must_in_range(-100.0, 100.0));
				}
				scaleOffset = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeLinearLabelStyle_FormatString")]
		[Localizable(true)]
		[DefaultValue("")]
		public string FormatString
		{
			get
			{
				return formatString;
			}
			set
			{
				formatString = value;
				formatStr = string.Empty;
				Invalidate();
			}
		}

		public LinearLabelStyle()
			: this(null)
		{
		}

		public LinearLabelStyle(object parent)
			: base(parent)
		{
		}

		internal string GetFormatStr()
		{
			if (string.IsNullOrEmpty(formatStr))
			{
				if (!string.IsNullOrEmpty(formatString))
				{
					formatStr = "{0:" + formatString + "}";
				}
				else
				{
					formatStr = "{0}";
				}
			}
			return formatStr;
		}
	}
}
