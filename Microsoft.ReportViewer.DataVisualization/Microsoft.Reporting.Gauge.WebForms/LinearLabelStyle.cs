using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;

namespace Microsoft.Reporting.Gauge.WebForms
{
	internal class LinearLabelStyle : GaugeObject
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

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeVisible6")]
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

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributePlacement5")]
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

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeFont3")]
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

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeFontUnit")]
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

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeFontAngle3")]
		[ValidateBound(0.0, 360.0)]
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
					throw new ArgumentException(Utils.SRGetStr("ExceptionOutOfrange", 0, 360));
				}
				fontAngle = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeTextColor5")]
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

		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeInterval")]
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
		[SRDescription("DescriptionAttributeIntervalOffset3")]
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
					throw new ArgumentException(Utils.SRGetStr("ExceptionIntervalOffsetNegative"));
				}
				intervalOffset = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeShowEndLabels")]
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

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeDistanceFromScale5")]
		[ValidateBound(-30.0, 30.0)]
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
					throw new ArgumentException(Utils.SRGetStr("ExceptionMustInRange", -100, 100));
				}
				scaleOffset = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeFormatString3")]
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

		protected LinearLabelStyle(object parent, Font font)
			: base(parent)
		{
			this.font = font;
		}

		internal string GetFormatStr()
		{
			if (formatStr == string.Empty)
			{
				if (formatString.Length > 0)
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
