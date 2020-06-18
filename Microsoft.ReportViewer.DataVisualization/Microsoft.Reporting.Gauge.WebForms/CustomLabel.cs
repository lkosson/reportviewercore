using System;
using System.ComponentModel;
using System.Drawing;

namespace Microsoft.Reporting.Gauge.WebForms
{
	[TypeConverter(typeof(CustomLabelConverter))]
	internal class CustomLabel : NamedElement
	{
		private double labelValue;

		private CustomTickMark tickMarkStyle;

		private string text = "";

		private Font font = new Font("Microsoft Sans Serif", 14f);

		private FontUnit fontUnit;

		private Color textColor = Color.Black;

		private bool visible = true;

		private Placement placement;

		private bool rotateLabels;

		private bool allowUpsideDown;

		private float fontAngle;

		private float scaleOffset;

		[SRCategory("CategoryMisc")]
		[SRDescription("DescriptionAttributeCustomLabel_Name")]
		public override string Name
		{
			get
			{
				return base.Name;
			}
			set
			{
				base.Name = value;
			}
		}

		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeCustomLabel_Value")]
		[DefaultValue(0.0)]
		public double Value
		{
			get
			{
				return labelValue;
			}
			set
			{
				labelValue = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeCustomLabel_TickMarkStyle")]
		[TypeConverter(typeof(NoNameExpandableObjectConverter))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[DefaultValue(typeof(CustomTickMark), "MarkerStyle.Trapezoid, 10F, 6F")]
		public CustomTickMark TickMarkStyle
		{
			get
			{
				if (tickMarkStyle == null)
				{
					tickMarkStyle = new CustomTickMark(this, MarkerStyle.Trapezoid, 10f, 6f);
				}
				return tickMarkStyle;
			}
			set
			{
				tickMarkStyle = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeText5")]
		[Localizable(true)]
		[DefaultValue("")]
		public string Text
		{
			get
			{
				return text;
			}
			set
			{
				text = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeCustomLabel_Font")]
		[DefaultValue(typeof(Font), "Microsoft Sans Serif, 14pt")]
		public Font Font
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
		[SRDescription("DescriptionAttributeCustomLabel_TextColor")]
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

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeCustomLabel_Visible")]
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
		[SRDescription("DescriptionAttributeCustomLabel_Placement")]
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
		[SRDescription("DescriptionAttributeCustomLabel_RotateLabel")]
		[DefaultValue(false)]
		public bool RotateLabel
		{
			get
			{
				return rotateLabels;
			}
			set
			{
				rotateLabels = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeCustomLabel_AllowUpsideDown")]
		[DefaultValue(false)]
		public bool AllowUpsideDown
		{
			get
			{
				return allowUpsideDown;
			}
			set
			{
				allowUpsideDown = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeCustomLabel_FontAngle")]
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
		[SRDescription("DescriptionAttributeCustomLabel_DistanceFromScale")]
		[ValidateBound(-30.0, 30.0)]
		[DefaultValue(0f)]
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

		public override string ToString()
		{
			return Name;
		}
	}
}
